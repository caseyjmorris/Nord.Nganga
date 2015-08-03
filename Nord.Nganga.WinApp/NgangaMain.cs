﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Nord.Nganga.Core;
using Nord.Nganga.Fs.Coordination;

namespace Nord.Nganga.WinApp
{
  public partial class NgangaMain : Form
  {
    private readonly Func<StringFormatProviderVisitor, IEnumerable<CoordinationResult>> coordinationResultProvider;

    public NgangaMain(Func<StringFormatProviderVisitor, IEnumerable<CoordinationResult>> coordinationResultProvider)
    {
      this.coordinationResultProvider = coordinationResultProvider;
      this.InitializeComponent();
    }

    private void NgangaMain_Load(object sender, EventArgs e)
    {
      this.SetId("Main");
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      var coordinationResults = this.coordinationResultProvider(NgangaLog.Instance.Log).ToList();
      if (!coordinationResults.Any())
      {
        return;
      }

      if (coordinationResults.Count() == 1)
      {
        (new CoordinationResultBrowser(coordinationResults.First())).Show();
      }
      else
      {
        (new CoordinationResultCollectionBrowser(coordinationResults)).Show();
      }
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      (new AppDomainAssemblyListBrowser()).Show();
    }
  }
}