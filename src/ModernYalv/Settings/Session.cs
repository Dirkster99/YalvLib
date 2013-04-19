namespace ModernYalv.Settings
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Windows;
  using System.Xml;
  using System.Xml.Serialization;

  using MRU.ViewModel;
  using YalvLib.Domain;
  using YalvLib.ViewModel;

  [Serializable]
  [XmlRoot(ElementName = "Session", IsNullable = false)]
  public class Session
  {
    #region constructor
    /// <summary>
    /// Standard Constructor
    /// </summary>
    public Session()
    {
    }

    /// <summary>
    /// Standard constructor from parameters
    /// </summary>
    public Session(IList<ColumnItem> dataGridColumns, MRUListVM mru, ViewPosSize mainWindowPosSz)
    {
      if (dataGridColumns != null)
        this.DataGridColumns = new List<ColumnItem>(dataGridColumns);

      if (mru != null)
        this.MRU = new MRUListVM(mru);

      if (mainWindowPosSz != null)
      {
        this.MainWindowPosSz = new ViewPosSize(mainWindowPosSz);
      }
    }
    #endregion constructor

    #region properties
    [XmlArray("ColumnLayout")]
    [XmlArrayItem("Column")]
    public List<ColumnItem> DataGridColumns { get; set; }

    [XmlElement(ElementName = "MRU")]
    public MRUListVM MRU { get; set; }

    /// <summary>
    /// Get/set position and size of MainWindow
    /// </summary>
    [XmlElement(ElementName = "MainWindowPos")]
    public ViewPosSize MainWindowPosSz { get; set; }

    /// <summary>
    /// Get/set path and file name to last log4net file loaded in application.
    /// </summary>
    [XmlElement(ElementName = "LastFileLoad")]
    public string LastFileLoad { get; set; }
    #endregion properties

    #region methods
    /// <summary>
    /// Save user session data for re-load at a later time
    /// </summary>
    /// <param name="pathFileName"></param>
    public void SaveSession(string pathFileName)
    {
      if (this.DataGridColumns == null)
        this.DataGridColumns = new List<ColumnItem>();
      else
      {
        // Copy actual width values into width field for persistence
        for (int i = 0; i < this.DataGridColumns.Count; i++)
        {
          if (this.DataGridColumns[i].ActualWidth != null)
            this.DataGridColumns[i].Width = this.DataGridColumns[i].ActualWidth.Width;
        }
      }

      if (this.MRU == null)
        this.MRU = new MRUListVM();

      SaveSession(pathFileName, this);
    }

    #region Load Save  Session Data
    /// <summary>
    /// Load user sesion data from persistence (usually at start of program)
    /// </summary>
    /// <param name="pathFileName"></param>
    /// <returns></returns>
    internal static Session LoadSession(string pathFileName)
    {
      Session loadedObj = null;

      if (System.IO.File.Exists(pathFileName))
      {
        using (FileStream readFileStream = new FileStream(pathFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          try
          {
            XmlSerializer serializerObj = new XmlSerializer(typeof(Session));

            loadedObj = (Session)serializerObj.Deserialize(readFileStream);
          }
          catch (Exception e)
          {
            Console.WriteLine(e.ToString());

            return null;
          }
          finally
          {
            readFileStream.Close();
          }
        }
      }

      if (loadedObj != null)
      {
        // Init parent/child structure (not nice but works for now)
        if (loadedObj.MRU != null)
          loadedObj.MRU.InitOnDeserialization();
      }

      return loadedObj;
    }

    private static bool SaveSession(string settingsFileName, Session objToSave)
    {
      try
      {
        XmlWriterSettings xws = new XmlWriterSettings();
        xws.NewLineOnAttributes = true;
        xws.Indent = true;
        xws.IndentChars = "  ";
        xws.Encoding = System.Text.Encoding.UTF8;

        using (XmlWriter xw = XmlWriter.Create(settingsFileName, xws))
        {
          XmlSerializer serializerObj = new XmlSerializer(typeof(Session));

          serializerObj.Serialize(xw, objToSave);

          xw.Close();

          return true;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());

        MessageBox.Show(e.StackTrace.ToString(), e.Message,
                        MessageBoxButton.OK, MessageBoxImage.Error);

        return false;
      }
    }
    #endregion Load Save Session Data
    #endregion methods
  }
}
