using System.Net.Sockets;
using System.Net;
using System.Text;
using TcpServer;

namespace TcpServer
{
    class Program
    {
        static HttpHandler HttpHandler = new HttpHandler();
        static HttpsHandler HttpsHandler = new HttpsHandler();
        /// <summary>
        /// Step one: make client and listener
        /// 2: listen for incoming requests on listener on port / Adress
        /// 3: Forward incoming request - requires getting a response from an endClient at the address the listener intercepted
        /// 4. return response from endClient to original client.
        /// 
        /// 
        /// </summary>

        public static async Task Main()
        {
            //listener that listens for incoming http calls on 127.0.0.1:5551
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:5551/");
            //start listening
            listener.Start();
            Console.WriteLine("Listening on port 5551 and url 127.0.0.1");
            while (true)
            {
                var context = listener.GetContext();
                //when a request comes in, pass it to HandleRequest
                await HandleRequest(listener);
            }
        }
        /// <summary>
        /// when an incoming request comes, handle it by figuring out where it wants to go, and then forward it to the correct server.
        /// </summary>
        /// <param name="listener"></param>
        static async Task<HttpResponseMessage> HandleRequest(HttpListener listener)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            HttpResponseMessage response = new HttpResponseMessage();
            //a connection is made
            Console.WriteLine("Connected on");
            Console.WriteLine(context.Request.Url);
            //figure out if it is http or https. If it is https, make a handshake with the endServer. else just forward it.
            if (request.IsSecureConnection)
            {
                //make handshake with endServer
                //SSLHandshake();
                HttpsHandler.HandleRequest(listener);
            }
            else
            {
                //forward request
                response = await HttpHandler.ForwardHttpCall(listener);
            }


            return response;
        }
    }
}

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

