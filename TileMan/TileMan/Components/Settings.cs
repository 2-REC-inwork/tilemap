using System;
using System.Windows;
using System.IO;
using System.Xml;

namespace TileMan.Components
{
    public static class Settings
    {
        private const string SETTINGS_FILE_NAME = "settings.xml";


        private const string DEFAULT_PROJECT_NAME = "untitled";

        private const int NB_TILES_H = 32;
        private const int NB_TILES_V = 16;
        private const int TILE_WIDTH = 32;
        private const int TILE_HEIGHT = 32;

//?        private const string TILEMAP_BKG_COLOR = "Grey";
//?        private const int TILEMAP_TILES_SIZE = 32;

        private const string COLLISION_TILES_FILENAME = "collision.png";
        private const string OBJECTS_DESCRIPTION_FILENAME = "objects.xml";
        private const string HOTSPOTS_DESCRIPTION_FILENAME = "hotspots.xml";


        private static string mApplicationPath;

        public static string mProjectName { get; set; }
        public static int mNbTilesH { get; set; }
        public static int mNbTilesV { get; set; }
        public static int mTileWidth { get; set; }
        public static int mTileHeight { get; set; }
        public static string mResourcesPath { get; set; }
        public static string mCollisionTilesFilename { get; set; }
        public static string mObjectsDescriptionFilename { get; set; }
        public static string mHotspotsDescriptionFilename { get; set; }


        static Settings()
        {
// !!!! TODO: should use a "%APPDATA%\<app>" directory !!!!
// ( directory created when installing app )
            mApplicationPath = System.IO.Directory.GetCurrentDirectory() + "\\";

            SetDefaultValues();
        }

        // If a config file exists, read values from it. If not, set default values.
        public static void Load()
        {
            // check if a settings file exists in current directory
            if ( !System.IO.File.Exists( mApplicationPath + SETTINGS_FILE_NAME ) )
            {
                SetDefaultValues();
            }
            else
            {
                FileStream fileSteam = File.OpenRead( mApplicationPath + SETTINGS_FILE_NAME );

                XmlReaderSettings settings;
                settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Document;

                XmlReader reader = XmlReader.Create( fileSteam, settings );

                while ( reader.Read() )
                {
                    if ( reader.IsStartElement() && ( reader.Name == "settings" ) )
                    {
                        while ( reader.MoveToNextAttribute() )
                        {
                            switch ( reader.Name )
                            {
                                case "default_project_name":
                                    mProjectName = reader.Value;
                                    break;

                                case "nb_tiles_h":
                                    mNbTilesH = Convert.ToInt32( reader.Value );
                                    break;

                                case "nb_tiles_v":
                                    mNbTilesV = Convert.ToInt32( reader.Value );
                                    break;

                                case "tile_width":
                                    mTileWidth = Convert.ToInt32( reader.Value );
                                    break;

                                case "tile_height":
                                    mTileHeight = Convert.ToInt32( reader.Value );
                                    break;

                                case "resources_path":
                                    mResourcesPath = reader.Value + "\\";
                                    break;

                                case "collision_tiles_filename":
                                    mCollisionTilesFilename = reader.Value;
                                    break;

                                case "objects_description_filename":
                                    mObjectsDescriptionFilename = reader.Value;
                                    break;

                                case "hotspots_description_filename":
                                    mHotspotsDescriptionFilename = reader.Value;
                                    break;
                            }
                        }
                    }
                }

                reader.Close();
            }
        }

        private static void SetDefaultValues()
        {
            mProjectName = DEFAULT_PROJECT_NAME;
            mNbTilesH = NB_TILES_H;
            mNbTilesV = NB_TILES_V;
            mTileWidth = TILE_WIDTH;
            mTileHeight = TILE_HEIGHT;
// !!!! TODO: set another directory ... !!!!
// ( a subdirectory such as "resources" ? )
            mResourcesPath = mApplicationPath;
            mCollisionTilesFilename = COLLISION_TILES_FILENAME;
            mObjectsDescriptionFilename = OBJECTS_DESCRIPTION_FILENAME;
            mHotspotsDescriptionFilename = HOTSPOTS_DESCRIPTION_FILENAME;
        }


        // write values to a config file ( through "DialogSettings" )
        //...
    }
}
