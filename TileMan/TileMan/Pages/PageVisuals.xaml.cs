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
    /// Interaction logic for PageVisuals.xaml
    /// </summary>
    public partial class PageVisuals : UserControl
    {
        private MainWindow mMainWindow;

        private Layer mMainLayer;
        private System.Collections.Generic.List<LayerVisual> mVisualLayers;

        private int mNbLayers;
        private int mCurrentLayer;

        private System.Collections.Generic.List<int> mLayersIndex;
        private int mCurrentLayerItemIndex;

        private int mNewLayerCount;


        public PageVisuals( Layer mainLayer, System.Collections.Generic.List<LayerVisual> visualLayers, MainWindow mainWindow )
        {
            this.InitializeComponent();

            mMainWindow = mainWindow;

            mMainLayer = mainLayer;
            mVisualLayers = visualLayers;

            mLayersIndex = new System.Collections.Generic.List<int>();
        }

        public void Reset()
        {
            // remove entries in list
            ListBoxLayers.Items.Clear();

            // create Main Layer entry
            ListBoxLayers.Items.Add( mMainLayer.mName );

            mNbLayers = 0;

            mLayersIndex.Clear();
            mLayersIndex.Add( -1 ); // for "main layer"
            mCurrentLayerItemIndex = 0;
            ListBoxLayers.SelectedIndex = 0;
            mCurrentLayer = -1;
            mNewLayerCount = 0;

            TextBoxName.Clear();

            TextBoxNbTilesH.Text = mMainLayer.mResource.mNbTilesH.ToString();
            TextBoxNbTilesV.Text = mMainLayer.mResource.mNbTilesV.ToString();

            TextBoxTilesWidth.Text = mMainLayer.mTileWidth.ToString();
            TextBoxTilesHeight.Text = mMainLayer.mTileHeight.ToString();

            TextBoxSizeFactorX.Text = ( 1.0 ).ToString();
            TextBoxSizeFactorY.Text = ( 1.0 ).ToString();

            TextBoxSpeedX.Text = ( 0.0 ).ToString();
            TextBoxSpeedY.Text = ( 0.0 ).ToString();


            // reset tilemap
            TextTileMap.Clear();
            UniformGridTiles.Children.Clear();

            // resets layers data
//            GenerateGrids(...);

        }

        private void ListBoxLayers_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            mCurrentLayerItemIndex = ListBoxLayers.SelectedIndex;
            if ( mCurrentLayerItemIndex != -1 )
            {
                mCurrentLayer = mLayersIndex[ mCurrentLayerItemIndex ];
            }
            else
            {
                mCurrentLayer = -1;
            }

            Refresh();

            if ( mCurrentLayer == -1 )
            {
                mMainWindow.ActivateCurrentLayer( Layer.TYPE_NONE );
            }
            else
            {
                mMainWindow.ActivateCurrentLayer( Layer.TYPE_VISUALS );
            }
        }

        public void Refresh()
        {
            if ( mCurrentLayer == -1 )
            {
                GroupBoxPageVisualLayer.Visibility = Visibility.Collapsed;
                GroupBoxTileMap.Visibility = Visibility.Collapsed;

                TextBoxName.Text = mMainLayer.mResource.mFileName;

                TextBoxNbTilesH.Text = mMainLayer.mResource.mNbTilesH.ToString();
                TextBoxNbTilesV.Text = mMainLayer.mResource.mNbTilesV.ToString();

                TextBoxTilesWidth.Text = mMainLayer.mTileWidth.ToString();
                TextBoxTilesHeight.Text = mMainLayer.mTileHeight.ToString();

                TextBoxSizeFactorX.Text = ( 1.0 ).ToString();
                TextBoxSizeFactorY.Text = ( 1.0 ).ToString();

                TextBoxSpeedX.Text = ( 0.0 ).ToString();
                TextBoxSpeedY.Text = ( 0.0 ).ToString();

                // reset tilemap
                UniformGridTiles.Children.Clear();
                TextTileMap.Clear();
            }
            else
            {
                GroupBoxPageVisualLayer.Visibility = Visibility.Visible;
                GroupBoxTileMap.Visibility = Visibility.Visible;

                LayerVisual layer = mVisualLayers[ mCurrentLayer ];
                TextBoxName.Text = layer.mResource.mFileName;

                TextBoxNbTilesH.Text = layer.mResource.mNbTilesH.ToString();
                TextBoxNbTilesV.Text = layer.mResource.mNbTilesV.ToString();

                TextBoxTilesWidth.Text = layer.mTileWidth.ToString();
                TextBoxTilesHeight.Text = layer.mTileHeight.ToString();

                TextBoxSizeFactorX.Text = layer.mSizeFactorX.ToString();
                TextBoxSizeFactorY.Text = layer.mSizeFactorY.ToString();

                TextBoxSpeedX.Text = layer.mSpeedX.ToString();
                TextBoxSpeedY.Text = layer.mSpeedY.ToString();

// !!!! ???? TODO: better to clear & regenerate tilemap every time or to keep all of them in memory ? ???? !!!!
                // reset tilemap
                UniformGridTiles.Children.Clear();
                TextTileMap.Text = layer.mTileMapFilename;
                UpdateTileMap();
            }
        }

        private void ButtonAdd_Click( object sender, RoutedEventArgs e )
        {
            // get values
            int nbTilesH = Convert.ToInt32( TextBoxNbTilesH.Text );
            int nbTilesV = Convert.ToInt32( TextBoxNbTilesV.Text );
            int tileWidth = Convert.ToInt32( TextBoxTilesWidth.Text );
            int tileHeight = Convert.ToInt32( TextBoxTilesHeight.Text );


//////// NEW_LAYER 20140320 - BEGIN
/*
            // create new layer
            LayerVisual layer = new LayerVisual();
            layer.Reset( "layer" + mNewLayerCount.ToString(), Layer.TYPE_VISUALS, nbTilesH, nbTilesV, tileWidth, tileHeight );
            ++mNewLayerCount;

            mVisualLayers.Add( layer );
            ++mNbLayers;

            ++mCurrentLayerItemIndex; // new layer will be next one from current in the list

            // add layer in layer maps & create associated grid
            mMainWindow.AddVisualLayer( mNbLayers - 1, mCurrentLayerItemIndex );


////////
// !!!! TODO: find a way to remove that !!!!
//=> horrible hack to fix the size of the ListBox ...
            double height = ListBoxLayers.ActualHeight;
            ListBoxLayers.Height = height;
            ListBoxLayers.MaxHeight = height;
            ListBoxLayers.MinHeight = height;
////////

            // add & select new line after current selection
            mLayersIndex.Insert( mCurrentLayerItemIndex, mNbLayers - 1 );
            ListBoxLayers.Items.Insert( mCurrentLayerItemIndex, layer.mName );
            ListBoxLayers.SelectedIndex = mCurrentLayerItemIndex;
*/
//////// NEW_LAYER 20140320 - MID
            // create new layer
            AddNewLayer( nbTilesH, nbTilesV, tileWidth, tileHeight );
//////// NEW_LAYER 20140320 - END
        }

        private void ButtonCopy_Click( object sender, RoutedEventArgs e )
        {
            DialogLayerCopy dlg = new DialogLayerCopy();
//            dlg.Owner = this;

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                Layer originalLayer = mMainLayer;
                if ( mCurrentLayer != -1 ) // not main layer
                {
                    originalLayer = mVisualLayers[ mCurrentLayer ];
                }

                // get values
                bool share = ( dlg.RadioButtonShared.IsChecked == true );
// useless
//                int nbTilesH = originalLayer.mResource.mNbTilesH;
//                int nbTilesV = originalLayer.mResource.mNbTilesV;
//                int tileWidth = originalLayer.mTileWidth;
//                int tileHeight = originalLayer.mTileHeight;

//////// NEW_LAYER 20140320 - BEGIN
/*
                // create new layer
                LayerVisual layer = new LayerVisual();
                string name = originalLayer.mName;
                if ( !name.Contains( "_copy" ) )
                {
                    name += "_copy";
                }
                name += mNewLayerCount.ToString();
                ++mNewLayerCount;
                layer.Copy( originalLayer, name, share );

                mVisualLayers.Add( layer );
                ++mNbLayers;

                ++mCurrentLayerItemIndex; // new layer will be next one from current in the list

                // add layer in layer maps & create associated grid
                mMainWindow.AddVisualLayer( mNbLayers - 1, mCurrentLayerItemIndex );

                if ( share )
                {
// !!!! ???? TODO: OK ? ???? !!!!
                    mMainWindow.AddDependencyLayer( mNbLayers - 1, mCurrentLayer ); // dependency between new layer and previous current layer
                }

////////
// !!!! TODO: find a way to remove that !!!!
//=> horrible hack to fix the size of the ListBox ...
                double height = ListBoxLayers.ActualHeight;
                ListBoxLayers.Height = height;
                ListBoxLayers.MaxHeight = height;
                ListBoxLayers.MinHeight = height;
////////


                // add & select new line after current selection
                mLayersIndex.Insert( mCurrentLayerItemIndex, mNbLayers - 1 );
                ListBoxLayers.Items.Insert( mCurrentLayerItemIndex, layer.mName );
                ListBoxLayers.SelectedIndex = mCurrentLayerItemIndex;
*/
//////// NEW_LAYER 20140320 - MID
                // create new layer
                CopyNewLayer( originalLayer, share );
//////// NEW_LAYER 20140320 - END


                mMainWindow.LoadTiles( Layer.TYPE_VISUALS + mCurrentLayer, originalLayer.mTileMapFilename );

                UpdateTileMap();
            }
        }

        private void ButtonRemove_Click( object sender, RoutedEventArgs e )
        {
            if ( mCurrentLayer != -1 )
            {
                int index = mCurrentLayerItemIndex;
                int layerIndex = mCurrentLayer;

                // remove layer
                mVisualLayers.RemoveAt( layerIndex );
                --mNbLayers;

                // remove layer from visualisation grids
                mMainWindow.RemoveVisualLayer( layerIndex, index );

                // remove element from list
                ListBoxLayers.Items.RemoveAt( index );
                mLayersIndex.RemoveAt( index );

                // update indices
                for ( int i = 0; i <= mNbLayers; ++i ) // '=' because there is the "main layer"
                {
                    if ( mLayersIndex[ i ] > layerIndex )
                    {
                        --mLayersIndex[ i ];
                    }
                }

                // select next entry ( or last )
                mCurrentLayerItemIndex = index;
                if ( mCurrentLayerItemIndex > mNbLayers )
                {
                    --mCurrentLayerItemIndex;
                }
                ListBoxLayers.SelectedIndex = mCurrentLayerItemIndex;
            }
        }

        private void ButtonMoveUp_Click( object sender, RoutedEventArgs e )
        {
//////// NEW_LAYER 20140320 - BEGIN
/*
            if ( mCurrentLayerItemIndex > 0 )
            {
                int index = mCurrentLayerItemIndex;
                int layerIndex = mCurrentLayer;
                string layerName = ListBoxLayers.Items[ index ].ToString();

                // move layer backward in visualisation grids
                mMainWindow.MoveVisualLayerBackward( index );

                // move entry in list
                ListBoxLayers.Items.RemoveAt( index );
                ListBoxLayers.Items.Insert( index - 1, layerName );

                // move entry in index
                mLayersIndex.RemoveAt( index );
                mLayersIndex.Insert( index - 1, layerIndex );

                ListBoxLayers.SelectedIndex = index - 1;
            }
*/
//////// NEW_LAYER 20140320 - MID
            MoveCurrentLayerUp();
//////// NEW_LAYER 20140320 - END
        }

        private void ButtonMoveDown_Click( object sender, RoutedEventArgs e )
        {
//////// NEW_LAYER 20140320 - BEGIN
/*
            if ( mCurrentLayerItemIndex < mNbLayers ) // consider "main layer"
            {
                int index = mCurrentLayerItemIndex;
                int layerIndex = mCurrentLayer;
                string layerName = ListBoxLayers.Items[ index ].ToString();

                // move layer forward in visualisation grids
                mMainWindow.MoveVisualLayerForward( index );

                // move entry in list
                ListBoxLayers.Items.RemoveAt( index );
                ListBoxLayers.Items.Insert( index + 1, layerName );

                // move entry in index
                mLayersIndex.RemoveAt( index );
                mLayersIndex.Insert( index + 1, layerIndex );

                ListBoxLayers.SelectedIndex = index + 1;
            }
*/
//////// NEW_LAYER 20140320 - MID
            MoveCurrentLayerDown();
//////// NEW_LAYER 20140320 - END
        }

        private void ButtonLoadLayer_Click( object sender, RoutedEventArgs e )
        {
            if ( mVisualLayers[ mCurrentLayer ].mTileMapFilename == "" )
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
                mMainWindow.LoadLayerResource( Layer.TYPE_VISUALS + mCurrentLayer, dlg.FileName );
                Refresh();
            }
        }

        private void ButtonExportLayer_Click( object sender, RoutedEventArgs e )
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.FileName = mVisualLayers[ mCurrentLayer ].mResource.mFileName;
            dlg.DefaultExt = ".bin";
            dlg.Filter = "BIN Files (*.bin)|*.bin";

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                mMainWindow.SaveLayerResource( Layer.TYPE_VISUALS + mCurrentLayer, dlg.FileName );
//                Refresh();
                string filename = dlg.FileName;
                filename = filename.Remove( 0, filename.LastIndexOf( "\\" ) + 1 ); // remove path
                filename = filename.Remove( filename.LastIndexOf( "." ) ); // remove extension
                TextBoxName.Text = filename;
            }
        }

        private void ButtonClearLayer_Click( object sender, RoutedEventArgs e )
        {
            if ( mCurrentLayer != -1 )
            {
                mMainWindow.ClearLayer( Layer.TYPE_VISUALS + mCurrentLayer );
            }
        }

        private void ButtonResizeMap_Click( object sender, RoutedEventArgs e )
        {
            DialogResizeMap dlg = new DialogResizeMap( mVisualLayers[ mCurrentLayer ].mResource.mNbTilesH,
                                                       mVisualLayers[ mCurrentLayer ].mResource.mNbTilesV );
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

                mMainWindow.UpdateVisualLayerTilesNb( dlg.NbTilesH, dlg.NbTilesV, horizOrient, verticOrient );
                Refresh();
            }
        }

        private void ButtonResizeTiles_Click( object sender, RoutedEventArgs e )
        {
            DialogResizeTiles dlg = new DialogResizeTiles( mVisualLayers[ mCurrentLayer ].mTileWidth,
                                                           mVisualLayers[ mCurrentLayer ].mTileHeight );
//            dlg.Owner = this;

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                mMainWindow.UpdateVisualLayerTileSizes( dlg.TileWidth, dlg.TileHeight );
            }
        }

        private void ButtonSetSizeFactors_Click( object sender, RoutedEventArgs e )
        {
            DialogSizeFactors dlg = new DialogSizeFactors( mVisualLayers[ mCurrentLayer ].mSizeFactorX,
                                                           mVisualLayers[ mCurrentLayer ].mSizeFactorY );
//            dlg.Owner = this;

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                UpdateSizeFactors( dlg.SizeFactorX, dlg.SizeFactorY );
            }
        }

        private void ButtonSetSpeeds_Click( object sender, RoutedEventArgs e )
        {
            DialogSpeeds dlg = new DialogSpeeds( mVisualLayers[ mCurrentLayer ].mSpeedX,
                                                 mVisualLayers[ mCurrentLayer ].mSpeedY );
//            dlg.Owner = this;

            Nullable<bool> result = dlg.ShowDialog();
            if ( result == true )
            {
                UpdateSpeeds( dlg.SpeedX, dlg.SpeedY );
            }
        }

        private void ButtonTileMap_Click( object sender, RoutedEventArgs e )
        {
// !!!! TODO: could let choose the file and generate tiles when needed ... !!!!
            if ( mCurrentLayer != -1 )
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".png";
                dlg.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

                Nullable<bool> result = dlg.ShowDialog();

                if ( result == true )
                {
                    TextTileMap.Text = dlg.FileName;
                    mMainWindow.LoadTiles( Layer.TYPE_VISUALS + mCurrentLayer, dlg.FileName );

                    UpdateTileMap();
                }
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

            // no need to check if mCurrentLayer != -1, else wouldn't be here
            mVisualLayers[ mCurrentLayer ].mCurrentTile = UniformGridTiles.Children.IndexOf( button );
        }

        public void UpdateTilesNb( int nbTilesH, int nbTilesV, int horizOrient, int verticOrient )
        {
            TextBoxNbTilesH.Text = nbTilesH.ToString();
            TextBoxNbTilesV.Text = nbTilesV.ToString();

            if ( mCurrentLayer != -1 )
            {
                mVisualLayers[ mCurrentLayer ].Resize( nbTilesH, nbTilesV, horizOrient, verticOrient );
            }
        }

        public void UpdateTileSizes( int tileWidth, int tileHeight )
        {
            TextBoxTilesWidth.Text = tileWidth.ToString();
            TextBoxTilesHeight.Text = tileHeight.ToString();

            if ( mCurrentLayer != -1 )
            {
                mVisualLayers[ mCurrentLayer ].SetTilesSizes( tileWidth, tileHeight );

                UpdateTileMap();
            }
        }

        public void UpdateSizeFactors( double sizeFactorX, double sizeFactorY )
        {
            TextBoxSizeFactorX.Text = sizeFactorX.ToString();
            TextBoxSizeFactorY.Text = sizeFactorY.ToString();

            if ( mCurrentLayer != -1 )
            {
                mVisualLayers[ mCurrentLayer ].SetSizeFactors( sizeFactorX, sizeFactorY );

                mMainWindow.UpdateMap( Layer.TYPE_VISUALS + mCurrentLayer );
            }
        }

        public void UpdateSpeeds( double speedX, double speedY )
        {
            TextBoxSpeedX.Text = speedX.ToString();
            TextBoxSpeedY.Text = speedY.ToString();

            if (mCurrentLayer != -1)
            {
                mVisualLayers[ mCurrentLayer ].SetMovingSpeeds( speedX, speedY );
            }

// !!!! ???? TODO: update the level Map ? ???? !!!!
//...
        }

        public void UpdateTileMap()
        {
            // remove all current buttons
            UniformGridTiles.Children.Clear();

            Layer layer = mVisualLayers[ mCurrentLayer ] as Layer;
            int currentTile = layer.mCurrentTile;
            layer.mCurrentTile = -1;
            if ( layer.mTileMap != null )
            {
                Thickness padding = new Thickness( -1, -1, -1, -1 );
                Thickness border = new Thickness( 2, 2, 2, 2 );

                UniformGridTiles.Columns = layer.mTileMap.PixelWidth / layer.mTileWidth;

                Button button;
                Image image;
                for ( int i = 0; i < layer.mNbTiles; ++i )
                {
                    image = new Image();
                    image.Source = layer.mTiles[ i ];

                    button = new Button();
                    button.Background = Brushes.Transparent;
                    button.BorderBrush = Brushes.Transparent;
                    button.Padding = padding;
                    button.BorderThickness = border;
                    button.Content = image;
                    button.Width = layer.mTileWidth;
                    button.Height = layer.mTileHeight;
                    button.Click += ButtonTile_Click;
                    UniformGridTiles.Children.Add( button );
                }
                button = null;
                image = null;

                if ( ( currentTile != -1 ) && ( currentTile < UniformGridTiles.Children.Count ) )
                {
                    Button btn = UniformGridTiles.Children[ currentTile ] as Button;
                    btn.BorderBrush = Brushes.Red;
                    layer.mCurrentTile = currentTile;
                }
            }
        }

