using System.Windows;


namespace TileMan.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogLayerCopy.xaml
    /// </summary>
    public partial class DialogLayerCopy : Window
    {
        public DialogLayerCopy()
        {
            InitializeComponent();
        }

        private void ButtonOK_Click( object sender, RoutedEventArgs e )
        {
            this.DialogResult = true;
        }
    }
}
