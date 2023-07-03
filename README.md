<div align="center">
<p><img src="netch-process/netch-process/public/icon.png" height="150"></p> 

# netch-process
#### Visual Studio 2022 LTSC 17.4.1
<a href="https://jq.qq.com/?_wv=1027&k=ucoIVfz4" target="_blank">QQ 群：1121552990</a>

![GitHub Repo stars](https://img.shields.io/github/stars/snltty/netch-process?style=social)
![GitHub Repo forks](https://img.shields.io/github/forks/snltty/netch-process?style=social)
[![star](https://gitee.com/snltty/netch-process/badge/star.svg?theme=dark)](https://gitee.com/snltty/netch-process/stargazers)
[![fork](https://gitee.com/snltty/netch-process/badge/fork.svg?theme=dark)](https://gitee.com/snltty/netch-process/members)

使用前请确保你已知其中风险

本软件仅供学习交流，请勿用于违法犯罪

</div>

## 做了个啥
1. 这是一个进程代理项目，使用Netfilter驱动拦截进程流量，将其代理
2. 因为很多软件都不会内置代理功能，且不会去读取系统代理设置，那么进程劫持代理将会很有用 
3. 这是对(<a href="https://github.com/netchx/netch" target="_blank">netch</a>)，项目的一个简化，只保留了进程代理模式，及只保留了socks5代理协议

## 重要的点
1. 如果你是使用域名，则应该勾选DNS代理，并且清除浏览器及操作系统的DNS缓存
2. 跟一般配置系统代理不一样的是，浏览器如果读取系统代理，使用代理时，是直接代理域名，域名在服务器解析，因此不会有这样的DNS污染问题

## 支持作者
请作者喝一杯咖啡，使其更有精力更新代码
<p><img src="netch-process/netch-process/public/qr.jpg" width="360"></p> 