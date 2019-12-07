using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

using TileMan.Components;
using TileMan.Dialogs;


namespace TileMan.Pages
{
    /// <summary>
    /// Interaction logic for PageMain.xaml
    /// </summary>
    public partial class PageMain : UserControl
    {
        private BackgroundScreenRatio mBkgScreenRatio;
        private BackgroundImageRatio mBkgImageRatio;

        private MainWindow mMainWindow;
        private Layer mMainLayer;
        private Background mBackground;


        public PageMain( Layer mainLayer, Background background, MainWindow mainWindow )
        {
            this.InitializeComponent();

            mBkgScreenRatio = new BackgroundScreenRatio( background );
            mBkgImageRatio = new BackgroundImageRatio( background );

            ComboBoxSizeConstraint.Items.Add( "screen" );
            ComboBoxSizeConstraint.Items.Add( "image" );

            ComboBoxSizeConstraint.SelectedItem = "screen";
            SetPage( mBkgScreenRatio );

            mMainLayer = mainLayer;
            mBackground = background;
            mMainWindow = mainWindow;
        }

        public void Reset()
        {
// !!!! TODO: unload image ... !!!!
            mBackground.Reset();
            TextBkgImage.Clear();
            mBkgScreenRatio.Reset();
            mBkgImageRatio.Reset();

            TextBoxName.Text = mMainLayer.mResource.mFileName;

            TextBoxNbTilesH.Text = mMainLayer.mResource.mNbTilesH.ToString();
            TextBoxNbTilesV.Text = mMainLayer.mResource.mNbTilesV.ToString();

            TextBoxTilesWidth.Text = mMainLayer.mTileWidth.ToString();
            TextBoxTilesHeight.Text = mMainLayer.mTileHeight.ToString();

            // reset tilemap
            TextTileMap.Clear();
            UniformGridTiles.Children.Clear();

            // resets layers data
//            GenerateGrids(...);
        }

        public void SetPage( UserControl page )
        {
            ContentSizeConstraint.Content = page;
        }

        public void Refresh()
        {
            TextBoxName.Text = mMainLayer.mResource.mFileName;

            TextBoxNbTilesH.Text = mMainLayer.mResource.mNbTilesH.ToString();
            TextBoxNbTilesV.Text = mMainLayer.mResource.mNbTilesV.ToString();

            TextBoxTilesWidth.Text = mMainLayer.mTileWidth.ToString();
            TextBoxTilesHeight.Text = mMainLayer.mTileHeight.ToString();

//?            UpdateTileMap();
        }

        private void ComboBoxSizeConstraint_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            if ( ComboBoxSizeConstraint.SelectedItem.Equals( "screen" ) )
            {
                SetPage( mBkgScreenRatio );
                if ( mBackground != null )
                {
                    mBackground.SetUseImageRatio( false );
                }
            }
            else if ( ComboBoxSizeConstraint.SelectedItem.Equals( "image" ) )
            {
                SetPage( mBkgImageRatio );
                if ( mBackground != null )
                {
                    mBackground.SetUseImageRatio( true );
                }
            }
        }

        private void ButtonBkgImage_Click( object sender, RoutedEventArgs e )
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            Nullable<bool> result = dlg.ShowDialog();

