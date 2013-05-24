using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace levelupspace 
{
    class HttpProvider
    {

        public static bool IsInternetConnection()
        {
            var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null);
        }

        public event EventHandler Complete;
        string _boundary = "----------" + DateTime.Now.Ticks.ToString("x");

        public void SendMultiPartRerquest(string url, Stream postStream, string fileName)
        {
            HttpWebRequest FileSendRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            FileSendRequest.ContentType = "multipart/form-data; boundary=" + _boundary ;
            FileSendRequest.Method = "POST";
            FileSendRequest.AllowReadStreamBuffering = false;
            try
            {
                // start the request
                FileSendRequest.BeginGetRequestStream(ar =>
                {
                    GetRequestStreamCallback(fileName, postStream, ar, FileSendRequest);
                }, null);
            }
            catch (Exception ex)
            {
                Logger.ShowMessage("Ошибка соединения с сервером");
            }
        }

        /// <summary>
        /// Get request strean for sending large post data.
        /// </summary>
        /// <param name="postData"></param>
        /// <param name="asynchronousResult"></param>
        /// <param name="webRequest"></param>
        private void GetRequestStreamCallback(string fileName, Stream postData, IAsyncResult asynchronousResult, HttpWebRequest webRequest)
        {
            // end the stream request operation
            Stream postStream = webRequest.EndGetRequestStream(asynchronousResult);

            // the post message header
            StringBuilder sb = new StringBuilder();

            // File
            sb.Append("--" + _boundary + "\r\n");
            sb.Append("Content-Disposition: form-data; name=\"photo\";");
            sb.Append("filename=\"" + fileName + "\"\r\n");
            sb.Append("Content-Type:application/octet-stream\r\n\r\n");

            //sb.Append("Content-Transfer-Encoding: binary\r\n\r\n");

            string strPostHeader = sb.ToString();

            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);

            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            int bytesRead = 0;
            int blockSize = checked((int)System.Math.Min(4096, (long)postData.Length));

            byte[] buffer = new Byte[blockSize];
            postData.Position = 0;


            while ((bytesRead = postData.Read(buffer, 0, buffer.Length)) != 0)
            {
                postStream.Write(buffer, 0, bytesRead);
            }
            postData.Dispose();

            byte[] finalyBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + _boundary + "--\r\n");

            postStream.Write(finalyBoundaryBytes, 0, finalyBoundaryBytes.Length);
            postStream.Dispose();

            //IAsyncResult asynchronousInputRead = postStream.BeginWrite(buffer, 0, blockSize, new AsyncCallback(ReadCallBack), myRequestState);
            // start the web request
            webRequest.BeginGetResponse(new AsyncCallback(GetResponseCallback), webRequest);
        }

        /// <summary>
        /// Get response from server.
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            
            HttpRresponseEventArgs eventArgs = new HttpRresponseEventArgs();
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;

                HttpWebResponse response;

                // end the get response operation
                response = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                Stream streamResponse = response.GetResponseStream();

                StreamReader streamReader = new StreamReader(streamResponse);
                string responseStr = streamReader.ReadToEnd();
                streamResponse.Dispose();
                streamReader.Dispose();
                response.Dispose();

                eventArgs.responseData = responseStr;


                if (Complete != null) Complete(this, eventArgs);
            }
            catch (WebException e)
            {
                /*eventArgs.code = 500;
                eventArgs.message = "Server error";*/
                if (Complete != null) Complete(this, eventArgs);
            }
        }


        public async Task<string> POSTrequest(Uri reuestURI)
        {
             WebRequest request = WebRequest.Create(reuestURI);
            request.Method = "POST";
            var Webresponse = await request.GetResponseAsync();
            if (Webresponse != null)
            {
                Stream responseStream = Webresponse.GetResponseStream();
                StreamReader responseReader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
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
