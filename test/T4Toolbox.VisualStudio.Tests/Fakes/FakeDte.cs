// <copyright file="FakeDte.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Tests.Fakes
{
    using System;
    using System.IO;

    using EnvDTE;

    internal class FakeDte : OleServiceProvider, DTE
    {
        private DirectoryInfo testDirectory;

        public FakeDte()
        {
            this.ObjectExtenders = new FakeObjectExtenders(this);
            this.AddService(typeof(DTE), this, false);
        }

        public FakeObjectExtenders ObjectExtenders { get; private set; }

        public FakeSolution Solution { get; set; }

        #region EnvDTE.DTE

        Document DTE.ActiveDocument
        {
            get { throw new NotImplementedException(); }
        }

        object DTE.ActiveSolutionProjects
        {
            get { throw new NotImplementedException(); }
        }

        Window DTE.ActiveWindow
        {
            get { throw new NotImplementedException(); }
        }

        AddIns DTE.AddIns
        {
            get { throw new NotImplementedException(); }
        }

        DTE DTE.Application
        {
            get { throw new NotImplementedException(); }
        }

        object DTE.CommandBars
        {
            get { throw new NotImplementedException(); }
        }

        string DTE.CommandLineArguments
        {
            get { throw new NotImplementedException(); }
        }

        Commands DTE.Commands
        {
            get { throw new NotImplementedException(); }
        }

        ContextAttributes DTE.ContextAttributes
        {
            get { throw new NotImplementedException(); }
        }

        DTE DTE.DTE
        {
            get { throw new NotImplementedException(); }
        }

        Debugger DTE.Debugger
        {
            get { throw new NotImplementedException(); }
        }

        vsDisplay DTE.DisplayMode
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        Documents DTE.Documents
        {
            get { throw new NotImplementedException(); }
        }

        string DTE.Edition
        {
            get { throw new NotImplementedException(); }
        }

        Events DTE.Events
        {
            get { throw new NotImplementedException(); }
        }

        void DTE.ExecuteCommand(string commandName, string commandArgs)
        {
            throw new NotImplementedException();
        }

        string DTE.FileName
        {
            get { throw new NotImplementedException(); }
        }

        Find DTE.Find
        {
            get { throw new NotImplementedException(); }
        }

        string DTE.FullName
        {
            get { throw new NotImplementedException(); }
        }

        object DTE.GetObject(string name)
        {
            throw new NotImplementedException();
        }

        Globals DTE.Globals
        {
            get { throw new NotImplementedException(); }
        }

        ItemOperations DTE.ItemOperations
        {
            get { throw new NotImplementedException(); }
        }

        wizardResult DTE.LaunchWizard(string file, ref object[] contextParams)
        {
            throw new NotImplementedException();
        }

        int DTE.LocaleID
        {
            get { throw new NotImplementedException(); }
        }

        Macros DTE.Macros
        {
            get { throw new NotImplementedException(); }
        }

        DTE DTE.MacrosIDE
        {
            get { throw new NotImplementedException(); }
        }

        Window DTE.MainWindow
        {
            get { throw new NotImplementedException(); }
        }

        vsIDEMode DTE.Mode
        {
            get { throw new NotImplementedException(); }
        }

        string DTE.Name
        {
            get { throw new NotImplementedException(); }
        }

        ObjectExtenders DTE.ObjectExtenders
        {
            get { return this.ObjectExtenders; }
        }

        Window DTE.OpenFile(string viewKind, string fileName)
        {
            throw new NotImplementedException();
        }

        void DTE.Quit()
        {
            throw new NotImplementedException();
        }

        string DTE.RegistryRoot
        {
            get { throw new NotImplementedException(); }
        }

        string DTE.SatelliteDllPath(string path, string mame)
        {
            throw new NotImplementedException();
        }

        SelectedItems DTE.SelectedItems
        {
            get { throw new NotImplementedException(); }
        }

        Solution DTE.Solution
        {
            get { return this.Solution; }
        }

        SourceControl DTE.SourceControl
        {
            get { throw new NotImplementedException(); }
        }

        StatusBar DTE.StatusBar
        {
            get { throw new NotImplementedException(); }
        }

        bool DTE.SuppressUI
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        UndoContext DTE.UndoContext
        {
            get { throw new NotImplementedException(); }
        }

        bool DTE.UserControl
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        string DTE.Version
        {
            get { throw new NotImplementedException(); }
        }

        WindowConfigurations DTE.WindowConfigurations
        {
            get { throw new NotImplementedException(); }
        }

        Windows DTE.Windows
        {
            get { throw new NotImplementedException(); }
        }

        bool DTE.get_IsOpenFile(string viewKind, string fileName)
        {
            throw new NotImplementedException();
        }

        Properties DTE.get_Properties(string category, string page)
        {
            throw new NotImplementedException();
        }

        #endregion

        internal DirectoryInfo TestDirectory
        {
            get { return this.testDirectory ?? (this.testDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()))); }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (this.testDirectory != null)
                {
                    this.testDirectory.Delete(true);
                }
            }
        }
    }
}
