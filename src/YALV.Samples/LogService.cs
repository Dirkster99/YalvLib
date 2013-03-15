namespace YALV.Samples
{
  using System;
  using log4net;
  using log4net.Config;

  public sealed class LogService
  {
    private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    static LogService()
    {
      log4net.GlobalContext.Properties["SampleDate"] = DateTime.Now.ToString("yyyyMMdd");

      // Log4Net Inizialization
      XmlConfigurator.Configure();
    }

    public static ILog Trace
    {
      get { return logger; }
    }
  }
}