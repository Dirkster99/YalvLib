namespace ModernYalv.Interfaces
{
  using System.Windows;

  public interface IWinSimple
  {
    bool? DialogResult { get; set; }

    Window Owner { get; set; }

    double Left { get; set; }

    double Top { get; set; }

    double Width { get; set; }

    double Height { get; set; }

    WindowState WindowState { get; set; }

    void Close();
  }
}