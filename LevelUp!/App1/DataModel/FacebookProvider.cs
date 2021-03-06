﻿using Facebook;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;

namespace levelupspace
{
    public class FacebookProvider : SocialProvider
    {
        private static readonly FacebookClient _fb = new FacebookClient();
        private const string appId = "337251873070006";
        private const string extendedPermissions = "user_about_me,read_stream,publish_stream,publish_actions";
        private string accessToken;

        public override event EventHandler SentEvent;
        

        public FacebookProvider()
        {
            var parameters = new Dictionary<string, object>();

            parameters["client_id"] = appId;
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
            parameters["display"] = "touch";

            // add the 'scope' only if we have extendedPermissions.
            if (!string.IsNullOrEmpty(extendedPermissions))
            {
                // A comma-delimited list of permissions
                parameters["scope"] = extendedPermissions;
            }

            _uri=_fb.GetLoginUrl(parameters);
        }


        public override bool URLParser(Uri URL)
        {
            FacebookOAuthResult oauthResult;
            if (!_fb.TryParseOAuthCallbackUrl(URL, out oauthResult))
                return false;

            if (oauthResult.IsSuccess)
            {
                accessToken = oauthResult.AccessToken;
                return true;
            }

            return false;
        }

        public override async void WallPost(String Message, String ImagePath)
        {
            var fb = new FacebookClient(accessToken);
            ImagePath = ImagePath.Replace('/','\\');
            var facebookUploader = new FacebookMediaObject { FileName = "funny-image.jpg", ContentType = "image/png" };
            
            var file = await StorageFile.GetFileFromPathAsync(ImagePath);
            var fileStream = await file.OpenStreamForReadAsync();

            var bytes = new Byte[fileStream.Length];
            fileStream.Position = 0;
            fileStream.Read(bytes, 0, (int)fileStream.Length);

            //imgStream.Close();

            facebookUploader.SetValue(bytes);

            var postInfo = new Dictionary<string, object> {{"message", Message}, {"image", facebookUploader}};
            fb.PostCompleted += FBPostSending_Completed;
            await fb.PostTaskAsync("/photos", postInfo);
        }

        private void FBPostSending_Completed(object sender, FacebookApiEventArgs e)
        {
            if (SentEvent != null)
            {
                SharePage.Current.SendingCompleted();
            }
        }

    }
}
