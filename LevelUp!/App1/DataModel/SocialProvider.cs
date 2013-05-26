using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
