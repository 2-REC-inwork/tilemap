using System;
using System.Windows;
using System.Windows.Controls;

using TileMan.Components;

namespace TileMan.Pages
{
    /// <summary>
    /// Interaction logic for BackgroundImageRatio.xaml
    /// </summary>
    public partial class BackgroundImageRatio : UserControl
    {
        private Background mBackground;

        private double mWidth = 1;
        private double mHeight = 1;


        public BackgroundImageRatio( Background background )
        {
            InitializeComponent();

            mBackground = background;
        }

        public void Reset()
        {
            SliderWidth.Value = 1;
            SliderHeight.Value = 1;
            TextBoxRatio.Clear();
            RadioButtonWidth.IsChecked = true;
            SliderSizeFactor.Value = 1;
        }

        private void SliderSizeFactor_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            if ( mBackground != null )
            {
                mBackground.SetSizeFactorX( SliderSizeFactor.Value );
                mBackground.SetSizeFactorY( SliderSizeFactor.Value );
            }
        }

        private void SliderWidth_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            Slider sliderWidth = sender as Slider;
            mWidth = SliderWidth.Value;

            if ( mBackground != null )
            {
                mBackground.SetWidth( ( int )mWidth );
            }

            double ratio = Math.Round( mWidth / mHeight, 2 );
            if ( ratio < 0.01 )
                ratio = 0.01;
            TextBoxRatio.Text = ratio.ToString();
        }

        private void SliderHeight_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            Slider sliderHeight = sender as Slider;
            mHeight = sliderHeight.Value;

            if ( mBackground != null )
            {
                mBackground.SetHeight( ( int )mHeight );
            }

            double ratio = Math.Round( mWidth / mHeight, 2 );
            if ( ratio < 0.01 )
                ratio = 0.01;
            TextBoxRatio.Text = ratio.ToString();
        }

        private void RadioButtonWidth_Checked( object sender, RoutedEventArgs e )
        {
            if ( mBackground != null )
            {
                mBackground.SetRatioKeepWidth( true );
            }
        }

        private void RadioButtonHeight_Checked( object sender, RoutedEventArgs e )
        {
            if ( mBackground != null )
            {
                mBackground.SetRatioKeepWidth( false );
            }
        }
    }
}
