using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.ApplicationModel.Resources;

namespace levelupspace
{
    public class VKProvider : SocialProvider
    {
        private const int appId = 3664063;
        private const string Scope = "wall,messages,photos";
        private string _accessToken = "";
        private string _message;

        private const string RedirectLink = "http://oauth.vk.com/blank.html";
        private const string GetWallUploadLink = "https://api.vkontakte.ru/method/photos.getWallUploadServer?access_token={0}";
        private const string SaveWallPhotoLink = "https://api.vkontakte.ru/method/photos.saveWallPhoto?access_token={0}&server={1}&photo={2}&hash={3}";
        private const string WallPostLink = "https://api.vkontakte.ru/method/wall.post?access_token={0}&attachments={1}&message={2}";

        private const string OAuthUrlLink = "http://oauth.vk.com/authorize?" + "client_id={0}" + "&scope={1}" + "&redirect_uri=" + "&display=touch" + "&response_type=token";

        public override event EventHandler SentEvent;

        public VKProvider()
        {
           _uri = new Uri(string.Format(OAuthUrlLink, appId, Scope), UriKind.Absolute);
        }

        public override bool URLParser(Uri URi)
        {
            
            if (!URi.AbsoluteUri.StartsWith(RedirectLink)) return false;
            var paramPairs = WebUtility.HtmlDecode(URi.Fragment).TrimStart('#').Split('&');
            try
            {
                foreach (var param in paramPairs)
                {

                    var paramkey = param.Split('=');
                    if (paramkey.Length == 2)
                    {
                        if (paramkey[0] == "access_token")
                            _accessToken = paramkey[1];

                        else if (paramkey[0] == "user_id")
                        {
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
            _message = Message;
            try
            {
                var uploadServerResponse=await GetWallUploadServer();
                UploadPhoto(uploadServerResponse, Picture);
            }
            catch (Exception)
            { }
        }

        private async Task<JSONGetWallUploadServerResponse> GetWallUploadServer()
        {
            var provider = new HttpProvider();
            var response = await provider.POSTrequest(new Uri(string.Format(GetWallUploadLink, _accessToken)));
            return JsonConvert.DeserializeObject<JSONGetWallUploadServerResponse>(response);
        }

        private async void UploadPhoto(JSONGetWallUploadServerResponse UploadServerResponse, String ImagePath)
        {
            var file = await StorageFile.GetFileFromPathAsync(ImagePath);
            var fileStream = await file.OpenStreamForReadAsync();

            var httpProvider = new HttpProvider();

            httpProvider.Complete += FileUpload_Complete;
            httpProvider.SendMultiPartRerquest(UploadServerResponse.response.upload_url, fileStream, file.Name);
        }

        // <summary>
        /// File-uploading is completed.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FileUpload_Complete(object sender, EventArgs e)
        {
            var httpRresponseEventArgs = e as HttpRresponseEventArgs;
            if (httpRresponseEventArgs != null)
            {
                var jsonMessage = httpRresponseEventArgs.responseData;
            
                var uploadFileResponse = JsonConvert.DeserializeObject<JSONUploadFileResponse>(jsonMessage);

                var saveWallPhotoUrl = string.Format(SaveWallPhotoLink, _accessToken, uploadFileResponse.server, uploadFileResponse.photo, uploadFileResponse.hash);

                var provider = new HttpProvider();
                var saveWallPhotoResponse = await provider.POSTrequest(new Uri(saveWallPhotoUrl));

                var str = saveWallPhotoResponse.Replace("[", "").Replace("]", "");

                var saveWallResponse = JsonConvert.DeserializeObject<JSONSaveWallPhotoResponse>(str);


                var wallPostUrl = string.Format(WallPostLink, _accessToken, saveWallResponse.response.id, _message);
                await provider.POSTrequest(new Uri(wallPostUrl));
            }

            var eventArgs = new EventArgs();
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
