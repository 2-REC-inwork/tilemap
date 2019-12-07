using System.Windows;

using TileMan.Pages;


namespace TileMan.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogResizeMap.xaml
    /// </summary>
    public partial class DialogResizeMap : Window
    {
        public int NbTilesH
        {
            get { return (int)SliderNbTilesH.Value; }
            set { SliderNbTilesH.Value = value; }
        }

        public int NbTilesV
        {
            get { return (int)SliderNbTilesV.Value; }
            set { SliderNbTilesV.Value = value; }
        }

        public DialogResizeMap( int nbTilesH, int nbTilesV )
        {
            InitializeComponent();
            SliderNbTilesH.Value = nbTilesH;
            SliderNbTilesV.Value = nbTilesV;
        }

        private void ButtonOK_Click( object sender, RoutedEventArgs e )
        {
            this.DialogResult = true;
        }
    }
}
