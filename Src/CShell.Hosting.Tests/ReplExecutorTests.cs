using Common.Logging;
using CShell.Framework.Services;
using CShell.Hosting;
using NSubstitute;
using NUnit.Framework;
using ScriptCs;
using ScriptCs.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;

namespace CShell.Hosting.Tests
{
    [TestFixture]
    public class ReplExecutorTests
    {
        [Test]
        public void ExecuteScript_WhenAddingNamespace_ShouldBeAddedToTheNamespaceCollection()
        {
            //Setup Test Pack
            var factory = new TestObjFactory();
            var replExecutor = factory.GetReplExecutor();

            var script = "using System.Diagnostics;";
            var expectedNamespace = "System.Diagnostics";
            var filePreProcessorResult = new FilePreProcessorResult();
            filePreProcessorResult.Namespaces.Add(expectedNamespace);
            factory.FilePreProcessor.ProcessScript(script).Returns<FilePreProcessorResult>(x => filePreProcessorResult);

            //Assert Precondition

            //Execute Test
            replExecutor.Execute(script);

            //Assert
            CollectionAssert.Contains(replExecutor.Namespaces, expectedNamespace);
        }

        [TestCase(":help","Available commands are:")]
        public void ExecuteScript_WhenHandlingHelpCommand_ShouldPrintAvailableCommands(string command,string expected)
        {
            //Setup Test Pack
            var factory = new TestObjFactory();
            var replExecutor = factory.GetReplExecutor();

            //Assert Precondition

            //Execute Test
            var result = replExecutor.Execute(command);

            //Assert
            Assert.True(result.ReturnValue.ToString().Contains(expected));
        }

        [Test]
        public void ExecuteScript_WhenHandlingClearCommand_ShouldClearRepl()
        {
            //Setup Test Pack
            var factory = new TestObjFactory();
            var replExecutor = factory.GetReplExecutor();
            string command=":clear";

            //Assert Precondition

            //Execute Test
            var result = replExecutor.Execute(command);

            //Assert
            Assert.IsNull(result.ReturnValue);
            factory.Repl.Received(1).Clear();
        }

        [Test]
        public void ExecuteScript_WhenHandlingInstallPackageCommand_ShouldLogInstallingInformation()
        {
            //Setup Test Pack
            var factory = new TestObjFactory();
            var replExecutor = factory.GetReplExecutor();
            string script = ":install NUnit";

            //Assert Precondition

            //Execute Test
            var result = replExecutor.Execute(script);

            //Assert
            factory.Log.Received(1).InfoFormat("Installing {0}","NUnit");
        }


        [Test]
        public void ExecuteScript_WhenHandlingInstallPackageCommand_ShouldInstallPackageWithVersion()
        {
            //Setup Test Pack
            var factory = new TestObjFactory();
            var replExecutor = factory.GetReplExecutor();
            string script = ":install NUnit 0.1.2.3";
            var packageRef = new PackageReference("NUnit", new FrameworkName(".NETFramework,Version=v4.0"), "0.1.2.3");
            var expectedPackageReferences = new[] { packageRef };
            var allowPrePackages = false;
            //Assert Precondition

            //Execute Test
            var result = replExecutor.Execute(script);

            //Assert
            factory.PackageInstaller.Received(1).InstallPackages(Arg.Is<IEnumerable<IPackageReference>>
                (x => x.First().PackageId == packageRef.PackageId && x.First().Version==packageRef.Version), allowPrePackages);
        }


        [Test]
        public void ExecuteScript_WhenHandlingInstallPackageCommandWithoutPackageName_ShouldNotInstallPackage()
        {
            //Setup Test Pack
            var factory = new TestObjFactory();
            var replExecutor = factory.GetReplExecutor();
            string script = ":install";

            //Assert Precondition

            //Execute Test
            var result = replExecutor.Execute(script);

            //Assert
            factory.PackageInstaller.DidNotReceive().InstallPackages(Arg.Any<IEnumerable<IPackageReference>>(), Arg.Any<bool>());
        }
    }
}
