﻿// <copyright file="TemplateLocatorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class TemplateLocatorTest : IntegrationTest
    {
        [TestMethod]
        public async Task TemplateLocatorFindsTemplateRelativeToInputFolderAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IntegrationTest.LoadT4ToolboxPackage();
            string inputPath = this.CreateTempFile(SolutionDirectory, string.Empty);
            string templatePath = this.CreateTempFile(SolutionDirectory, string.Empty);
            var templateLocator = (TemplateLocator)ServiceProvider.GetService(typeof(TemplateLocator));
            string resolvedPath = Path.GetFileName(templatePath);
            Assert.IsTrue(templateLocator.LocateTemplate(inputPath, ref resolvedPath));
            Assert.AreEqual(templatePath, resolvedPath);
        }

        [TestMethod, Ignore] // VS 2017 loads TemplateLocator from the TestResults/Out folder instead of the extension install root
        public async Task TemplateLocatorFindsTemplateRelativeToIncludeFolderAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IntegrationTest.LoadT4ToolboxPackage();
            string inputPath = this.CreateTempFile(SolutionDirectory, string.Empty);
            string includeFolder = Path.Combine(Path.GetDirectoryName(typeof(TemplateLocator).Assembly.Location), "Include");
            string templatePath = this.CreateTempFile(includeFolder, string.Empty);

            var templateLocator = (TemplateLocator)ServiceProvider.GetService(typeof(TemplateLocator));
            string resolvedPath = Path.GetFileName(templatePath);
            Assert.IsTrue(templateLocator.LocateTemplate(inputPath, ref resolvedPath));
            Assert.AreEqual(templatePath, resolvedPath);
        }
    }
}