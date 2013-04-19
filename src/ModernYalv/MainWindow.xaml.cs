namespace ModernYalv
{
  using FirstFloor.ModernUI.Windows.Controls;
  using ModernYalv.Interfaces;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : ModernWindow, IWinSimple
  {
    public MainWindow()
    {
      this.InitializeComponent();
    }
  }
}
