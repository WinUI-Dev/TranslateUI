> Chisese only.  
> You can translate it yourself using Edge translate.  
> Thanks to Edge, we can use Microsoft's translate API happily.
# 这个方法基于什么
`Edge Translate API。`
就是选中文本，点 `Translate to English` 后 Edge 调用的 API。
# 请求
> [!NOTE]  
> 这个 API 很宽松，似乎没有鉴权，Cookie 和请求头随便填写...大概吧

## Request 精简版

| METHOD      | URL                                                                  | CONTENT                   |
| ----------- | -------------------------------------------------------------------- | --------------------------
| POST        | `https://edge.microsoft.com/translate/translatetext?from=&to=en`     | `["你要翻译的"]`   |

> [!TIP]
> `to=en` 中的 en 换为你想要的值
> 例如：`to=cn`

### Request Body

```
[
    " 你要翻译的"
]
```

## Request Header

<details>
    <summary>
        <h3>点我查看无关紧要的 Header...</h3>
    </summary>

### 访问概述
```
:method: POST
:authority: /translate/translatetext?from=&to=en
:scheme: https
:path: /translate/translatetext?from=&to=en
```
### 真正的 Header
```
content-length: 25
sec-ch-ua-platform: "Windows"
user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0
sec-ch-ua: "Microsoft Edge";v="131", "Chromium";v="131", "Not_A Brand";v="24"
content-type: application/json
sec-ch-ua-mobile: ?0
accept: */*
sec-mesh-client-edge-version: 131.0.2903.70
sec-mesh-client-edge-channel: stable
sec-mesh-client-os: Windows
sec-mesh-client-os-version: 10.0.26100
sec-mesh-client-arch: x86_64
sec-mesh-client-webview: 0
origin: https://github.com
x-edge-shopping-flag: 1
sec-fetch-site: cross-site
sec-fetch-mode: cors
sec-fetch-dest: empty
referer: https://github.com/lingbopro/lingbos-sussy-mod/discussions/1
accept-encoding: gzip, deflate, br, zstd
accept-language: en,zh-CN;q=0.9,zh;q=0.8,en-GB;q=0.7,en-US;q=0.6
priority: u=1, i
```
### 解释
`referer` 不用管，随便填  
`origin` 填 `referer` 对应的域名  
`accept-language`：要译为的语言的前两个字符`,`原本的语言`;q=0.9,`原本的语言的前两位`;q=0.8`要译为的语言`;q=0.7,`要译为的语言`q=0.6`  
（`accept-language`没有验证，但我猜是这样）  
其他的不要改  
有的是自动生成的，也不要管，例如`content-length`  
</details>

# 返回内容

> [!IMPORTANT]
> 此处不再省略。

## Header
emm，Header似乎没有有价值的内容，但是也贴一下
```
cache-control: max-age=0, no-cache, no-store, must-revalidate
content-length: 170
content-type: text/plain; charset=utf-8
access-control-allow-origin: *
x-content-type-options: nosniff
x-requestid: 67f9bd84-3530-4473-bcfa-0c86147e7629.EDEA.1206T0230
x-cache: CONFIG_NOCACHE
x-msedge-ref: Ref A: 4107B96CE2E147D2B34E5E0ABD28B613 Ref B: SIN30EDGE0518 Ref C: 2024-12-06T02:30:22Z
date: Fri, 06 Dec 2024 02:30:22 GMT
```
## 返回的 Body
### 原内容
```
[{"detectedLanguage":{"language":"zh-Hans","score":0.77},"translations":[{"text":" So the header contains","to":"en","sentLen":{"srcSentLen":[13],"transSentLen":[23]}}]}]
```
### JSON化过了之后的内容
就是好看一点
```
[
    {
        "detectedLanguage": {
            "language": "zh-Hans",
            "score": 0.77
        },
        "translations": [
            {
                "text": " So the header contains",
                "to": "en",
                "sentLen": {
                    "srcSentLen": [
                        13
                    ],
                    "transSentLen": [
                        23
                    ]
                }
            }
        ]
    }
]
```
### 解释
emm，实际上有价值的只有 `translations` 中的 `text`  
OK，就这样，我觉得解释的够清楚了。  

# 作者与发现者
> _Article by_  
> **_`Luke Zhang`_**  
> [@zsr-lukezhang](github.com/zsr-lukezhang)  
  
> _Original Idea by_  
> **_`Lingbo`_**  
> [@lingbopro](github.com/lingbopro)  
> [原文章](https://github.com/lingbopro/lingbos-sussy-mod/discussions/1#discussioncomment-11461648)
