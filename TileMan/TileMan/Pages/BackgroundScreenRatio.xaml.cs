using System.Windows;
using System.Windows.Controls;

using TileMan.Components;


namespace TileMan.Pages
{
    /// <summary>
    /// Interaction logic for BackgroundScreenRatio.xaml
    /// </summary>
    public partial class BackgroundScreenRatio : UserControl
    {
        private Background mBackground;

        public BackgroundScreenRatio( Background background )
        {
            InitializeComponent();

            mBackground = background;
        }

        public void Reset()
        {
            SliderSizeFactorX.Value = 1.0;
            SliderSizeFactorY.Value = 1.0;
        }

        private void SliderSizeFactorX_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            if ( mBackground != null )
            {
                mBackground.SetSizeFactorX( SliderSizeFactorX.Value );
            }
        }

        private void SliderSizeFactorY_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            if ( mBackground != null )
            {
                mBackground.SetSizeFactorY( SliderSizeFactorY.Value );
            }
        }
    }
}
