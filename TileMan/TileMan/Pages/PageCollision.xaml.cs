using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TileMan.Components;
using TileMan.Dialogs;

/*
 !!!! TODO: could have a different tile size for the collision layer !!!!
  => change stuff between "COLLISION_SIZE" tags

*/

namespace TileMan.Pages
{
    /// <summary>
    /// Interaction logic for PageCollision.xaml
    /// </summary>
    public partial class PageCollision : UserControl
    {
        private MainWindow mMainWindow;
        private Layer mCollisionLayer;

        public PageCollision( Layer collisionLayer, MainWindow mainWindow )
        {
            InitializeComponent();

            mMainWindow = mainWindow;
            mCollisionLayer = collisionLayer;
        }

        public void Reset()
        {
            CheckBoxAsMain.IsChecked = true;

            GroupBoxPageCollision.Visibility = Visibility.Collapsed;
            GroupBoxTileMap.Visibility = Visibility.Collapsed;

            TextBoxName.Text = mCollisionLayer.mResource.mFileName;

            // reset tilemap
            TextTileMap.Clear();
            UniformGridTiles.Children.Clear();
            mCollisionLayer.mCurrentTile = -1;

            string filename = Settings.mResourcesPath + Settings.mCollisionTilesFilename;
            if ( !System.IO.File.Exists( filename ) )
            {
                filename = "";
            }
            TextTileMap.Text = filename;
            mMainWindow.LoadTiles( Layer.TYPE_COLLISION, filename );
            UpdateTileMap();

            // resets layers data
//            GenerateGrids(...);
        }

        public void Refresh()
        {
            TextBoxName.Text = mCollisionLayer.mResource.mFileName;

//?            UpdateTileMap();
        }

        private void CheckBoxAsMain_Checked( object sender, RoutedEventArgs e )
        {
            GroupBoxPageCollision.Visibility = Visibility.Collapsed;
            GroupBoxTileMap.Visibility = Visibility.Collapsed;

            mMainWindow.SetCollisionSameAsMain( true );
        }

        private void CheckBoxAsMain_Unchecked( object sender, RoutedEventArgs e )
        {
            GroupBoxPageCollision.Visibility = Visibility.Visible;
            GroupBoxTileMap.Visibility = Visibility.Visible;

            mMainWindow.SetCollisionSameAsMain( false );
        }

        private void ButtonLoadLayer_Click( object sender, RoutedEventArgs e )
        {
            if ( mCollisionLayer.mTileMapFilename == "" )
            {
                MessageBox.Show( "Please select a tilemap before loading layer resource." );
                return;
            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".bin";
            dlg.Filter = "BIN Files (*.bin)|*.bin";

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                mMainWindow.LoadLayerResource( Layer.TYPE_COLLISION, dlg.FileName );
                Refresh();
            }
        }

        private void ButtonExportLayer_Click( object sender, RoutedEventArgs e )
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.FileName = mCollisionLayer.mResource.mFileName;
            dlg.DefaultExt = ".bin";
            dlg.Filter = "BIN Files (*.bin)|*.bin";

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                mMainWindow.SaveLayerResource( Layer.TYPE_COLLISION, dlg.FileName );
//                Refresh();
                string filename = dlg.FileName;
                filename = filename.Remove( 0, filename.LastIndexOf( "\\" ) + 1 ); // remove path
                filename = filename.Remove( filename.LastIndexOf( "." ) ); // remove extension
                TextBoxName.Text = filename;
            }
        }

        private void ButtonClearLayer_Click( object sender, RoutedEventArgs e )
        {
            mMainWindow.ClearLayer( Layer.TYPE_COLLISION );
        }

        private void ButtonTileMap_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            Nullable<bool> result = dlg.ShowDialog();

            if ( result == true )
            {
                TextTileMap.Text = dlg.FileName;
                mMainWindow.LoadTiles( Layer.TYPE_COLLISION, dlg.FileName );

                UpdateTileMap();
            }
        }

        private void ButtonTile_Click( object sender, System.Windows.RoutedEventArgs e )
        {
            Button button = sender as Button;

            for ( int i = 0; i < UniformGridTiles.Children.Count; ++i )
            {
                Button btn = UniformGridTiles.Children[ i ] as Button;
                btn.BorderBrush = Brushes.Transparent;
            }

            button.BorderBrush = Brushes.Red;

            mCollisionLayer.mCurrentTile = UniformGridTiles.Children.IndexOf( button );
        }

        public void UpdateTilesNb( int nbTilesH, int nbTilesV, int horizOrient, int verticOrient )
        {
            mCollisionLayer.Resize( nbTilesH, nbTilesV, horizOrient, verticOrient );
        }

        public void UpdateTileSizes( int tileWidth, int tileHeight )
        {
            mCollisionLayer.SetTilesSizes( tileWidth, tileHeight );

            UpdateTileMap();
        }

        public void UpdateTileMap()
        {
            // remove all current buttons
            UniformGridTiles.Children.Clear();
// !!!! TODO: set "selected tile" as null !!!!

            if ( mCollisionLayer.mTileMap != null )
            {
                Thickness padding = new Thickness( -1, -1, -1, -1 );
                Thickness border = new Thickness( 2, 2, 2, 2 );

                UniformGridTiles.Columns = mCollisionLayer.mTileMap.PixelWidth / mCollisionLayer.mTileWidth;

                Button button;
                Image image;
                for ( int i = 0; i < mCollisionLayer.mNbTiles; ++i )
                {
                    image = new Image();
                    image.Source = mCollisionLayer.mTiles[ i ];

                    button = new Button();
                    button.Background = Brushes.Transparent;
                    button.BorderBrush = Brushes.Transparent;
                    button.Padding = padding;
                    button.BorderThickness = border;
                    button.Content = image;
                    button.Width = mCollisionLayer.mTileWidth;
                    button.Height = mCollisionLayer.mTileHeight;
                    button.Click += ButtonTile_Click;
                    UniformGridTiles.Children.Add( button );
                }
                button = null;
                image = null;
            }
        }

    }
}
