using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

class Client()
{
    const string ServerAddress = "127.0.0.1";
    const int Port = 8888;
    const int ArraySize = 20000;
    const int TestRuns = 100;

    static void Main(string[] args)
    {
        double totalTime = 0;
        double avgTime = 0;
        Random rnd = new Random();

        for (int run = 0; run < TestRuns; run++)
        {
            int[] array = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                array[i] = rnd.Next();
            }

            TcpClient client = new TcpClient(ServerAddress, Port);
            NetworkStream stream = client.GetStream();

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                byte[] lengthBuffer = BitConverter.GetBytes(array.Length);
                stream.Write(lengthBuffer, 0, lengthBuffer.Length);

                byte[] buffer = new byte[array.Length * sizeof(int)];
                Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
                stream.Write(buffer, 0, buffer.Length);

                byte[] receivedBuffer = new byte[array.Length * sizeof(int)];
                stream.Read(receivedBuffer, 0, receivedBuffer.Length);

                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalMicroseconds;
            }
            finally
            {
                client.Close();
            }
        }

        avgTime = totalTime / TestRuns;

        Console.WriteLine($"Average time: {avgTime} ms.");
    }
}