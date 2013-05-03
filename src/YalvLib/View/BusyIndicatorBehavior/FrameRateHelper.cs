using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace YalvLib.View.BusyIndicatorBehavior
{
    public static class FrameRateHelper
  {
    private static int? mDesiredFrameRate;

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

    public static void SetTimelineDefaultFramerate(int? framerate)
    {
      Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata
      {
        DefaultValue = framerate
      });
    }
  }
}