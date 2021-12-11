// <copyright file="TemplateAnalyzerTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Xunit;
    using Template = T4Toolbox.TemplateAnalysis.Template;

    public static class TemplateAnalyzerTest
    {
        [Fact]
        public static void TemplateAnalyzerIsInternalAndNotIntendedForConsumptionOutsideOfT4Toolbox()
        {
            Assert.True(typeof(TemplateAnalyzer).IsNotPublic);
        }

        [Fact]
        public static void TemplateAnalyzerIsSealedAndNotIntendedToHaveChildClasses()
        {
            Assert.True(typeof(TemplateAnalyzer).IsSealed);
        }

        [Fact]
        public static void CurrentAnalysisReturnsLastTemplateAnalysisResult()
        {
            var buffer = new FakeTextBuffer("<#@");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            TemplateAnalysis result = target.CurrentAnalysis;
            Assert.NotNull(result);
        }

        [Fact]
        public static void CurrentAnalysisReturnsSyntaxErrorsDetectedInTextBuffer()
        {
            var buffer = new FakeTextBuffer("<#@");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            Assert.Equal(1, target.CurrentAnalysis.Errors.Count);
        }

        [Fact]
        public static void CurrentAnalysisReturnsSemanticErrorsDetectedInTextBuffer()
        {
            var buffer = new FakeTextBuffer("<#@ template bad=\"puppy\" #>");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            Assert.Equal(1, target.CurrentAnalysis.Errors.Count);
        }

        [Fact]
        public static void CurrentAnalysisReturnsUpdatedErrorsWhenTextBufferChanges()
        {
            var buffer = new FakeTextBuffer("<#@");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            Assert.Equal(1, target.CurrentAnalysis.Errors.Count); // Need to touch lazy property before buffer change for test to be valid 

            buffer.CurrentSnapshot = new FakeTextSnapshot(string.Empty);
            Assert.Equal(0, target.CurrentAnalysis.Errors.Count);
        }

        [Fact]
        public static void CurrentAnalysisReturnsTemplateParsedFromTextBuffer()
        {
            var buffer = new FakeTextBuffer("<#@ template language=\"VB\" #>");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            Template template = target.CurrentAnalysis.Template;
            Assert.NotNull(template);
            Assert.Single(template.ChildNodes());
        }

        [Fact]
        public static void CurrentAnalysisReturnsDefaultTemplateIfParserCouldNotCreateOne()
        {
            var buffer = new FakeTextBuffer("<#@ t");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            Template template = target.CurrentAnalysis.Template;
            Assert.NotNull(template);
            Assert.Empty(template.ChildNodes());
        }

        [Fact]
        public static void CurrentAnalysisReturnsUpdatedTemplateWhenTextBufferChanges()
        {
            var buffer = new FakeTextBuffer("<#@ template language=\"VB\" #>");
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            Assert.Single(target.CurrentAnalysis.Template.ChildNodes()); // Need to touch lazy Template before buffer change for test to be valid

            buffer.CurrentSnapshot = new FakeTextSnapshot(string.Empty);
            Assert.Empty(target.CurrentAnalysis.Template.ChildNodes());
        }

        [Fact]
        public static void CurrentAnalysisReturnsTextSnapshotForWhichItWasCreated()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var analyzer = TemplateAnalyzer.GetOrCreate(buffer);
            ITextSnapshot analysisSnapshot = analyzer.CurrentAnalysis.TextSnapshot;
            Assert.Same(buffer.CurrentSnapshot, analysisSnapshot);
        }

        [Fact]
        public static void GetOrCreateReturnsNewTemplateAnalyzerFirstTimeItIsRequestedForTextBuffer()
        {
            ITextBuffer buffer = new FakeTextBuffer(string.Empty);
            TemplateAnalyzer analyzer = TemplateAnalyzer.GetOrCreate(buffer);
            Assert.NotNull(analyzer);
        }

        [Fact]
        public static void GetOrCreateReturnsExistingTemplateAnalyzerSecondTimeItIsRequestedForTextBuffer()
        {
            ITextBuffer buffer = new FakeTextBuffer(string.Empty);
            TemplateAnalyzer analyzer1 = TemplateAnalyzer.GetOrCreate(buffer);
            TemplateAnalyzer analyzer2 = TemplateAnalyzer.GetOrCreate(buffer);
            Assert.Same(analyzer1, analyzer2);
        }

        [Fact, SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "This is a test of garbage collection")]
        public static void GetOrCreateDoesNotPreventGarbageCollectionOfPreviouslyCreatedTemplateAnalyzers()
        {
            WeakReference GetTemplateAnalyzerReference()
            {
                var textBuffer = new FakeTextBuffer(string.Empty);
                var analyzer = TemplateAnalyzer.GetOrCreate(textBuffer);

                return new WeakReference(analyzer);
            }

            var analyzerReference = GetTemplateAnalyzerReference();

            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            Assert.False(analyzerReference.IsAlive);
        }

        [Fact, SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "This is a test of garbage collection")]
        public static void GetOrCreateDoesNotPreventGarbageCollectionOfTextBuffers()
        {
            WeakReference GetTextBufferReference()
            {
                var textBuffer = new FakeTextBuffer(string.Empty);
                var weakReference = new WeakReference(textBuffer);
                TemplateAnalyzer.GetOrCreate(textBuffer);

                return weakReference;
            }

            var bufferReference = GetTextBufferReference();

            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            Assert.False(bufferReference.IsAlive);
        }

        [Fact]
        public static void TemplateChangedEventIsRaisedWhenTextBufferChanges()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            bool templateChanged = false;
            target.TemplateChanged += (sender, args) => { templateChanged = true; };
            buffer.CurrentSnapshot = new FakeTextSnapshot("42");
            Assert.True(templateChanged);
        }

        [Fact]
        public static void TemplateChangeEventArgumentSuppliesCurrentTemplateAnalysis()
        {
            var buffer = new FakeTextBuffer(string.Empty);
            var target = TemplateAnalyzer.GetOrCreate(buffer);
            TemplateAnalysis eventArgument = null;
            target.TemplateChanged += (sender, args) => { eventArgument = args; };
            buffer.CurrentSnapshot = new FakeTextSnapshot("42");
            Assert.Same(target.CurrentAnalysis, eventArgument);
        }
    }
}