using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace TileMapper
{
    public class Tile : INotifyPropertyChanged
    {
        public int Height
        {
            get { return mHeight; }
            set
            {
                if (mHeight != value)
                {
                    mHeight = value;
                    RaisePropertyChanged("Height");
                }
            }
        }

        public int Width
        {
            get { return mWidth; }
            set
            {
                if (mWidth != value)
                {
                    mWidth = value;
                    RaisePropertyChanged("Width");
                }
            }
        }

        public string TileType
        {
            get { return mTileType; }
            set
            {
                if (mTileType != value)
                {
                    mTileType = value;
                    RaisePropertyChanged("TileType");
                }
            }
        }

        public CroppedBitmap CroppedBitmapImage
        {
            get { return mCroppedBitmapImage; }
            set
            {
                if (mCroppedBitmapImage != value)
                {
                    mCroppedBitmapImage = value;
                    RaisePropertyChanged("CroppedBitmapImage");
                }
            }
        }

        private int mHeight;
        private int mWidth;
        private string mTileType;
        private CroppedBitmap mCroppedBitmapImage;

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
