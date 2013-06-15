using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace YalvLib.View.BusyIndicatorBehavior
{
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

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {

    }
  }
}