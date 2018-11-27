namespace YalvViewModelsLib.Interfaces
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;
    using YalvLib.ViewModels;
    using YalvViewModelsLib.Common;

    public interface IMainWindowVM : IDisposable, INotifyPropertyChanged
    {
        #region properties
        ICommand CommandCancelProcessing { get; }
        ICommand CommandExit { get; }
        ICommand CommandExport { get; }
        ICommand CommandOpenFile { get; }
        ICommand CommandOpenLogAnalysisSession { get; }
        ICommand CommandOpenSqliteDatabase { get; }

        RecentFileList RecentFileList { get; set; }

        string Title { get; }

        YalvViewModel YalvLogViewModel { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Save the column layout of the main data grid control.
        /// </summary>
        void SaveColumnLayout();

        void LoadLog4NetFile(string filePath);
        #endregion methods
    }
}