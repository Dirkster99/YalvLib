namespace YalvViewModelsLib
{
    using YalvViewModelsLib.Common;
    using YalvViewModelsLib.Interfaces;
    using YalvViewModelsLib.ViewModels;

    public static class Factory
    {
        public static IMainWindowVM CreateMainViewModel(IWinSimple win,
                                                        RecentFileList recentFileList)
        {
            return new MainWindowVM(win, recentFileList);
        }
    }
}
