
using System.Windows;

using TileMan.Pages;


namespace TileMan.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogResizeTiles.xaml
    /// </summary>
    public partial class DialogResizeTiles : Window
    {
        public int TileWidth
        {
            get { return (int)SliderTileWidth.Value; }
            set { SliderTileWidth.Value = value; }
        }

        public int TileHeight
        {
            get { return (int)SliderTileHeight.Value; }
            set { SliderTileHeight.Value = value; }
        }

        public DialogResizeTiles( int tileWidth, int tileHeight )
        {
            InitializeComponent();
            SliderTileWidth.Value = tileWidth;
            SliderTileHeight.Value = tileHeight;
        }

        private void ButtonOK_Click( object sender, RoutedEventArgs e )
        {
            this.DialogResult = true;
        }
    }
}
