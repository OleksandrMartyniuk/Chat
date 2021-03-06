﻿using Core;
using MultiRoomChatClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiRoomChatClient
{
    public static class Client
    {
        public static string Username { get; set; }
        static TcpClient client;
        static NetworkStream stream;
        static LinkedList<string> messageQue = new LinkedList<string>();
        static Thread processThread;
        public static HistoryDataprovider RoomHistory = new HistoryDataprovider("Msg");
        public static HistoryDataprovider PrivateHistory = new HistoryDataprovider("Private");

        public delegate void responseHandler(string json);
        public static event responseHandler responseReceived;

        public static void StartClient()
        {
            Client.Start("127.0.0.1", 8888);
        }
        
        static void Start(string host, int port)
        {
            if(client == null || stream == null)
            {
                client = new TcpClient();
            }
            else
            {
                return;
            }
            //try
            //{
                client.Connect(host, port);
                stream = client.GetStream();
                processThread = new Thread(new ThreadStart(Process));

                processThread.Start();
            //}
            //catch (Exception ex)
            //{
                //Console.WriteLine(ex.Message);
            //}
            //finally
            //{
           //     Disconnect();
            //}
        }
        public static void AddRequest(string message)
        {
            messageQue.AddLast(message);
        }

        static void WriteStream()
        {
            if (messageQue.Count > 0)
            {
                StreamWriter sw = new StreamWriter(stream);
                while (messageQue.Count > 0)
                {
                    string message = messageQue.First.Value;

                    sw.WriteLine(message);
                    sw.Flush();

                    messageQue.RemoveFirst();
                }
            }
        }

        private static void ReadStream()
        {
            StreamReader reader = new StreamReader(stream);
            string response = "";
            while (stream.DataAvailable)
            {
                response += reader.ReadLine();
            }
            if(response != null && response.Length > 0)
            responseReceived(response);
        }
       
        static void Process()
        {
            while (true)
            {
                //try
                //{
                ReadStream();
                    
                    WriteStream();

                Thread.Sleep(20);
                //}
                //catch
                //{
                //    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                //    Console.ReadLine();
                //    Disconnect();
                //}
            }
        }

        public static void Disconnect()
        {
            processThread.Abort();
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
        }
    }
}
