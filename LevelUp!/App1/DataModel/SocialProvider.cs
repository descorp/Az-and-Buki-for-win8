using System;

namespace levelupspace
{
    public abstract class SocialProvider
    {
        public virtual event EventHandler SentEvent;

        protected Uri _uri;
        public Uri LoginUri
        { get { return _uri; } }

        public abstract void WallPost(String ImagePath, String Message);

        public abstract bool URLParser(Uri URL);

    }
}