            if ( result == true )
            {
                mBackground.SetResource( dlg.FileName );
                TextBkgImage.Text = dlg.FileName;
// !!!! TODO: load image ... & do something with it ... !!!!
// => set width & height in mBackground
            }
        }

        private void ButtonLoadLayer_Click( object sender, RoutedEventArgs e )
        {
// !!!! TODO: need to check that all dependent layers have a filename !!!!
            if ( mMainLayer.mTileMapFilename == "" )
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
                mMainWindow.LoadLayerResource( Layer.TYPE_MAIN, dlg.FileName );
                Refresh();
            }
        }

        private void ButtonExportLayer_Click( object sender, RoutedEventArgs e )
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = mMainLayer.mResource.mFileName;
            dlg.DefaultExt = ".bin";
            dlg.Filter = "BIN Files (*.bin)|*.bin";

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                mMainWindow.SaveLayerResource( Layer.TYPE_MAIN, dlg.FileName );
//                Refresh();
                string filename = dlg.FileName;
                filename = filename.Remove( 0, filename.LastIndexOf( "\\" ) + 1 ); // remove path
                filename = filename.Remove( filename.LastIndexOf( "." ) ); // remove extension
                TextBoxName.Text = filename;
            }
        }

        private void ButtonClearLayer_Click( object sender, RoutedEventArgs e )
        {
//            mMainWindow.ClearLayer( Layer.TYPE_MAIN );
            mMainWindow.ClearMainLayer();
        }

        private void ButtonResizeMap_Click( object sender, RoutedEventArgs e )
        {
            DialogResizeMap dlg = new DialogResizeMap( mMainLayer.mResource.mNbTilesH, mMainLayer.mResource.mNbTilesV );
//            dlg.Owner = this;

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                int horizOrient = LayerResource.ORIENTATION_CENTRE;
                if ( dlg.RadioHLeft.IsChecked == true )
                    horizOrient = LayerResource.ORIENTATION_LEFT;
                else if ( dlg.RadioHRight.IsChecked == true )
                    horizOrient = LayerResource.ORIENTATION_RIGHT;

                int verticOrient = LayerResource.ORIENTATION_CENTRE;
                if ( dlg.RadioVLeft.IsChecked == true )
                    verticOrient = LayerResource.ORIENTATION_LEFT;
                else if ( dlg.RadioVRight.IsChecked == true )
                    verticOrient = LayerResource.ORIENTATION_RIGHT;

                mMainWindow.UpdateTilesNb( dlg.NbTilesH, dlg.NbTilesV, horizOrient, verticOrient );

////////
// !!!! TODO: remove that & find another way ... !!!!
// ( horrible hack )
//                mMainWindow.ActivateCurrentLayer( Layer.TYPE_MAIN );
////////
                Refresh();
            }
        }

        private void ButtonResizeTiles_Click( object sender, RoutedEventArgs e )
        {
            DialogResizeTiles dlg = new DialogResizeTiles( mMainLayer.mTileWidth, mMainLayer.mTileHeight );
//            dlg.Owner = this;

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                mMainWindow.UpdateTileSizes( dlg.TileWidth, dlg.TileHeight );
            }
        }

        private void ButtonTileMap_Click( object sender, RoutedEventArgs e )
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            Nullable<bool> result = dlg.ShowDialog();

            if ( result == true )
            {
                TextTileMap.Text = dlg.FileName;
                mMainWindow.LoadTiles( Layer.TYPE_MAIN, dlg.FileName );

                UpdateTileMap();
            }
        }

        private void ButtonTile_Click( object sender, RoutedEventArgs e )
        {
            Button button = sender as Button;

            for ( int i = 0; i < UniformGridTiles.Children.Count; ++i )
            {
                Button btn = UniformGridTiles.Children[ i ] as Button;
                btn.BorderBrush = Brushes.Transparent;
            }

            button.BorderBrush = Brushes.Red;

            mMainLayer.mCurrentTile = UniformGridTiles.Children.IndexOf( button );
        }



        public void UpdateTilesNb( int nbTilesH, int nbTilesV, int horizOrient, int verticOrient )
        {
            TextBoxNbTilesH.Text = nbTilesH.ToString();
            TextBoxNbTilesV.Text = nbTilesV.ToString();

            mMainLayer.Resize( nbTilesH, nbTilesV, horizOrient, verticOrient );
        }

        public void UpdateTileSizes( int tileWidth, int tileHeight )
        {
            TextBoxTilesWidth.Text = tileWidth.ToString();
            TextBoxTilesHeight.Text = tileHeight.ToString();

            mMainLayer.SetTilesSizes( tileWidth, tileHeight );

            UpdateTileMap();
        }

        public void UpdateTileMap()
        {
            // remove all current buttons
            UniformGridTiles.Children.Clear();
// !!!! TODO: set "selected tile" as null !!!!

            if ( mMainLayer.mTileMap != null )
            {
                Thickness padding = new Thickness( -1, -1, -1, -1 );
                Thickness border = new Thickness( 2, 2, 2, 2 );

                UniformGridTiles.Columns = mMainLayer.mTileMap.PixelWidth / mMainLayer.mTileWidth;

                Button button;
                Image image;
                for ( int i = 0; i < mMainLayer.mNbTiles; ++i )
                {
                    image = new Image();
                    image.Source = mMainLayer.mTiles[ i ];

                    button = new Button();
                    button.Background = Brushes.Transparent;
                    button.BorderBrush = Brushes.Transparent;
                    button.Padding = padding;
                    button.BorderThickness = border;
                    button.Content = image;
                    button.Width = mMainLayer.mTileWidth;
                    button.Height = mMainLayer.mTileHeight;
                    button.Click += ButtonTile_Click;
                    UniformGridTiles.Children.Add( button );
                }
                button = null;
                image = null;
            }
        }

        public void UpdateBackground()
        {
            TextBkgImage.Text = mBackground.GetResourceFullPath();

            mBkgScreenRatio.SliderSizeFactorX.Value = mBackground.GetSizeFactorX();
            mBkgScreenRatio.SliderSizeFactorY.Value = mBackground.GetSizeFactorY();

            mBkgImageRatio.SliderWidth.Value = mBackground.GetWidth();
            mBkgImageRatio.SliderHeight.Value = mBackground.GetHeight();

            if ( mBackground.GetRatioKeepWidth() )
            {
                mBkgImageRatio.RadioButtonWidth.IsChecked = true;
                mBkgImageRatio.RadioButtonHeight.IsChecked = false;
                mBkgImageRatio.SliderSizeFactor.Value = mBackground.GetSizeFactorX();
            }
            else
            {
                mBkgImageRatio.RadioButtonWidth.IsChecked = false;
                mBkgImageRatio.RadioButtonHeight.IsChecked = true;
                mBkgImageRatio.SliderSizeFactor.Value = mBackground.GetSizeFactorY();
            }

            if ( mBackground.GetUseImageRatio() )
            {
                ComboBoxSizeConstraint.SelectedItem = "image";
            }
            else
            {
                ComboBoxSizeConstraint.SelectedItem = "screen";
            }

// !!!! TODO: load image ... & do something with it ... !!!!
// => set width & height in mBackground ?
        }

    }
}
