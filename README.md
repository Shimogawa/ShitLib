# ShitLib v0.7.3

实现斗鱼与Bilibili弹幕连接接口。

An implementation of the danmaku api of Douyu and Bilibili Live platform.

## Usage 使用说明

#### 1. Setup 启用

斗鱼平台使用`DDanmakuGetter`，b站直播使用`BDanmakuGetter`。

Use `DDanmakuGetter` for Douyu, and `BDanmakuGetter` for Bilibili. For example I will use `BDanmakuGetter`。

```cs
var linker = new BDanmakuGetter(roomId);
```

声明可以使用`DanmakuGetter`，是它们的基类。

#### 2. Start Listening 开始监听

```cs
linker.Connect();
```

#### 3. Get Danmaku 获取弹幕

**第一种方法 First Way**

```cs
while (linker.IsConnected) {
	if (linker.DanmakuList.IsEmpty()) continue;
	var message = linker.DanmakuList.GetFirst();
	...
}
```

**第二种方法 Second Way**

```cs
foreach (var message in linker.DanmakuList.KeepGetting())
{
	if (!linker.IsConnected) break;
	...
}
```

`message`的类型是`MessageInfo<MessageType, Message>`，如果使用`BDanmakuGetter`，获取的是`BMessage : Message`，如果使用`DDanmakuGetter`，获取的是`DMessage : Message`。

`message`中的`MessageType`是一个enum，因为只有一个这样的enum，泛型使用完全是因为历史遗留问题，我懒得改了。

`message`中的`Message`是弹幕本体，建议使用强制转换进一步操作。

别的没有文档，看源码吧。

#### 4. Stop 退出

```cs
linker.Disconnect();
```