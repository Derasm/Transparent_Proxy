using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    internal class HttpHandler : IDisposable
    {
        private static HttpClient sharedClient = new HttpClient();

        public void Dispose()
        {
            sharedClient.Dispose();
        }

        /// <summary>
        /// Takes a Listener, finds out the URL of the request, and forwards it to the correct server. 
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ForwardHttpCall(HttpListener listener)
        {
            //Get URL to call, then call that url, get response, return response.
            var context = listener.GetContext();
            HttpRequestMessage forwardRequest = new HttpRequestMessage();
            var incomingRequest = listener.GetContext().Request;
            var url = listener.GetContext().Request.Url;
            //var response = sharedClient.GetAsync(url);
            forwardRequest.Method = new HttpMethod(listener.GetContext().Request.HttpMethod);
            // for each of the headers, add them to the request.
            foreach (var header in incomingRequest.Headers.AllKeys)
            {
                forwardRequest.Headers.TryAddWithoutValidation(header, incomingRequest.Headers[header]);
            }
            // if there is a body in the incomingRequest
            if (incomingRequest.HasEntityBody)
            {
                using (Stream body = incomingRequest.InputStream)  // here we have data
                {
                    using (var reader = new StreamReader(body, incomingRequest.ContentEncoding))
                    {
                        forwardRequest.Content = new StringContent(reader.ReadToEnd());
                    }
                }
            }

            // Receive the response from the server and forward it back to the client
            HttpResponseMessage responseFromServer = await sharedClient.SendAsync(forwardRequest);
            HttpListenerResponse responseToClient = context.Response;
            //iterate over the response
            if (responseFromServer.Content != null)
            {
                byte[] responseBytes = await responseFromServer.Content.ReadAsByteArrayAsync();
                responseToClient.ContentLength64 = responseBytes.Length;
                await responseToClient.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
            // close the connection
            responseToClient.Close();
            return responseFromServer;
        }
    }
}
