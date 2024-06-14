using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public int port = 10002;
    public String IP = "127.0.0.1";
    public int count = 0;
   

    private Queue<byte[]> datas;

    void Start () {
		buffer = new byte[1024 * 1024 * 10];

        // 创建服务器, 以Tcp的方式
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Connect(IPAddress.Parse(IP), port);

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
            int size = 0;
            List<byte> a = new List<byte>();
            bool ending = false;
            byte[] sending = new byte[6]{1,1,4,5,1,4};
            do
            {
                int count = socket.Receive(buffer);
                size += count;
                Byte[] result = new Byte[count];
                Array.Copy(buffer, 0, result, 0, count);
                a.AddRange(result);
                if (count >= 6)
                {
                    Byte[] results = new Byte[6];
                    Array.Copy(buffer, count - 6, results, 0, 6);
                    ending = results.SequenceEqual(sending);
                }
            }
            while(!ending);
            if (receState && a.Count > 0)
            {
                receState = false;
                BytesToImage(a.Count - 6, a.ToArray());
                Debug.Log(a.ToArray().Length);
                Debug.Log(size);
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
            //Debug.Log("接收數據:" + (float)datas.Count);
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

