using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace TcpServer
{
    internal class HttpProgram
    {
        private static HttpClient sharedClient = new HttpClient();
        private CertificateHandler CertificateHandler = new CertificateHandler();
        /// <summary>
        /// Step one: make client and listener
        /// 2: listen for incoming requests on listener on port / Adress
        /// 3: Forward incoming request - requires getting a response from an endClient at the address the listener intercepted
        /// 4. return response from endClient to original client.
        /// 
        /// 
        /// </summary>

        public async static void Main()
        {            
            HttpProgram program = new HttpProgram();
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
               await program.HandleRequest(listener);
            }
        }
        /// <summary>
        /// when an incoming request comes, handle it by figuring out where it wants to go, and then forward it to the correct server.
        /// </summary>
        /// <param name="listener"></param>
        async Task<HttpRequestMessage> HandleRequest(HttpListener listener)
        {
            Console.Out.WriteLine("Entered HandleRequest");
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            //figure out if it is http or https. If it is https, make a handshake with the endServer. else just forward it.
            if (request.IsSecureConnection)
            {
                //make handshake with endServer
                //SSLHandshake();
            }
            else
            {
                //forward request
                await ForwardRequest(request, context);
            }

            HttpRequestMessage destinationRequest = new HttpRequestMessage(new HttpMethod(request.HttpMethod), request.Url.ToString());
            foreach (string header in request.Headers)
            {
                destinationRequest.Headers.Add(header, request.Headers[header]);
            }
            Console.WriteLine("Connected on");
            Console.WriteLine(context.Request.Url);
            var forwardedResponse = await ForwardRequest(destinationRequest);
            return forwardedResponse;
        }

        /// <summary>
        /// Forward the Request by the client to the endServer, and return the response from the endServer to the original client.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        async Task<HttpRequestMessage> ForwardRequest(HttpRequestMessage httpRequest)
        {
            var response = await sharedClient.SendAsync(httpRequest);
            await Console.Out.WriteLineAsync("Response from server: ");

            await Console.Out.WriteLineAsync(response.Content.ToString());
            return response.RequestMessage;
        }
        /// <summary>
        /// This happens when an async call is made - this is used for the SSL portion when that gets implemented. 
        /// This will make a handshake with the endServer, which will then be used to forward the request to the endServer. From there the response will be returned to the original client.
        /// </summary>
        /// <param name="result"></param>
        public void SSLHandshake(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            // Call EndGetContext to complete the asynchronous operation.
            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // make handshake with the server using the proxy server's own root certificate. 
            X509Certificate2 certificate = CertificateHandler.Certificate;

        }

    }
}
