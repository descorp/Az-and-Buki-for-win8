using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Networking.Connectivity;

namespace levelupspace 
{
    class HttpProvider
    {

        public static bool IsInternetConnection()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null);
        }

        public event EventHandler Complete;
        readonly string _boundary = "----------" + DateTime.Now.Ticks.ToString("x");

        public void SendMultiPartRerquest(string url, Stream postStream, string fileName)
        {
            var fileSendRequest = (HttpWebRequest)WebRequest.Create(url);
            fileSendRequest.ContentType = "multipart/form-data; boundary=" + _boundary ;
            fileSendRequest.Method = "POST";
            fileSendRequest.AllowReadStreamBuffering = false;
            try
            {
                // start the request
                fileSendRequest.BeginGetRequestStream(ar => GetRequestStreamCallback(fileName, postStream, ar, fileSendRequest), null);
            }
            catch
            {
                var res = new ResourceLoader();

                Logger.ShowMessage(res.GetString("ConnectionError"));
            }
        }

        /// <summary>
        /// Get request strean for sending large post data.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="postData"></param>
        /// <param name="asynchronousResult"></param>
        /// <param name="webRequest"></param>
        private void GetRequestStreamCallback(string fileName, Stream postData, IAsyncResult asynchronousResult, HttpWebRequest webRequest)
        {
            // end the stream request operation
            var postStream = webRequest.EndGetRequestStream(asynchronousResult);

            // the post message header
            var sb = new StringBuilder();

            // File
            sb.Append("--" + _boundary + "\r\n");
            sb.Append("Content-Disposition: form-data; name=\"photo\";");
            sb.Append("filename=\"" + fileName + "\"\r\n");
            sb.Append("Content-Type:application/octet-stream\r\n\r\n");

            //sb.Append("Content-Transfer-Encoding: binary\r\n\r\n");

            var strPostHeader = sb.ToString();

            var postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);

            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            int bytesRead;
            var blockSize = checked((int)Math.Min(4096, postData.Length));

            var buffer = new Byte[blockSize];
            postData.Position = 0;


            while ((bytesRead = postData.Read(buffer, 0, buffer.Length)) != 0)
            {
                postStream.Write(buffer, 0, bytesRead);
            }
            postData.Dispose();

            var finalyBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + _boundary + "--\r\n");

            postStream.Write(finalyBoundaryBytes, 0, finalyBoundaryBytes.Length);
            postStream.Dispose();

            //IAsyncResult asynchronousInputRead = postStream.BeginWrite(buffer, 0, blockSize, new AsyncCallback(ReadCallBack), myRequestState);
            // start the web request
            webRequest.BeginGetResponse(GetResponseCallback, webRequest);
        }

        /// <summary>
        /// Get response from server.
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            
            var eventArgs = new HttpRresponseEventArgs();
            try
            {
                var webRequest = (HttpWebRequest)asynchronousResult.AsyncState;

                // end the get response operation
                var response = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                var streamResponse = response.GetResponseStream();

                var streamReader = new StreamReader(streamResponse);
                var responseStr = streamReader.ReadToEnd();
                streamResponse.Dispose();
                streamReader.Dispose();
                response.Dispose();

                eventArgs.responseData = responseStr;


                if (Complete != null) Complete(this, eventArgs);
            }
            catch
            {
                /*eventArgs.code = 500;
                eventArgs.message = "Server error";*/
                if (Complete != null) Complete(this, eventArgs);
            }
        }


        public async Task<string> POSTrequest(Uri reuestURI)
        {
             var request = WebRequest.Create(reuestURI);
            request.Method = "POST";
            
            var Webresponse = await request.GetResponseAsync();
            if (Webresponse != null)
            {
                var responseStream = Webresponse.GetResponseStream();
                var responseReader = new StreamReader(responseStream, Encoding.UTF8);
                return responseReader.ReadToEnd();
                
            }
            return String.Empty;
        }
   }

            /// <summary>
        /// Response args from server.
        /// </summary>
        public class HttpRresponseEventArgs : EventArgs
        {
            public string responseData { get; set; }
        }
}
