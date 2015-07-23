using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Nord.Nganga.WinControls
{
  public partial class FontSelector : ContextMenuStrip
  {
    private Func<float> CurrentSizeProvider { get; set; }
    private Func<FontFamily> CurrentFamilyProvider { get; set; }
    private Dictionary<string, ToolStripMenuItem> fontFamilyMenuDictionary;
    private Dictionary<float, ToolStripMenuItem> fontSizeMenuDictionary;

    private ToolStripMenuItem activeFamilyItem;
    private ToolStripMenuItem activeSizeItem;

    private EventHandler<FontChangedEventArgs> ChangeHandler { get; set; }

    public FontSelector()
    {
      this.InitializeComponent();
    }

    public void Bind(Control bindingTarget )
    {
      this.CurrentSizeProvider = () => bindingTarget.Font.Size;
      this.CurrentFamilyProvider = () => bindingTarget.Font.FontFamily;
      this.LoadMenuItems();
      this.ChangeHandler = (sender, args) => bindingTarget.Font = args.NewFont ;
    }

    private void LoadMenuItems()
    {
      this.Items.Add("Font", null, null);
      var fontMenu = this.Items[0] as ToolStripMenuItem;
      if (fontMenu == null) return;
      fontMenu.DropDownItems.Add("Name", null, null);
      fontMenu.DropDownItems.Add("Size", null, null);

      this.fontFamilyMenuDictionary = new Dictionary<string, ToolStripMenuItem>();
      var currentFamily = this.CurrentFamilyProvider();
      var fontsCollection = new InstalledFontCollection();
      var fontFamilies = fontsCollection.Families;
      foreach (var fontFamily in fontFamilies)
      {
        var mi = new ToolStripMenuItem(fontFamily.Name) { Tag = fontFamily };
        mi.Click += this.font_Click;
        ((ToolStripMenuItem)fontMenu.DropDownItems[0]).DropDownItems.Add(mi);
        this.fontFamilyMenuDictionary[fontFamily.Name] = mi;
      }
      this.activeFamilyItem = this.fontFamilyMenuDictionary[currentFamily.Name];
      this.activeFamilyItem.Font =
        new Font(this.activeFamilyItem.Font.FontFamily,
          this.activeFamilyItem.Font.Size, FontStyle.Bold);

      this.fontSizeMenuDictionary = new Dictionary<float, ToolStripMenuItem>(); 
      var currentSize = this.CurrentSizeProvider();
      for (var size = (float) 2.0; size <= 72.0; size++)
      {
        var newItem = this.AddFontSize(((ToolStripMenuItem)fontMenu.DropDownItems[1]), size);
        newItem.Font = newItem.Font = new Font(newItem.Font.FontFamily, newItem.Font.Size, FontStyle.Regular);
        newItem.Click += this.size_Click;
        this.fontSizeMenuDictionary[size] = newItem;
      }
      if (!this.fontSizeMenuDictionary.ContainsKey(currentSize))
      {
        var sizeMenu = ((ToolStripMenuItem) fontMenu.DropDownItems[1]);
        var newItem = this.AddFontSize(sizeMenu, currentSize);
        newItem.Click += this.size_Click;
        this.fontSizeMenuDictionary[currentSize] = newItem;
        this.ResortToolStripItemCollection(sizeMenu.DropDownItems);
      }
      this.activeSizeItem = this.fontSizeMenuDictionary[currentSize];
      this.activeSizeItem.Font =
        new Font(this.activeSizeItem.Font.FontFamily,
          this.activeSizeItem.Font.Size, FontStyle.Bold);
    }

    private ToolStripMenuItem AddFontSize(ToolStripMenuItem sizeMenu, float size)
    {
      var mi = new ToolStripMenuItem(size.ToString(CultureInfo.InvariantCulture)) { Tag = size };
      mi.Click += this.size_Click;
      sizeMenu.DropDownItems.Add(mi);
      return mi;
    }

    private void font_Click(object sender, EventArgs e)
    {
      var menu = sender as ToolStripMenuItem;
      if (menu == null) return;
      this.activeFamilyItem.Font = new Font(this.activeFamilyItem.Font, FontStyle.Regular);

      menu.Font = new Font(this.activeFamilyItem.Font, FontStyle.Bold);
      this.activeFamilyItem = menu;
      this.NotifySettingsChange();
    }

    void size_Click(object sender, EventArgs e)
    {
      var menu = sender as ToolStripMenuItem;
      if (menu == null) return;
      this.activeSizeItem.Font = new Font(this.activeSizeItem.Font, FontStyle.Regular);
      
      menu.Font = new Font(this.activeSizeItem.Font, FontStyle.Bold);
      this.activeSizeItem = menu;
      this.NotifySettingsChange();
    }

    private void NotifySettingsChange()
    {
      if (this.ChangeHandler != null)
      {
        this.ChangeHandler(
          this, 
          new FontChangedEventArgs
          {
            NewFont = new Font((FontFamily)this.activeFamilyItem.Tag, (float) this.activeSizeItem.Tag)
          });
      }
    }
    private void ResortToolStripItemCollection(ToolStripItemCollection coll)
    {
      System.Collections.ArrayList oAList = new System.Collections.ArrayList(coll);
      oAList.Sort(new ToolStripItemComparer());
      coll.Clear();

      foreach (ToolStripItem oItem in oAList)
      {
        coll.Add(oItem);
      }
    }

    public class ToolStripItemComparer : System.Collections.IComparer
    {
      public int Compare(object x, object y)
      {
        var oItem1 = (ToolStripItem)x;
        var oItem2 = (ToolStripItem)y;
        return (float) oItem1.Tag > (float) oItem2.Tag ? 1 : -1;
      }
    }
  }
}
