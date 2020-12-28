using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace MessageClient
{
    class Program
    {
        static void Main(string[] args)
        {
            bool check = false;
            var message = "";
            var repetition = "";

            do
            {
                Console.WriteLine("\nВведите сообщение (не более 20 символов)");
                message = Console.ReadLine();
                if (message.Length > 20)
                    {
                    Console.WriteLine("В сообщении больше 20 символов!");
                }
                else
                {
                    check = true;
                }
            }
            while (check == false);

            check = false;
            do
            {
                Console.WriteLine("\nВведите количество повторений (не более 100)");
                repetition = Console.ReadLine();
                if (int.Parse(repetition) > 100)
                {
                    Console.WriteLine("Повторений должно быть не больше 100!");
                }
                else
                {
                    check = true;
                }
            }
            while (check == false);
            
        connection:
            try
            {
                int count_repetition = int.Parse(repetition);

                for (int i = 0; i < count_repetition; i++)
                {
                    TcpClient client = new TcpClient("127.0.0.1", 1302);
                    string messageToSend = message;
                    
                    int byteCount = Encoding.ASCII.GetByteCount(messageToSend + 1);
                    byte[] sendData = new byte[byteCount];
                    sendData = Encoding.ASCII.GetBytes(messageToSend);
                    NetworkStream stream = client.GetStream();
                    stream.Write(sendData, 0, sendData.Length);
                    Console.WriteLine("sending data to server...");

                    StreamReader sr = new StreamReader(stream);
                    string response = sr.ReadLine();
                    Console.WriteLine(response);

                    stream.Close();
                    client.Close();
                }

            }
            catch(Exception e)
            {
                Console.WriteLine("failed to connect...");
                goto connection;
            }


        }
    }
}
