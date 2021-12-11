// <copyright file="CustomToolTemplateTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CustomToolTemplateTest : IntegrationTest
    {
        private const string TextFileItemTemplate = "Text File";

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task TemplatePropertyIsAvailableForAllProjectItemsOfTargetProjectAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            ProjectItem projectItem = this.CreateTestProjectItem(TextFileItemTemplate);
            Assert.IsNotNull(projectItem.Properties.Item(ProjectItemProperty.CustomToolTemplate));
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task TemplatePropertyGetsValueOfTemplateMetadataAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            const string TemplateFileName = "Test.tt";
            ProjectItem projectItem = this.CreateTestProjectItem(TextFileItemTemplate);
            projectItem.SetItemAttribute(ItemMetadata.Template, TemplateFileName);
            Assert.AreEqual(TemplateFileName, projectItem.Properties.Item(ProjectItemProperty.CustomToolTemplate).Value);
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task TemplatePropertySetsValueOfTemplateMetadataAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
            ProjectItem templateItem = this.CreateTestProjectItem(TextFileItemTemplate);
            inputItem.Properties.Item(ProjectItemProperty.CustomToolTemplate).Value = templateItem.FileNames[1];
            Assert.AreEqual(templateItem.FileNames[1], inputItem.GetItemAttribute(ItemMetadata.Template));
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task TemplatePropertyDoesNotConvertRelativePathsToFullAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
            ProjectItem templateItem = this.CreateTestProjectItem(TextFileItemTemplate);
            string templateFileName = Path.GetFileName(templateItem.FileNames[1]);
            inputItem.Properties.Item(ProjectItemProperty.CustomToolTemplate).Value = templateFileName;
            Assert.AreEqual(templateFileName, inputItem.GetItemAttribute(ItemMetadata.Template));
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task TemplatePropertyChangesCustomToolToTemplatedFileGeneratorWhenItIsNotSpecifiedAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
            ProjectItem templateItem = this.CreateTestProjectItem(TextFileItemTemplate);
            inputItem.Properties.Item(ProjectItemProperty.CustomToolTemplate).Value = templateItem.FileNames[1];
            Assert.AreEqual(TemplatedFileGenerator.Name, inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value);
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task TemplatePropertyThrowsExceptionWhenCustomToolIsIncompatibleAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
            inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value = "TextTemplatingFileGenerator";
            try
            {
                inputItem.Properties.Item(ProjectItemProperty.CustomToolTemplate).Value = "Test.tt";
                Assert.Fail();
            }
            catch (Exception e)
            {
                StringAssert.Contains(e.Message, TemplatedFileGenerator.Name);
            }
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task TemplatePropertyThrowsExceptionWhenTemplateCannotBeResolvedAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string nonExistentTemplateFile = Path.ChangeExtension(Path.GetRandomFileName(), ".tt");
            ProjectItem inputItem = this.CreateTestProjectItem(TextFileItemTemplate);
            try
            {
                inputItem.Properties.Item(ProjectItemProperty.CustomToolTemplate).Value = nonExistentTemplateFile;
                Assert.Fail();
            }
            catch (Exception e)
            {
                StringAssert.Contains(e.Message, nonExistentTemplateFile);
                Assert.AreEqual(string.Empty, inputItem.Properties.Item(ProjectItemProperty.CustomTool).Value);
            }
        }
    }
}