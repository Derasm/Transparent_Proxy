using SharpPcap;
using PacketDotNet;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

// THis will be a basic Man in the Middle proxy server that will forward traffic to the destination server without altering it.
public class ProxyServerProgram
{
    public static async Task Main()
    {
        int[] Ports = { 13, 80 }; // 80 for HTTP, 443 for HTTPS, 13 because test.

        IPAddress ipAddress = IPAddress.Any;
        int[] ports = new[] { 80, 13 }; // List of ports to listen on
        List<TcpListener> listeners = new List<TcpListener>();
        GetDevices();
        //foreach (int port in ports)
        //{
        //    TcpListener listener = new TcpListener(ipAddress, port);
        //    listener.Start();
        //    listeners.Add(listener);
        //    Console.WriteLine($"Proxy server listening on port {port}...");
        //}
        ////Set each listener to listen for actions. When a task has come in, send the task to the HandleClientAsync function.
        //while (true)
        //{
        //    Task<TcpClient>[] tasks = listeners.Select(listener => listener.AcceptTcpClientAsync()).ToArray();
        //    Task<TcpClient> completedTask = await Task.WhenAny(tasks);
        //    TcpClient client = await completedTask;
        //    _ = HandleClientAsync(client); // this disposes the returnvalue of whatever is done.
        //}
    }

    private static async Task HandleClientAsync(TcpClient tcPclient, string host, int port)
    {
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                HttpClient client = new HttpClient();
                //client.PostAsync(request);
                HttpWebRequest forwardRequest = (HttpWebRequest)WebRequest.Create(request.RawUrl);
                forwardRequest.Method = request.HttpMethod;
                HttpWebResponse forwardResponse = (HttpWebResponse)await forwardRequest.GetResponseAsync();

                response.StatusCode = (int)forwardResponse.StatusCode;
                using Stream receiveStream = forwardResponse.GetResponseStream();
                using Stream responseStream = response.OutputStream;
                await receiveStream.CopyToAsync(responseStream);
                response.Close();
            }
        }
    }

    private static async void GetDevices()
    {

        // Retrieve the device list
        CaptureDeviceList devices = CaptureDeviceList.Instance;

        // If no devices were found print an error
        if (devices.Count < 1)
        {
            Console.WriteLine("No devices were found on this machine");
            return;
        }

        Console.WriteLine("\nThe following devices are available on this machine:");
        Console.WriteLine("----------------------------------------------------\n");

        // Print out the available network devices
        foreach (ICaptureDevice dev in devices)
            Console.WriteLine("{0}\n", dev.ToString());

        Console.Write("Hit 'Enter' to exit...");
        Console.ReadLine();
    }
}