﻿//------------------------------------------------------------------------------
// <copyright file="NgangaGenerate.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Process = System.Diagnostics.Process;

namespace Nord.Nganga.Vsix
{
  /// <summary>
  /// Command handler
  /// </summary>
  internal sealed class NgangaGenerate
  {
    /// <summary>
    /// Command ID.
    /// </summary>
    public const int CommandId = 0x0100;

    /// <summary>
    /// Command menu group (command set GUID).
    /// </summary>
    public static readonly Guid CommandSet = new Guid("9dbd2813-773c-4c2a-b132-abedac8c6ded");

    /// <summary>
    /// VS Package that provides this command, not null.
    /// </summary>
    private readonly Package package;

    /// <summary>
    /// Initializes a new instance of the <see cref="NgangaGenerate"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    private NgangaGenerate(Package package)
    {
      if (package == null)
      {
        throw new ArgumentNullException(nameof(package));
      }

      this.package = package;

      DevEnv = ((NgangaPackage) package).DevEnv;

      var commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
      if (commandService != null)
      {
        var menuCommandID = new CommandID(CommandSet, CommandId);
        //var menuItem = new MenuCommand(this.StartNotepad, menuCommandID);
        var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
        commandService.AddCommand(menuItem);
      }
    }

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static NgangaGenerate Instance { get; private set; }

    /// <summary>
    /// Gets the service provider from the owner package.
    /// </summary>
    private IServiceProvider ServiceProvider
    {
      get { return this.package; }
    }

    /// <summary>
    /// Initializes the singleton instance of the command.
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    public static void Initialize(Package package)
    {
      Instance = new NgangaGenerate(package);


    }
        public static DTE2 DevEnv { get; private set; }

    //private void StartNotepad(object sender, EventArgs e)
    //{
    //  var proc = new Process {StartInfo = {FileName = "notepad.exe"}};
    //  proc.Start();
    //}

    ///// <summary>
    ///// This function is the callback used to execute the command when the menu item is clicked.
    ///// See the constructor to see how the menu item is associated with this function using
    ///// OleMenuCommandService service and MenuCommand class.
    ///// </summary>
    ///// <param name="sender">Event sender.</param>
    ///// <param name="e">Event args.</param>
    private void MenuItemCallback (object sender, EventArgs e)
    {
      string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
      string title = "NgangaGenerate " + DevEnv.FullName;

      // Show a message box to prove we were here
      VsShellUtilities.ShowMessageBox(
          this.ServiceProvider,
          message,
          title,
          OLEMSGICON.OLEMSGICON_INFO,
          OLEMSGBUTTON.OLEMSGBUTTON_OK,
          OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
    }
  }
}