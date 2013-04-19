﻿namespace MRU.ViewModel
{
  using System.Xml.Serialization;

  public class MRUEntryVM : Base.BaseViewModel
  {
    #region fields
    private Model.MRUEntry mMRUEntry;

    private MRUListVM mParent;
    #endregion fields

    #region Constructor
    public MRUEntryVM(MRUListVM parent)
      : this()
    {
      this.mParent = parent;
    }

    /// <summary>
    /// Constructor from model
    /// </summary>
    /// <param name="model"></param>
    public MRUEntryVM(Model.MRUEntry model,
                      MRUListVM parent) : this()
    {
      this.mMRUEntry = new Model.MRUEntry(model);
      this.mParent = parent;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource"></param>
    public MRUEntryVM(MRUEntryVM copySource,
                      MRUListVM parent)
      : this()
    {
      this.mParent = parent;

      if (copySource == null)
        return;

      this.mMRUEntry = new Model.MRUEntry(copySource.mMRUEntry);
      this.IsPinned = copySource.IsPinned;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    protected MRUEntryVM()
    {
      this.mMRUEntry = new Model.MRUEntry();
      this.IsPinned = false;
    }
    #endregion Constructor

    #region Properties
    [XmlIgnore]
    public MRUListVM Parent
    {
      get
      {
        return this.mParent;
      }

      internal set
      {
        if (this.mParent != value)
        {
          this.mParent = value;
          this.NotifyPropertyChanged(() => this.Parent);
        }
      }
    }

    [XmlAttribute(AttributeName = "PathFileName")]
    public string PathFileName
    {
      get
      {
        return this.mMRUEntry.PathFileName;
      }

      set
      {
        if (this.mMRUEntry.PathFileName != value)
        {
          this.mMRUEntry.PathFileName = value;
          this.NotifyPropertyChanged(() => this.PathFileName);
          this.NotifyPropertyChanged(() => this.DisplayPathFileName);
        }
      }
    }

    [XmlIgnore]
    public string DisplayPathFileName
    {
      get
      {
        if (this.mMRUEntry == null)
          return string.Empty;

        if (this.mMRUEntry.PathFileName == null)
          return string.Empty;

        int n = 32;
        return (this.mMRUEntry.PathFileName.Length > n ? this.mMRUEntry.PathFileName.Substring(0, 3) +
                                                "... " + this.mMRUEntry.PathFileName.Substring(this.mMRUEntry.PathFileName.Length - n)
                                              : this.mMRUEntry.PathFileName);
      }
    }

    [XmlAttribute(AttributeName = "IsPinned")]
    public bool IsPinned
    {
      get
      {
        return this.mMRUEntry.IsPinned;
      }

      set
      {
        if (this.mMRUEntry.IsPinned != value)
        {
          this.mMRUEntry.IsPinned = value;
          this.NotifyPropertyChanged(() => this.IsPinned);
        }
      }
    }
    #endregion Properties
  }
}