//////// NEW_LAYER 20140320 - MID
        // create new layer
        public void AddNewLayer( int nbTilesH, int nbTilesV, int tileWidth, int tileHeight )
        {
            LayerVisual layer = new LayerVisual();
            layer.Reset( "layer" + mNewLayerCount.ToString(), Layer.TYPE_VISUALS, nbTilesH, nbTilesV, tileWidth, tileHeight );
            ++mNewLayerCount;

            mVisualLayers.Add( layer );
            ++mNbLayers;

            ++mCurrentLayerItemIndex; // new layer will be next one from current in the list

            // add layer in layer maps & create associated grid
            mMainWindow.AddVisualLayer( mNbLayers - 1, mCurrentLayerItemIndex );

////////
/*
// !!!! TODO: find a way to remove that !!!!
//=> horrible hack to fix the size of the ListBox ...
            double height = ListBoxLayers.ActualHeight;
            ListBoxLayers.Height = height;
            ListBoxLayers.MaxHeight = height;
            ListBoxLayers.MinHeight = height;
*/
////////

            // add & select new line after current selection
            mLayersIndex.Insert( mCurrentLayerItemIndex, mNbLayers - 1 );
            ListBoxLayers.Items.Insert( mCurrentLayerItemIndex, layer.mName );
            ListBoxLayers.SelectedIndex = mCurrentLayerItemIndex;
        }

        // create copy layer
        public void CopyNewLayer( Layer originalLayer, bool share )
        {
            LayerVisual layer = new LayerVisual();
            string name = originalLayer.mName;
            if ( !name.Contains( "_copy" ) )
            {
                name += "_copy";
            }
            name += mNewLayerCount.ToString();
            ++mNewLayerCount;
            layer.Copy( originalLayer, name, share );

            mVisualLayers.Add( layer );
            ++mNbLayers;

            ++mCurrentLayerItemIndex; // new layer will be next one from current in the list

            // add layer in layer maps & create associated grid
            mMainWindow.AddVisualLayer( mNbLayers - 1, mCurrentLayerItemIndex );

            if ( share )
            {
// !!!! ???? TODO: OK ? ???? !!!!
                mMainWindow.AddDependencyLayer( mNbLayers - 1, mCurrentLayer ); // dependency between new layer and previous current layer
            }

////////
/*
// !!!! TODO: find a way to remove that !!!!
//=> horrible hack to fix the size of the ListBox ...
            double height = ListBoxLayers.ActualHeight;
            ListBoxLayers.Height = height;
            ListBoxLayers.MaxHeight = height;
            ListBoxLayers.MinHeight = height;
*/
////////

            // add & select new line after current selection
            mLayersIndex.Insert( mCurrentLayerItemIndex, mNbLayers - 1 );
            ListBoxLayers.Items.Insert( mCurrentLayerItemIndex, layer.mName );
            ListBoxLayers.SelectedIndex = mCurrentLayerItemIndex;
        }

        public void MoveCurrentLayerUp()
        {
            if ( mCurrentLayerItemIndex > 0 )
            {
                int index = mCurrentLayerItemIndex;
                int layerIndex = mCurrentLayer;
                string layerName = ListBoxLayers.Items[ index ].ToString();

                // move layer backward in visualisation grids
                mMainWindow.MoveVisualLayerBackward( index );

                // move entry in list
                ListBoxLayers.Items.RemoveAt( index );
                ListBoxLayers.Items.Insert( index - 1, layerName );

                // move entry in index
                mLayersIndex.RemoveAt( index );
                mLayersIndex.Insert( index - 1, layerIndex );

                ListBoxLayers.SelectedIndex = index - 1;
            }
        }

        public void MoveCurrentLayerDown()
        {
            if ( mCurrentLayerItemIndex < mNbLayers ) // consider "main layer"
            {
                int index = mCurrentLayerItemIndex;
                int layerIndex = mCurrentLayer;
                string layerName = ListBoxLayers.Items[ index ].ToString();

                // move layer forward in visualisation grids
                mMainWindow.MoveVisualLayerForward( index );

                // move entry in list
                ListBoxLayers.Items.RemoveAt( index );
                ListBoxLayers.Items.Insert( index + 1, layerName );

                // move entry in index
                mLayersIndex.RemoveAt( index );
                mLayersIndex.Insert( index + 1, layerIndex );

                ListBoxLayers.SelectedIndex = index + 1;
            }
        }

//////// NEW_LAYER 20140320 - END

        public LayerVisual GetCurrentLayer()
        {
            LayerVisual layer = null;
            if ( mCurrentLayer != -1 )
            {
                layer = mVisualLayers[ mCurrentLayer ];
            }
            return layer;
        }

        public int GetCurrentLayerIndex()
        {
            return mCurrentLayer;
        }

        public int GetCurrentLayerListIndex()
        {
            return mCurrentLayerItemIndex;
        }

        public int GetNbLayers()
        {
            return mNbLayers;
        }

        public int GetMainLayerListIndex()
        {
            return ListBoxLayers.Items.IndexOf( mMainLayer.mName );
        }

        public int GetLayerIndex( int listIndex )
        {
            int index = -1;
            if ( ( listIndex >= 0 ) && ( listIndex < ListBoxLayers.Items.Count ) )
            {
                index = mLayersIndex[ listIndex ];
            }
            return index;
        }

    }
}
