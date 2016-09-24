// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Xunit;

namespace System.Tests
{
    public partial class AppContextTests : RemoteExecutorTestBase
    {
        [Fact]
        public void UnhandledException_Add_Remove()
        {
            var handler = new UnhandledExceptionEventHandler((sender, args) => { });
            AppContext.UnhandledException += handler;
            AppContext.UnhandledException -= handler;
        }

        [Fact]
        public void UnhandledException_NotCalled_When_Handled()
        {
            var unhanlder = new UnhandledExceptionEventHandler(NotExpectedToBeCalledHandler);
            AppContext.UnhandledException += unhanlder;
            try
            {
                throw new Exception();
            }
            catch
            {
            }
            AppContext.UnhandledException -= unhanlder;
        }

        [Fact]
        public void UnhandledException_Called()
        {
            System.IO.File.Delete("success.txt");
            RemoteInvokeOptions options = new RemoteInvokeOptions();
            options.CheckExitCode = false;
            RemoteInvoke(() =>
            {
                AppContext.UnhandledException += new UnhandledExceptionEventHandler((sender, args) =>
                {
                    System.IO.File.Create("success.txt");
                });
                throw new Exception("****This Unhandled Exception is excepted***");
#pragma warning disable 0162
                return SuccessExitCode;
#pragma warning restore 0162
            }, options).Dispose();

            Assert.True(System.IO.File.Exists("success.txt"));
        }

        private static void NotExpectedToBeCalledHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Assert.True(false, "UnhandledException handler not expected to be called");
        }
    }
}