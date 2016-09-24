// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.ExceptionServices;
using System.Diagnostics;
using Xunit;

namespace System.Tests
{
    public partial class AppContextTests : RemoteExecutorTestBase
    {
        private static bool s_flag = false;
        [Fact]
        public void FirstChanceException_Add_Remove()
        {
            AppContext.FirstChanceException += FirstChanceHandler;
            AppContext.FirstChanceException -= FirstChanceHandler;
        }

        [Fact]
        public void FirstChanceException_Called()
        {
            bool _flag = s_flag;
            AppContext.FirstChanceException += FirstChanceHandler;
            try
            {
                throw new Exception();
            }
            catch
            {
            }
            AppContext.FirstChanceException -= FirstChanceHandler;
            if (_flag == s_flag)
                Assert.True(false, "FirstChanceHandler not called");
        }

        private static void FirstChanceHandler(object source, FirstChanceExceptionEventArgs e)
        {
            s_flag = !s_flag;
        }

        [Fact]
        public void ProcessExit_Add_Remove()
        {
            EventHandler handler = (sender, e) =>
            {
            };
            AppContext.ProcessExit += handler;
            AppContext.ProcessExit -= handler;
        }

        [Fact]
        public void ProcessExit_Called()
        {
            System.IO.File.Delete("success.txt");
            RemoteInvoke(() =>
            {
                EventHandler handler = (sender, e) =>
                {
                    System.IO.File.Create("success.txt");
                };

                AppContext.ProcessExit += handler;
                return SuccessExitCode;
            }).Dispose();

            Assert.True(System.IO.File.Exists("success.txt"));
        }

        [Fact]
        public void SetData()
        {
            Assert.Throws<ArgumentNullException>(() => { AppContext.SetData(null, null); });
            AppContext.SetData("", null);
            Assert.Null(AppContext.GetData(""));
            AppContext.SetData("randomkey", 4);
            Assert.Equal(4, AppContext.GetData("randomkey"));
        }
    }
}