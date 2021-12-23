﻿// <copyright file="TransformationContextProviderTest.cs" company="Oleg Sych">
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
    public class TransformationContextProviderTest : IntegrationTest
    {
        private Project project;
        private ITransformationContextProvider provider;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            this.project = this.CreateTestProject();
            this.provider = (ITransformationContextProvider)ServiceProvider.GetService(typeof(ITransformationContextProvider));
        }

        #region GetMetdataValue

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public void GetMetadataValueReturnsValueOfMetadataWithSpecifiedName()
        {
            ProjectItem testItem = this.CreateTestProjectItem("Text File");
            const string MetadataName = "TestMetadata";
            const string MetadataValue = "TestValue";
            testItem.SetItemAttribute(MetadataName, MetadataValue);
            Assert.AreEqual(MetadataValue, this.provider.GetMetadataValue(this.project.AsHierarchy(), testItem.FileNames[1], MetadataName));
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public void GetMetadataValueReturnsValueOfMetadataFromParentItemToSupportParametersForScriptFileGenerator()
        {
            Project project = this.CreateTestProject();

            string parentFile = Path.Combine(Path.GetDirectoryName(project.FullName), Path.GetRandomFileName());
            ProjectItem parentItem = IntegrationTest.CreateTestProjectItemFromFile(project.ProjectItems, parentFile);
            const string MetadataName = "TestMetadata";
            const string MetadataValue = "TestValue";
            parentItem.SetItemAttribute(MetadataName, MetadataValue);

            string testFile = Path.Combine(Path.GetDirectoryName(parentFile), Path.GetRandomFileName());
            ProjectItem testItem = IntegrationTest.CreateTestProjectItemFromFile(parentItem.ProjectItems, testFile);

            Assert.AreEqual(MetadataValue, this.provider.GetMetadataValue(this.project.AsHierarchy(), testItem.FileNames[1], MetadataName));
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public void GetMetadataValueReturnsEmptyStringMetadataDoesNotExist()
        {
            ProjectItem testItem = this.CreateTestProjectItem("Text File");
            Assert.AreEqual(string.Empty, this.provider.GetMetadataValue(this.project.AsHierarchy(), testItem.FileNames[1], "NonExistentMetadata"));
        }

        #endregion

        #region GetPropertyValue

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public void GetPropertyValueReturnsValueOfPropertyWithSpecifiedName()
        {
            string projectGuidString = this.provider.GetPropertyValue(this.project.AsHierarchy(), "ProjectGuid");
            Guid projectGuid;
            Assert.IsTrue(Guid.TryParse(projectGuidString, out projectGuid));
            Assert.AreNotEqual(Guid.Empty, projectGuid);
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public void GetPropertyValueSupportsReservedPropertiesNotStoredInProjectFile()
        {
            string propertyValue = this.provider.GetPropertyValue(this.project.AsHierarchy(), "MSBuildProjectFullPath");
            Assert.AreEqual(this.project.FullName, propertyValue);
        }

        [TestMethod, DataSource(TargetProject.Provider, TargetProject.Connection, TargetProject.Table, DataAccessMethod.Sequential)]
        public void GetPropertyValueReturnsEmptyStringWhenPropertyDoesNotExist()
        {
            Assert.AreEqual(string.Empty, this.provider.GetPropertyValue(this.project.AsHierarchy(), "NonExistentProperty"));
        }

        #endregion
    }
}