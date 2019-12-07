using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using System.Windows.Media.Imaging;
using System.IO;


namespace TileMan.Components
{
    public class LayerObjects : Layer
    {
        public System.Collections.Generic.List<ObjectDescription> mObjects;


        public LayerObjects() : base()
        {
            mObjects = new System.Collections.Generic.List<ObjectDescription>();
        }

        public override void Reset( string name, int type, int nbTilesH, int nbTilesV, int tileWidth, int tileHeight )
        {
            base.Reset( name, type, nbTilesH, nbTilesV, tileWidth, tileHeight );

            mObjects.Clear();
        }

        public override void LoadResource( string filename )
        {
            // load resource
//            mResource.Load( filename );
            base.LoadResource( filename );

            // convert resource values
            int nbObjects = mObjects.Count;
            if ( nbObjects != 0 )
            {
                // create reverse convertion table
                Dictionary<byte, byte> converter = new Dictionary<byte, byte>();
                converter.Add( 255, 255 );
                for (int i = 0; i < nbObjects; ++i)
                {
                    byte id = mObjects[ i ].mId;

                    byte value;
                    if ( !converter.TryGetValue( id, out value ) )
                    {
                        converter.Add( id, (byte)i );
                    }
                }

                // convert values
                int nbTilesH = mResource.mNbTilesH;
                int nbTilesV = mResource.mNbTilesV;
                byte[,] tmpMap = mResource.mMap;
                List<byte> ignoredIndices = new List<byte>();

                for ( int i = 0; i < nbTilesH; ++i )
                {
                    for ( int j = 0; j < nbTilesV; ++j )
                    {
                        byte key = tmpMap[ i, j ];
                        byte value;
                        if ( !converter.TryGetValue( key, out value ) )
                        {
                            tmpMap[ i, j ] = 255;
                            if ( !ignoredIndices.Contains( key ) )
                            {
                                ignoredIndices.Add( key );
                            }
                        }
                        else
                        {
                            tmpMap[ i, j ] = value;
                        }
                    }
                }

                if ( ignoredIndices.Count != 0 )
                {
                    ignoredIndices.Sort();

                    string message = "Some indices were ignored as there are no corresponding hotspot object:";
                    for ( int i = 0; i < ignoredIndices.Count; ++i )
                    {
                        message += "\n - " + ignoredIndices[ i ];
                    }

                    MessageBox.Show( message, mName );
                }
            }
        }

        public override void SaveResource( string filename )
        {
            // convert resource values
            int nbTilesH = mResource.mNbTilesH;
            int nbTilesV = mResource.mNbTilesV;
            byte[,] tmpMap = new byte[ nbTilesH, nbTilesV ];
            for ( int i = 0; i < nbTilesH; ++i )
            {
                for ( int j = 0; j < nbTilesV; ++j )
                {
                    int value = mResource.mMap[ i, j ];
                    if ( value == 255 )
                    {
                        tmpMap[ i, j ] = 255;
                    }
                    else
                    {
                        tmpMap[ i, j ] = ( byte )mObjects[ value ].mId;
                    }
                }
            }
            byte[,] bkpMap = mResource.mMap;
            mResource.mMap = tmpMap;

            // save resource
//            mResource.Save( filename );
            base.SaveResource(filename);

            // recover resource values
            mResource.mMap = bkpMap;
            bkpMap = null;
            tmpMap = null;
        }

        protected override void UnloadData()
        {
            mObjects.Clear();
        }

        protected override void LoadData()
        {
            //load ODF file and all related icons/images

            string category = "";

// !!!! TODO: add a "try-catch" block !!!!
// => if error, display message box to tell XML file is not valid ...

            XmlReader xmlReader = XmlReader.Create( mTileMapFilename );
            while ( xmlReader.Read() )
            {
//MessageBox.Show("Name : " + xmlReader.Name);
                if ( ( xmlReader.NodeType == XmlNodeType.Element ) && ( xmlReader.Name == "Objects" ) )
                {
                    while ( xmlReader.Read() )
                    {
//MessageBox.Show("Name 2 : " + xmlReader.Name);
                        if ( xmlReader.NodeType == XmlNodeType.Element )
                        {
                            if ( xmlReader.Name == "Category" )
                            {
                                if ( xmlReader.HasAttributes )
                                {
                                    category = xmlReader.GetAttribute( "type" );
                                }
                            }
                            else if ( xmlReader.Name == "Object" )
                            {
                                if ( xmlReader.HasAttributes )
                                {
                                    // if icon doesn't exist => ignore object
                                    string iconPath = Settings.mResourcesPath + xmlReader.GetAttribute( "icon" );
                                    if ( File.Exists( iconPath ) )
                                    {
                                        byte id = Convert.ToByte(xmlReader.GetAttribute( "id" ) );
                                        string name = xmlReader.GetAttribute( "name" );
                                        string description = xmlReader.GetAttribute( "description" );

// !!!! ???? TODO: use relative or absolute paths ? ???? !!!!
                                        Uri uri = new Uri( iconPath, UriKind.Relative );
                                        mTileMap = new BitmapImage( uri );
                                        uri = null;

                                        CroppedBitmap icon = new CroppedBitmap( mTileMap, new Int32Rect( 0, 0, mTileMap.PixelWidth, mTileMap.PixelHeight ) );
//                                        mTiles.Add( icon );

                                        ObjectDescription objDesc = new ObjectDescription( id, name, icon, category, description );
                                        mObjects.Add( objDesc );
                                    }
                                }
                            }
                        }
                    }
                }

            }
            xmlReader.Close();

        }

        public override void UpdateTileMap()
        {
            mTiles.Clear();
            mNbTiles = 0;
            mCurrentTile = -1;

            for ( int i = 0; i < mObjects.Count; ++i )
            {
                mTiles.Add( mObjects[ i ].mIcon );
            }
// !!!! ???? TODO: OK ? ???? !!!!
// ( for an empty image ... )
            mTiles.Add( new CroppedBitmap() );

            // only allow 256 tiles
            if ( mTiles.Count > 256 )
            {
                mTiles.RemoveRange( 256, mTiles.Count - 256 );
            }
            mNbTiles = mTiles.Count;
        }

    }
}
