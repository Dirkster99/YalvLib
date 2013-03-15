namespace YALV.View
{
  #region About
  /*
 * YALV! - Yet Another Log4Net Viewer
 * 
 * YALV! is a log viewer for Log4Net that allow to compare multiple logs file simultaneously.
 * Log4Net config file must be setup with XmlLayoutSchemaLog4j layout.
 * It is a WPF Application based on .NET Framework 4.0 and written with C# language.
 *
 * An open source application developed by Luca Petrini - http://www.linkedin.com/in/lucapetrini
 * 
 * Copyright: (c) 2012 Luca Petrini
 * 
 * YALV! is a free software distributed on CodePlex: http://yalv.codeplex.com/ under the Microsoft Public License (Ms-PL)
 */
  #endregion

  using System.Windows;
  using YALV.Interfaces;

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
