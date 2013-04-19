﻿namespace MRU.View
{
  using System.Windows;
  using System.Windows.Controls;

  public class PinableListView : ListView
  {
    // Getting CustomControl style from Themes/Generic.xaml does not work ???
    static PinableListView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PinableListView),
                new FrameworkPropertyMetadata(typeof(PinableListView)));
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
      return new PinableListViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is PinableListViewItem;
    }
  }
}
