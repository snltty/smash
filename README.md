<div align="center">
<p><img src="smash/public/icon.png" height="150"></p> 

# smash
#### Visual Studio 2022 LTSC 17.4.1
<a href="https://jq.qq.com/?_wv=1027&k=ucoIVfz4" target="_blank">QQ 群：1121552990</a>

![GitHub Repo stars](https://img.shields.io/github/stars/snltty/smash?style=social)
![GitHub Repo forks](https://img.shields.io/github/forks/snltty/smash?style=social)
[![star](https://gitee.com/snltty/smash/badge/star.svg?theme=dark)](https://gitee.com/snltty/smash/stargazers)
[![fork](https://gitee.com/snltty/smash/badge/fork.svg?theme=dark)](https://gitee.com/snltty/smash/members)

使用前请确保你已知其中风险

本软件仅供学习交流，请勿用于违法犯罪

</div>

## 做了个啥
1. smash
    1. 使用Netfilter驱动拦截进程流量，将其代理
    2. 设置系统代理，设置PAC自动代理或环境变量代理
2. smash.proxy 代理协议

## smash
一个winform，运行exe即可

## smash.proxy
#### 运行参数

**--mode client | server** 以客户端或服务端运行

**--server 127.0.0.1:5413** 以客户端运行时需要填写服务器地址

**--fake 127.0.0.1:80** 以服务端运行时，需要填写一个伪装地址，当遇到非代理协议时，使用本服务作为数据返回

**--port 5413** 运行在哪个端口

**--key SNLTTY** key,客户端与服务端key一致时，可以使用代理

**--buff 3** buffer size 0-10,分别表示2^n次方

#### 部署
1. docker镜像 **snltty/smash.proxy-alpine-x64**
```
docker run -it -d --name="smash.proxy.server" -p 5413:5413/tcp \
--entrypoint ./smash.proxy.run --mode server --key SNLTTY --fake 127.0.0.1:80 \
 snltty/smash.proxy-alpine-x64 
```
2. windows 可以使用nssm部署为windows service
2. linux 使用 systemd 托管
```
//1、下载linux版本程序，放到 /usr/local/smash.proxy 文件夹

//3、写配置文件
vim /etc/systemd/system/smash.proxy.service

[Unit]
Description=smash.proxy

[Service]
WorkingDirectory=/usr/local/smash.proxy
ExecStart=/usr/local/smash.proxy/smash.proxy
ExecStop=/bin/kill $MAINPID
ExecReload=/bin/kill -HUP $MAINPID
Restart=always

[Install]
WantedBy=multi-user.target


//4、重新加载配置文件
systemctl daemon-reload
//5、启动，或者重新启动
systemctl start smash.proxy
systemctl restart smash.proxy
```
