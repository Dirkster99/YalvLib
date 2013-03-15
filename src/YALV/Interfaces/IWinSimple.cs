namespace YALV.Interfaces
{
  using System.Windows;

  public interface IWinSimple
  {
    bool? DialogResult { get; set; }

    Window Owner { get; set; }

    void Close();
  }
}