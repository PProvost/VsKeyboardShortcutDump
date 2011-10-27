﻿using System;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.Text;

namespace PeterProvost.VsKeyboardShortcutsDump
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidVsKeyboardShortcutsDumpPkgString)]
    public sealed class VsKeyboardShortcutsDumpPackage : Package
    {
        DTE dte;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public VsKeyboardShortcutsDumpPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            this.dte = (DTE)GetService(typeof(SDTE));
            Debug.Assert(this.dte != null);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidVsKeyboardShortcutsDumpCmdSet, (int)PkgCmdIDList.cmdidVsKeyboardShortcutDump);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var outputWindow = (IVsOutputWindow)GetService(typeof(SVsOutputWindow));
            var pane = default(IVsOutputWindowPane);
            Guid paneGuid = Guid.NewGuid();
            outputWindow.CreatePane(ref paneGuid, "Keybindings", 1, 0);
            outputWindow.GetPane(ref paneGuid, out pane);

            List<string> lines = new List<string>();
            foreach (Command cmd in dte.Commands)
            {
                if (String.IsNullOrWhiteSpace(cmd.Name))
                    continue;

                var bindingsSafeArray = (object[]) cmd.Bindings;
                if (bindingsSafeArray.Length > 0)
                {
                    List<string> bindingStrings = new List<string>();
                    foreach (var binding in bindingsSafeArray)
                        bindingStrings.Add(binding.ToString());

                    var combinedBindingString = bindingStrings.Aggregate((current, next) => current + ", " + next);
                    lines.Add(string.Format("{0}\t{1}", cmd.Name, combinedBindingString));
                }
            }

            lines.Sort();

            foreach( var line in lines)
                pane.OutputString(line + "\r\n");
        }

    }
}
