
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace resource.package
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(CONSTANT.GUID)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class PreviewVIDEO : AsyncPackage
    {
        internal static class CONSTANT
        {
            public const string APPLICATION = "Visual Studio";
            public const string COMPANY = "Viacheslav Lozinskyi";
            public const string COPYRIGHT = "Copyright (c) 2020-2023 by Viacheslav Lozinskyi. All rights reserved.";
            public const string DESCRIPTION = "Quick preview the most popular video files";
            public const string GUID = "DF3B9597-5265-4147-9042-13AC92159207";
            public const string HOST = "MetaOutput";
            public const string NAME = "Preview-VIDEO";
            public const string VERSION = "1.0.11";
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            {
                extension.AnyPreview.Connect();
                extension.AnyPreview.Register(".3G2", new resource.preview.Extractor());
                extension.AnyPreview.Register(".3GP", new resource.preview.Extractor());
                extension.AnyPreview.Register(".ASF", new resource.preview.Taglib());
                extension.AnyPreview.Register(".AVI", new resource.preview.Taglib());
                extension.AnyPreview.Register(".F4V", new resource.preview.Extractor());
                extension.AnyPreview.Register(".M4V", new resource.preview.Taglib());
                extension.AnyPreview.Register(".MKV", new resource.preview.Taglib());
                extension.AnyPreview.Register(".MOV", new resource.preview.Extractor());
                extension.AnyPreview.Register(".MP4", new resource.preview.Taglib());
                extension.AnyPreview.Register(".MPEG", new resource.preview.Taglib());
                extension.AnyPreview.Register(".MPG", new resource.preview.Taglib());
                extension.AnyPreview.Register(".MP1", new resource.preview.Taglib());
                extension.AnyPreview.Register(".MP2", new resource.preview.Taglib());
                extension.AnyPreview.Register(".OGV", new resource.preview.Taglib());
                extension.AnyPreview.Register(".WMV", new resource.preview.Taglib());
            }
            {
                await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            }
            try
            {
                if (string.IsNullOrEmpty(atom.Trace.GetFailState(CONSTANT.APPLICATION)) == false)
                {
                    var a_Context = Package.GetGlobalService(typeof(SDTE)) as DTE2;
                    if (a_Context != null)
                    {
                        var a_Context1 = (OutputWindowPane)null;
                        for (var i = a_Context.ToolWindows.OutputWindow.OutputWindowPanes.Count; i >= 1; i--)
                        {
                            if (a_Context.ToolWindows.OutputWindow.OutputWindowPanes.Item(i).Name == CONSTANT.HOST)
                            {
                                a_Context1 = a_Context.ToolWindows.OutputWindow.OutputWindowPanes.Item(i);
                                break;
                            }
                        }
                        if (a_Context1 == null)
                        {
                            a_Context1 = a_Context.ToolWindows.OutputWindow.OutputWindowPanes.Add(CONSTANT.HOST);
                        }
                        if (a_Context1 != null)
                        {
                            a_Context1.OutputString("\r\n" + CONSTANT.NAME + " extension doesn't work without MetaOutput.\r\n    Please install it --> https://www.metaoutput.net/download\r\n");
                            a_Context1.Activate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override int QueryClose(out bool canClose)
        {
            {
                extension.AnyPreview.Disconnect();
                canClose = true;
            }
            return VSConstants.S_OK;
        }
    }
}
