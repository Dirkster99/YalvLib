using YalvLib.View.BusyIndicatorBehavior;

namespace ModernYalv
{
  using System;
  using System.Globalization;
  using System.Windows;
  using ModernYalv.Settings;
  using ModernYalv.ViewModel;

  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public MainWindow MainWin { get; set; }

    /// <summary>
    /// Create a mainwindow instance and return it.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public MainWindow CreateMainWindow(string[] args)
    {
      this.MainWin = new MainWindow();

      MainWindowVM viewmodel = new MainWindowVM(this.MainWin);
      viewmodel.LoadSession();

      this.MainWin.DataContext = viewmodel;

      // Assign events
      this.MainWin.Loaded += delegate
      {
        if (args != null && args.Length > 0)
          viewmodel.LoadFileList(args[0]);
      };

      this.MainWin.Closing += delegate
      {
        viewmodel.SaveSession(new ViewPosSize(this.MainWin));
        viewmodel.Dispose();
      };

      return this.MainWin;
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
        var culture = System.Configuration.ConfigurationManager.AppSettings["Culture"];

        if (!string.IsNullOrWhiteSpace(culture))
          YalvLib.Strings.Resources.Culture = new CultureInfo(culture);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Exclamation);
      }
    }
  }
}
