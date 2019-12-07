using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Xml;
using System.IO;
using System.Globalization;

using TileMan.Components;
using TileMan.Dialogs;
using TileMan.Pages;


namespace TileMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string mProjectName;

        private Level mLevel;

        private PageMain mPageMain;
        private PageVisuals mPageVisuals;
        private PageCollision mPageCollision;
        private PageObjects mPageObjects;
        private PageHotspots mPageHotspots;

        private int mCurrentLayerType;
        private Layer mCurrentLayer;
        private UniformGrid mCurrentGrid;

        private System.Collections.Generic.List<LayerMap> mLayerMaps;
        private LayerMap mCurrentLayerMap;


        public MainWindow()
        {
            InitializeComponent();

            mLevel = new Level();

            mPageMain = new PageMain( mLevel.mMainLayer, mLevel.mBackground, this );
            mPageCollision = new PageCollision( mLevel.mCollisionLayer, this );
            mPageObjects = new PageObjects( mLevel.mObjectsLayer, this );
            mPageHotspots = new PageHotspots( mLevel.mHotspotsLayer, this );

            mPageVisuals = new PageVisuals( mLevel.mMainLayer, mLevel.mVisualLayers, this );

            mLayerMaps = new System.Collections.Generic.List<LayerMap>();
            LayerMap layerMap = new LayerMap( mLevel.mMainLayer, UniformGridMapMain );
            mLayerMaps.Add( layerMap );
            layerMap = new LayerMap( mLevel.mCollisionLayer, UniformGridMapCollision );
            mLayerMaps.Add( layerMap );
            layerMap = new LayerMap( mLevel.mObjectsLayer, UniformGridMapObjects );
            mLayerMaps.Add( layerMap );
            layerMap = new LayerMap( mLevel.mHotspotsLayer, UniformGridMapHotspots );
            mLayerMaps.Add( layerMap );

            GenerateMaps();

            FillCombo();

            Reset();
        }

        private void Reset()
        {
            // loads settings
            Settings.Load();

            mProjectName = Settings.mProjectName;

            // clear map
            UniformGridMapMain.Children.Clear();
            UniformGridMapCollision.Children.Clear();
            UniformGridMapObjects.Children.Clear();
            UniformGridMapHotspots.Children.Clear();

            // reset level
            mLevel.Reset();

            // reset pages
            mPageMain.Reset();
            mPageCollision.Reset();
            mPageObjects.Reset();
            mPageHotspots.Reset();

            mPageVisuals.Reset();

            // reset the first 4 LayerMaps & delete the others
            // ( main, collision, objects, hotspots )
            for ( int i = 0; i < Layer.TYPE_VISUALS; ++i )
            {
                mLayerMaps[ i ].Reset();
            }
            while ( mLayerMaps.Count > Layer.TYPE_VISUALS )
            {
                mLayerMaps.RemoveAt( mLayerMaps.Count - 1 ); // remove last one
            }

            GenerateMaps();
            ClearVisualLayersMaps();

            ComboBoxLayers.SelectedIndex = -1; // to force value change
            ComboBoxLayers.SelectedIndex = Layer.TYPE_MAIN;
        }


        public void SetPage( int layerType )
        {
            if ( layerType == Layer.TYPE_MAIN )
            {
                ContentMain.Content = mPageMain;
                mPageMain.Refresh();
            }
            else if ( layerType == Layer.TYPE_COLLISION )
            {
                ContentMain.Content = mPageCollision;
                mPageCollision.Refresh();
            }
            else if ( layerType == Layer.TYPE_OBJECTS )
            {
                ContentMain.Content = mPageObjects;
                mPageObjects.Refresh();
            }
            else if ( layerType == Layer.TYPE_HOTSPOTS )
            {
                ContentMain.Content = mPageHotspots;
                mPageHotspots.Refresh();
            }
            else if ( layerType == Layer.TYPE_VISUALS )
            {
                ContentMain.Content = mPageVisuals;
                mPageVisuals.Refresh();
            }
        }

        private void FillCombo()
        {
            ComboBoxLayers.Items.Add( "Main" );
            ComboBoxLayers.Items.Add( "Collision" );
            ComboBoxLayers.Items.Add( "Objects" );
            ComboBoxLayers.Items.Add( "Hotspots" );
            ComboBoxLayers.Items.Add( "Visuals" );
        }

        private void ComboBoxLayers_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            ActivateCurrentLayer( ComboBoxLayers.SelectedIndex );
        }

        // Menus Functions
        private void MenuNew_Click( object sender, RoutedEventArgs e )
        {
// !!!! TODO: ask if sure before resetting !!!!
// => " Are you sure ? Changes will be lost. YES|NO"

            Reset();
        }

        private void MenuOpen_Click( object sender, RoutedEventArgs e )
        {
            //...
        }

        private void MenuSave_Click( object sender, RoutedEventArgs e )
        {
            //...
        }

        private void MenuSaveAs_Click( object sender, RoutedEventArgs e )
        {
            //...
        }

        private void MenuImport_Click( object sender, RoutedEventArgs e )
        {
            TextStatusBar.Text = "";

// !!!! TODO: use dlg.InitialDirectory everywhere it makes sense !!!!
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "Import Level";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (*.xml)|*.xml";

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                // clear level data
                Reset();

                TextStatusBar.Text = "Please wait - Importing level ...";
                string fullpath = dlg.FileName;
                string path = GetPath( fullpath ) + "\\";
                string basename = GetBaseName( fullpath );

                FileStream fileStream = File.OpenRead( fullpath );

                XmlReaderSettings settings;
                settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Document;

                XmlReader reader = XmlReader.Create( fileStream, settings );

                // - import XML data
                // - for each layer
                //  - check if BIN file exists
                //   - if yes, load layer
                //  - check if TileMap file exists
                //   - if yes, load image ( & tiles )
                //   - if not, clear layer's map
                while ( reader.Read() )
                {
                    if ( reader.IsStartElement() )
                    {
                        int width;
                        int height;
                        string filepath;

                        switch ( reader.Name )
                        {
                            case "background":
                                TextStatusBar.Text = "Please wait - Importing Background ...";

                                // Background
                                //  Default values :
                                //    <background
                                //      resource="@drawable/..." => need to specify !
                                //      sizeFactorX="1.0"        => optional
                                //      sizeFactorY="1.0"        => optional
                                //      width="512"              => optional
                                //      height="512"             => optional
                                //      ratio="width"            => optional
                                //    />

                                Background bkg = mLevel.mBackground;

                                while ( reader.MoveToNextAttribute() )
                                {
                                    switch ( reader.Name )
                                    {
                                        case "resource":
                                            string resource = reader.Value;
                                            resource = resource.Replace( "@drawable/", basename + "\\drawable\\" ); // replace "drawable" tag
                                            resource = path + resource; // add the full path
// !!!! ???? TODO: OK to hardcode ".png" ? ???? !!!!
// => should look in directory what file exists, & get its extension
                                            resource = resource + ".png"; // add the extension
                                            bkg.SetResource( resource );
                                            break;
                                        case "sizeFactorX":
                                            bkg.SetSizeFactorX( Convert.ToDouble( reader.Value, CultureInfo.InvariantCulture ) );
                                            break;
                                        case "sizeFactorY":
                                            bkg.SetSizeFactorY( Convert.ToDouble( reader.Value, CultureInfo.InvariantCulture ) );
                                            break;
                                        case "width":
                                            bkg.SetWidth( Convert.ToInt32( reader.Value ) );
                                            break;
                                        case "height":
                                            bkg.SetHeight( Convert.ToInt32( reader.Value ) );
                                            break;
                                        case "ratio":
                                            bkg.SetRatioKeepWidth( reader.Value == "width" );
                                            break;
                                    }
                                }
                                if ( ( bkg.GetWidth() != 0 ) && ( bkg.GetHeight() != 0 ) )
                                {
                                    bkg.SetUseImageRatio( true );
                                }
                                mPageMain.UpdateBackground();
                                break;


                            case "layerMain":
                                TextStatusBar.Text = "Please wait - Importing main layer ...";

                                // Main Layer ( & Collision, Objects & Hotspots Layers )
                                //  Default values :
                                //    <layerMain
                                //      tilesheet="@drawable/..."        => need to specify ! ( PNG or image file )
                                //      tileWidth="32"                   => need to specify ! ( int > 0 )
                                //      tileHeight="32"                  => need to specify ! ( int > 0 )
                                //      resource="@raw/..."              => need to specify ! ( BIN file )
                                //      collisionResource=""             => if not specified uses same as "resource" ( BIN file )
                                //      objectsResource="@raw/..."       => need to specify ! ( BIN file )
                                //      hotspotsResource="@raw/..."      => need to specify ! ( BIN file )
                                //    />

                                width = Convert.ToInt32( reader.GetAttribute( "tileWidth" ) );
                                height = Convert.ToInt32( reader.GetAttribute( "tileHeight" ) );
                                mPageMain.UpdateTileSizes( width, height );
                                mPageCollision.UpdateTileSizes( width, height );
                                mPageObjects.UpdateTileSizes( width, height );
                                mPageHotspots.UpdateTileSizes( width, height );
                                // no dependent layers yet

                                filepath = reader.GetAttribute( "tilesheet" );
                                filepath = filepath.Replace( "@drawable/", basename + "\\drawable\\" ); // replace "drawable" tag
                                filepath = path + filepath; // add the full path
// !!!! ???? TODO: OK to hardcode ".png" ? ???? !!!!
// => should look in directory what file exists, & get its extension
                                filepath = filepath + ".png"; // add the extension
                                //mLayerMaps[ Layer.TYPE_MAIN ].mLayer.LoadTiles( filepath );
                                mLevel.mMainLayer.LoadTiles( filepath );

                                filepath = reader.GetAttribute( "resource" );
                                filepath = filepath.Replace( "@raw/", basename + "\\raw\\" ); // replace "raw" tag
                                filepath = path + filepath; // add the full path
                                filepath = filepath + ".bin"; // add the extension
                                LoadLayerResource( Layer.TYPE_MAIN, filepath );
                                mPageMain.UpdateTileMap();
                                string mainfilepath = filepath;

                                TextStatusBar.Text = "Please wait - Importing collision layer ...";
                                filepath = Settings.mResourcesPath + Settings.mCollisionTilesFilename;
                                if ( System.IO.File.Exists( filepath ) )
                                {
                                    LoadTiles( Layer.TYPE_COLLISION, filepath );
                                }
                                filepath = reader.GetAttribute( "collisionResource" );
                                if ( filepath != null )
                                {
                                    mPageCollision.CheckBoxAsMain.IsChecked = false;
                                    filepath = filepath.Replace( "@raw/", basename + "\\raw\\" ); // replace "raw" tag
                                    filepath = path + filepath; // add the full path
                                    filepath = filepath + ".bin"; // add the extension
                                }
                                else
                                {
                                    mPageCollision.CheckBoxAsMain.IsChecked = true;
                                    filepath = mainfilepath;
                                }
                                LoadLayerResource( Layer.TYPE_COLLISION, filepath );
                                mPageCollision.UpdateTileMap();

                                TextStatusBar.Text = "Please wait - Importing objects layer ...";
                                filepath = Settings.mResourcesPath + Settings.mObjectsDescriptionFilename;
                                if ( System.IO.File.Exists( filepath ) )
                                {
                                    LoadTiles( Layer.TYPE_OBJECTS, filepath );
                                }
                                filepath = reader.GetAttribute( "objectsResource" );
                                filepath = filepath.Replace( "@raw/", basename + "\\raw\\" ); // replace "raw" tag
                                filepath = path + filepath; // add the full path
                                filepath = filepath + ".bin"; // add the extension
                                LoadLayerResource( Layer.TYPE_OBJECTS, filepath );
                                mPageObjects.UpdateObjectsMap();


                                TextStatusBar.Text = "Please wait - Importing hotspots layer ...";
                                filepath = Settings.mResourcesPath + Settings.mHotspotsDescriptionFilename;
                                if ( System.IO.File.Exists( filepath ) )
                                {
                                    LoadTiles( Layer.TYPE_HOTSPOTS, filepath );
                                }
                                filepath = reader.GetAttribute( "hotspotsResource" );
                                filepath = filepath.Replace( "@raw/", basename + "\\raw\\" ); // replace "raw" tag
                                filepath = path + filepath; // add the full path
                                filepath = filepath + ".bin"; // add the extension
                                LoadLayerResource( Layer.TYPE_HOTSPOTS, filepath );
                                mPageHotspots.UpdateHotspotsMap();

                                // update text boxes on main layer page
                                // too many redundant operations ...
                                //UpdateTilesNb( mLevel.mMainLayer.mResource.mNbTilesH, mLevel.mMainLayer.mResource.mNbTilesV,
                                //               LayerResource.ORIENTATION_LEFT, LayerResource.ORIENTATION_LEFT );
                                mPageMain.TextBoxNbTilesH.Text = mLevel.mMainLayer.mResource.mNbTilesH.ToString();
                                mPageMain.TextBoxNbTilesV.Text = mLevel.mMainLayer.mResource.mNbTilesV.ToString();

                                break;


                            case "layer":
                                TextStatusBar.Text = "Please wait - Importing visual layer ...";

                                // Visual Layer
                                //  Default values :
                                //    <layer
                                //      tilesheet="@drawable/..." => need to specify ! ( PNG or image file )
                                //      tileWidth="32"            => need to specify ! ( int > 0 )
                                //      tileHeight="32"           => need to specify ! ( int > 0 )
                                //      resource="@raw/..."       => need to specify ! ( BIN file )
                                //      foreground="false"
                                //      sizeFactorX="1.0"         => must be > 0.0
                                //      sizeFactorY="1.0"         => must be > 0.0
                                //      movingSpeedX="0.0"
                                //      movingSpeedY="0.0"
                                //    />
//////// NEW_LAYER 20140320 - MID
                                bool background = true;
                                string foreground = reader.GetAttribute( "foreground" );
                                if ( foreground == "true" )
                                {
                                    background = false;
                                }


                                if ( background )
                                {
                                    mPageVisuals.ListBoxLayers.SelectedIndex = mPageVisuals.GetMainLayerListIndex();
                                }
                                else
                                {
                                    int layerIndex = mPageVisuals.GetNbLayers();
                                    if ( layerIndex == 0 )
                                    {
                                        mPageVisuals.ListBoxLayers.SelectedIndex = mPageVisuals.GetMainLayerListIndex();
                                    }
                                    else
                                    {
                                        mPageVisuals.ListBoxLayers.SelectedIndex = mPageVisuals.GetNbLayers();
                                    }
                                }


                                // check if share a resource with another layer already loaded
                                Layer originalLayer = null;
                                string resourceFile = reader.GetAttribute( "resource" );
                                resourceFile = resourceFile.Replace( "@raw/", "" );

                                if ( mLayerMaps[ Layer.TYPE_MAIN ].mLayer.mResource.mFileName == resourceFile )
                                {
                                    originalLayer = mLayerMaps[ Layer.TYPE_MAIN ].mLayer;
                                }
                                else
                                {
                                    int copyLayerId = -1;
                                    for ( int i = Layer.TYPE_VISUALS; i < mLayerMaps.Count; ++i )
                                    {
                                        if ( mLayerMaps[ i ].mLayer.mResource.mFileName == resourceFile )
                                        {
                                            copyLayerId = i;
                                            break;
                                        }
                                    }
                                    if ( copyLayerId != -1 )
                                    {
                                        originalLayer = mLayerMaps[ copyLayerId ].mLayer;
                                    }
                                }


                                LayerMap layerMap;
                                if ( originalLayer == null )
                                {
                                    // create a new independdent layer

                                    width = Convert.ToInt32( reader.GetAttribute( "tileWidth" ) );
                                    height = Convert.ToInt32( reader.GetAttribute( "tileHeight" ) );
                                    mPageVisuals.AddNewLayer( 0, 0, width, height );

                                    layerMap = mLayerMaps[ Layer.TYPE_VISUALS + mPageVisuals.GetCurrentLayerIndex() ];

                                    filepath = reader.GetAttribute( "tilesheet" );
                                    filepath = filepath.Replace( "@drawable/", basename + "\\drawable\\" ); // replace "drawable" tag
                                    filepath = path + filepath; // add the full path
// !!!! ???? TODO: OK to hardcode ".png" ? ???? !!!!
// => should look in directory what file exists, & get its extension
                                    filepath = filepath + ".png"; // add the extension
                                    mLayerMaps[ Layer.TYPE_VISUALS + mPageVisuals.GetCurrentLayerIndex() ].mLayer.LoadTiles( filepath );


//                                    filepath = reader.GetAttribute( "resource" );
//                                    filepath = filepath.Replace( "@raw/", basename + "\\raw\\" ); // replace "raw" tag
                                    filepath = basename + "\\raw\\" + resourceFile;
                                    filepath = path + filepath; // add the full path
                                    filepath = filepath + ".bin"; // add the extension
// !!!! ???? TODO: OK like this or better to keep track of how many layers have been added ? ???? !!!!
                                    LoadLayerResource( Layer.TYPE_VISUALS + mPageVisuals.GetCurrentLayerIndex(), filepath );

                                    mPageVisuals.UpdateTileMap();
                                }
                                else
                                {
                                    // create a new layer sharing a resource with another layer

                                    width = Convert.ToInt32( reader.GetAttribute( "tileWidth" ) );
                                    height = Convert.ToInt32( reader.GetAttribute( "tileHeight" ) );
                                    mPageVisuals.CopyNewLayer( originalLayer, true );

                                    layerMap = mLayerMaps[ Layer.TYPE_VISUALS + mPageVisuals.GetCurrentLayerIndex() ];
                                    layerMap.mLayer.SetTilesSizes( width, height );

                                    filepath = reader.GetAttribute( "tilesheet" );
                                    filepath = filepath.Replace( "@drawable/", basename + "\\drawable\\" ); // replace "drawable" tag
                                    filepath = path + filepath; // add the full path
// !!!! ???? TODO: OK to hardcode ".png" ? ???? !!!!
// => should look in directory what file exists, & get its extension
                                    filepath = filepath + ".png"; // add the extension
                                    layerMap.mLayer.LoadTiles( filepath );

                                    GenerateMap( layerMap.mLayer, layerMap.mGrid );

                                }


                                // size factors
                                string stringValue;
                                double sizeFactorX = 1.0;
                                double sizeFactorY = 1.0;

                                stringValue = reader.GetAttribute( "sizeFactorX" );
                                if ( stringValue != null )
                                {
                                    sizeFactorX = Convert.ToDouble( stringValue, CultureInfo.InvariantCulture );
                                }

                                stringValue = reader.GetAttribute( "sizeFactorY" );
                                if ( stringValue != null )
                                {
                                    sizeFactorY = Convert.ToDouble( stringValue, CultureInfo.InvariantCulture );
                                }
                                mPageVisuals.UpdateSizeFactors( sizeFactorX, sizeFactorY );


                                // moving speeds
                                double speedX = 0.0;
                                double speedY = 0.0;

                                stringValue = reader.GetAttribute( "movingSpeedX" );
                                if ( stringValue != null )
                                {
                                    speedX = Convert.ToDouble( stringValue, CultureInfo.InvariantCulture );
                                }

                                stringValue = reader.GetAttribute( "movingSpeedY" );
                                if ( stringValue != null )
                                {
                                    speedY = Convert.ToDouble( stringValue, CultureInfo.InvariantCulture );
                                }
// !!!! ???? TODO: OK ? ???? !!!!
                                LayerVisual layerVisual = layerMap.mLayer as LayerVisual;
                                layerVisual.SetMovingSpeeds( speedX, speedY );



                                if ( background )
                                {
                                    mPageVisuals.MoveCurrentLayerUp();
                                }

//?                                mPageVisuals.Refresh();


//////// NEW_LAYER 20140320 - END

/*
//? public void UpdateVisualLayerTilesNb( int nbTilesH, int nbTilesV, int horizOrient, int verticOrient )
public void UpdateVisualLayerTileSizes( int tileWidth, int tileHeight )
*/
// !!!! TODO: check for dependent data !!!!
//...

                                break;
                        }
                    }
                }

// ????
//?                mPageVisuals.Refresh();

                ActivateCurrentLayer( Layer.TYPE_MAIN);

                reader.Close();
                fileStream.Close();

// !!!! ???? TODO: leave this until next message don't display it ? ???? !!!!
                TextStatusBar.Text = "Import OK.";
            }
        }

        private void MenuExport_Click( object sender, RoutedEventArgs e )
        {
            TextStatusBar.Text = "";

            // Check that everything is ok & that export can be done
            string strErrMsg = "";
            if ( !CheckLevel( out strErrMsg ) )
            {
                MessageBox.Show( "The current level can't be exported!\n\n" + strErrMsg );
                return;
            }

            // saves the XML file where the user specifies it, and create a subfolder for all the other resources
// !!!! TODO: should have an "OpenFolderDialog" dialog & save everything as in the game structure !!!!
// => BUT that dialog doesn't exist in WPF ! ( can use WinForms or write it ... )

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Title = "Export Level";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (*.xml)|*.xml";
            dlg.FileName = mProjectName;

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                TextStatusBar.Text = "Please wait - Exporting level ...";

                // ask user if want to save related resources
                bool cancel = false;
                bool saveResources = false;
                string question = "Do you also want to export the associated resources ?\n( layers, images, etc. )\n\nExisting files will be deleted !";
                string title = "Export";
                MessageBoxResult msgBox = MessageBox.Show( question, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question );
                switch ( msgBox )
                {
                    case MessageBoxResult.Yes:
                        saveResources = true;
                        break;

                    case MessageBoxResult.No:
                        saveResources = false;
                        break;

                    case MessageBoxResult.Cancel:
                        cancel = true;
                        TextStatusBar.Text = "";
                        break;
                }

                // process with export
                if ( !cancel )
                {
                    string filename = dlg.FileName;

                    // export XML data
                    TextStatusBar.Text = "Please wait - Creating XML ...";
//                    ExportXml( mProjectName, mLevel );
                    XmlTextWriter textWriter = new XmlTextWriter( filename, System.Text.Encoding.UTF8 );
                    textWriter.Formatting = Formatting.Indented;
                    textWriter.Indentation = 4;
//                    textWriter.Settings.NewLineOnAttributes = true;

                    textWriter.WriteStartDocument();
                    textWriter.WriteComment( " XML generated by TileMan " );
                    textWriter.WriteStartElement( "level" );

                    // Background
                    //  Default values :
                    //    <background
                    //      resource="@drawable/..." => need to specify !
                    //      sizeFactorX="1.0"        => must be >= 1.0
                    //      sizeFactorY="1.0"        => must be >= 1.0
                    //      width="512"              => if both specified, one of the size factors will not be considered, depending on "ratio"
                    //      height="512"                ( if only one specified => ignored )
                    //      ratio="width"            => must be "width" or "height" ( default : "width" )
                    //    />
                    Background bkg = mLevel.mBackground;
                    textWriter.WriteStartElement( "background" );
                    textWriter.WriteAttributeString( "resource", "@drawable/" + GetBaseName( bkg.GetResourceFullPath() ) );
                    textWriter.WriteAttributeString( "sizeFactorX", bkg.GetSizeFactorX().ToString( "F1", CultureInfo.InvariantCulture ) );
                    textWriter.WriteAttributeString( "sizeFactorY", bkg.GetSizeFactorY().ToString( "F1", CultureInfo.InvariantCulture ) );

                    if ( bkg.GetUseImageRatio() )
                    {
                        textWriter.WriteAttributeString( "width", bkg.GetWidth().ToString() );
                        textWriter.WriteAttributeString( "height", bkg.GetHeight().ToString() );
                        if ( bkg.GetRatioKeepWidth() )
                        {
                            textWriter.WriteAttributeString( "ratio", "width" );
                        }
                        else
                        {
                            textWriter.WriteAttributeString( "ratio", "height" );
                        }
                    }
                    textWriter.WriteEndElement();


                    // Main Layer ( & Collision, Objects & Hotspots Layers )
                    //  Default values :
                    //    <layerMain
                    //      tilesheet="@drawable/..."        => need to specify ! ( PNG or image file )
                    //      tileWidth="32"                   => need to specify ! ( int > 0 )
                    //      tileHeight="32"                  => need to specify ! ( int > 0 )
                    //      resource="@raw/..."              => need to specify ! ( BIN file )
                    //      collisionResource=""             => if not specified uses same as "resource" ( BIN file )
                    //      objectsResource="@raw/..."       => need to specify ! ( BIN file )
                    //      hotspotsResource="@raw/..."      => need to specify ! ( BIN file )
                    //    />
                    Layer layerMain = mLevel.mMainLayer;
                    textWriter.WriteStartElement( "layerMain" );
                    textWriter.WriteAttributeString( "tilesheet", "@drawable/" + GetBaseName( layerMain.mTileMapFilename ) );
                    textWriter.WriteAttributeString( "tileWidth", layerMain.mTileWidth.ToString() );
                    textWriter.WriteAttributeString( "tileHeight", layerMain.mTileHeight.ToString() );
                    textWriter.WriteAttributeString( "resource", "@raw/" + layerMain.mResource.mFileName );
                    if ( !mLevel.mCollisionSameAsMain )
                    {
                        textWriter.WriteAttributeString( "collisionResource", "@raw/" + mLevel.mCollisionLayer.mResource.mFileName );
                    }
                    textWriter.WriteAttributeString( "objectsResource", "@raw/" + mLevel.mObjectsLayer.mResource.mFileName );
                    textWriter.WriteAttributeString( "hotspotsResource", "@raw/" + mLevel.mHotspotsLayer.mResource.mFileName );
                    textWriter.WriteEndElement();


                    // Visual Layers
                    int nbVisualLayers = mLevel.mVisualLayers.Count;
                    if ( nbVisualLayers != 0 )
                    {
                        int mainLayerGridIndex = mPageVisuals.GetMainLayerListIndex();
                        bool foreground = false;

                        // get layers in the order of the list in PageVisuals
                        for ( int i = 0; i <= nbVisualLayers; ++i ) // "+1" as consider "main layer"
                        {
                            if ( i == mainLayerGridIndex )
                            {
                                // skip layer & set to foreground
                                foreground = true;
                                continue;
                            }

                            //  Default values :
                            //    <layer
                            //      tilesheet="@drawable/..." => need to specify ! ( PNG or image file )
                            //      tileWidth="32"            => need to specify ! ( int > 0 )
                            //      tileHeight="32"           => need to specify ! ( int > 0 )
                            //      resource="@raw/..."       => need to specify ! ( BIN file )
                            //      foreground="false"
                            //      sizeFactorX="1.0"         => must be > 0.0
                            //      sizeFactorY="1.0"         => must be > 0.0
                            //      movingSpeedX="0.0"
                            //      movingSpeedY="0.0"
                            //    />
                            LayerVisual layerVisual = mLevel.mVisualLayers[ mPageVisuals.GetLayerIndex( i ) ];
                            textWriter.WriteStartElement( "layer" );
                            textWriter.WriteAttributeString( "tilesheet", "@drawable/" + GetBaseName( layerVisual.mTileMapFilename ) );
                            textWriter.WriteAttributeString( "tileWidth", layerVisual.mTileWidth.ToString() );
                            textWriter.WriteAttributeString( "tileHeight", layerVisual.mTileHeight.ToString() );
                            textWriter.WriteAttributeString( "resource", "@raw/" + layerVisual.mResource.mFileName );
                            if ( foreground )
                            {
                                textWriter.WriteAttributeString( "foreground", "true" );
                            }
                            if ( layerVisual.mSizeFactorX != 1.0 )
                            {
                                textWriter.WriteAttributeString( "sizeFactorX", layerVisual.mSizeFactorX.ToString( "F1", CultureInfo.InvariantCulture ) );
                            }
                            if ( layerVisual.mSizeFactorY != 1.0 )
                            {
                                textWriter.WriteAttributeString( "sizeFactorY", layerVisual.mSizeFactorY.ToString( "F1", CultureInfo.InvariantCulture ) );
                            }
                            if ( layerVisual.mSpeedX != 0.0 )
                            {
                                textWriter.WriteAttributeString( "movingSpeedX", layerVisual.mSpeedX.ToString( "F1", CultureInfo.InvariantCulture ) );
                            }
                            if ( layerVisual.mSpeedY != 0.0 )
                            {
                                textWriter.WriteAttributeString( "movingSpeedY", layerVisual.mSpeedY.ToString( "F1", CultureInfo.InvariantCulture ) );
                            }
                            textWriter.WriteEndElement();
                        }
                    }

// !!!! ???? TODO: manage dialogs ... ? ???? !!!!
// could handle the whole dialog stuff, but not really necessary
// => make it manually, and add tag in XML : <dialogs resource="@xml/..." />

                    textWriter.WriteEndDocument();
                    textWriter.Close();


                    // save associated resources
                    if ( saveResources )
                    {
                        TextStatusBar.Text = "Please wait - Exporting resources ...";

                        // create directories
                        string basePath = GetPath( filename ) + "\\" + GetBaseName( filename );
                        if ( Directory.Exists( basePath ) )
                        {
// !!!! TODO: need to check that the files are not used by application !!!!
// ( or another one ) - if it's the case => we have an exception!
                            Directory.Delete( basePath, true );
                        }
                        Directory.CreateDirectory( basePath );

                        string drawablePath = basePath + "\\drawable";
                        if ( !Directory.Exists( drawablePath ) )
                        {
                            Directory.CreateDirectory( drawablePath );
                        }
                        drawablePath += "\\";

                        string rawPath = basePath + "\\raw";
                        if ( !Directory.Exists( rawPath ) )
                        {
                            Directory.CreateDirectory( rawPath );
                        }
                        rawPath += "\\";

                        // background image
                        File.Copy( bkg.GetResourceFullPath(), drawablePath + GetFileName( bkg.GetResourceFullPath() ) );

                        // main layer tile sheet
                        string tilemap = mLayerMaps[ Layer.TYPE_MAIN ].mLayer.mTileMapFilename;
                        string dest = drawablePath + GetFileName( tilemap );
                        if ( !File.Exists( dest ) )
                        {
                            File.Copy( tilemap, dest );
                        }

                        // main layers resource files
                        Layer layer = mLayerMaps[ Layer.TYPE_MAIN ].mLayer;
                        dest = rawPath + layer.mResource.mFileName + ".bin";
                        if ( !File.Exists( dest ) )
                        {
                            layer.SaveResource( dest );
                        }
                        if ( !mLevel.mCollisionSameAsMain )
                        {
                            layer = mLayerMaps[ Layer.TYPE_COLLISION ].mLayer;
                            dest = rawPath + layer.mResource.mFileName + ".bin";
                            if ( !File.Exists( dest ) )
                            {
                                layer.SaveResource( dest );
                            }
                        }
                        layer = mLayerMaps[ Layer.TYPE_OBJECTS ].mLayer;
                        dest = rawPath + layer.mResource.mFileName + ".bin";
                        if ( !File.Exists( dest ) )
                        {
                            layer.SaveResource( dest );
                        }
                        layer = mLayerMaps[ Layer.TYPE_HOTSPOTS ].mLayer;
                        dest = rawPath + layer.mResource.mFileName + ".bin";
                        if ( !File.Exists( dest ) )
                        {
                            layer.SaveResource( dest );
                        }

                        // visual layers data
                        for ( int i = Layer.TYPE_VISUALS; i < mLayerMaps.Count; ++i )
                        {
                            layer = mLayerMaps[ i ].mLayer;

                            // visual layer tile sheet
                            tilemap = layer.mTileMapFilename;
                            dest = drawablePath + GetFileName( tilemap );
                            if ( !File.Exists( dest ) )
                            {
                                File.Copy( tilemap, dest );
                            }

                            // visual layer resource file
                            dest = rawPath + layer.mResource.mFileName + ".bin";
                            if ( !File.Exists( dest ) )
                            {
                                layer.SaveResource( dest );
                            }
                        }
                    }

                    TextStatusBar.Text = "Export OK.";
                }

            }

        }

        private void MenuExit_Click( object sender, RoutedEventArgs e )
        {
            //check if saved?
            //ask if want to save before exit
            //exit
        }

        private void MenuSettings_Click( object sender, RoutedEventArgs e )
        {
            DialogSettings dlg = new DialogSettings();
//            dlg.Owner = this;

            Nullable<bool> result = dlg.ShowDialog();
        }

