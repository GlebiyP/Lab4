using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Server
{
    const int Port = 8888;

    static void Main(string[] args)
    {
        TcpListener server = null;
        try
        {
            server = new TcpListener(IPAddress.Any, Port);
            server.Start();

            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Task.Run(() => HandleClient(client));
            }
        }
        finally
        {
            server?.Stop();
        }
    }

    private static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();

        try
        {
            byte[] lenghtBuffer = new byte[4];
            stream.Read(lenghtBuffer, 0, lenghtBuffer.Length);
            int length = BitConverter.ToInt32(lenghtBuffer, 0);

            byte[] buffer = new byte[length * sizeof(int)];
            stream.Read(buffer, 0, buffer.Length);

            int[] array = new int[length];
            Buffer.BlockCopy(buffer, 0, array, 0, buffer.Length);

            QuickSort(array, 0, array.Length - 1);

            Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
            stream.Write(buffer, 0, buffer.Length);
        }
        finally
        {
            client.Close();
        }
    }

    private static void QuickSort(int[] array, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(array, low, high);
            QuickSort(array, low, pivotIndex - 1);
            QuickSort(array, pivotIndex + 1, high);
        }
    }

    private static int Partition(int[] array, int low, int high)
    {
        int pivot = array[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (array[j] <= pivot)
            {
                i++;
                Swap(array, i, j);
            }
        }

        Swap(array, i + 1, high);
        return i + 1;
    }

    private static void Swap(int[] array, int i, int j)
    {
        int temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
}