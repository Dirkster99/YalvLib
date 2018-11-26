namespace EditableListLib.Commands
{
    using System.Windows.Input;

    public static class TransactionCommands
    {
        public static RoutedUICommand Edit
            = new RoutedUICommand("Edit", "Edit", typeof(TransactionCommands));

        public static RoutedUICommand Cancel
            = new RoutedUICommand("Cancel", "Cancel", typeof(TransactionCommands));

        public static RoutedUICommand Commit
            = new RoutedUICommand("Commit", "Commit", typeof(TransactionCommands));
    }
}
