namespace MRU.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Input;
  using System.Xml.Serialization;

  using Command;
  using Model;

  /// <summary>
  /// This enumeration is used to control the behaviour of pinned entries.
  /// </summary>
  public enum MRUSortMethod
  {
    /// <summary>
    /// Pinned entries are sorted and displayed at the beginning of the list or just be bookmarked
    /// and stay wehere they are in the list.
    /// </summary>
    PinnedEntriesFirst = 0,

    /// <summary>
    /// Pinned entries are just be bookmarked and stay wehere they are in the list. This can be useful
    /// for a list of favourites (which stay if pinned) while other unpinned entries are changed as the
    /// user keeps opening new items and thus, changing the MRU list...
    /// </summary>
    UnsortedFavourites = 1
  }

  [Serializable]
  public class MRUListVM : Base.BaseViewModel
  {
    #region Fields
    private MRUSortMethod mPinEntryAtHeadOfList = MRUSortMethod.PinnedEntriesFirst;

    private ObservableCollection<MRUEntryVM> mListOfMRUEntries;
    
    private int mMaxMruEntryCount;

    private RelayCommand mRemoveLastEntryCommand;
    private RelayCommand mRemoveFirstEntryCommand;
    private RelayCommand<MRUEntryVM> mCommandPinUnpin;
    private RelayCommand<MRUEntryVM> mCommandRemoveMruEntry;
    private RelayCommand<string> mCommandLoadFile;
    #endregion Fields

    #region Constructor
    public MRUListVM()
    {
      this.mListOfMRUEntries = null;
      this.mMaxMruEntryCount = 15;
      this.mPinEntryAtHeadOfList = MRUSortMethod.PinnedEntriesFirst;
    }

    public MRUListVM(MRUSortMethod pinEntryAtHeadOfList = MRUSortMethod.PinnedEntriesFirst)
      : this()
    {
      this.mPinEntryAtHeadOfList = pinEntryAtHeadOfList;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copy"></param>
    public MRUListVM(MRUListVM copy)
      : this()
    {
      if (copy == null)
        return;

      this.mPinEntryAtHeadOfList = copy.mPinEntryAtHeadOfList;

      if (copy.mListOfMRUEntries != null)
        this.mListOfMRUEntries = new ObservableCollection<MRUEntryVM>(copy.mListOfMRUEntries);

      this.mMaxMruEntryCount = copy.mMaxMruEntryCount;
    }
    #endregion Constructor

    #region Properties
    [XmlAttribute(AttributeName = "MinValidMRUCount")]
    public int MinValidMruEntryCount
    {
      get
      {
        return 5;
      }
    }

    [XmlAttribute(AttributeName = "MaxValidMRUCount")]
    public int MaxValidMruEntryCount
    {
      get
      {
        return 256;
      }
    }

    [XmlAttribute(AttributeName = "MaxMruEntryCount")]
    public int MaxMruEntryCount
    {
      get
      {
        return this.mMaxMruEntryCount;
      }

      set
      {
        if (this.mMaxMruEntryCount != value)
        {
          if (value < this.MinValidMruEntryCount || value > this.MaxValidMruEntryCount)
            throw new ArgumentOutOfRangeException("MaxMruEntryCount", value, "Valid values are: value >= 5 and value <= 256");

          this.mMaxMruEntryCount = value;

          this.NotifyPropertyChanged(() => this.MaxMruEntryCount);
        }
      }
    }

    /// <summary>
    /// Get/set property to determine whether a pinned entry is shown
    /// 1> at the beginning of the MRU list
    /// or
    /// 2> remains where it currently is.
    /// </summary>
    [XmlAttribute(AttributeName = "SortMethod")]
    public MRUSortMethod PinSortMode
    {
      get
      {
        return this.mPinEntryAtHeadOfList;
      }

      set
      {
        if (this.mPinEntryAtHeadOfList != value)
        {
          this.mPinEntryAtHeadOfList = value;
          this.NotifyPropertyChanged(() => this.PinSortMode);
        }
      }
    }

    [XmlArrayItem("MRUList", IsNullable = false)]
    public ObservableCollection<MRUEntryVM> ListOfMRUEntries
    {
      get
      {
        return this.mListOfMRUEntries;
      }

      set
      {
        if (this.mListOfMRUEntries != value)
        {
          this.mListOfMRUEntries = value;

          this.NotifyPropertyChanged(() => this.ListOfMRUEntries);
        }
      }
    }

    #region EntryCommands
    [XmlIgnore]
    public ICommand RemoveFirstEntryCommand
    {
      get
      {
        if (this.mRemoveFirstEntryCommand == null)
          this.mRemoveFirstEntryCommand = new RelayCommand(() => this.OnRemoveMRUEntry(Model.MRUList.Spot.First));

        return this.mRemoveFirstEntryCommand;
      }
    }
    
    [XmlIgnore]
    public ICommand RemoveLastEntryCommand
    {
      get
      {
        if (this.mRemoveLastEntryCommand == null)
          this.mRemoveLastEntryCommand = new RelayCommand(() => this.OnRemoveMRUEntry(Model.MRUList.Spot.Last));

        return this.mRemoveLastEntryCommand;
      }
    }

    /// <summary>
    /// Remove the MRU entry that is supplied as parameter of this command.
    /// </summary>
    [XmlIgnore]
    public ICommand CommandRemoveMruEntry
    {
      get
      {
        if (this.mCommandRemoveMruEntry == null)
          this.mCommandRemoveMruEntry = new RelayCommand<MRUEntryVM>(p => this.OnRemoveMruEntry_Executed(p));

        return this.mCommandRemoveMruEntry;
      }
    }

    /// <summary>
    /// Change the status of an MRU entry from pinned to unpinned or vice versa
    /// </summary>
    [XmlIgnore]
    public ICommand CommandPinUnpin
    {
      get
      {
        if (this.mCommandPinUnpin == null)
          this.mCommandPinUnpin = new RelayCommand<MRUEntryVM>(p => this.OnPinUnpin(p));

        return this.mCommandPinUnpin;
      }
    }

    /// <summary>
    /// Change the status of an MRU entry from pinned to unpinned or vice versa
    /// </summary>
    [XmlIgnore]
    public ICommand CommandLoadFile
    {
      get
      {
////        if (this.mCommandLoadFile == null)
////          this.mCommandLoadFile = new RelayCommand<string>(p => this.OnPinUnpin(p));

        // Is initialized via BindLoadFileCommand method to allow binding to command
        // implementation outside this viewmodel
        return this.mCommandLoadFile;
      }
    }
    #endregion EntryCommands

    [XmlIgnore]
    private string AppName
    {
      get
      {
        return Application.ResourceAssembly.GetName().Name;
      }
    }
    #endregion Properties

    #region Methods
    // Summary:
    //     Runs when the entire object graph has been deserialized.
    //
    // Parameters:
    //   sender:
    //     The object that initiated the callback. The functionality for this parameter
    public void InitOnDeserialization()
    {
      if (this.mListOfMRUEntries != null)
      {
        for (int i = 0; i < this.mListOfMRUEntries.Count; i++)
        {
          if (this.mListOfMRUEntries[i] != null)
            this.mListOfMRUEntries[i].Parent = this;
        }
      }
    }

    public void BindLoadFileCommand(Func<object, object> commandExecuted,
                                    Func<object, bool> commandCanExecute)
    {
      if (commandCanExecute != null && commandCanExecute != null)
      {
        this.mCommandLoadFile = new RelayCommand<string>(p => commandExecuted(p),
                                                         p => commandCanExecute(p));
      }
    }

    /// <summary>
    /// Add another file name - path reference into the MRU collection.
    /// </summary>
    /// <param name="filePath"></param>
    public void AddNewEntryIntoMRU(string filePath)
    {
      if (this.FindMRUEntry(filePath) == null)
      {
        MRUEntryVM e = new MRUEntryVM(this) { IsPinned = false, PathFileName = filePath };

        this.AddMRUEntry(e);
      }
    }

    private MRUEntryVM FindMRUEntry(string filePathName)
    {
      try
      {
        if (this.mListOfMRUEntries == null)
          return null;

        return this.mListOfMRUEntries.SingleOrDefault(mru => mru.PathFileName == filePathName);
      }
      catch (Exception exp)
      {
        MessageBox.Show(this.AppName + " encountered an error when removing an entry:" + Environment.NewLine
                      + Environment.NewLine
                      + exp.ToString(), "Error when pinning an MRU entry", MessageBoxButton.OK, MessageBoxImage.Error);

        return null;
      }
    }

    #region AddRemove Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pinOrUnPinMruEntry"></param>
    /// <param name="mruEntry"></param>
    private bool PinUnpinEntry(bool pinOrUnPinMruEntry, MRUEntryVM mruEntry)
    {
      try
      {
        if (this.mListOfMRUEntries == null)
          return false;

        int pinnedMruEntryCount = this.CountPinnedEntries();

        // pin an MRU entry into the next available pinned mode spot
        if (pinOrUnPinMruEntry == true)
        {
          MRUEntryVM e = this.mListOfMRUEntries.Single(mru => mru.IsPinned == false && mru.PathFileName == mruEntry.PathFileName);

          if (this.PinSortMode == MRUSortMethod.PinnedEntriesFirst)
            this.mListOfMRUEntries.Remove(e);

          e.IsPinned = true;

          if (this.PinSortMode == MRUSortMethod.PinnedEntriesFirst)
            this.mListOfMRUEntries.Insert(pinnedMruEntryCount, e);

          pinnedMruEntryCount += 1;
          //// this.NotifyPropertyChanged(() => this.ListOfMRUEntries);

          return true;
        }
        else
        {
          // unpin an MRU entry into the next available unpinned spot
          MRUEntryVM e = this.mListOfMRUEntries.Single(mru => mru.IsPinned == true && mru.PathFileName == mruEntry.PathFileName);

          if (this.PinSortMode == MRUSortMethod.PinnedEntriesFirst)
            this.mListOfMRUEntries.Remove(e);
          
          e.IsPinned = false;
          pinnedMruEntryCount -= 1;

          if (this.PinSortMode == MRUSortMethod.PinnedEntriesFirst)
            this.mListOfMRUEntries.Insert(pinnedMruEntryCount, e);

          //// this.NotifyPropertyChanged(() => this.ListOfMRUEntries);

          return true;
        }
      }
      catch (Exception exp)
      {
        MessageBox.Show(this.AppName + " encountered an error when pinning an entry:" + Environment.NewLine
                      + Environment.NewLine
                      + exp.ToString(), "Error when pinning an MRU entry", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      return false;
    }

    /// <summary>
    /// Standard short-cut method to add a new unpinned entry from a string
    /// </summary>
    /// <param name="newEntry">File name and path file</param>
    private void AddMRUEntry(string newEntry)
    {
      if (newEntry == null || newEntry == string.Empty)
        return;

      this.AddMRUEntry(new MRUEntryVM(this) { IsPinned = false, PathFileName = newEntry });
    }

    private void AddMRUEntry(MRUEntryVM newEntry)
    {
      if (newEntry == null) return;

      try
      {
        if (this.mListOfMRUEntries == null)
          this.mListOfMRUEntries = new ObservableCollection<MRUEntryVM>();

        // Remove all entries that point to the path we are about to insert
        MRUEntryVM e = this.mListOfMRUEntries.SingleOrDefault(item => newEntry.PathFileName == item.PathFileName);

        if (e != null)
        {
          // Do not change an entry that has already been pinned -> its pinned in place :)
          if (e.IsPinned == true)
            return;

          this.mListOfMRUEntries.Remove(e);
        }

        // Remove last entry if list has grown too long
        if (this.MaxMruEntryCount <= this.mListOfMRUEntries.Count)
          this.mListOfMRUEntries.RemoveAt(this.mListOfMRUEntries.Count - 1);

        // Add model entry in ViewModel collection (First pinned entry or first unpinned entry)
        if (newEntry.IsPinned == true)
          this.mListOfMRUEntries.Insert(0, new MRUEntryVM(newEntry, this));
        else
        {
          this.mListOfMRUEntries.Insert(this.CountPinnedEntries(), new MRUEntryVM(newEntry, this));
        }
      }
      catch (Exception exp)
      {
        MessageBox.Show(exp.ToString(), "An error has occurred", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private bool RemoveEntry(string fileName)
    {
      try
      {
        if (this.mListOfMRUEntries == null)
          return false;

        MRUEntryVM e = this.mListOfMRUEntries.Single(mru => mru.PathFileName == fileName);

        e.Parent = null;

        this.mListOfMRUEntries.Remove(e);

        return true;
      }
      catch (Exception exp)
      {
        MessageBox.Show(this.AppName + " encountered an error when removing an entry:" + Environment.NewLine
                      + Environment.NewLine
                      + exp.ToString(), "Error when pinning an MRU entry", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      return false;
    }

    private bool RemovePinEntry(MRUEntryVM mruEntry)
    {
      try
      {
        if (this.mListOfMRUEntries == null)
          return false;

        MRUEntryVM e = this.mListOfMRUEntries.Single(mru => mru.PathFileName == mruEntry.PathFileName);

        e.Parent = null;
        
        this.mListOfMRUEntries.Remove(e);

        return true;
      }
      catch (Exception exp)
      {
        MessageBox.Show(this.AppName + " encountered an error when removing an entry:" + Environment.NewLine
                      + Environment.NewLine
                      + exp.ToString(), "Error when pinning an MRU entry", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      return false;
    }

    private void OnRemoveMruEntry_Executed(object o)
    {
      try
      {
        MRUEntryVM cmdParam = o as MRUEntryVM;

        if (cmdParam == null)
          return;

        this.RemovePinEntry(cmdParam);
      }
      catch (Exception exp)
      {
        MessageBox.Show(exp.ToString(), exp.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
      }
    }

    /// <summary>
    /// Change the status of an MRU entry from pinned to unpinned or vice versa
    /// </summary>
    /// <param name="o"></param>
    private void OnPinUnpin(object o)
    {
      try
      {
        MRUEntryVM cmdParam = o as MRUEntryVM;

        if (cmdParam == null)
          return;

        this.PinUnpinEntry(!cmdParam.IsPinned, cmdParam);
      }
      catch (Exception exp)
      {
        MessageBox.Show(exp.ToString(), exp.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
      }
    }

    private void OnRemoveMRUEntry(Model.MRUList.Spot addInSpot = Model.MRUList.Spot.Last)
    {
      if (this.mListOfMRUEntries == null)
        return;

      if (this.mListOfMRUEntries.Count == 0)
        return;

      switch (addInSpot)
      {
        case MRUList.Spot.Last:
          this.mListOfMRUEntries.RemoveAt(this.mListOfMRUEntries.Count - 1);
          break;

        case MRUList.Spot.First:
          this.mListOfMRUEntries.RemoveAt(0);
          break;

        default:
          break;
      }
    }

    private int CountPinnedEntries()
    {
      if (this.mListOfMRUEntries != null)
        return this.mListOfMRUEntries.Count(mru => mru.IsPinned == true);

      return 0;
    }
    #endregion AddRemove Methods
    #endregion Methods
  }
}
