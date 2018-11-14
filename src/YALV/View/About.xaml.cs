namespace YALV.View
{
    using System;
    using System.Diagnostics;
    using System.Windows;

    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            this.InitializeComponent();

            FileVersionInfo verInfo = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string version = string.Format(YalvLib.Strings.Resources.About_Version_Text, verInfo != null ? verInfo.FileVersion : "---");
            this.lblVersion.Text = version;

            string config1 = @"<log4net>
    <appender name=""FileAppender"" type=""log4net.Appender.FileAppender"">
        <file type=""log4net.Util.PatternString"" value=""sample-log.xml""/>
        <appendToFile value=""true""/>        
        <layout type=""log4net.Layout.XmlLayoutSchemaLog4j"">
            <locationInfo value=""true""/>
        </layout>
    </appender>

    <root>
        <level value=""ALL"" />
        <appender-ref ref=""FileAppender"" />
    </root>
</log4net>";
            this.tbConfig1.Text = config1;

            string config2 = @"<log4net>
    <appender name=""RollingFileAppender"" type=""log4net.Appender.RollingFileAppender"">
        <file type=""log4net.Util.PatternString"" value=""sample-log.xml""/>
        <appendToFile value=""true""/>
        <datePattern value=""yyyyMMdd""/>
        <rollingStyle value=""Size""/>
        <maxSizeRollBackups value=""5""/>
        <maximumFileSize value=""5000KB""/>
        <layout type=""log4net.Layout.XmlLayoutSchemaLog4j"">
            <locationInfo value=""true""/>
        </layout>
    </appender>

    <root>
        <level value=""ALL"" />
        <appender-ref ref=""RollingFileAppender"" />
    </root>
</log4net>";
            this.tbConfig2.Text = config2;
        }

        /// <summary>
        /// Method resolves the target URL when a Hyperlink is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch {}
        }
    }
}
