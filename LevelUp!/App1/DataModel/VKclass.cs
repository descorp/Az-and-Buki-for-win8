using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using Windows.Networking.BackgroundTransfer;
using System.Threading.Tasks;
using Windows.Storage;

namespace LevelUP
{
    public class VKProvider
    {
        private static int appId = 3664063;
        private static string Scope = "wall,messages,photos";
        private static string PKey = "8YVM4CrD0CHTaqx57gC5";
        private static int Accesskey = 8192;
        private string accessToken = "";
        private string userId = "";
        private getWallUploadServerResponse ServerUploadResponse;

        private static string RedirectLink = "http://oauth.vk.com/blank.html";
        private static string GetWallUploadLink = "https://api.vkontakte.ru/method/photos.getWallUploadServer?access_token={0}";
        private static string SaveWallPhotoLink = "https://api.vkontakte.ru/method/photos.saveWallPhoto?access_token={0}&server={1}&photo={2}&hash={3}";
        private static string wallPostLink = "https://api.vkontakte.ru/method/wall.post?access_token={0}&attachments={1}";
        private static string OAuthUrlLink = "http://oauth.vk.com/authorize?" +
            "client_id={0}" +
            "&scope={1}" +
            "&redirect_uri=" +
            "&display=touch" +
            "&response_type=token";

        public static Uri AuthorizationUri
        {
            get { return new Uri(string.Format(OAuthUrlLink, appId, Scope), UriKind.Absolute); }
        }

        public bool URLParser(Uri URi)
        {
            
            if (!URi.AbsoluteUri.StartsWith(RedirectLink)) return false;
            var paramPairs = System.Net.WebUtility.HtmlDecode(URi.Fragment).TrimStart('#').Split('&');
            try
            {
                foreach (var param in paramPairs)
                {

                    var paramkey = param.Split('=');
                    if (paramkey.Length == 2)
                    {
                        if (paramkey[0] == "access_token")
                            accessToken = paramkey[1];

                        else if (paramkey[0] == "user_id")
                        {
                            userId = paramkey[1];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.ShowMessage("Ошибка соединения!");  
            }
            return true;
        }

        public async void WallPost(String Message, String Picture)
        {
           // GetWallUploadServer();
            UploadPhoto(Picture);
        }

        private async void GetWallUploadServer()
        {
            WebRequest request = WebRequest.Create(new Uri(string.Format(GetWallUploadLink, accessToken)));
            request.Method = "POST";
            var Webresponse = await request.GetResponseAsync();
            if (Webresponse != null)
            {
                Stream responseStream = Webresponse.GetResponseStream();
                StreamReader responseReader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
                string responseString = responseReader.ReadToEnd();
                ServerUploadResponse = JsonConvert.DeserializeObject<getWallUploadServerResponse>(responseString);
            }
        }

        private async void UploadPhoto(String ImagePath)
        {
            WebRequest request = WebRequest.Create(new Uri(string.Format(GetWallUploadLink, accessToken)));
            request.Method = "POST";
            var Webresponse = await request.GetResponseAsync();
            if (Webresponse != null)
            {
                Stream responseStream = Webresponse.GetResponseStream();
                StreamReader responseReader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
                string responseString = responseReader.ReadToEnd();
                ServerUploadResponse = JsonConvert.DeserializeObject<getWallUploadServerResponse>(responseString);
            }

            BackgroundUploader uploader = new BackgroundUploader();
            var file = await StorageFile.GetFileFromPathAsync(ImagePath); 
            UploadOperation upload = uploader.CreateUpload(new Uri(ServerUploadResponse.response.upload_url), file);

            HandleUpload(upload);
        }


        private async void HandleUpload(UploadOperation upload)
        {
            try
            {                
                await upload.StartAsync();
                
                ResponseInformation response = upload.GetResponseInformation();                
            }
            catch (Exception ex)
            {
                Logger.ShowMessage("Ошибка соединения!");                
            }
        }

        
    }

    public class getWallUploadServerResponse
    {
        public UploadServerResponse response { get; set; }
    }

    public class UploadServerResponse
    {
        public string upload_url { get; set; }
    }
}
