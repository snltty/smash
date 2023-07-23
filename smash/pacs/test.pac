
var domains = {
    "bilibili.com":1
}

var iplist = {}  
var proxy = "{proxy}; DIRECT;";
var direct = 'DIRECT;';

function FindProxyForURL(url, host) {
    var lastPos;
    var index1 = host.indexOf('.');
    var index2 = host.lastIndexOf('.');
    var minHost = host
    if(index1 == index2)
        minHost = host.substring(0,index1)
    else
        minHost = host.substring(index1 + 1, index2)
    
    if (domains.hasOwnProperty(minHost))
    {
        return proxy;
    }
    do {
            if (domains.hasOwnProperty(host)) {
                        return proxy;
                    }
            lastPos = host.indexOf('.') + 1;
            host = host.slice(lastPos);
        } while (lastPos >= 1);
    return direct;
}
