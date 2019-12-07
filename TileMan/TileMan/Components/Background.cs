
namespace TileMan.Components
{
    public class Background
    {
        private string mResourceFullPath;
        private double mSizeFactorX;
        private double mSizeFactorY;

        private bool mUseImageRatio;
        private int mWidth;
        private int mHeight;
        private bool mRatioKeepWidth;


        public Background()
        {
            Reset();
        }

        public void Reset()
        {
            mUseImageRatio = false;
            mResourceFullPath = "";
            mSizeFactorX = 1.0;
            mSizeFactorY = 1.0;
            mWidth = 0;
            mHeight = 0;
            mRatioKeepWidth = true;
        }

        public void SetUseImageRatio( bool useImageRatio )
        {
            mUseImageRatio = useImageRatio;
        }
        public bool GetUseImageRatio()
        {
            return mUseImageRatio;
        }

        public void SetResource( string fullPath )
        {
            mResourceFullPath = fullPath;
        }
        public string GetResourceFullPath()
        {
            return mResourceFullPath;
        }

        public void SetSizeFactorX( double sizeFactorX )
        {
            mSizeFactorX = sizeFactorX;
        }
        public double GetSizeFactorX()
        {
            return mSizeFactorX;
        }

        public void SetSizeFactorY( double sizeFactorY )
        {
            mSizeFactorY = sizeFactorY;
        }
        public double GetSizeFactorY()
        {
            return mSizeFactorY;
        }

        public void SetWidth( int width )
        {
            mWidth = width;
        }
        public int GetWidth()
        {
            return mWidth;
        }

        public void SetHeight( int height )
        {
            mHeight = height;
        }
        public int GetHeight()
        {
            return mHeight;
        }

        public void SetRatioKeepWidth( bool keepWidth )
        {
            mRatioKeepWidth = keepWidth;
        }
        public bool GetRatioKeepWidth()
        {
            return mRatioKeepWidth;
        }
    }
}
