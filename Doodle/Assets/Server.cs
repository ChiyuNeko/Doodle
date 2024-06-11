using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Server : MonoBehaviour {

	Socket socket = null;
	Thread thread = null;
	byte[] buffer = null;
	bool receState = true;

	int readTimes = 0;

    public RawImage rawImage;
    public Texture2D texture2D;

    private Queue<byte[]> datas;

    void Start () {
		buffer = new byte[1024 * 1024 * 10];

        // 创建服务器, 以Tcp的方式
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Connect(IPAddress.Parse("127.0.0.1"), 10002);

        // 开启一个线程, 用于接受数据
		thread = new Thread(new ThreadStart(Receive));
		thread.Start();

        datas = new Queue<byte[]>();
    }

    private void Receive()
    {
        while (thread.ThreadState == ThreadState.Running && socket.Connected)
        {
            // 接受数据Buffer count是数据的长度
			int count = socket.Receive(buffer);
            if (receState && count > 0)
            {
				receState = false;
                BytesToImage(count, buffer);
            }
        }
    }

	MemoryStream ms = null;
	public void BytesToImage(int count, byte[] bytes)
    {
        try
        {
            ms = new MemoryStream(bytes, 0, count);
            datas.Enqueue(ms.ToArray());    // 将数据存储在一个队列中，在主线程中解析数据。这是一个多线程的处理。

            readTimes++;

            if (readTimes > 5000)
            {
                readTimes = 0;
                GC.Collect(2);  // 达到一定次数的时候，开启GC，释放内存
            }
        }
        catch
        {

        }
        receState = true;
    }

    void Update()
    {
        if (datas.Count > 0)
        {
            // 处理纹理数据，并显示
            Texture2D texture2D = new Texture2D(Screen.width, Screen.height);
            texture2D.LoadImage(datas.Dequeue());
            rawImage.texture = texture2D;
        }
    }

    void OnDestroy()
    {
        try
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
        }
        catch { }

        try
        {
            if (thread != null)
            {
                thread.Abort();
            }
        }
        catch { }

        datas.Clear();
    }
}
