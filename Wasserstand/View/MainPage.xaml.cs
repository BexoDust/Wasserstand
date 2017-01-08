using Wasserstand.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

namespace Wasserstand
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public BasicController ViewModel
        {
            get { return (BasicController)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(BasicController), typeof(BasicController), new PropertyMetadata(0));

        public MainPage()
        {
            var controller = new BasicController();
            this.DataContext = controller;
            this.ViewModel = controller;
            this.InitializeComponent();
        }

        private void Instance_Unloaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.SaveTagsToXml();
        }
    }
}
