namespace EditableListLib
{
    using EditableListLib.Commands;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for EditableList.xaml
    /// </summary>
    public partial class EditableList : UserControl
    {
        #region fields
        /// <summary>
        /// Implements the backing store of the <see cref="EditableListems"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty EditableListemsProperty =
            DependencyProperty.Register("EditableListems", typeof(object),
                typeof(EditableList), new PropertyMetadata(null));

        /// <summary>
        /// Implements the backing store of the <see cref="ItemTemplateSelector"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector),
                typeof(EditableList), new PropertyMetadata(null));

        /// <summary>
        /// Implements the backing store of the <see cref="SelectedItem"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object),
                typeof(EditableList), new PropertyMetadata(null));

        private ICommand _IsKeyboardFocusWithinChangedCommand;
        #endregion fields

        #region ctor
        /// <summary>
        /// Class constructor
        /// </summary>
        public EditableList()
        {
            InitializeComponent();

            // once the element tree is loaded, focus the first listbox item
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)delegate ()
            {
                IInputElement container = lb.ItemContainerGenerator.ContainerFromIndex(0) as IInputElement;
                if (container != null)
                {
                    container.Focus();
                }
            });
        }
        #endregion ctor

        #region properties
        /// <summary>
        /// Gets/sets the source of the items that are displayed in the
        /// <see cref="EditableList"/> control.
        /// </summary>
        public object EditableListems
        {
            get { return (object)GetValue(EditableListemsProperty); }
            set { SetValue(EditableListemsProperty, value); }
        }

        /// <summary>
        /// Gets/sets a <see cref="DataTemplateSelector"/> which is used
        /// to associate a specific viewmodel or interface implementation
        /// with a specific <see cref="DataTemplate"/>.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Gets/sets the selected item from the <see cref="EditableList"/>.
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets a command that will commit the current text if the focus is lost
        /// during editing.
        /// 
        /// Expected Command Parameter:
        /// bool -> true to indicate that the control gained focus.
        ///      -> false to indicate that the control lost focus
        ///         (text edits are commited to bound viewmodel).
        /// </summary>
        public ICommand IsKeyboardFocusWithinChangedCommand
        {
            get
            {
                if (_IsKeyboardFocusWithinChangedCommand == null)
                {
                    _IsKeyboardFocusWithinChangedCommand = new RelayCommand<object>(
                    (p) =>
                    {
                        if ((p is bool) == false)
                            return;

                        bool newValue = (bool)p;

                        CommitEditOnLostFocus(newValue);
                    });
                }

                return _IsKeyboardFocusWithinChangedCommand;
            }
        }
        #endregion properties

        #region methods
        private void CommitEditOnLostFocus(object sender,
                                           DependencyPropertyChangedEventArgs e)
        {
            CommitEditOnLostFocus((bool)e.NewValue);
        }

        /// <summary>
        /// Method will commit the edit text if the given
        /// parameter <paramref name="newValue"/> is false.
        /// </summary>
        /// <param name="newValue"></param>
        private void CommitEditOnLostFocus(bool newValue)
        {
            // if the root element of the edit mode template loses focus, commit the edit
            if (newValue == false)
            {
                this.CommitChanges(null, null);
            }
        }

        /// <summary>
        /// Executes when an item of in the collection has been double clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemDoubleClick(object sender, RoutedEventArgs e)
        {
            this.EditChanges(null, null);
        }

        #region change methods
        /// <summary>
        /// Executes when the currently selected item should start its editing mode
        /// (show an editing control eg. TextBox instead of TextBlock).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditChanges(object sender, ExecutedRoutedEventArgs e)
        {
            IEditableCollectionView ecv = lb.Items as IEditableCollectionView;
            object selectedItem = lb.Items.CurrentItem;

            if (selectedItem != null && !ecv.IsEditingItem)
            {
                ecv.EditItem(selectedItem);

                // invoke focus update at loaded priority so that template swap has time to complete
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)delegate ()
                {
                    UIElement container = lb.ItemContainerGenerator.ContainerFromItem(selectedItem) as UIElement;
                    if (container != null)
                    {
                        container.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }
                });
            }
        }

        /// <summary>
        /// Executes when the edit mode on the currently selected item
        /// should be finished and the results of the editing should be kept.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommitChanges(object sender, ExecutedRoutedEventArgs e)
        {
            IEditableCollectionView ecv = lb.Items as IEditableCollectionView;
            object selectedItem = lb.Items.CurrentItem;

            if (selectedItem != null && ecv.IsEditingItem && ecv.CurrentEditItem == selectedItem)
            {
                ecv.CommitEdit();
                lb.Items.MoveCurrentTo(selectedItem);
            }
        }

        /// <summary>
        /// Executes when the edit mode on the currently selected item
        /// should be finished and the results of the editing should NOT
        /// be kept (viewmodel can rollback or implement other fallback methods).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelChanges(object sender, ExecutedRoutedEventArgs e)
        {
            IEditableCollectionView ecv = lb.Items as IEditableCollectionView;
            object selectedItem = lb.SelectedItem;

            if (selectedItem != null && ecv.IsEditingItem && ecv.CurrentEditItem == selectedItem)
            {
                ecv.CancelEdit();
                lb.Items.MoveCurrentTo(selectedItem);
            }
        }
        #endregion change methods
        #endregion methods
    }
}
