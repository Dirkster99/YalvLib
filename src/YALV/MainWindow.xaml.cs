namespace YALV
{
    using System.Windows;
    using YalvViewModelsLib.Interfaces;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWinSimple
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }
    }
}
