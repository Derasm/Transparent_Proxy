using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    internal class HttpHandler
    {
        private static HttpClient sharedClient = new HttpClient();
        /// <summary>
        /// Takes a Listener, finds out the URL of the request, and forwards it to the correct server. 
        /// </summary>
        /// <returns></returns>
        Task<HttpResponseMessage> ForwardHttpCall(HttpListener listener)
        {
            //Get URL to call, then call that url, get response, return response.
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            var url = listener.GetContext().Request.Url;
            var response = sharedClient.GetAsync(url);
            responseMessage = response.Result;
            return Task.FromResult(responseMessage);
        }
    }
}
