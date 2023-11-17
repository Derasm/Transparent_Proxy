using System.Net.Sockets;
using System.Net;
using System.Text;
using TcpServer;
using System.Security.Cryptography.X509Certificates;

namespace TcpServer
{
    class Program
    {
        static HttpHandler httpHandler = new HttpHandler();
        static HttpsHandler httpsHandler = new HttpsHandler();
        static HttpListener listener = new HttpListener();
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
            string[] prefixes = { "https://127.0.0.1:5552/", "http://127.0.0.1:5551/" };
            
            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
                await Console.Out.WriteLineAsync($"listening on {prefix}");
            }
            // get a certificate. This is used for the SSL handshake and bound using netsh command instead of here in the code. 
            CertificateHandler certificateHandler = new CertificateHandler();
            //X509Certificate2 certificate = certificateHandler.Certificate;
            //start listening
            listener.Start();
            // program loop
            while (true)
            {
                var context = listener.GetContext();
                //when a request comes in, pass it to HandleRequest
                //listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);
                HandleRequest(listener);
            }
        }
        /// <summary>
        /// when an incoming request comes, handle it by figuring out where it wants to go, and then forward it to the correct server.
        /// </summary>
        /// <param name="listener"></param>
        static async void HandleRequest(HttpListener listener)
        {
            HttpListenerContext listenerContext = await listener.GetContextAsync();
            HttpListenerRequest request = listenerContext.Request;
            HttpResponseMessage response = new HttpResponseMessage();
            //a connection is made
            await Console.Out.WriteLineAsync("Connected on");
            await Console.Out.WriteLineAsync(listenerContext.Request.Url.ToString());
            //figure out if it is http or https. If it is https, make a handshake with the endServer. else just forward it.
            if (request.IsSecureConnection)
            {
                //make handshake with endServer
                //SSLHandshake();
                httpsHandler.HandleRequest(listener);
            }
            else
            {
                //forward request
                await Console.Out.WriteLineAsync("Http request received");
                httpHandler.ForwardHttpCall(listenerContext);
            }
            //listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);
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