// !!!! TODO: change that to handle mouse movement instead of clicks !!!!
// ( register mouse down/up events, and in movement function check buttons status & do required opertaion )
// => but makes the process heavier ... ( constant operations due to mouse movement )
        private void Grid_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            Point point = e.MouseDevice.GetPosition( CanvasGrid );

            if ( mCurrentLayer != null )
            {
                if ( CheckTileValidity() )
                {

                    // set tile in grid
                    int index = mCurrentLayer.SetTile( point.X, point.Y );
                    if ( index != -1 )
                    {
                        Image image = mCurrentGrid.Children[ index ] as Image;
                        image.Source = mCurrentLayer.mTiles[ mCurrentLayer.mCurrentTile ];

                        // update grids of dependent layers
                        for ( int i = 0; i < mCurrentLayerMap.mAssociatedLayerMaps.Count; ++i )
                        {
                            LayerMap layerMap = mCurrentLayerMap.mAssociatedLayerMaps[ i ];
                            image = layerMap.mGrid.Children[ index ] as Image;
                            image.Source = layerMap.mLayer.mTiles[ mCurrentLayer.mCurrentTile ];
                        }
                    }
                }
                else
                {
                    MessageBox.Show( "Tile is not valid with the dependent layers!\nChange the tilemap or the tiles sizes to fit the other layers." );
                }
            }
        }

        private void Grid_MouseRightButtonDown( object sender, MouseButtonEventArgs e )
        {
            Point point = e.MouseDevice.GetPosition( CanvasGrid );

            if ( mCurrentLayer != null )
            {
                int index = mCurrentLayer.UnsetTile( point.X, point.Y );
                if ( index != -1 )
                {
                    Image image = mCurrentGrid.Children[ index ] as Image;
                    image.Source = mCurrentLayer.mTiles[ mCurrentLayer.mNbTiles - 1 ];

                    // update grids of dependent layers
                    for ( int i = 0; i < mCurrentLayerMap.mAssociatedLayerMaps.Count; ++i )
                    {
                        LayerMap layerMap = mCurrentLayerMap.mAssociatedLayerMaps[ i ];
                        image = layerMap.mGrid.Children[ index ] as Image;
                        image.Source = layerMap.mLayer.mTiles[ layerMap.mLayer.mNbTiles - 1];
                    }
                }
            }
        }

        public void SetCollisionSameAsMain( bool collisionSameAsMain )
        {
            mLevel.mCollisionSameAsMain = collisionSameAsMain;
        }

        public void LoadTiles( int layerMapIndex, string tileMapFileName )
        {
            Layer layer = mLayerMaps[ layerMapIndex ].mLayer;
            layer.LoadTiles( tileMapFileName );

            UpdateDependentLayersTileCount( layerMapIndex );

            UniformGrid grid = mLayerMaps[ layerMapIndex ].mGrid;
            GenerateMap( layer, grid );
        }


        public void UpdateTilesNb( int nbTilesH, int nbTilesV, int horizOrient, int verticOrient )
        {
            mPageMain.UpdateTilesNb( nbTilesH, nbTilesV, horizOrient, verticOrient );
            mPageCollision.UpdateTilesNb( nbTilesH, nbTilesV, horizOrient, verticOrient );
            mPageObjects.UpdateTilesNb( nbTilesH, nbTilesV, horizOrient, verticOrient );
            mPageHotspots.UpdateTilesNb( nbTilesH, nbTilesV, horizOrient, verticOrient );

            UpdateDependentLayersTilesNb( Layer.TYPE_MAIN, horizOrient, verticOrient );

            GenerateMaps();
        }

        public void UpdateVisualLayerTilesNb( int nbTilesH, int nbTilesV, int horizOrient, int verticOrient )
        {
            mPageVisuals.UpdateTilesNb( nbTilesH, nbTilesV, horizOrient, verticOrient );

            UpdateDependentLayersTilesNb( Layer.TYPE_VISUALS + mPageVisuals.GetCurrentLayerIndex(), horizOrient, verticOrient );

            GenerateMap( mCurrentLayer, mCurrentGrid );
            GenerateGridLines();
        }

        public void UpdateDependentLayersTilesNb( int layerMapIndex, int horizOrient, int verticOrient )
        {
            LayerMap layerMap = mLayerMaps[ layerMapIndex ];

            int nbTilesH = layerMap.mLayer.mResource.mNbTilesH;
            int nbTilesV = layerMap.mLayer.mResource.mNbTilesV;
            for ( int i = 0; i < layerMap.mAssociatedLayerMaps.Count; ++i )
            {
                LayerMap depLayerMap = layerMap.mAssociatedLayerMaps[ i ];
                if ( depLayerMap.mLayer != mLevel.mMainLayer )
                {
                    depLayerMap.mLayer.Resize( nbTilesH, nbTilesV, horizOrient, verticOrient );
                    GenerateMap( depLayerMap.mLayer, depLayerMap.mGrid );
                }
                else
                {
                    mPageMain.UpdateTilesNb( nbTilesH, nbTilesV, horizOrient, verticOrient );
                    mPageCollision.UpdateTilesNb( nbTilesH, nbTilesV, horizOrient, verticOrient );
                    mPageObjects.UpdateTilesNb( nbTilesH, nbTilesV, horizOrient, verticOrient );
                    mPageHotspots.UpdateTilesNb( nbTilesH, nbTilesV, horizOrient, verticOrient );
                    GenerateMaps();
                }
            }
        }

        public void UpdateTileSizes( int tileWidth, int tileHeight )
        {
            mPageMain.UpdateTileSizes( tileWidth, tileHeight );
            mPageCollision.UpdateTileSizes( tileWidth, tileHeight );
            mPageObjects.UpdateTileSizes( tileWidth, tileHeight );
            mPageHotspots.UpdateTileSizes( tileWidth, tileHeight );

            UpdateDependentLayersTileCount( Layer.TYPE_MAIN );

            GenerateMaps();
        }

        public void UpdateVisualLayerTileSizes( int tileWidth, int tileHeight )
        {
            mPageVisuals.UpdateTileSizes( tileWidth, tileHeight );

            UpdateDependentLayersTileCount( Layer.TYPE_VISUALS + mPageVisuals.GetCurrentLayerIndex() );

            GenerateMap( mCurrentLayer, mCurrentGrid );
            GenerateGridLines();
        }

        private void UpdateDependentLayersTileCount( int layerMapIndex )
        {
            LayerMap layerMap = mLayerMaps[ layerMapIndex ];
            int nbTiles = layerMap.mLayer.mNbTiles;

            for ( int i = 0; i < layerMap.mAssociatedLayerMaps.Count; ++i )
            {
                LayerMap depLayerMap = layerMap.mAssociatedLayerMaps[ i ];
                depLayerMap.mLayer.mResource.FixIndices( nbTiles );

                if ( depLayerMap.mLayer != mLevel.mMainLayer )
                {
                    GenerateMap( depLayerMap.mLayer, depLayerMap.mGrid );
                }
                else
                {
                    GenerateMaps();
                }
            }
        }

        public void UpdateMap( int layerMapIndex )
        {
            GenerateMap( mLayerMaps[ layerMapIndex ].mLayer, mLayerMaps[ layerMapIndex ].mGrid );
// !!!! ???? TODO: OK to do it here ? ???? !!!!
            GenerateGridLines();
        }

        private bool CheckTileValidity()
        {
            bool ok = true;
            for ( int i = 0; ok && ( i < mCurrentLayerMap.mAssociatedLayerMaps.Count ); ++i)
            {
                if ( mCurrentLayerMap.mAssociatedLayerMaps[ i ].mLayer.mNbTiles <= mCurrentLayer.mCurrentTile )
                {
                    ok = false;
                }
            }
            return ok;
        }

        private void GenerateMaps()
        {
            Layer layer = mLevel.mMainLayer;
            int nbTilesH = layer.mResource.mNbTilesH;
            int nbTilesV = layer.mResource.mNbTilesV;
            int tileWidth = layer.mTileWidth;
            int tileHeight = layer.mTileHeight;

            GenerateMap( mLevel.mMainLayer, UniformGridMapMain );
            GenerateMap( mLevel.mCollisionLayer, UniformGridMapCollision );
            GenerateMap( mLevel.mObjectsLayer, UniformGridMapObjects );
            GenerateMap( mLevel.mHotspotsLayer, UniformGridMapHotspots );

            GenerateGridLines();
        }

        private void GenerateMap( Layer layer, UniformGrid grid )
        {
////////
//            GenerateGrid( grid, layer.mResource.mNbTilesH, layer.mResource.mNbTilesV, layer.mTileWidth, layer.mTileHeight );
////////
            int width = layer.mTileWidth;
            int height = layer.mTileHeight;
            LayerVisual layerVisual = layer as LayerVisual;
            if ( layerVisual != null )
            {
                width = ( int )( width * layerVisual.mSizeFactorX );
                height = ( int )( height * layerVisual.mSizeFactorY );
            }
            GenerateGrid( grid, layer.mResource.mNbTilesH, layer.mResource.mNbTilesV, width, height );
////////
            PopulateGrid( grid, layer );
        }

        private void ClearVisualLayersMaps()
        {
            GridVisualLayersBackground.Children.Clear();
            GridVisualLayersForeground.Children.Clear();
        }

        public void LoadLayerResource( int layerMapIndex, string filename )
        {
            LayerMap layerMap = mLayerMaps[ layerMapIndex ];
            Layer layer = layerMap.mLayer;
//            layer.mResource.Load( filename );
            layer.LoadResource( filename );

            // fix indices for layer
            layer.mResource.FixIndices( layer.mNbTiles );

            // update all dependent layers
// !!!! ???? TODO: OK ? ???? !!!!
            UpdateDependentLayersTilesNb( layerMapIndex, LayerResource.ORIENTATION_CENTRE, LayerResource.ORIENTATION_CENTRE );

// !!!! ???? TODO: need to call that here ? ???? !!!!
// => too much work to regenerate ALL the main layers !
            // update grid data
            GenerateMaps();

            // update visual grid data
            if ( layerMapIndex >= Layer.TYPE_VISUALS )
            {
                GenerateMap( layer, layerMap.mGrid );
            }
        }

        public void SaveLayerResource( int layerMapIndex, string filename )
        {
            LayerMap layerMap = mLayerMaps[ layerMapIndex ];
            Layer layer = layerMap.mLayer;
//            layer.mResource.Save( filename );
            layer.SaveResource( filename );
        }

        public void ClearMainLayer()
        {
            ClearLayer( Layer.TYPE_MAIN );
            if ( mPageCollision.CheckBoxAsMain.IsChecked == true )
            {
                ClearLayer( Layer.TYPE_COLLISION );
            }
        }

        public void ClearLayer( int layerMapIndex )
        {
            LayerMap layerMap = mLayerMaps[ layerMapIndex ];
            Layer layer = layerMap.mLayer;
            UniformGrid grid = layerMap.mGrid;

            layer.mResource.Clear();
            GenerateGrid( grid, layer.mResource.mNbTilesH, layer.mResource.mNbTilesV, layer.mTileWidth, layer.mTileHeight );

            ClearDependentLayers( layerMapIndex );
        }

        public void ClearDependentLayers( int layerMapIndex )
        {
            LayerMap layerMap = mLayerMaps[ layerMapIndex ];

            for ( int i = 0; i < layerMap.mAssociatedLayerMaps.Count; ++i )
            {
                Layer layer = layerMap.mAssociatedLayerMaps[ i ].mLayer;
                UniformGrid grid = layerMap.mAssociatedLayerMaps[ i ].mGrid;

                GenerateGrid( grid, layer.mResource.mNbTilesH, layer.mResource.mNbTilesV, layer.mTileWidth, layer.mTileHeight );
            }
        }


        public void AddVisualLayer( int layerIndex, int listIndex )
        {
            // get the layer
            Layer layer = mLevel.mVisualLayers[ layerIndex ];

            // create new visualisation grid
            UniformGrid grid = new UniformGrid();
            GenerateGrid( grid, layer.mResource.mNbTilesH, layer.mResource.mNbTilesV, layer.mTileWidth, layer.mTileHeight );
            PopulateGrid( grid, layer );

            // create new LayerMap entry
            LayerMap layerMap = new LayerMap( layer, grid );
            mLayerMaps.Add( layerMap );

            // add grid in corresponding panel
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Top;
            int mainLayerGridIndex = mPageVisuals.GetMainLayerListIndex();
            if ( listIndex <= mainLayerGridIndex )
            {
//MessageBox.Show( "BKG - adding layer " + listIndex + " to bkg ( layer id : " + layerIndex + " )" );
                GridVisualLayersBackground.Children.Insert( listIndex, grid );
            }
            else
            {
                listIndex -= ( mainLayerGridIndex + 1 );
//MessageBox.Show( "FORE - adding layer " + listIndex + " to fore ( layer id : " + layerIndex + " )");
                GridVisualLayersForeground.Children.Insert( listIndex, grid );
            }
        }

        public void RemoveVisualLayer(int layerIndex, int listIndex)
        {
            // remove grid from panel
            int mainLayerGridIndex = mPageVisuals.GetMainLayerListIndex();
            if ( listIndex < mainLayerGridIndex )
            {
//MessageBox.Show("BKG - removing layer " + listIndex + " from bkg" );
                GridVisualLayersBackground.Children.RemoveAt( listIndex );
            }
            else
            {
                listIndex -= ( mainLayerGridIndex + 1 );
//MessageBox.Show("FORE - removing layer " + listIndex + " from fore" );
                GridVisualLayersForeground.Children.RemoveAt( listIndex );
            }

            // remove LayerMap entry
            mLayerMaps[ Layer.TYPE_VISUALS + layerIndex ].RemoveAssociatedLayerMaps(); // remove dependencies
            mLayerMaps.Remove( mLayerMaps[ Layer.TYPE_VISUALS + layerIndex ] );
        }

        public void MoveVisualLayerForward( int gridIndex )
        {
            UniformGrid grid = null;

            int mainLayerGridIndex = mPageVisuals.GetMainLayerListIndex();
            if ( gridIndex == mainLayerGridIndex )
            {
                // moving Main Layer forward
                // => remove first layer of foreground & add it as last layer in foreground
//MessageBox.Show( "moving first layer of fore to last of bkg" );
                grid = GridVisualLayersForeground.Children[ 0 ] as UniformGrid;
                GridVisualLayersForeground.Children.RemoveAt( 0 );
                GridVisualLayersBackground.Children.Add( grid );
//MessageBox.Show( "last index : " + ( GridVisualLayersBackground.Children.Count - 1 ) );
            }
            else
            {
                int oldIndex, newIndex;
                if ( gridIndex < mainLayerGridIndex )
                {
                    oldIndex = gridIndex;
//MessageBox.Show( "BKG - removing layer " + oldIndex + " from bkg" );
                    grid = GridVisualLayersBackground.Children[ oldIndex ] as UniformGrid;
                    GridVisualLayersBackground.Children.RemoveAt( oldIndex );
                }
                else
                {
//                    oldIndex = gridIndex - ( mainLayerGridIndex + 1 );
                    --gridIndex;
                    oldIndex = gridIndex - mainLayerGridIndex;
//MessageBox.Show( "FORE - removing layer " + oldIndex + "from fore" );
                    grid = GridVisualLayersForeground.Children[ oldIndex ] as UniformGrid;
                    GridVisualLayersForeground.Children.RemoveAt( oldIndex );
                }

                ++gridIndex;
                if ( gridIndex < mainLayerGridIndex )
                {
                    newIndex = gridIndex;
//MessageBox.Show( "BKG - adding layer " + newIndex + " to bkg" );
                    GridVisualLayersBackground.Children.Insert( newIndex, grid );
                }
                else
                {
                    newIndex = gridIndex - mainLayerGridIndex;
//MessageBox.Show( "FORE - adding layer " + newIndex + " to fore" );
                    GridVisualLayersForeground.Children.Insert( newIndex, grid );
                }

            }
        }

        public void MoveVisualLayerBackward( int gridIndex )
        {
            UniformGrid grid = null;

            int mainLayerGridIndex = mPageVisuals.GetMainLayerListIndex();
            if ( gridIndex == mainLayerGridIndex )
            {
                // moving Main Layer backward
                // => remove last layer of background & add it as first layer in foreground
                int indexLast = GridVisualLayersBackground.Children.Count - 1;
//MessageBox.Show( "moving layer " + indexLast + " from bkg to first of fore" );
                grid = GridVisualLayersBackground.Children[ indexLast ] as UniformGrid;
                GridVisualLayersBackground.Children.RemoveAt( indexLast );
                GridVisualLayersForeground.Children.Insert( 0, grid );
            }
            else
            {
                int oldIndex, newIndex;
                if ( gridIndex < mainLayerGridIndex )
                {
                    oldIndex = gridIndex;
//MessageBox.Show( "BKG - removing layer " + oldIndex + " from bkg" );
                    grid = GridVisualLayersBackground.Children[ oldIndex ] as UniformGrid;
                    GridVisualLayersBackground.Children.RemoveAt( oldIndex );
                }
                else
                {
                    oldIndex = gridIndex - ( mainLayerGridIndex + 1 );
//MessageBox.Show( "FORE - removing layer " + oldIndex + "from fore" );
                    grid = GridVisualLayersForeground.Children[ oldIndex ] as UniformGrid;
                    GridVisualLayersForeground.Children.RemoveAt( oldIndex );
                }

                --gridIndex;
                if ( gridIndex <= mainLayerGridIndex )
                {
                    newIndex = gridIndex;
//MessageBox.Show( "BKG - adding layer " + newIndex + " to bkg" );
                    GridVisualLayersBackground.Children.Insert( newIndex, grid );
                }
                else
                {
                    newIndex = gridIndex - ( mainLayerGridIndex + 1 );
//MessageBox.Show( "FORE - adding layer " + newIndex + " to fore" );
                    GridVisualLayersForeground.Children.Insert( newIndex, grid );
                }
            }
        }

        public void AddDependencyLayer( int newLayer, int originalLayer )
        {
            int newLayerMapIndex = Layer.TYPE_VISUALS + newLayer;

            int originalLayerMapIndex;
            if ( originalLayer == -1 ) // main layer
            {
                originalLayerMapIndex = Layer.TYPE_MAIN;
            }
            else
            {
                originalLayerMapIndex = Layer.TYPE_VISUALS + originalLayer;
            }

            mLayerMaps[ newLayerMapIndex ].AddAssociatedLayerMap( mLayerMaps[ originalLayerMapIndex ] );
        }

        private void GenerateGrid( UniformGrid grid, int nbTilesH, int nbTilesV, int tileWidth, int tileHeight )
        {
            grid.Children.Clear();
            grid.Columns = nbTilesH;
            grid.Rows = nbTilesV;

            int nbTiles = nbTilesH * nbTilesV;
            for ( int i = 0; i < nbTiles; ++i )
            {
                Image image = new Image();
                image.Source = null;
                image.Stretch = Stretch.Fill;

                image.Width = tileWidth;
                image.Height = tileHeight;
                grid.Children.Add( image );
            }
        }

        private void PopulateGrid( UniformGrid grid, Layer layer )
        {
            if ( layer.mTileMap != null )
            {
                int nbTilesH = layer.mResource.mNbTilesH;
                int nbTilesV = layer.mResource.mNbTilesV;

                byte[,] tmpMap = layer.mResource.mMap;

                for ( int j = 0; j < nbTilesV; ++j )
                {
                    for ( int i = 0; i < nbTilesH; ++i )
                    {
                        Image image = grid.Children[ ( j * nbTilesH ) + i ] as Image;

                        int index = tmpMap[ i, j ];
                        if ( index == 255 ) // => last tile
                        {
                            index = layer.mNbTiles - 1;
                        }
                        image.Source = layer.mTiles[ index ];
                    }
                }
            }
        }

