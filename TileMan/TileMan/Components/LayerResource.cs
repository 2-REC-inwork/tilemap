using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;


namespace TileMan.Components
{
    public class LayerResource
    {
        public const int ORIENTATION_LEFT = -1;
        public const int ORIENTATION_CENTRE = 0;
        public const int ORIENTATION_RIGHT = 1;

        private static int mCopyCount = 0;

        public string mFileName;

        public int mNbTilesH;
        public int mNbTilesV;
        public byte[,] mMap;


        public LayerResource()
        {
            mFileName = "";

            mNbTilesH = 0;
            mNbTilesV = 0;
            mMap = null;
        }

        public virtual void Reset( string name, int nbTilesH, int nbTilesV )
        {
            mFileName = name;

            mNbTilesH = nbTilesH;
            mNbTilesV = nbTilesV;
            mMap = new byte[ mNbTilesH, mNbTilesV ];
            Clear();
        }

        public void Load( string filename )
        {
            FileStream fileStream = new FileStream( filename, FileMode.Open, FileAccess.Read );
            BinaryReader binaryReader = new BinaryReader( fileStream );

            // signature ( 1 byte - hex ) => must be "42"
            byte signature = binaryReader.ReadByte();
            if ( signature != 42 )
            {
                MessageBox.Show( "Invalid file!" );
            }
            else
            {
                mFileName = filename.Remove( 0, filename.LastIndexOf( "\\" ) + 1 ); // remove path
                mFileName = mFileName.Remove( mFileName.LastIndexOf(".") ); // remove extension

                // width ( 4 bytes - int )
                mNbTilesH = binaryReader.ReadInt32();

                // height ( 4 bytes - int )
                mNbTilesV = binaryReader.ReadInt32();

                mMap = new byte[ mNbTilesH, mNbTilesV ];

                // map ( width * height bytes - hex
                for ( int j = 0; j < mNbTilesV; ++j )
                {
                    for ( int i = 0; i < mNbTilesH; ++i )
                    {
                        mMap[ i, j ] = binaryReader.ReadByte();
                    }
                }
            }

            binaryReader.Close();
            fileStream.Close();
        }

        public void Save( string filename )
        {
            FileStream fileStream = new FileStream( filename, FileMode.Create, FileAccess.Write );
            BinaryWriter binaryWriter = new BinaryWriter( fileStream );

            mFileName = filename.Remove( 0, filename.LastIndexOf( "\\" ) + 1 ); // remove path
            mFileName = mFileName.Remove( mFileName.LastIndexOf( "." ) ); // remove extension

            // signature ( 1 byte - hex ) => must be "42"
            byte signature = 42;
            binaryWriter.Write( signature );

            // width ( 4 bytes - int )
            binaryWriter.Write( mNbTilesH );

            // height ( 4 bytes - int )
            binaryWriter.Write( mNbTilesV );

            // map ( width * height bytes - hex
            for (int j = 0; j < mNbTilesV; ++j)
            {
                for (int i = 0; i < mNbTilesH; ++i)
                {
                    binaryWriter.Write( mMap[ i, j ] );
                }
            }

            binaryWriter.Close();
            fileStream.Close();
        }

        public void Clear()
        {
            for ( int i = 0; i < mNbTilesH; ++i )
            {
                for ( int j = 0; j < mNbTilesV; ++j )
                {
                    mMap[ i, j ] = 255;
                }
            }
        }

        public void Copy( LayerResource resource )
        {
            mFileName = resource.mFileName + "_copy" + mCopyCount.ToString();
            ++mCopyCount;

            mNbTilesH = resource.mNbTilesH;
            mNbTilesV = resource.mNbTilesV;

            mMap = new byte[ mNbTilesH, mNbTilesV ];
            for ( int i = 0; i < mNbTilesH; ++i )
            {
                for ( int j = 0; j < mNbTilesV; ++j )
                {
                    mMap[ i, j ] = resource.mMap[ i, j ];
                }
            }
        }

        public void Resize( int nbTilesH, int nbTilesV, int horizOrient, int verticOrient )
        {
            if ( ( nbTilesH == mNbTilesH ) && ( nbTilesV == mNbTilesV ) )
            {
                return;
            }

            if ( mMap != null )
            {
                // get horizontal offsets
                int srcOffsetH = 0;
                int dstOffsetH = 0;
                GetOffsets( mNbTilesH, nbTilesH, horizOrient, out srcOffsetH, out dstOffsetH );

                // get vertical offsets
                int srcOffsetV = 0;
                int dstOffsetV = 0;
                GetOffsets( mNbTilesV, nbTilesV, verticOrient, out srcOffsetV, out dstOffsetV );

                // get sizes to copy
                int smallH = Math.Min( mNbTilesH, nbTilesH );
                int smallV = Math.Min( mNbTilesV, nbTilesV );

                // keep old map and create new one
                mNbTilesH = nbTilesH;
                mNbTilesV = nbTilesV;
                byte[,] tmpMap = mMap;
                mMap = new byte[ mNbTilesH, mNbTilesV ];
                Clear();

                // copy old map into new map
                for ( int i = 0; i < smallH; ++i )
                {
                    for ( int j = 0; j < smallV; ++j )
                    {
                        mMap[ dstOffsetH + i, dstOffsetV + j ] = tmpMap[ srcOffsetH + i, srcOffsetV + j ];
                    }
                }
            }
            else
            {
                mNbTilesH = nbTilesH;
                mNbTilesV = nbTilesV;
            }
        }

        public void FixIndices( int nbValues )
        {
            for ( int i = 0; i < mNbTilesH; ++i )
            {
                for ( int j = 0; j < mNbTilesV; ++j )
                {
                    if ( mMap[ i, j ] >= nbValues )
                    {
                        mMap[ i, j ] = 255;
                    }
                }
            }
        }

        private void GetOffsets( int oldSize, int newSize, int orientation, out int srcOffset, out int dstOffset )
        {
            srcOffset = 0;
            dstOffset = 0;

            int diffH = newSize - oldSize;
            if ( diffH > 0 ) // new bigger width
            {
                //srcOffsetH = 0;
                if ( orientation == ORIENTATION_CENTRE )
                {
                    dstOffset = diffH / 2;
                }
                else if ( orientation == ORIENTATION_RIGHT )
                {
                    dstOffset = diffH;
                }
            }
            else if ( diffH < 0 ) // new smaller width
            {
                //dstOffsetH = 0;
                diffH = -diffH;
                if ( orientation == ORIENTATION_CENTRE )
                {
                    srcOffset = diffH / 2;
                }
                else if ( orientation == ORIENTATION_RIGHT )
                {
                    srcOffset = diffH;
                }
            }
        }

        public bool SetTile( int cellX, int cellY, byte tile )
        {
            bool ok = false;
            if ( ( cellX < mNbTilesH ) && ( cellY < mNbTilesV ) )
            {
                // set tile in map
                mMap[ cellX, cellY ] = tile;
                ok = true;
            }
            return ok;
        }

        public bool UnsetTile( int cellX, int cellY )
        {
            bool ok = false;
            if ( ( cellX < mNbTilesH ) && ( cellY < mNbTilesV ) )
            {
                // set tile in map
                mMap[ cellX, cellY ] = 255;
                ok = true;
            }
            return ok;
        }

    }
}
