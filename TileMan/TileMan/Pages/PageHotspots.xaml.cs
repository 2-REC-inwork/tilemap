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
    /// Interaction logic for PageHotspots.xaml
    /// </summary>
    public partial class PageHotspots : UserControl
    {
        private MainWindow mMainWindow;
        private LayerObjects mHotspotsLayer;


        public PageHotspots( LayerObjects hotspotsLayer, MainWindow mainWindow )
        {
            InitializeComponent();

            mMainWindow = mainWindow;
            mHotspotsLayer = hotspotsLayer;
        }

        public void Reset()
        {
            TextBoxName.Text = mHotspotsLayer.mResource.mFileName;

            // reset tilemap
            TextHotspotsMap.Clear();

            ListBoxHotspots.Items.Clear();
            mHotspotsLayer.mCurrentTile = -1;

            string filename = Settings.mResourcesPath + Settings.mHotspotsDescriptionFilename;
            if ( !System.IO.File.Exists( filename ) )
            {
                filename = "";
            }
            TextHotspotsMap.Text = filename;
            mMainWindow.LoadTiles( Layer.TYPE_HOTSPOTS, filename );
            UpdateHotspotsMap();

            // resets layers data
//            GenerateGrids(...);
        }

        public void Refresh()
        {
            TextBoxName.Text = mHotspotsLayer.mResource.mFileName;

//?            UpdateHotspotsMap();
        }

        private void ButtonLoadLayer_Click( object sender, RoutedEventArgs e )
        {
            if ( mHotspotsLayer.mTileMapFilename == "" )
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
                mMainWindow.LoadLayerResource( Layer.TYPE_HOTSPOTS, dlg.FileName );
                Refresh();
            }
        }

        private void ButtonExportLayer_Click( object sender, RoutedEventArgs e )
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
//            dlg.Owner = this;
            dlg.FileName = mHotspotsLayer.mResource.mFileName;
            dlg.DefaultExt = ".bin";
            dlg.Filter = "BIN Files (*.bin)|*.bin";

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                mMainWindow.SaveLayerResource( Layer.TYPE_HOTSPOTS, dlg.FileName );
//                Refresh();
                string filename = dlg.FileName;
                filename = filename.Remove( 0, filename.LastIndexOf( "\\" ) + 1 ); // remove path
                filename = filename.Remove( filename.LastIndexOf( "." ) ); // remove extension
                TextBoxName.Text = filename;
            }
        }

        private void ButtonClearLayer_Click( object sender, RoutedEventArgs e )
        {
            mMainWindow.ClearLayer( Layer.TYPE_HOTSPOTS );
        }

        private void ButtonHotspotsMap_Click( object sender, RoutedEventArgs e )
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
//            dlg.Owner = this;
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (*.xml)|*.xml";

            Nullable<bool> result = dlg.ShowDialog();

            if ( result == true )
            {
                TextHotspotsMap.Text = dlg.FileName;
                mMainWindow.LoadTiles( Layer.TYPE_HOTSPOTS, dlg.FileName );

                UpdateHotspotsMap();
            }
        }

        private void ListBoxHotspots_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            int item = ListBoxHotspots.SelectedIndex;

            if ( item == -1 )
            {
                LabelHotspotName.Content = "";
                LabelHotspotType.Content = "";
                TextBoxHotspotDesc.Text = "";

                ImageHotspotIcon.Source = null;
//                ImageHotspotIcon.Width = image.PixelWidth;
//                ImageHotspotIcon.Height = image.PixelHeight;
            }
            else
            {
                ObjectDescription objDesc = mHotspotsLayer.mObjects[ item ];
                LabelHotspotName.Content = objDesc.mName;
                LabelHotspotType.Content = objDesc.mType;
                TextBoxHotspotDesc.Text = objDesc.mDescription;

                CroppedBitmap image = mHotspotsLayer.mTiles[ item ];
                ImageHotspotIcon.Source = image;
                ImageHotspotIcon.Width = image.PixelWidth;
                ImageHotspotIcon.Height = image.PixelHeight;

                mHotspotsLayer.mCurrentTile = item;
            }
        }


        public void UpdateTilesNb( int nbTilesH, int nbTilesV, int horizOrient, int verticOrient )
        {
            mHotspotsLayer.Resize( nbTilesH, nbTilesV, horizOrient, verticOrient );
        }

        public void UpdateTileSizes( int tileWidth, int tileHeight )
        {
            mHotspotsLayer.SetTilesSizes( tileWidth, tileHeight );

            UpdateHotspotsMap();
        }

        public void UpdateHotspotsMap()
        {
            // remove all list entries
            ListBoxHotspots.Items.Clear();
// !!!! TODO: set "selected tile" as null !!!!

            for ( int i = 0; i < mHotspotsLayer.mObjects.Count; ++i )
            {
                ListBoxHotspots.Items.Add( mHotspotsLayer.mObjects[ i ].mName );
            }
        }

    }
}