/*
        public void ActivateCurrentLayer( int layerIndex )
        {
            mCurrentLayerType = layerIndex;
////////
            mCurrentLayerMap = null;
////////
            mCurrentLayer = null;
            mCurrentGrid = null;

            SetPage( mCurrentLayerType );

            if ( mCurrentLayerType == Layer.TYPE_MAIN )
            {
////////
                mCurrentLayerMap = mLayerMaps[ 0 ];
////////
                mCurrentLayer = mLevel.mMainLayer;
                mCurrentGrid = UniformGridMapMain;
            }
            else if ( mCurrentLayerType == Layer.TYPE_VISUALS )
            {
////////
                if ( mPageVisuals.GetCurrentLayerIndex() != -1 )
                {
                    mCurrentLayerMap = mLayerMaps[ mPageVisuals.GetCurrentLayerIndex() ];
////////
                    mCurrentLayer = mPageVisuals.GetCurrentLayer();
// !!!! TODO: this test should be useless !!!!
                    if ( mCurrentLayer != null )
                    {
                        int visualLayerIndex = mPageVisuals.GetCurrentLayerListIndex();
                        int mainLayerIndex = mPageVisuals.GetMainLayerListIndex();
                        if ( visualLayerIndex < mainLayerIndex )
                        {
                            mCurrentGrid = GridVisualLayersBackground.Children[ visualLayerIndex ] as UniformGrid;
                        }
                        else
                        {
                            mCurrentGrid = GridVisualLayersForeground.Children[ visualLayerIndex - ( mainLayerIndex + 1 ) ] as UniformGrid;
                        }
                    }
                }
            }
            else if ( mCurrentLayerType == Layer.TYPE_COLLISION )
            {
////////
                mCurrentLayerMap = mLayerMaps[ 1 ];
////////
                mCurrentLayer = mLevel.mCollisionLayer;
                mCurrentGrid = UniformGridMapCollision;
            }
            else if ( mCurrentLayerType == Layer.TYPE_OBJECTS )
            {
////////
                mCurrentLayerMap = mLayerMaps[ 2 ];
////////
                mCurrentLayer = mLevel.mObjectsLayer;
                mCurrentGrid = UniformGridMapObjects;
            }
            else if ( mCurrentLayerType == Layer.TYPE_HOTSPOTS )
            {
////////
                mCurrentLayerMap = mLayerMaps[ 3 ];
////////
                mCurrentLayer = mLevel.mHotspotsLayer;
                mCurrentGrid = UniformGridMapHotspots;
            }


// !!!! TODO: should change opacities of all layers ... !!!!
//...

            GenerateGridLines();
        }
*/
// !!!! ???? TODO: OK ? ???? !!!!
        public void ActivateCurrentLayer( int layerType )
        {
            mCurrentLayerType = layerType;
            mCurrentLayerMap = null;
            mCurrentLayer = null;
            mCurrentGrid = null;

            SetPage( mCurrentLayerType );

            int layerMapIndex = layerType;
            if ( layerType == Layer.TYPE_VISUALS )
            {
                if ( mPageVisuals.GetCurrentLayerIndex() != -1 )
                {
                    layerMapIndex += mPageVisuals.GetCurrentLayerIndex();
                }
                else
                {
                    layerMapIndex = -1;
                }
            }

            if ( layerMapIndex != -1 )
            {
                mCurrentLayerMap = mLayerMaps[ layerMapIndex ];
                mCurrentLayer = mCurrentLayerMap.mLayer;
                mCurrentGrid = mCurrentLayerMap.mGrid;
            }


// !!!! TODO: should change opacities of all layers ... !!!!
//...

            GenerateGridLines();
        }

        private void GenerateGridLines()
        {
            CanvasGrid.Children.Clear();

            if ( mCurrentLayer != null )
            {
                SolidColorBrush colorBrush = new SolidColorBrush();
                colorBrush.Color = Colors.Black;

                int nbTilesH = mCurrentLayer.mResource.mNbTilesH;
                int nbTilesV = mCurrentLayer.mResource.mNbTilesV;
                int tileWidth = mCurrentLayer.mTileWidth;
                int tileHeight = mCurrentLayer.mTileHeight;
////////
                LayerVisual layerVisual = mCurrentLayer as LayerVisual;
                if ( layerVisual != null )
                {
                    tileWidth = ( int )( tileWidth * layerVisual.mSizeFactorX );
                    tileHeight = ( int )( tileHeight * layerVisual.mSizeFactorY );
                }
////////

                // horizontal grid lines
                for ( int j = 0; j <= nbTilesV; ++j )
                {
                    Line line = new Line();
                    line.X1 = 0;
                    line.Y1 = j * tileHeight;
                    line.X2 = tileWidth * nbTilesH;
                    line.Y2 = j * tileHeight;
                    line.Stroke = colorBrush;
                    CanvasGrid.Children.Add( line );
                }

                // vertical grid lines
                for ( int i = 0; i <= nbTilesH; ++i )
                {
                    Line line = new Line();
                    line.X1 = i * tileWidth;
                    line.Y1 = 0;
                    line.X2 = i * tileWidth;
                    line.Y2 = tileHeight * nbTilesV;
                    line.StrokeThickness = 1;
                    line.Stroke = colorBrush;
                    CanvasGrid.Children.Add( line );
                }
            }
        }
