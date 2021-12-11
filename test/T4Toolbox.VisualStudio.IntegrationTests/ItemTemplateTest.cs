// <copyright file="ItemTemplateTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.IntegrationTests
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ItemTemplateTest : IntegrationTest
    {
        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task VerifyGeneratorItemTemplateAsync()
        {
            await this.VerifyPartialTemplateAsync("Generator");
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task VerifyScriptItemTemplateAsync()
        {
            await this.VerifyFullTemplateAsync("Script");
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public async Task VerifyTemplateItemTemplateAsync()
        {
            await this.VerifyPartialTemplateAsync("Template");
        }

        private async Task VerifyPartialTemplateAsync(string templateName)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            ProjectItem projectItem = this.CreateTestProjectItem(templateName);

            // Assert that project item created from partial template doesn't have CustomTool/Generator set to prevent T4 transformation
            Assert.AreEqual(string.Empty, projectItem.GetItemAttribute(ItemMetadata.Generator));
            Assert.AreEqual(string.Empty, projectItem.Properties.Item(ProjectItemProperty.CustomTool).Value);
            Assert.AreEqual(0, projectItem.ProjectItems.Count);
        }

        private async Task VerifyFullTemplateAsync(string templateName)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            ProjectItem projectItem = this.CreateTestProjectItem(templateName);

            // Verify that project item created from full template has CustomTool/Generator set to enable T4 transformation
            const string TextTemplatingFileGenerator = "TextTemplatingFileGenerator";
            Assert.AreEqual(TextTemplatingFileGenerator, projectItem.GetItemAttribute(ItemMetadata.Generator));
            Assert.AreEqual(TextTemplatingFileGenerator, projectItem.Properties.Item(ProjectItemProperty.CustomTool).Value);

            // Verify that output file was automatically generated
            ProjectItem outputItem = projectItem.ProjectItems.Cast<ProjectItem>().Single();

            // Verify that output file has extension default for the target language
            string outputFileName = outputItem.FileNames[1];
            Assert.AreEqual(this.TargetProject.CodeFileExtension, Path.GetExtension(outputFileName));

            // Verify that output file does not contain the T4 "ErrorGeneratingOutput"
            string generatedOutput = File.ReadAllText(outputFileName);
            Assert.AreEqual(string.Empty, generatedOutput.Trim());

            // Verify that no errors were reported in the Error List winodw for the new project item or its output
            Assert.IsFalse(IntegrationTest.ErrorItems.Any(error => error.FileName == projectItem.FileNames[1]));
            Assert.IsFalse(IntegrationTest.ErrorItems.Any(error => error.FileName == outputItem.FileNames[1]));
        }
    }
}
