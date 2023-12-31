﻿using common.libs;
using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using common.libs.extends;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Concurrent;
using System.Buffers;
using smash.proxy.protocol;
using common.libs.socks5;

namespace smash.proxy.client
{
    internal sealed class ProxyClient
    {
        private readonly ProxyClientConfig proxyClientConfig;

        private Socket Socket;
        private UdpClient UdpClient;
        private ConcurrentDictionary<IPEndPoint, ProxyClientUserToken> udpConnections = new ConcurrentDictionary<IPEndPoint, ProxyClientUserToken>();
        private Random random = new Random();

        public ProxyClient(ProxyClientConfig proxyClientConfig)
        {
            this.proxyClientConfig = proxyClientConfig;
        }

        public bool Start()
        {
            BindAccept();
            return true;
        }

        private void BindAccept()
        {
            var endpoint = new IPEndPoint(IPAddress.Any, proxyClientConfig.ListenPort);
            Socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(endpoint);
            Socket.Listen(int.MaxValue);

            UdpClient = new UdpClient(endpoint);
            UdpClient.EnableBroadcast = true;
            UdpClient.Client.WindowsUdpBug();

            SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
            acceptEventArg.UserToken = new ProxyClientUserToken
            {
                Saea = acceptEventArg,
                Step = Socks5EnumStep.Request,
                Request = new ProxyInfo
                {
                    AddressType = Socks5EnumAddressType.IPV4,
                    Command = Socks5EnumRequestCommand.Connect
                }
            };
            acceptEventArg.Completed += IO_Completed;
            StartAccept(acceptEventArg);

            IAsyncResult result = UdpClient.BeginReceive(ProcessReceiveUdp, new ProxyClientUserToken
            {
                Step = Socks5EnumStep.ForwardUdp,
                PoolBuffer = new byte[(1 << (byte)proxyClientConfig.BufferSize) * 1024],
                Request = new ProxyInfo
                {
                    AddressType = Socks5EnumAddressType.IPV4,
                    Command = Socks5EnumRequestCommand.UdpAssociate
                }
            });
        }
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            try
            {

                acceptEventArg.AcceptSocket = null;
                ProxyClientUserToken token = ((ProxyClientUserToken)acceptEventArg.UserToken);
                if (Socket.AcceptAsync(acceptEventArg) == false)
                {
                    ProcessAccept(acceptEventArg);
                }
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    ProcessAccept(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                default:
                    Logger.Instance.Error(e.LastOperation.ToString());
                    break;
            }
        }
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            BindReceive(e);
            StartAccept(e);
        }
        private void BindReceive(SocketAsyncEventArgs e)
        {
            try
            {
                Socket socket = e.AcceptSocket;
                ProxyClientUserToken acceptToken = (e.UserToken as ProxyClientUserToken);

                ProxyClientUserToken token = new ProxyClientUserToken
                {
                    ClientSocket = socket,
                    Step = Socks5EnumStep.Request,
                    Request = new ProxyInfo
                    {
                        AddressType = Socks5EnumAddressType.IPV4,
                    },
                };

                SocketAsyncEventArgs readEventArgs = new SocketAsyncEventArgs
                {
                    UserToken = token,
                    SocketFlags = SocketFlags.None
                };
                token.Saea = readEventArgs;

                token.ClientSocket.KeepAlive();
                token.PoolBuffer = new byte[(1 << (byte)proxyClientConfig.BufferSize) * 1024];
                readEventArgs.SetBuffer(token.PoolBuffer, 0, (1 << (byte)proxyClientConfig.BufferSize) * 1024);
                readEventArgs.Completed += IO_Completed;

                if (token.ClientSocket.ReceiveAsync(readEventArgs) == false)
                {
                    ProcessReceive(readEventArgs);
                }
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
        private async void ProcessReceive(SocketAsyncEventArgs e)
        {
            ProxyClientUserToken token = (ProxyClientUserToken)e.UserToken;
            try
            {
                if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success)
                {
                    CloseClientSocket(e);
                    return;
                }

                int totalLength = e.BytesTransferred;
                bool canNext = true;
                token.Request.Data = e.Buffer.AsMemory(0, totalLength);
                //有些客户端，会把一个包拆开发送，很奇怪，不得不验证一下数据完整性
                if (token.Step <= Socks5EnumStep.Command)
                {
                    canNext = await ReceiveCommandData(token, e, totalLength);
                }
                if (canNext)
                {
                    await Receive(token);
                }

                if (token.ClientSocket.Available > 0)
                {
                    while (token.ClientSocket.Available > 0)
                    {
                        int length = await token.ClientSocket.ReceiveAsync(e.Buffer.AsMemory(), SocketFlags.None);
                        if (length == 0)
                        {
                            CloseClientSocket(e);
                            return;
                        }
                        token.Request.Data = e.Buffer.AsMemory(0, length);
                        await Receive(token);
                    }
                }

                if (token.ClientSocket.ReceiveAsync(e) == false)
                {
                    ProcessReceive(e);
                }
            }
            catch (Exception ex)
            {
                CloseClientSocket(e);
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
        private async void ProcessReceiveUdp(IAsyncResult result)
        {
            IPEndPoint rep = new IPEndPoint(IPAddress.Any, 0);
            ProxyClientUserToken receiveToken = result.AsyncState as ProxyClientUserToken;
            try
            {
                receiveToken.Request.Data = UdpClient.EndReceive(result, ref rep);
                receiveToken.SourceEP = rep;
                if (udpConnections.TryGetValue(rep, out ProxyClientUserToken token) == false)
                {
                    token = new ProxyClientUserToken
                    {
                        SourceEP = receiveToken.SourceEP,
                        Request = new ProxyInfo
                        {
                            AddressType = receiveToken.Request.AddressType,
                            TargetAddress = receiveToken.Request.TargetAddress,
                            TargetPort = receiveToken.Request.TargetPort,
                            Command = receiveToken.Request.Command,
                            Data = receiveToken.Request.Data
                        },
                        Step = receiveToken.Step,
                    };

                    GetRemoteEndPoint(token.Request, out int index);
                    bool res = await ConnectServer(token, token.Request);
                    if (res == false)
                    {
                        result = UdpClient.BeginReceive(ProcessReceiveUdp, token);
                        return;
                    }
                    udpConnections.TryAdd(rep, token);
                }
                else
                {
                    token.Request.Data = receiveToken.Request.Data;
                    token.SourceEP = receiveToken.SourceEP = rep;
                }

                if (token.Request.Data.Length > 0)
                {
                    if (await HandleRequestData(token, token.Request))
                    {
                        if (token.Request.Data.Length > 0)
                        {
                            await Request(token);
                            token.Request.Data = Helper.EmptyArray;
                        }
                    }
                }
                result = UdpClient.BeginReceive(ProcessReceiveUdp, receiveToken);
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error($"listen udp -> error " + ex);
            }
        }
        private async Task<bool> ReceiveCommandData(ProxyClientUserToken token, SocketAsyncEventArgs e, int totalLength)
        {
            EnumProxyValidateDataResult validate = Socks5Parser.ValidateData(token.Step, token.Request.Data);
            if ((validate & EnumProxyValidateDataResult.TooShort) == EnumProxyValidateDataResult.TooShort)
            {
                //太短
                while ((validate & EnumProxyValidateDataResult.TooShort) == EnumProxyValidateDataResult.TooShort)
                {
                    totalLength += await token.ClientSocket.ReceiveAsync(e.Buffer.AsMemory(e.Offset + totalLength), SocketFlags.None);
                    token.Request.Data = e.Buffer.AsMemory(e.Offset, totalLength);
                    validate = Socks5Parser.ValidateData(token.Step, token.Request.Data);
                }
            }

            //不短，又不相等，直接关闭连接
            if ((validate & EnumProxyValidateDataResult.Equal) != EnumProxyValidateDataResult.Equal)
            {
                CloseClientSocket(e);
                return false;
            }
            return true;
        }


        private async Task<bool> ConnectServer(ProxyClientUserToken token, ProxyInfo info)
        {
            try
            {
                token.ServerSocket = new Socket(proxyClientConfig.ServerEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                token.ServerSocket.KeepAlive();
                await token.ServerSocket.ConnectAsync(proxyClientConfig.ServerEP);

                if (proxyClientConfig.IsSSL)
                {
                    SslStream sslStream = new SslStream(new NetworkStream(token.ServerSocket), true, ValidateServerCertificate, null);
                    await sslStream.AuthenticateAsClientAsync(proxyClientConfig.Domain);
                    token.ServerStream = sslStream;
                }
                token.BuildConnectData(info, proxyClientConfig.KeyMemory);
                BindServerReceive(token);
                return true;
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error($"connect server -> error " + ex);
            }
            return false;
        }
        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        private void BindServerReceive(ProxyClientUserToken token)
        {
            if (proxyClientConfig.IsSSL)
            {
                if (token.ServerStream == null || token.ServerStream.CanRead == false) return;

                token.ServerPoolBuffer = new byte[(1 << (byte)proxyClientConfig.BufferSize) * 1024];
                IAsyncResult result = token.ServerStream.BeginRead(token.ServerPoolBuffer, 0, token.ServerPoolBuffer.Length, ServerReceiveCallback, token);
                if (result.CompletedSynchronously)
                {
                    ServerReceiveCallback(result);
                }
            }
            else
            {
                token.ServerPoolBuffer = new byte[(1 << (byte)proxyClientConfig.BufferSize) * 1024];
                token.ServerSocket.BeginReceive(token.ServerPoolBuffer, 0, token.ServerPoolBuffer.Length, SocketFlags.None, ServerReceiveRawCallback, token);
            }
        }
        private async void ServerReceiveRawCallback(IAsyncResult result)
        {
            ProxyClientUserToken token = result.AsyncState as ProxyClientUserToken;
            try
            {
                int length = token.ServerSocket.EndReceive(result);

                token.Request.ResponseData = token.ServerPoolBuffer.AsMemory(0, length);
                await InputData(token, token.Request);

                token.ServerSocket.BeginReceive(token.ServerPoolBuffer, 0, token.ServerPoolBuffer.Length, SocketFlags.None, ServerReceiveRawCallback, token);
            }
            catch (Exception ex)
            {
                CloseClientSocket(token);
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
        private async void ServerReceiveCallback(IAsyncResult result)
        {
            ProxyClientUserToken token = result.AsyncState as ProxyClientUserToken;
            try
            {
                int length = token.ServerStream.EndRead(result);
                if (length == 0)
                {
                    CloseClientSocket(token);
                    return;
                }

                token.Request.ResponseData = token.ServerPoolBuffer.AsMemory(0, length);
                await InputData(token, token.Request);
                token.ServerStream.BeginRead(token.ServerPoolBuffer, 0, token.ServerPoolBuffer.Length, ServerReceiveCallback, token);
            }
            catch (Exception ex)
            {
                CloseClientSocket(token);
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }


        private async Task Receive(ProxyClientUserToken token)
        {
            try
            {
                if (token.Request.Data.Length == 0 && token.Step == Socks5EnumStep.Command)
                {
                    return;
                }
                if (await HandleRequestData(token, token.Request) == false)
                {
                    return;
                }
                bool res = await Request(token);
                if (res == false)
                {
                    CloseClientSocket(token);
                }
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
        private async Task<bool> Request(ProxyClientUserToken token)
        {
            Memory<byte> data = token.Request.Data;
            byte[] bytes = Helper.EmptyArray;
            if (token.ConnectDataLength > 0)
            {
                int padding = random.Next(100, 512);
                bytes = token.Request.PackConnect(token.ConnectData, token.ConnectDataLength, data, padding, out int length);
                data = bytes.AsMemory(0, length);
                token.ReturnConnectData();
            }

            byte[] packData = Helper.EmptyArray;
            try
            {
                if (proxyClientConfig.IsSSL)
                {
                    await token.ServerStream.WriteAsync(data);
                }
                else
                {
                    await token.ServerSocket.SendAsync(data);
                }
            }
            finally
            {
                if (bytes.Length > 0)
                {
                    ArrayPool<byte>.Shared.Return(bytes);
                }
                if (packData.Length > 0)
                {
                    ArrayPool<byte>.Shared.Return(packData);
                }
            }

            return true;
        }
        private async Task InputData(ProxyClientUserToken token, ProxyInfo info)
        {
            try
            {
                if (info.ResponseData.Length == 0)
                {
                    CloseClientSocket(token);
                    return;
                }

                if (HandleAnswerData(token, info))
                {
                    if (token.Step == Socks5EnumStep.ForwardUdp)
                    {
                        //组装udp包
                        byte[] bytes = Socks5Parser.MakeUdpResponse(new IPEndPoint(new IPAddress(info.TargetAddress.Span), info.TargetPort), info.ResponseData, out int legnth);
                        await UdpClient.SendAsync(bytes.AsMemory(0, legnth), token.SourceEP);
                        Socks5Parser.Return(bytes);
                    }
                    else
                    {
                        int length = await token.ClientSocket.SendAsync(info.ResponseData, SocketFlags.None);
                    }
                }
            }
            catch (Exception ex)
            {
                CloseClientSocket(token);
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }


        private async Task<bool> HandleRequestData(ProxyClientUserToken token, ProxyInfo info)
        {
            //request  auth 的 直接通过,跳过验证部分
            if (token.Step < Socks5EnumStep.Command)
            {
                info.ResponseData = new byte[] { 0x00 };
                token.Step++;
                await InputData(token, info);
                return false;
            }
            //command 的
            if (token.Step == Socks5EnumStep.Command)
            {
                Socks5EnumRequestCommand command = (Socks5EnumRequestCommand)info.Data.Span[1];
                //将socks5的command转化未通用command
                if (command == Socks5EnumRequestCommand.Bind)
                {
                    info.ResponseData = new byte[] { (byte)Socks5EnumResponseCommand.CommandNotAllow };
                    await InputData(token, info);
                    return false;
                }


                if (command == Socks5EnumRequestCommand.Connect)
                {
                    //解析出目标地址
                    GetRemoteEndPoint(info, out int index);
                    bool res = await ConnectServer(token, info);
                    if (res == false)
                    {
                        info.ResponseData = new byte[] { (byte)Socks5EnumResponseCommand.NetworkError };
                        await InputData(token, info);
                        return false;
                    }

                }

                info.ResponseData = new byte[] { (byte)Socks5EnumResponseCommand.ConnecSuccess };
                await InputData(token, info);
                return false;
            }
            else
            {
                if (token.Step == Socks5EnumStep.ForwardUdp)
                {
                    //解析出目标地址
                    GetRemoteEndPoint(info, out int index);
                    //解析出udp包的数据部分
                    info.Data = Socks5Parser.GetUdpData(info.Data);
                }
            }

            return true;
        }
        private bool HandleAnswerData(ProxyClientUserToken token, ProxyInfo info)
        {
            //request auth 步骤的，只需回复一个字节的状态码
            if (token.Step < Socks5EnumStep.Command)
            {
                info.ResponseData = new byte[] { 5, info.ResponseData.Span[0] };
                token.Step++;
                return true;
            }
            switch (token.Step)
            {
                case Socks5EnumStep.Command:
                    {
                        //command的，需要区分成功和失败，成功则回复指定数据，失败则关闭连接
                        info.ResponseData = Socks5Parser.MakeConnectResponse(new IPEndPoint(IPAddress.Any, proxyClientConfig.ListenPort), info.ResponseData.Span[0]);
                        token.Step = Socks5EnumStep.Forward;
                    }
                    break;
                case Socks5EnumStep.Forward:
                    {
                        token.Step = Socks5EnumStep.Forward;
                        if (token.FirstPack == false)
                        {
                            token.FirstPack = true;
                            if (info.ResponseData.Length < 2048)
                            {
                                info.ResponseData = ProxyInfo.UnPackFirstResponse(info.ResponseData);
                            }
                        }
                    }
                    break;
                case Socks5EnumStep.ForwardUdp:
                    {
                        token.Step = Socks5EnumStep.ForwardUdp;
                        if (token.FirstPack == false)
                        {
                            token.FirstPack = true;
                            if (info.ResponseData.Length < 2048)
                            {
                                info.ResponseData = ProxyInfo.UnPackFirstResponse(info.ResponseData);
                            }
                        }
                    }
                    break;
            }
            return true;
        }
        private void GetRemoteEndPoint(ProxyInfo info, out int index)
        {
            try
            {
                info.TargetAddress = Socks5Parser.GetRemoteEndPoint(info.Data, out Socks5EnumAddressType addressType, out ushort port, out index);
                info.AddressType = addressType;
                info.TargetPort = port;
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
                throw;
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            ProxyClientUserToken token = e.UserToken as ProxyClientUserToken;
            CloseClientSocket(token);
        }
        private void CloseClientSocket(ProxyClientUserToken token)
        {
            if (token.SourceEP != null)
            {
                udpConnections.TryRemove(token.SourceEP, out _);
            }
            token.ClientSocket?.SafeClose();
            if (proxyClientConfig.IsSSL)
            {
                token.ServerStream?.Dispose();
            }
            else
            {
                token.ServerSocket?.SafeClose();
            }
            token.PoolBuffer = null;
            token.ServerPoolBuffer = null;
            token.Request = null;
            token.Saea?.Dispose();
            if (token.ConnectDataLength > 0)
            {
                token.ConnectDataLength = 0;
                ArrayPool<byte>.Shared.Return(token.ConnectData);
            }
            GC.Collect();
        }
        public void Stop()
        {
            try
            {
                Socket?.SafeClose();
                UdpClient?.Dispose();
            }
            catch (Exception ex)
            {
                if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    Logger.Instance.Error(ex);
            }
        }
    }

    internal sealed class ProxyClientUserToken
    {
        #region 客户端
        public Socket ClientSocket { get; set; }
        public SocketAsyncEventArgs Saea { get; set; }
        public byte[] PoolBuffer { get; set; }
        public ProxyInfo Request { get; set; } = new ProxyInfo { };
        public Socks5EnumStep Step { get; set; } = Socks5EnumStep.Request;
        public IPEndPoint SourceEP { get; set; }
        #endregion

        #region 服务端
        public Socket ServerSocket { get; set; }
        public SslStream ServerStream { get; set; }
        public byte[] ServerPoolBuffer { get; set; }

        public byte[] ConnectData { get; set; }
        public int ConnectDataLength { get; set; }
        public bool FirstPack { get; set; }
        public void BuildConnectData(ProxyInfo info, Memory<byte> keyMemory)
        {
            ConnectData = info.PackPrevConnect(keyMemory, out int length);
            ConnectDataLength = length;
        }
        public void ReturnConnectData()
        {
            ConnectDataLength = 0;
            Request.Return(ConnectData);
            ConnectData = Helper.EmptyArray;
        }
        #endregion
    }

    public sealed class ProxyClientConfig
    {
        public ushort ListenPort { get; set; }
        public string Key
        {
            set
            {
                keyMemory = value.ToBytes();
            }
        }

        private Memory<byte> keyMemory;
        public Memory<byte> KeyMemory { get => keyMemory; }

        public IPEndPoint ServerEP { get; set; }

        public EnumBufferSize BufferSize { get; set; } = EnumBufferSize.KB_8;

        public string Domain { get; set; }

#if DEBUG
        public bool IsSSL { get; set; } = true;
#else
        public bool IsSSL { get; set; } = true;
#endif
    }
}

//https://raw.githubusercontent.com/snltty/p2p-tunnel/master/readme/size.jpg