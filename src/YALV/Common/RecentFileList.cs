namespace YALV.Common
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Windows;
  using System.Windows.Controls;
  using System.Xml;
  using Microsoft.Win32;

  /// <summary>
  /// Refs: http://www.codeproject.com/Articles/23731/RecentFileList-a-WPF-MRU
  /// </summary>
  public class RecentFileList : Separator
  {
    #region fields
    private static Version mVersion = null;

    private Separator mSeparator = null;
    private List<RecentFile> mRecentFiles = null;
    #endregion fields

    #region constructor
    public RecentFileList()
    {
      this.Persister = new RegistryPersister();

      this.MaxNumberOfFiles = 10;
      this.MaxPathLength = 150;
      this.MenuItemFormatOneToNine = "_{0}:  {2}";
      this.MenuItemFormatTenPlus = "{0}:  {2}";

      this.Loaded += (s, e) => this.HookFileMenu();
    }
    #endregion constructor

    public delegate string GetMenuItemTextDelegate(int index, string filepath);

    #region events
    public event EventHandler<MenuClickEventArgs> MenuClick;
    #endregion events

    #region interface
    public interface IPersist
    {
      List<string> RecentFiles(int max);

      void InsertFile(string filepath, int max);

      void RemoveFile(string filepath, int max);
    }
    #endregion interface

    #region properties
    public IPersist Persister { get; set; }

    public int MaxNumberOfFiles { get; set; }

    public int MaxPathLength { get; set; }

    public MenuItem FileMenu { get; private set; }

    /// <summary>
    /// Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
    /// Default = "_{0}:  {2}"
    /// </summary>
    public string MenuItemFormatOneToNine { get; set; }

    /// <summary>
    /// Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
    /// Default = "{0}:  {2}"
    /// </summary>
    public string MenuItemFormatTenPlus { get; set; }

    public GetMenuItemTextDelegate GetMenuItemTextHandler { get; set; }

    public List<string> RecentFiles
    {
      get
      {
        return this.Persister.RecentFiles(this.MaxNumberOfFiles);
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Shortens a pathname for display purposes.
    /// 
    /// Source: This method is taken from Joe Woodbury's article at: http://www.codeproject.com/KB/cs/mrutoolstripmenu.aspx
    /// </summary>
    /// <param labelName="pathname">The pathname to shorten.</param>
    /// <param labelName="maxLength">The maximum number of characters to be displayed.</param>
    /// <remarks>Shortens a pathname by either removing consecutive components of a path
    /// and/or by removing characters from the end of the filename and replacing
    /// then with three elipses (...)
    /// <para>In all cases, the root of the passed path will be preserved in it's entirety.</para>
    /// <para>If a UNC path is used or the pathname and maxLength are particularly short,
    /// the resulting path may be longer than maxLength.</para>
    /// <para>This method expects fully resolved pathnames to be passed to it.
    /// (Use Path.GetFullPath() to obtain this.)</para>
    /// </remarks>
    /// <returns></returns>
    public static string ShortenPathname(string pathname, int maxLength)
    {
      if (pathname.Length <= maxLength)
        return pathname;

      string root = Path.GetPathRoot(pathname);
      if (root.Length > 3)
        root += Path.DirectorySeparatorChar;

      string[] elements = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

      int filenameIndex = elements.GetLength(0) - 1;

      if (elements.GetLength(0) == 1) // pathname is just a root and filename
      {
        if (elements[0].Length > 5) // long enough to shorten
        {
          // if path is a UNC path, root may be rather long
          if (root.Length + 6 >= maxLength)
          {
            return root + elements[0].Substring(0, 3) + "...";
          }
          else
          {
            return pathname.Substring(0, maxLength - 3) + "...";
          }
        }
      }
      else if ((root.Length + 4 + elements[filenameIndex].Length) > maxLength) // pathname is just a root and filename
      {
        root += "...\\";

        int len = elements[filenameIndex].Length;
        if (len < 6)
          return root + elements[filenameIndex];

        if ((root.Length + 6) >= maxLength)
        {
          len = 3;
        }
        else
        {
          len = maxLength - root.Length - 3;
        }

        return root + elements[filenameIndex].Substring(0, len) + "...";
      }
      else if (elements.GetLength(0) == 2)
      {
        return root + "...\\" + elements[1];
      }
      else
      {
        int len = 0;
        int begin = 0;

        for (int i = 0; i < filenameIndex; i++)
        {
          if (elements[i].Length > len)
          {
            begin = i;
            len = elements[i].Length;
          }
        }

        int totalLength = pathname.Length - len + 3;
        int end = begin + 1;

        while (totalLength > maxLength)
        {
          if (begin > 0)
            totalLength -= elements[--begin].Length - 1;

          if (totalLength <= maxLength)
            break;

          if (end < filenameIndex)
            totalLength -= elements[++end].Length - 1;

          if (begin == 0 && end == filenameIndex)
            break;
        }

        // assemble final string
        for (int i = 0; i < begin; i++)
        {
          root += elements[i] + '\\';
        }

        root += "...\\";

        for (int i = end; i < filenameIndex; i++)
        {
          root += elements[i] + '\\';
        }

        return root + elements[filenameIndex];
      }

      return pathname;
    }

    public void UseRegistryPersister()
    {
      this.Persister = new RegistryPersister();
    }

    public void UseRegistryPersister(string key)
    {
      this.Persister = new RegistryPersister(key);
    }

    public void UseXmlPersister()
    {
      this.Persister = new XmlPersister();
    }

    public void UseXmlPersister(string filepath)
    {
      this.Persister = new XmlPersister(filepath);
    }

    public void UseXmlPersister(Stream stream)
    {
      this.Persister = new XmlPersister(stream);
    }

    public void RemoveFile(string filepath)
    {
      this.Persister.RemoveFile(filepath, this.MaxNumberOfFiles);
    }

    public void InsertFile(string filepath)
    {
      this.Persister.InsertFile(filepath, this.MaxNumberOfFiles);
    }

    protected virtual void OnMenuClick(MenuItem menuItem)
    {
      string filepath = this.GetFilepath(menuItem);

      if (string.IsNullOrEmpty(filepath))
        return;

      EventHandler<MenuClickEventArgs> dMenuClick = this.MenuClick;
      
      if (dMenuClick != null)
        dMenuClick(menuItem, new MenuClickEventArgs(filepath));
    }

    private void HookFileMenu()
    {
      MenuItem parent = Parent as MenuItem;
      
      if (parent == null)
        throw new ApplicationException("Parent must be a MenuItem");

      if (this.FileMenu == parent)
        return;

      if (this.FileMenu != null)
        this.FileMenu.SubmenuOpened -= this.FileMenu_SubmenuOpened;

      this.FileMenu = parent;
      this.FileMenu.SubmenuOpened += this.FileMenu_SubmenuOpened;
    }

    private void FileMenu_SubmenuOpened(object sender, RoutedEventArgs e)
    {
      this.SetMenuItems();
    }

    private void SetMenuItems()
    {
      this.RemoveMenuItems();

      this.LoadRecentFiles();

      this.InsertMenuItems();
    }

    private void RemoveMenuItems()
    {
      if (this.mSeparator != null)
        this.FileMenu.Items.Remove(this.mSeparator);

      if (this.mRecentFiles != null)
      {
        foreach (RecentFile r in this.mRecentFiles)
        {
          if (r.MenuItem != null)
            this.FileMenu.Items.Remove(r.MenuItem);
        }
      }

      this.mSeparator = null;
      this.mRecentFiles = null;
    }

    private void InsertMenuItems()
    {
      if (this.mRecentFiles == null)
        return;

      if (this.mRecentFiles.Count == 0)
        return;

      int iMenuItem = this.FileMenu.Items.IndexOf(this);

      foreach (RecentFile r in this.mRecentFiles)
      {
        string header = this.GetMenuItemText(r.Number + 1, r.Filepath, r.DisplayPath);

        r.MenuItem = new MenuItem { Header = header };
        r.MenuItem.Click += this.MenuItem_Click;

        this.FileMenu.Items.Insert(++iMenuItem, r.MenuItem);
      }

      this.mSeparator = new Separator();
      this.FileMenu.Items.Insert(++iMenuItem, this.mSeparator);
    }

    private string GetMenuItemText(int index, string filepath, string displaypath)
    {
      GetMenuItemTextDelegate delegateGetMenuItemText = this.GetMenuItemTextHandler;
      if (delegateGetMenuItemText != null) return delegateGetMenuItemText(index, filepath);

      string format = (index < 10 ? this.MenuItemFormatOneToNine : this.MenuItemFormatTenPlus);

      string shortPath = ShortenPathname(displaypath, this.MaxPathLength);

      return string.Format(format, index, filepath, shortPath);
    }

    private void LoadRecentFiles()
    {
      this.mRecentFiles = this.LoadRecentFilesCore();
    }

    private List<RecentFile> LoadRecentFilesCore()
    {
      List<string> list = this.RecentFiles;

      List<RecentFile> files = new List<RecentFile>(list.Count);

      int i = 0;
      foreach (string filepath in list)
        files.Add(new RecentFile(i++, filepath));

      return files;
    }

    private void MenuItem_Click(object sender, EventArgs e)
    {
      MenuItem menuItem = sender as MenuItem;

      this.OnMenuClick(menuItem);
    }

    private string GetFilepath(MenuItem menuItem)
    {
      foreach (RecentFile r in this.mRecentFiles)
        if (r.MenuItem == menuItem)
          return r.Filepath;

      return string.Empty;
    }
    #endregion methods

    #region subClasses

    public class MenuClickEventArgs : EventArgs
    {
      public MenuClickEventArgs(string filepath)
      {
        this.Filepath = filepath;
      }

      public string Filepath { get; private set; }
    }

    //-----------------------------------------------------------------------------------------
    private static class ApplicationAttributes
    {
      #region fields
      private static readonly Assembly mAssembly = null;

      private static readonly AssemblyTitleAttribute mTitle = null;
      private static readonly AssemblyCompanyAttribute mCompany = null;
      private static readonly AssemblyCopyrightAttribute mCopyright = null;
      private static readonly AssemblyProductAttribute mProduct = null;
      #endregion fields

      #region constructor
      static ApplicationAttributes()
      {
        try
        {
          ApplicationAttributes.Title = string.Empty;
          ApplicationAttributes.CompanyName = string.Empty;
          ApplicationAttributes.Copyright = string.Empty;
          ApplicationAttributes.ProductName = string.Empty;
          ApplicationAttributes.Version = string.Empty;

          ApplicationAttributes.mAssembly = Assembly.GetEntryAssembly();

          if (mAssembly != null)
          {
            object[] attributes = mAssembly.GetCustomAttributes(false);

            foreach (object attribute in attributes)
            {
              Type type = attribute.GetType();

              if (type == typeof(AssemblyTitleAttribute))
                ApplicationAttributes.mTitle = (AssemblyTitleAttribute)attribute;
              
              if (type == typeof(AssemblyCompanyAttribute))
                ApplicationAttributes.mCompany = (AssemblyCompanyAttribute)attribute;
              
              if (type == typeof(AssemblyCopyrightAttribute))
                ApplicationAttributes.mCopyright = (AssemblyCopyrightAttribute)attribute;
              
              if (type == typeof(AssemblyProductAttribute))
                ApplicationAttributes.mProduct = (AssemblyProductAttribute)attribute;
            }

            RecentFileList.mVersion = mAssembly.GetName().Version;
          }

          if (ApplicationAttributes.mTitle != null)
            ApplicationAttributes.Title = mTitle.Title;

          if (ApplicationAttributes.mCompany != null)
            ApplicationAttributes.CompanyName = mCompany.Company;

          if (ApplicationAttributes.mCopyright != null)
            ApplicationAttributes.Copyright = mCopyright.Copyright;

          if (ApplicationAttributes.mProduct != null)
            ApplicationAttributes.ProductName = mProduct.Product;

          if (RecentFileList.mVersion != null)
            ApplicationAttributes.Version = mVersion.ToString();
        }
        catch
        {
        }
      }
      #endregion constructor

      #region properties
      public static string Title { get; private set; }

      public static string CompanyName { get; private set; }

      public static string Copyright { get; private set; }

      public static string ProductName { get; private set; }

      public static string Version { get; private set; }
      #endregion properties
    }

    private class RecentFile
    {
      public RecentFile()
      {
        this.Number = 0;
        this.Filepath = string.Empty;
        this.MenuItem = null;
      }

      public RecentFile(int number, string filepath) : this()
      {
        this.Number = number;
        this.Filepath = filepath;
      }

      public int Number { get; set; }

      public string Filepath { get; set; }

      public MenuItem MenuItem { get; set; }

      public string DisplayPath
      {
        get
        {
          return Path.Combine(
              Path.GetDirectoryName(this.Filepath),
              Path.GetFileNameWithoutExtension(this.Filepath));
        }
      }
    }

    //-----------------------------------------------------------------------------------------
    private class RegistryPersister : IPersist
    {
      public RegistryPersister()
      {
        this.RegistryKey =
            "Software\\" +
            ApplicationAttributes.CompanyName + "\\" +
            ApplicationAttributes.ProductName + "\\" +
            "RecentFileList";
      }

      public RegistryPersister(string key)
      {
        this.RegistryKey = key;
      }

      public string RegistryKey { get; set; }

      public List<string> RecentFiles(int max)
      {
        RegistryKey k = Registry.CurrentUser.OpenSubKey(this.RegistryKey);
        if (k == null) k = Registry.CurrentUser.CreateSubKey(this.RegistryKey);

        List<string> list = new List<string>(max);

        for (int i = 0; i < max; i++)
        {
          string filename = (string)k.GetValue(this.Key(i));

          if (string.IsNullOrEmpty(filename))
            break;

          list.Add(filename);
        }

        return list;
      }

      public void InsertFile(string filepath, int max)
      {
        RegistryKey k = Registry.CurrentUser.OpenSubKey(this.RegistryKey);

        if (k == null)
          Registry.CurrentUser.CreateSubKey(this.RegistryKey);

        k = Registry.CurrentUser.OpenSubKey(this.RegistryKey, true);

        this.RemoveFile(filepath, max);

        for (int i = max - 2; i >= 0; i--)
        {
          string sThis = this.Key(i);
          string sNext = this.Key(i + 1);

          object oThis = k.GetValue(sThis);
          if (oThis == null) continue;

          k.SetValue(sNext, oThis);
        }

        k.SetValue(this.Key(0), filepath);
      }

      public void RemoveFile(string filepath, int max)
      {
        RegistryKey k = Registry.CurrentUser.OpenSubKey(this.RegistryKey);
        if (k == null) return;

        for (int i = 0; i < max; i++)
        {
        again:
          string s = (string)k.GetValue(this.Key(i));
          if (s != null && s.Equals(filepath, StringComparison.CurrentCultureIgnoreCase))
          {
            this.RemoveFile(i, max);
            goto again;
          }
        }
      }

      private string Key(int i)
      {
        return i.ToString("00");
      }

      private void RemoveFile(int index, int max)
      {
        RegistryKey k = Registry.CurrentUser.OpenSubKey(this.RegistryKey, true);

        if (k == null)
          return;

        k.DeleteValue(this.Key(index), false);

        for (int i = index; i < max - 1; i++)
        {
          string sThis = this.Key(i);
          string sNext = this.Key(i + 1);

          object oNext = k.GetValue(sNext);
          if (oNext == null) break;

          k.SetValue(sThis, oNext);
          k.DeleteValue(sNext);
        }
      }
    }

    //-----------------------------------------------------------------------------------------
    private class XmlPersister : IPersist
    {
      #region constructor
      public XmlPersister()
      {
        this.Filepath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ApplicationAttributes.CompanyName + "\\" +
                ApplicationAttributes.ProductName + "\\" +
                "RecentFileList.xml");
      }

      public XmlPersister(string filepath)
      {
        this.Filepath = filepath;
      }

      public XmlPersister(Stream stream)
      {
        Stream = stream;
      }
      #endregion constructor

      #region properties
      public string Filepath { get; set; }

      public Stream Stream { get; set; }
      #endregion properties

      #region methods
      public List<string> RecentFiles(int max)
      {
        return this.Load(max);
      }

      public void InsertFile(string filepath, int max)
      {
        this.Update(filepath, true, max);
      }

      public void RemoveFile(string filepath, int max)
      {
        this.Update(filepath, false, max);
      }

      private void Update(string filepath, bool insert, int max)
      {
        List<string> old = this.Load(max);

        List<string> list = new List<string>(old.Count + 1);

        if (insert)
          list.Add(filepath);

        this.CopyExcluding(old, filepath, list, max);

        this.Save(list, max);
      }

      private void CopyExcluding(List<string> source, string exclude, List<string> target, int max)
      {
        foreach (string s in source)
          if (!string.IsNullOrEmpty(s))
            if (!s.Equals(exclude, StringComparison.OrdinalIgnoreCase))
              if (target.Count < max)
                target.Add(s);
      }

      private SmartStream OpenStream(FileMode mode)
      {
        if (!string.IsNullOrEmpty(this.Filepath))
        {
          return new SmartStream(this.Filepath, mode);
        }
        else
        {
          return new SmartStream(Stream);
        }
      }

      private List<string> Load(int max)
      {
        List<string> list = new List<string>(max);

        using (MemoryStream ms = new MemoryStream())
        {
          using (SmartStream ss = this.OpenStream(FileMode.OpenOrCreate))
          {
            if (ss.Stream.Length == 0) return list;

            ss.Stream.Position = 0;

            byte[] buffer = new byte[1 << 20];
            for (;;)
            {
              int bytes = ss.Stream.Read(buffer, 0, buffer.Length);

              if (bytes == 0)
                break;

              ms.Write(buffer, 0, bytes);
            }

            ms.Position = 0;
          }

          XmlTextReader x = null;

          try
          {
            x = new XmlTextReader(ms);

            while (x.Read())
            {
              switch (x.NodeType)
              {
                case XmlNodeType.XmlDeclaration:
                case XmlNodeType.Whitespace:
                  break;

                case XmlNodeType.Element:
                  switch (x.Name)
                  {
                    case "RecentFiles": break;

                    case "RecentFile":
                      if (list.Count < max) list.Add(x.GetAttribute(0));
                      break;

                    default:
                    Debug.Assert(false, "Case not implemented");
                    break;
                  }

                  break;

                case XmlNodeType.EndElement:
                  switch (x.Name)
                  {
                    case "RecentFiles":
                      return list;
                    default:
                      Debug.Assert(false, "Case not implemented");
                    break;
                  }

                  break;

                default:
                  Debug.Assert(false, "Case not implemented");
                  break;
              }
            }
          }
          finally
          {
            if (x != null) x.Close();
          }
        }

        return list;
      }

      private void Save(List<string> list, int max)
      {
        using (MemoryStream ms = new MemoryStream())
        {
          XmlTextWriter x = null;

          try
          {
            x = new XmlTextWriter(ms, Encoding.UTF8);
            
            if (x == null)
            {
              Debug.Assert(false, "Cannot construct XmlTextWriter from memorystream");
              return;
            }

            x.Formatting = Formatting.Indented;

            x.WriteStartDocument();

            x.WriteStartElement("RecentFiles");

            foreach (string filepath in list)
            {
              x.WriteStartElement("RecentFile");
              x.WriteAttributeString("Filepath", filepath);
              x.WriteEndElement();
            }

            x.WriteEndElement();

            x.WriteEndDocument();

            x.Flush();

            using (SmartStream ss = this.OpenStream(FileMode.Create))
            {
              ss.Stream.SetLength(0);

              ms.Position = 0;

              byte[] buffer = new byte[1 << 20];
              
              for (;;)
              {
                int bytes = ms.Read(buffer, 0, buffer.Length);
                
                if (bytes == 0)
                  break;
                
                ss.Stream.Write(buffer, 0, bytes);
              }
            }
          }
          finally
          {
            if (x != null) x.Close();
          }
        }
      }
      #endregion methods

      #region internalClass
      private class SmartStream : IDisposable
      {
        private bool mIsStreamOwned = true;
        private Stream mStream = null;

        #region constructor
        public SmartStream(string filepath, FileMode mode)
        {
          this.mIsStreamOwned = true;

          Directory.CreateDirectory(Path.GetDirectoryName(filepath));

          this.mStream = File.Open(filepath, mode);
        }

        public SmartStream(Stream stream)
        {
          this.mIsStreamOwned = false;
          this.mStream = stream;
        }
        #endregion constructor

        public Stream Stream
        {
          get
          {
            return this.mStream;
          }
        }

        public static implicit operator Stream(SmartStream me)
        {
          return me.Stream;
        }

        public void Dispose()
        {
          if (this.mIsStreamOwned && this.mStream != null)
            this.mStream.Dispose();

          this.mStream = null;
        }
      }
      #endregion internalClass
    }
    #endregion subClasses
  }
}
