using System;

namespace Nord.Nganga.WinControls
{
  public class SelectionChangedEventArgs<TDataType> : EventArgs
  {
    public TDataType SelectedValue { get; set; }
  }
}