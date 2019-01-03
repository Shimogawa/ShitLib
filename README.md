# ShitLib v0.7.3

ʵ�ֶ�����Bilibili��Ļ���ӽӿڡ�

An implementation of the danmaku api of Douyu and Bilibili Live platform.

## Usage ʹ��˵��

#### 1. Setup ����

����ƽ̨ʹ��`DDanmakuGetter`��bվֱ��ʹ��`BDanmakuGetter`��

Use `DDanmakuGetter` for Douyu, and `BDanmakuGetter` for Bilibili. For example I will use `BDanmakuGetter`��

```cs
var linker = new BDanmakuGetter(roomId);
```

��������ʹ��`DanmakuGetter`�������ǵĻ��ࡣ

#### 2. Start Listening ��ʼ����

```cs
linker.Connect();
```

#### 3. Get Danmaku ��ȡ��Ļ

**��һ�ַ��� First Way**

```cs
while (linker.IsConnected) {
	if (linker.DanmakuList.IsEmpty()) continue;
	var message = linker.DanmakuList.GetFirst();
	...
}
```

**�ڶ��ַ��� Second Way**

```cs
foreach (var message in linker.DanmakuList.KeepGetting())
{
	if (!linker.IsConnected) break;
	...
}
```

`message`��������`MessageInfo<MessageType, Message>`�����ʹ��`BDanmakuGetter`����ȡ����`BMessage : Message`�����ʹ��`DDanmakuGetter`����ȡ����`DMessage : Message`��

`message`�е�`MessageType`��һ��enum����Ϊֻ��һ��������enum������ʹ����ȫ����Ϊ��ʷ�������⣬�����ø��ˡ�

`message`�е�`Message`�ǵ�Ļ���壬����ʹ��ǿ��ת����һ��������

���û���ĵ�����Դ��ɡ�

#### 4. Stop �˳�

```cs
linker.Disconnect();
```