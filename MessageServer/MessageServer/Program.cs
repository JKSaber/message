using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Serilog;
using System.Reflection;

namespace MessageServer
{
    class Program
    {
        private static void ProcessClientRequests(object argument)
        {
            TcpClient client = (TcpClient)argument;
            try
            {
                StreamReader sr = new StreamReader(client.GetStream());
                StreamWriter sw = new StreamWriter(client.GetStream());
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                stream.Read(buffer, 0, buffer.Length);
                int recv = 0;
                foreach (byte b in buffer)
                {
                    if (b != 0)
                    {
                        recv++;
                    }
                }
                string request = Encoding.ASCII.GetString(buffer, 0, recv);
                Console.WriteLine("request received: " + request);
                byte[] asciiBytes = Encoding.ASCII.GetBytes(request);
                int asciiBytesSum = 0;
                foreach (byte b in asciiBytes)
                {
                    asciiBytesSum = asciiBytesSum + b;
                }
                string answer = String.Join("+", asciiBytes) + "=" + asciiBytesSum.ToString();
               
                sw.WriteLine(answer);
                var pi = stream.GetType().GetProperty("Socket", BindingFlags.NonPublic | BindingFlags.Instance);
                var socketIp = ((Socket)pi.GetValue(stream, null)).RemoteEndPoint.ToString();
                Log.Information(socketIp + " " + request + " " + answer);
                
                sw.Flush();
                sr.Close();
                sw.Close();
                client.Close();
            }
            catch (IOException)
            {
                Console.WriteLine("Problem with client communication. Exiting thread");
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("consoleapp.log")
                .CreateLogger();

            TcpListener listener = null;
                try
                {
                    listener = new TcpListener(System.Net.IPAddress.Any, 1302);
                    listener.Start();
                    while (true)
                    {
                        Console.WriteLine("Waiting for a connection");
                        TcpClient client = listener.AcceptTcpClient();
                        Console.WriteLine("Client accepted");
                        Thread t = new Thread(ProcessClientRequests);
                        t.Start(client);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

        }
    }
}
