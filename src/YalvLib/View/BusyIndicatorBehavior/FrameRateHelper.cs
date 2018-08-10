namespace YalvLib.View.BusyIndicatorBehavior
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    /// <summary>
    /// Implements a helper to determine the best frame rate for animation
    /// display based on the available graphics acceleration of a given computer.
    /// </summary>
    public static class FrameRateHelper
    {
        private static int? mDesiredFrameRate;

        /// <summary>
        /// Static class constructor
        /// </summary>
        static FrameRateHelper()
        {
            switch (RenderCapability.Tier >> 16)
            {
                case 2:     // mostly hardware
                    DesiredFrameRate = new int?(30);
                    break;

                case 1:     // partially hardware
                    DesiredFrameRate = new int?(20);
                    break;

                case 0:     // software
                default:
                    DesiredFrameRate = new int?(10);
                    break;
            }
        }

        /// <summary>
        /// Gets/Sets the best frame rate to support smooth animation display
        /// based on the graphics acceleration of a given computer.
        /// </summary>
        public static int? DesiredFrameRate
        {
            get
            {
                return FrameRateHelper.mDesiredFrameRate;
            }

            set
            {
                FrameRateHelper.mDesiredFrameRate = value;
            }
        }

        /// <summary>
        /// Sets the metadata property of the <see cref="Timeline"/> framerate class.
        /// </summary>
        /// <param name="framerate"></param>
        public static void SetTimelineDefaultFramerate(int? framerate)
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata
            {
                DefaultValue = framerate
            });
        }
    }
}