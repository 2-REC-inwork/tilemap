using System.Windows;

namespace TileMan.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogSizeFactors.xaml
    /// </summary>
    public partial class DialogSizeFactors : Window
    {
        public double SizeFactorX
        {
            get { return SliderSizeFactorX.Value; }
            set { SliderSizeFactorX.Value = value; }
        }

        public double SizeFactorY
        {
            get { return SliderSizeFactorY.Value; }
            set { SliderSizeFactorY.Value = value; }
        }

        public DialogSizeFactors( double tileWidth, double tileHeight )
        {
            InitializeComponent();
            SliderSizeFactorX.Value = tileWidth;
            SliderSizeFactorY.Value = tileHeight;
        }

        private void ButtonOK_Click( object sender, RoutedEventArgs e )
        {
            this.DialogResult = true;
        }
    }
}
