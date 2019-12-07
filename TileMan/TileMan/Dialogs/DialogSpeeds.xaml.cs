using System.Windows;

namespace TileMan.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogSpeeds.xaml
    /// </summary>
    public partial class DialogSpeeds : Window
    {
        public double SpeedX
        {
            get { return SliderSpeedX.Value; }
            set { SliderSpeedX.Value = value; }
        }

        public double SpeedY
        {
            get { return SliderSpeedY.Value; }
            set { SliderSpeedY.Value = value; }
        }

        public DialogSpeeds( double speedX, double speedY )
        {
            InitializeComponent();
            SliderSpeedX.Value = speedX;
            SliderSpeedY.Value = speedY;
        }

        private void ButtonOK_Click( object sender, RoutedEventArgs e )
        {
            this.DialogResult = true;
        }
    }
}
