using System;

namespace Nord.AngularUiGen.WinControls
{
  public class SelectionChangedEventArgs<TDataType>:EventArgs
  {
    public TDataType SelectedValue { get; set; }
  }
}