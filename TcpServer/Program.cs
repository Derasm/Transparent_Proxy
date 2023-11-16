//using System.Net.Sockets;
//using System.Net;
//using System.Text;

//class Program { 
//    public static void Main() {
//        Program program = new Program();
//        program.StartAsync(5551).Wait();
//    }
//    public async Task StartAsync(int port)
//    {
//        var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
//        listener.Start();
//        Console.WriteLine("Server started on port: " + port);

//        while (true)
//        {
//            var clientTask = listener.AcceptTcpClientAsync(); // Get the client

//            if (clientTask.Result != null)
//            {
//                Console.WriteLine("New connection established.");
//                TcpClient client = clientTask.Result;
//                await HandleClientAsync(client);
//            }
//        }
//    }
//    ///At this point we've gotten the request, and now need to forward it to the receiving server
//    private async Task HandleClientAsync(TcpClient client)
//    {
//        var stream = client.GetStream();
//        var buffer = new byte[1024];
//        while (true)
//        {
//            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
//            if (bytesRead == 0) break; // Connection closed by the client
//            var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
//            Console.WriteLine("Received a request: " + request);

//            var response = await ForwardRequestAsync(request);
//            var responseBytes = Encoding.UTF8.GetBytes(response);
//            await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
//        }
//        client.Close();
//    }

//    private async Task<string> ForwardRequestAsync(string request)
//    {
//        // Here you can implement the logic to forward the request to the destination server
//        // and return its response.
//        // For simplicity, in this example, we just echo back the same request.
//        return await Task.FromResult(request);
//    }
//}
