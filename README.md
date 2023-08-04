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

## smash 进程劫持，系统代理
一个winform，运行exe即可

## smash.proxy 代理协议客户端、服务端
代理协议本身不具备加密，需要通过nginx部署SSL，可以使得代理流量与正常HTTP无异
1. 配置 443 SSL监听，代理至 服务端 127.0.0.1:5413
2. 配置 其它任意端口监听，部署网站，作为 --fake 站点
#### 参数

**--mode client | server** 客户端或服务端，**默认server**

**--server 127.0.0.1:5413** 客户端填写服务器地址，必填

**--fake 127.0.0.1:8080** 服务端填写伪装地址，必填

**--port 5413** 运行在哪个端口，**默认5413**

**--key SNLTTY** 需要两端一致，客户端必填，服务端**默认随机**，**只包含字母**

**--buff 3** bufferSize 0-10，2^n次方，**默认3**

#### 托管
1. windows 可以使用nssm部署为windows service
2. docker镜像 **snltty/smash.proxy-alpine-x64** or **snltty/smash.proxy-alpine-arm64**
```
docker run -it -d --name="smash.proxy.server" -p 5413:5413/tcp snltty/smash.proxy-alpine-x64 \
--entrypoint ./smash.proxy.run --mode server --key SNLTTY --fake 127.0.0.1:8080
 
```
3. linux 使用 systemd 托管
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

#### nginx
1. wordpress站点  127.0.0.1:8080
2. 协议服务端      127.0.0.1:5413  **--fake 127.0.0.1:8080**
3. nginx **proxy_pass 127.0.0.1:5413**

```
user www-data;
worker_processes auto;
pid /run/nginx.pid;
include /etc/nginx/modules-enabled/*.conf;

events {
	worker_connections 102400;
}

stream {
	server{
		listen 80;
		listen 443 ssl;
		ssl_protocols TLSv1 TLSv1.1 TLSv1.2 TLSv1.3; # Dropping SSLv3, ref: POODLE
		ssl_ciphers ECDHE-RSA-AES128-GCM-SHA256:ECDHE:ECDH:AES:HIGH:!NULL:!aNULL:!MD5:!ADH:!RC4;
		ssl_prefer_server_ciphers on;
		ssl_certificate  /usr/local/smash.proxy/pem.pem;
    	ssl_certificate_key /usr/local/smash.proxy/key.key;
        	
		proxy_ssl_session_reuse on;
		proxy_pass 127.0.0.1:5413;
		
	}
}


```
