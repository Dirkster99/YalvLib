namespace YalvLib.View.BusyIndicatorBehavior
{
    using System.Windows.Controls;
    using System.Windows.Media.Animation;

    /// <summary>
    /// Provides a circular progress indicator
    /// </summary>
    public partial class CircularProgressBar : UserControl
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public CircularProgressBar()
        {
            this.InitializeComponent();

            this.tbMessage.Text = log4netLib.Strings.Resources.CircularProgressBar_CircularProgressBar_BusyText;

            Timeline.SetDesiredFrameRate(this.sbAnimation, BusyIndicatorBehavior.FRAMERATE);
        }
    }
}