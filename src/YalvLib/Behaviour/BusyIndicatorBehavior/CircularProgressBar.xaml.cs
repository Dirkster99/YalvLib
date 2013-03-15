namespace YalvLib.Behaviour.BusyIndicatorBehavior
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media.Animation;

  /// <summary>
  /// Provides a circular progress bar
  /// </summary>
  public partial class CircularProgressBar : UserControl
  {
    public CircularProgressBar()
    {
      this.InitializeComponent();

      this.tbMessage.Text = YalvLib.Strings.Resources.CircularProgressBar_CircularProgressBar_BusyText;

      Timeline.SetDesiredFrameRate(this.sbAnimation, BusyIndicatorBehavior.FRAMERATE);
    }
  }
}