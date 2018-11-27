namespace YalvViewModelsLib.Views
{
    using System.Windows.Controls;
    using YalvViewModelsLib.Common;

    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Menu
    {
        public MainMenu()
        {
            this.InitializeComponent();
        }

        public RecentFileList RecentFileList
        {
            get
            {
                return this.RecentFileListMenu;
            }
        }
    }
}
