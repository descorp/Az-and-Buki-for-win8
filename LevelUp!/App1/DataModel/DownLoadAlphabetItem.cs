using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Xaml;

namespace levelupspace.DataModel
{
    public class DownLoadAlphabetItem : AlphabetItem, INotifyPropertyChanged
    {
        public DownLoadAlphabetItem(String uniqueId, String title, String imagePath, String description, long ID, long Length, bool IsSystem)
            : base(uniqueId, title, imagePath, description, ID)
        {
            _downloadVisible = Visibility.Collapsed;
            DownLoadProgressMax = Length;
            this.IsSystem = IsSystem;
        }

        private DownloadOperation _downloadOperation;

        public void SetDownloadOperation(DownloadOperation operation)
        {
            _downloadOperation = operation;
            var progress = new Progress<DownloadOperation>(ProgressCallback);
        }

        private void ProgressCallback(DownloadOperation obj)
        {
            var progress
                = ((double)obj.Progress.BytesReceived / obj.Progress.TotalBytesToReceive);
           
            _LoadPos = (long)(progress * 100);
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public String SizeInBytes
        {
            get
            {
                var offsetInKBytes = (_LoadMax / 1024).ToString() + "KB ";
                if (_LoadMax > 1024 * 1024)
                    return ((double)_LoadMax / 1024 / 1024).ToString("F1") + "MB ";
                return offsetInKBytes;
            }
        }

        private string _packageFileName;
        public string PackageFileName
        {
            get { return _packageFileName; }
            set { _packageFileName = value; }
        }

        private bool _isSystem;
        public bool IsSystem
        {
            get { return _isSystem; }
            set { _isSystem = value; }
        }

        private Visibility _downloadVisible;
        public Visibility DownLoadProcessVisible
        {
            get { return _downloadVisible; }
            set
            {
                _downloadVisible = value;
                NotifyPropertyChanged();
            }
        }

        private String _downloadStat;
        public String DownloadStatus
        {
            get { return _downloadStat; }
            set
            {
                _downloadStat = value;
                NotifyPropertyChanged();
            }
        }

        private long _LoadPos;
        public long DownLoadProgessPos
        {
            get { return _LoadPos; }
            set
            {
                _LoadPos = value;
                NotifyPropertyChanged();
            }
        }

        private long _LoadMax;
        public long DownLoadProgressMax
        {
            get { return _LoadMax; }
            set
            {
                _LoadMax = value;
                NotifyPropertyChanged();
            }
        }
    }
}