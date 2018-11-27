namespace YALV
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Windows;

    using YalvLib.Views.BusyIndicatorBehavior;
    using YalvViewModelsLib;
    using YalvViewModelsLib.Interfaces;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Create a mainwindow instance and return it.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public MainWindow CreateMainWindow(string[] args)
        {
            MainWindow win = new MainWindow();

            IMainWindowVM viewmodel = Factory.CreateMainViewModel(
                                        win as IWinSimple, win.mainMenu.RecentFileList);

            win.DataContext = viewmodel;

            // Assign events
            win.Loaded += delegate
            {
                if (args != null && args.Length > 0) // Just attempt to load the first entry
              viewmodel.LoadLog4NetFile(args[0]);
            };

            win.Closing += delegate
            {
                viewmodel.SaveColumnLayout();
                viewmodel.Dispose();
            };

            return win;
        }

        /// <summary>
        /// Execute application start-up sequence
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            int? framerate = FrameRateHelper.DesiredFrameRate;
            BusyIndicatorBehavior.FRAMERATE = framerate;
            FrameRateHelper.SetTimelineDefaultFramerate(framerate);

            this.initCulture();

            MainWindow win = this.CreateMainWindow(e.Args);

            if (win != null)
                win.Show();
        }

        /// <summary>
        /// Initialize thread culture for this application
        /// </summary>
        private void initCulture()
        {
            try
            {
                var culture = ConfigurationManager.AppSettings["Culture"];

                if (!string.IsNullOrWhiteSpace(culture))
                    log4netLib.Strings.Resources.Culture = new CultureInfo(culture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
