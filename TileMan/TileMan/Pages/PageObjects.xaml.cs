using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TileMan.Components;
using TileMan.Dialogs;


namespace TileMan.Pages
{
    /// <summary>
    /// Interaction logic for PageObjects.xaml
    /// </summary>
    public partial class PageObjects : UserControl
    {
        private MainWindow mMainWindow;
        private LayerObjects mObjectsLayer;


        public PageObjects( LayerObjects objectsLayer, MainWindow mainWindow )
        {
            InitializeComponent();

            mMainWindow = mainWindow;
            mObjectsLayer = objectsLayer;
        }

        public void Reset()
        {
            TextBoxName.Text = mObjectsLayer.mResource.mFileName;

            // reset tilemap
            TextObjectsMap.Clear();

            ListBoxObjects.Items.Clear();
            mObjectsLayer.mCurrentTile = -1;

            string filename = Settings.mResourcesPath + Settings.mObjectsDescriptionFilename;
            if ( !System.IO.File.Exists( filename ) )
            {
                filename = "";
            }
            TextObjectsMap.Text = filename;
            mMainWindow.LoadTiles( Layer.TYPE_OBJECTS, filename );
            UpdateObjectsMap();

            // resets layers data
//            GenerateGrids(...);
        }

        public void Refresh()
        {
            TextBoxName.Text = mObjectsLayer.mResource.mFileName;

//?            UpdateObjectsMap();
        }

        private void ButtonLoadLayer_Click( object sender, RoutedEventArgs e )
        {
            if ( mObjectsLayer.mTileMapFilename == "" )
            {
                MessageBox.Show( "Please select a tilemap before loading layer resource." );
                return;
            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
//            dlg.Owner = this;
            dlg.DefaultExt = ".bin";
            dlg.Filter = "BIN Files (*.bin)|*.bin";

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                mMainWindow.LoadLayerResource( Layer.TYPE_OBJECTS, dlg.FileName );
                Refresh();
            }
        }

        private void ButtonExportLayer_Click( object sender, RoutedEventArgs e )
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
//            dlg.Owner = this;
            dlg.FileName = mObjectsLayer.mResource.mFileName;
            dlg.DefaultExt = ".bin";
            dlg.Filter = "BIN Files (*.bin)|*.bin";

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                mMainWindow.SaveLayerResource( Layer.TYPE_OBJECTS, dlg.FileName );
//                Refresh();
                string filename = dlg.FileName;
                filename = filename.Remove( 0, filename.LastIndexOf( "\\" ) + 1 ); // remove path
                filename = filename.Remove( filename.LastIndexOf( "." ) ); // remove extension
                TextBoxName.Text = filename;
            }
        }

        private void ButtonClearLayer_Click( object sender, RoutedEventArgs e )
        {
            mMainWindow.ClearLayer( Layer.TYPE_OBJECTS );
        }

        private void ButtonObjectsMap_Click( object sender, RoutedEventArgs e )
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
//            dlg.Owner = this;
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (*.xml)|*.xml";

            Nullable<bool> result = dlg.ShowDialog();

            if ( result == true )
            {
                TextObjectsMap.Text = dlg.FileName;
                mMainWindow.LoadTiles( Layer.TYPE_OBJECTS, dlg.FileName );

                UpdateObjectsMap();
            }
        }

        private void ListBoxObjects_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            int item = ListBoxObjects.SelectedIndex;

            if ( item == -1 )
            {
                LabelObjectName.Content = "";
                LabelObjectType.Content = "";
                TextBoxObjectDesc.Text = "";

                ImageObjectIcon.Source = null;
//                ImageObjectIcon.Width = image.PixelWidth;
//                ImageObjectIcon.Height = image.PixelHeight;
            }
            else
            {
                ObjectDescription objDesc = mObjectsLayer.mObjects[ item ];
                LabelObjectName.Content = objDesc.mName;
                LabelObjectType.Content = objDesc.mType;
                TextBoxObjectDesc.Text = objDesc.mDescription;

                CroppedBitmap image = mObjectsLayer.mTiles[ item ];
                ImageObjectIcon.Source = image;
                ImageObjectIcon.Width = image.PixelWidth;
                ImageObjectIcon.Height = image.PixelHeight;

                mObjectsLayer.mCurrentTile = item;
            }
        }


        public void UpdateTilesNb( int nbTilesH, int nbTilesV, int horizOrient, int verticOrient )
        {
            mObjectsLayer.Resize( nbTilesH, nbTilesV, horizOrient, verticOrient );
        }

        public void UpdateTileSizes( int tileWidth, int tileHeight )
        {
            mObjectsLayer.SetTilesSizes( tileWidth, tileHeight );

            UpdateObjectsMap();
        }

        public void UpdateObjectsMap()
        {
            // remove all list entries
            ListBoxObjects.Items.Clear();
// !!!! TODO: set "selected tile" as null !!!!

            for ( int i = 0; i < mObjectsLayer.mObjects.Count; ++i )
            {
                ListBoxObjects.Items.Add( mObjectsLayer.mObjects[ i ].mName );
            }
        }

    }
}
