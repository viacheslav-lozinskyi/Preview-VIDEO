using MetadataExtractor;
using System;
using System.IO;

namespace resource.preview
{
    internal class Extractor : extension.AnyPreview
    {
        protected override void _Execute(atom.Trace context, int level, string url, string file)
        {
            var a_Context = ImageMetadataReader.ReadMetadata(file);
            {
                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.HEADER, level, "[[[Info]]]");
                {
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[File Name]]]", url);
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[File Size]]]", (new FileInfo(file)).Length.ToString());
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[Media Types]]]", "Audio, Video");
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[Raw Format]]]", __GetString(a_Context, "Expected File Name Extension").ToUpper());
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[Tag Types]]]", __GetString(a_Context, "Detected File Type Name"));
                }
            }
            {
                var a_Size = GetProperty(NAME.PROPERTY.PREVIEW_MEDIA_SIZE, true);
                {
                    a_Size = Math.Min(a_Size, __GetInteger(a_Context, "Height") / CONSTANT.OUTPUT.PREVIEW_ITEM_HEIGHT);
                    a_Size = Math.Max(a_Size, CONSTANT.OUTPUT.PREVIEW_MIN_SIZE);
                }
                for (var i = 0; i < a_Size; i++)
                {
                    context.
                        Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PREVIEW, level);
                }
            }
            {
                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FOOTER, level, "[[[Size]]]: " + __GetInteger(a_Context, "Width").ToString() + " x " + __GetInteger(a_Context, "Height").ToString());
                {
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FOLDER, level + 1, "[[[Codecs]]]");
                    {
                        context.
                            SetComment("[[[Audio]]]", "[[[Media Type]]]").
                            Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FILE, level + 2, __GetString(a_Context, "Expected File Name Extension").ToUpper() + " Audio (" + __GetString(a_Context, "Major Brand") + ")");
                        {
                            context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 3, "[[[Header]]]");
                            {
                                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 4, "[[[Channels]]]", __GetChannels(a_Context).ToString());
                                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 4, "[[[Duration]]]", __GetDuration(a_Context));
                                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 4, "[[[Media Types]]]", "[[[Audio]]]");
                                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 4, "[[[Preferred Rate]]]", __GetInteger(a_Context, "Preferred Rate").ToString());
                                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 4, "[[[Preferred Volume]]]", __GetInteger(a_Context, "Preferred Volume").ToString());
                            }
                        }
                    }
                    {
                        context.
                            SetComment("[[[Video]]]", "[[[Media Type]]]").
                            Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FILE, level + 2, __GetString(a_Context, "Expected File Name Extension").ToUpper() + " Video (" + __GetString(a_Context, "Major Brand") + ")");
                        {
                            context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 3, "[[[Header]]]");
                            {
                                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 4, "[[[Duration]]]", __GetDuration(a_Context));
                                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 4, "[[[Media Types]]]", "[[[Video]]]");
                            }
                        }
                        {
                            context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 3, "[[[Size]]]");
                            {
                                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 4, "[[[Width]]]", __GetInteger(a_Context, "Width").ToString());
                                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 4, "[[[Height]]]", __GetInteger(a_Context, "Height").ToString());
                            }
                        }
                    }
                }
            }
        }

        private static int __GetChannels(System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory> data)
        {
            var a_Result = 0;
            foreach (var a_Context in data)
            {
                if (a_Context.Name == "QuickTime Track Header")
                {
                    a_Result++;
                }
            }
            return a_Result;
        }

        private static string __GetDuration(System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory> data)
        {
            var a_Result = TimeSpan.FromMilliseconds(__GetInteger(data, "Duration"));
            {
                return
                    a_Result.Hours.ToString("D2") + ":" +
                    a_Result.Minutes.ToString("D2") + ":" +
                    a_Result.Seconds.ToString("D2") + "." +
                    a_Result.Milliseconds.ToString("D3");
            }
        }

        private static string __GetString(System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory> data, string name)
        {
            foreach (var a_Context in data)
            {
                foreach (var a_Context1 in a_Context.Tags)
                {
                    if ((a_Context1.Name == name) && (string.IsNullOrEmpty(a_Context1.Description) == false))
                    {
                        return a_Context1.Description?.Trim();
                    }
                }
            }
            return "";
        }

        private static int __GetInteger(System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory> data, string name)
        {
            foreach (var a_Context in data)
            {
                foreach (var a_Context1 in a_Context.Tags)
                {
                    var a_Result = 0.0;
                    if ((a_Context1.Name == name) && double.TryParse(a_Context1.Description, out a_Result))
                    {
                        if (a_Result > 0)
                        {
                            return (int)a_Result;
                        }
                    }
                }
            }
            return 0;
        }
    };
}
