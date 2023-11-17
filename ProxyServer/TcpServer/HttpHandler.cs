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
        /// Takes a Listener, finds out the URL of the request, and forwards it to the correct server. Returns response from server.
        /// </summary>
        /// <returns></returns>
        public async void ForwardHttpCall(HttpListenerContext context)
        {
            await Console.Out.WriteLineAsync("Context of message is: ");
            await Console.Out.WriteLineAsync(context.ToString());
            //Get URL to call, then call that url, get response, return response.
            HttpRequestMessage forwardRequest = new HttpRequestMessage();
            var incomingRequest = context.Request;
            var incomingRequestURL = context.Request.Url;
            //var response = sharedClient.GetAsync(url);
            forwardRequest.Method = new HttpMethod(context.Request.HttpMethod);

            forwardRequest.RequestUri = ChangePortToDefaultHTTP(incomingRequest);

            // for each of the headers, add them to the request.
            foreach (var header in incomingRequest.Headers.AllKeys)
            {
                forwardRequest.Headers.TryAddWithoutValidation(header, incomingRequest.Headers[header]);
            }


            // if there is a body in the incomingRequest, copy it.
            if (incomingRequest.HasEntityBody)
            {
                using (Stream body = incomingRequest.InputStream)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        body.CopyTo(stream);
                        forwardRequest.Content = new ByteArrayContent(stream.ToArray());
                    }
                }
            }

            // Receive the response from the server and forward it back to the client
            await Console.Out.WriteLineAsync("Getting response from server");

            HttpResponseMessage responseFromServer = await sharedClient.SendAsync(forwardRequest);
            //HttpResponseMessage responseFromServer = sharedClient.GetAsync("http://httpbin.org/").Result;
            await Console.Out.WriteLineAsync(responseFromServer.ToString());


            //Copy content of responseFromServer to client.

            HttpListenerResponse responseToClient = context.Response;
            responseToClient.StatusCode = (int)responseFromServer.StatusCode;
            responseToClient.StatusDescription = responseFromServer.ReasonPhrase ?? "Error in StatusDescription";

            foreach (var header in responseFromServer.Headers)
            {
                responseToClient.Headers.Add(header.Key, header.Value.First());
            }

            //iterate over the response
            if (responseFromServer.Content != null)
            {
                byte[] responseBytes = await responseFromServer.Content.ReadAsByteArrayAsync();
                responseToClient.ContentLength64 = responseBytes.Length;
                await responseToClient.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
            // close the connection
            responseToClient.Close();
            //return responseToClient;
        }
        private Uri ChangePortToDefaultHTTP(HttpListenerRequest request)
        {
            //get the url from the request
            UriBuilder uriBuilder = new UriBuilder(request.Url);
            //change the port to 80
            uriBuilder.Port = 80;
            //return the new url
            return uriBuilder.Uri;
        }   
    }
}
