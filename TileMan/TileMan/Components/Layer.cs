using System;
using System.Windows;
using System.Windows.Media.Imaging;


namespace TileMan.Components
{
    public class Layer
    {
        public const int TYPE_NONE = -1;
        public const int TYPE_MAIN = 0;
        public const int TYPE_COLLISION = 1;
        public const int TYPE_OBJECTS = 2;
        public const int TYPE_HOTSPOTS = 3;
        public const int TYPE_VISUALS = 4;


        public string mName;
        public int mType;

        public int mTileWidth;
        public int mTileHeight;
        public string mTileMapFilename;
        public BitmapImage mTileMap;
        public System.Collections.Generic.List<CroppedBitmap> mTiles;
        public int mNbTiles;
        public int mCurrentTile;

        public LayerResource mResource;


        public Layer()
        {
            mName = "";
            mType = TYPE_NONE;

            mTileWidth = 32;
            mTileHeight = 32;
            mTileMapFilename = "";
            mTileMap = null;
//            mTiles = null;
            mTiles = new System.Collections.Generic.List<CroppedBitmap>();
            mNbTiles = 0;
            mCurrentTile = -1;

            mResource = new LayerResource();
        }

        public virtual void Reset( string name, int type, int nbTilesH, int nbTilesV, int tileWidth, int tileHeight )
        {
            mName = name;
            mType = type;

            mResource.Reset( name + "_resource", nbTilesH, nbTilesV );

            mTileWidth = tileWidth;
            mTileHeight = tileHeight;
            mTileMapFilename = "";
            mTileMap = null;
            mTiles.Clear();
            mNbTiles = 0;
            mCurrentTile = -1;
        }

        public void Copy( Layer originalLayer, string name, bool share )
        {
            mName = name;
            mType = originalLayer.mType;

            mTileWidth = originalLayer.mTileWidth;
            mTileHeight = originalLayer.mTileHeight;

            LoadTiles( originalLayer.mTileMapFilename );

            SetResource( originalLayer.mResource, share );
        }

        public void SetResource( LayerResource resource, bool share )
        {
            if ( share )
            {
                // keep same resource
                mResource = resource;
            }
            else
            {
                // make a copy
                mResource.Copy( resource );
            }

            // remove from map all indices that are out of range
            mResource.FixIndices( mNbTiles );
        }

        public virtual void LoadResource( string filename )
        {
            mResource.Load( filename );
        }

        public virtual void SaveResource( string filename )
        {
            mResource.Save( filename );
        }

        public void Resize( int nbTilesH, int nbTilesV, int horizOrient, int verticOrient )
        {
            mResource.Resize( nbTilesH, nbTilesV, horizOrient, verticOrient );
        }

        public void SetTilesSizes( int tileWidth, int tileHeight )
        {
            mTileWidth = tileWidth;
            mTileHeight = tileHeight;

            if ( !mTileMapFilename.Equals( "" ) )
            {
                UpdateTileMap();
            }

            // remove from map all indices that are out of range
            mResource.FixIndices( mNbTiles );
        }

        public void LoadTiles( string filename )
        {
            UnloadData();

            mTileMapFilename = "";
            mTileMap = null;
            mTiles.Clear();
            mNbTiles = 0;
            mCurrentTile = -1;

            if ( !filename.Equals( "" ) )
//            if ( !filename.Equals( "" ) && System.IO.File.Exists( filename ) )
            {
                mTileMapFilename = filename;

                LoadData();

                UpdateTileMap();
            }

            // remove from map all indices that are out of range
            mResource.FixIndices( mNbTiles );
        }

        protected virtual void UnloadData()
        {
// !!!! ???? TODO: need to close/free the resource ? ???? !!!!
            if ( mTileMap != null )
            {
                //...
            }
        }

        protected virtual void LoadData()
        {
            Uri uri = new Uri( mTileMapFilename, UriKind.Absolute );
            mTileMap = new BitmapImage( uri );
            uri = null;
        }

        public virtual void UpdateTileMap()
        {
            int maxWidth = mTileMap.PixelWidth - mTileWidth;
            int maxHeight = mTileMap.PixelHeight - mTileHeight;

            mTiles.Clear();
            mNbTiles = 0;
            mCurrentTile = -1;

            for ( int offsetY = 0; offsetY <= maxHeight; offsetY += mTileHeight )
            {
                for ( int offsetX = 0; offsetX <= maxWidth; offsetX += mTileWidth )
                {
                    mTiles.Add( new CroppedBitmap( mTileMap,
                                                   new Int32Rect( offsetX, offsetY, mTileWidth, mTileHeight ) ) );
                }
            }
            // only allow 256 tiles
            if ( mTiles.Count > 256 )
            {
                mTiles.RemoveRange( 256, mTiles.Count - 256 );
            }
            mNbTiles = mTiles.Count;
        }

        public virtual int SetTile( double x, double y )
        {
            int index = -1;
            if ( mCurrentTile != -1 )
            {
                int cellX = ( int ) x / mTileWidth;
                int cellY = ( int ) y / mTileHeight;

                if ( mResource.SetTile( cellX, cellY, ( byte )mCurrentTile ) )
                {
                    // get tile index for grid
                    index = cellY * mResource.mNbTilesH + cellX;
                }
            }
            return index;
        }

        public virtual int UnsetTile( double x, double y )
        {
            int index = -1;

            int cellX = ( int ) x / mTileWidth;
            int cellY = ( int ) y / mTileHeight;

            if ( mResource.UnsetTile( cellX, cellY ) )
            {
                // get tile index for grid
                index = cellY * mResource.mNbTilesH + cellX;
            }

            return index;
        }

    }
}
