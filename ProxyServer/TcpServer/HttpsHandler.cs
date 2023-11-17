using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    internal class HttpsHandler
    {
        private static HttpClient sharedClient = new HttpClient();
        private CertificateHandler CertificateHandler = new CertificateHandler();


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
        public void HandleRequest(HttpListener listener)
        {
            throw new NotImplementedException();
        }
    }

}