/*
        private UniformGrid GetGrid( Layer layer )
        {
            if ( layer == null )
                return null;

            UniformGrid grid = null;
            if ( layer.mType == Layer.TYPE_MAIN )
            {
                grid = UniformGridMapMain;
            }
            else if ( layer.mType == Layer.TYPE_VISUALS )
            {
                int visualLayerIndex = mPageVisuals.GetCurrentLayerListIndex();
                int mainLayerIndex = mPageVisuals.GetMainLayerListIndex();
                if (visualLayerIndex < mainLayerIndex)
                {
                    grid = GridVisualLayersBackground.Children[visualLayerIndex] as UniformGrid;
                }
                else
                {
                    grid = GridVisualLayersForeground.Children[visualLayerIndex - (mainLayerIndex + 1)] as UniformGrid;
                }
            }
            else if (mCurrentLayerType == LAYER_COLLISION)
            {
                mCurrentLayer = mLevel.mCollisionLayer;
                mCurrentGrid = UniformGridMapCollision;
            }
            else if (mCurrentLayerType == LAYER_OBJECTS)
            {
                mCurrentLayer = mLevel.mObjectsLayer;
                mCurrentGrid = UniformGridMapObjects;
            }
            else if (mCurrentLayerType == LAYER_HOTSPOTS)
            {
                mCurrentLayer = mLevel.mHotspotsLayer;
                mCurrentGrid = UniformGridMapHotspots;
            }
        }
*/


        // Check that everything is ok & that export can be done
        private bool CheckLevel( out string strErrMsg )
        {
            bool ok = true;
            strErrMsg = "";

            // check there is a background image
            if ( mLevel.mBackground.GetResourceFullPath() == "" )
            {
                ok = false;
                strErrMsg += "- no background image\n";
            }

            // check that every layer has a tilemap defined
            if ( mLayerMaps[ Layer.TYPE_MAIN ].mLayer.mTileMapFilename == "" )
            {
                ok = false;
                strErrMsg += "- main layer has no tile map\n";
            }
// !!!! ???? TODO: is this test necessary ? ???? !!!!
// ( as we are not saving the tilemap - only the "collision.bin" file is needed )
            if ( !mLevel.mCollisionSameAsMain && ( mLayerMaps[ Layer.TYPE_COLLISION ].mLayer.mTileMapFilename == "" ) )
            {
                ok = false;
                strErrMsg += "- collision layer has no tile map\n";
            }
            if ( mLayerMaps[ Layer.TYPE_OBJECTS ].mLayer.mTileMapFilename == "" )
            {
                ok = false;
                strErrMsg += "- objects layer has no tile map\n";
            }
            if ( mLayerMaps[ Layer.TYPE_HOTSPOTS ].mLayer.mTileMapFilename == "" )
            {
                ok = false;
                strErrMsg += "- hotspots layer has no tile map\n";
            }
            for ( int i = Layer.TYPE_VISUALS; i < mLayerMaps.Count; ++i )
            {
                if ( mLayerMaps[ i ].mLayer.mTileMapFilename == "" )
                {
                    ok = false;
                    strErrMsg += "- visual layer " + mLayerMaps[ i ].mLayer.mName + " has no tile map\n";
                }
            }

            return ok;
        }


        private static string GetBaseName( string fullPath )
        {
            string baseName = GetFileName( fullPath );

            int index = baseName.LastIndexOf( "." );
            if ( index != -1 )
            {
                baseName = baseName.Remove( index ); // remove extension
            }

            return baseName;
        }

        private static string GetFileName( string fullPath )
        {
            string fileName = fullPath;

            int index = fileName.LastIndexOf( "\\" );
            if ( index != -1 )
            {
                fileName = fileName.Remove( 0, index + 1 ); // remove path
            }
            return fileName;
        }

        private static string GetPath( string fullPath )
        {
            string path = fullPath;

            int index = path.LastIndexOf( "\\" );
            if ( index != -1 )
            {
                path = path.Remove( index ); // remove end
            }
            return path;
        }
    }
}
