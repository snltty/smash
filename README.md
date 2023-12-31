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

## 1、smash 进程劫持，系统代理
一个winform，运行exe即可

## 2、smash.proxy 代理协议
1. nginx转发
2. tls over tls
3. ssl hello padding，下层tls hello随机填充长度
4. fake，非代理协议正常请求网站

<p><img src="./readme/smash.jpg"></p>

## 2.1、运行参数

1. **【--mode】 client | server** 客户端或服务端，**默认server**

2. **【--server】 xxx.xxx.com:443** 客户端填写服务器地址，必填，使用域名，将会作为SNI

3. **【--fake】 127.0.0.1:8080** 服务端填写伪装地址，必填

4. **【--dns】 8.8.8.8** dns ip,会自动获取系统dns ip，如果失败，可以手动填写

5. **【--port】 5413** 运行在哪个端口，**默认5413**

6. **【--key】 SNLTTY** 需要两端一致，客户端必填，服务端**默认随机**

7. **【--buff】 3** bufferSize 0-10，2^n次方，**默认3**

## 2.2、托管示例

###### 参数示例
```
1. --mode client --buff 3 --port 5413 --key SNLTTY --server xxx.xxx.com:443
2. --mode server --buff 3 --port 5413 --key SNLTTY --fake 127.0.0.1:8080 --dns 8.8.8.8
```

###### 在windows部署客户端或服务端
```
1. 可以使用nssm部署为windows service
```

###### 在linux部署客户端或服务端
```
1. snltty/smash.proxy-alpine-x64
2. snltty/smash.proxy-alpine-arm64

docker run -it -d --name="smash.proxy.server" \
-p 5413:5413/tcp -p 5413:5413/udp snltty/smash.proxy-alpine-x64 \
--entrypoint ./smash.proxy.run --mode server --key SNLTTY --fake 127.0.0.1:8080
```
```
//1、下载linux版本程序，放到 /usr/local/smash.proxy 文件夹

//3、写配置文件
vim /etc/systemd/system/smash.proxy.service

[Unit]
Description=smash.proxy

[Service]
WorkingDirectory=/usr/local/smash.proxy
ExecStart=/usr/local/smash.proxy/smash.proxy --mode server --key SNLTTY --fake 127.0.0.1:8080
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

###### 在服务端部署nginx
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
		proxy_pass 127.0.0.1:5413; #smash.proxy 服务端
		
	}
}
```
