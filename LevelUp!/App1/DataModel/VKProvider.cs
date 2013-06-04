using System;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.ApplicationModel.Resources;

namespace levelupspace
{
    public class VKProvider : SocialProvider
    {
        private static int appId = 3664063;
        private static string Scope = "wall,messages,photos";
        private string accessToken = "";
        private string userId = "";
        private string message;

        private static string RedirectLink = "http://oauth.vk.com/blank.html";
        private static string GetWallUploadLink = "https://api.vkontakte.ru/method/photos.getWallUploadServer?access_token={0}";
        private static string SaveWallPhotoLink = "https://api.vkontakte.ru/method/photos.saveWallPhoto?access_token={0}&server={1}&photo={2}&hash={3}";
        private static string wallPostLink = "https://api.vkontakte.ru/method/wall.post?access_token={0}&attachments={1}&message={2}";
        private static string OAuthUrlLink = "http://oauth.vk.com/authorize?" +
            "client_id={0}" +
            "&scope={1}" +
            "&redirect_uri=" +
            "&display=touch" +
            "&response_type=token";

        public override event EventHandler SentEvent;

        public VKProvider()
        {
           _uri = new Uri(string.Format(OAuthUrlLink, appId, Scope), UriKind.Absolute);
        }

        public override bool URLParser(Uri URi)
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
            catch
            {
                var res = new ResourceLoader();

                Logger.ShowMessage(res.GetString("ConnectionError"));  
            }
            return true;
        }

        public override async void WallPost(String Message, String Picture)
        {
            message = Message;
            var UploadResponse = await GetWallUploadServer();
            UploadPhoto(UploadResponse, Picture);
        }

        private async Task<JSONGetWallUploadServerResponse> GetWallUploadServer()
        {
            HttpProvider provider = new HttpProvider();
            var response = await provider.POSTrequest(new Uri(string.Format(GetWallUploadLink, accessToken)));
            return JsonConvert.DeserializeObject<JSONGetWallUploadServerResponse>(response);
        }

        private async void UploadPhoto(JSONGetWallUploadServerResponse UploadServerResponse, String ImagePath)
        {
            var file = await StorageFile.GetFileFromPathAsync(ImagePath);
            var fileStream = await file.OpenStreamForReadAsync();

            HttpProvider httpProvider = new HttpProvider();

            httpProvider.Complete += new EventHandler(FileUpload_Complete);
            httpProvider.SendMultiPartRerquest(UploadServerResponse.response.upload_url, fileStream, file.Name);
        }

        // <summary>
        /// File-uploading is completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FileUpload_Complete(object sender, EventArgs e)
        {
            string jsonMessage = (e as HttpRresponseEventArgs).responseData;
            
            JSONUploadFileResponse UploadFileResponse = JsonConvert.DeserializeObject<JSONUploadFileResponse>(jsonMessage);

            string SaveWallPhotoUrl = string.Format(SaveWallPhotoLink, accessToken, UploadFileResponse.server, UploadFileResponse.photo, UploadFileResponse.hash);

            HttpProvider provider = new HttpProvider();
            var SaveWallPhotoResponse = await provider.POSTrequest(new Uri(SaveWallPhotoUrl));

            string str = SaveWallPhotoResponse.Replace("[", "").Replace("]", "");

            JSONSaveWallPhotoResponse SaveWallResponse = JsonConvert.DeserializeObject<JSONSaveWallPhotoResponse>(str);


            string WallPostUrl = string.Format(wallPostLink, accessToken, SaveWallResponse.response.id, message);
            var WallPostResponse = await provider.POSTrequest(new Uri(WallPostUrl));

            EventArgs eventArgs = new EventArgs();
            if (SentEvent != null) SentEvent(this, eventArgs);
        }
        
    }

    public class JSONGetWallUploadServerResponse
    {
        public JSONUploadServerResponse response { get; set; }
    }

    public class JSONUploadServerResponse
    {
        public string upload_url { get; set; }
    }

    public class JSONUploadFileResponse
    {
        public string server;
        public string photo;
        public string hash;
    }

    public class JSONSaveWallPhotoResponse
    {
        public WallPhotoInfo response { get; set; }
    }
    
    public class WallPhotoInfo
    {
        public string id { get; set; }
    }
}
