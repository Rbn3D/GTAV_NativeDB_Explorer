using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using ICSharpCode.SharpZipLib.Zip;
using NativeDb_Explorer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NativeDb_Explorer.Business
{
    public static class HTMLNativeParser
    {
        public static List<GTAVNative> ParseLocalHtmlFile(String filename)
        {
            String nativesStr = File.ReadAllText(filename);

            return ParseHtmlStr(nativesStr);
        }

        public static List<GTAVNative> ParseZippedLocalHtmlFile(String filename)
        {
            return ParseZippedFile(filename);
        }

        public static List<GTAVNative> ParseZippedHtmlUrl(String url)
        {
            WebClient Client = new WebClient();
            string zipFilename = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Client.DownloadFile(url, zipFilename);

            return ParseZippedFile(zipFilename);
        }

        private static List<GTAVNative> ParseHtmlStr(String rawHtml)
        {
            List<GTAVNative> natives = new List<GTAVNative>();

            var options = new HtmlParserOptions()
            {
                IsEmbedded = false,
                IsScripting = false,
                IsStrictMode = false,
            };

            var parser = new HtmlParser(options);
            var document = parser.Parse(rawHtml);

            var namespacesContainer = document.QuerySelector("div#mw");
            var nsTree = namespacesContainer.QuerySelector("ul.nstree");

            foreach (var ns in nsTree.Children)
            {
                var currentNamespace = ns.GetExclusiveText().Trim();

                var fnTree = ns.QuerySelector("ul.fntree");

                foreach (var fn in fnTree.Children)
                {
                    var fndelc = fn.QuerySelector(".fndecl");

                    var fnName = fndelc.GetExclusiveText().Trim().TrimEnd("()").TrimEnd("(");
                    var fnParamsStr = extractParametersSignature(fndelc, fnName);

                    var fdesc = fn.QuerySelector(".fdesc");
                    var fnDescription = fdesc?.TextContent?.Trim() ?? "";

                    var fnReturnType = fndelc.FirstChild.TextContent.Trim();
                    var fnCommentary = fndelc.LastChild.TextContent.Trim();

                    natives.Add(new GTAVNative() { Namespace = currentNamespace, FunctionName = fnName, ParametersSignature = fnParamsStr, Description = fnDescription, ReturnType = fnReturnType, Commentary = fnCommentary });
                }
                var x = "y";
            }

            return natives;
        }

        private static List<GTAVNative> ParseZippedFile(string zipFilename)
        {
            String rawHtml;

            using (var fs = new FileStream(zipFilename, FileMode.Open, FileAccess.Read))
            using (var zf = new ZipFile(fs))
            {
                var htmlFileName = "reference.html";
                var ze = zf.GetEntry(htmlFileName);
                if (ze == null)
                {
                    throw new ArgumentException($"{htmlFileName} not found in Zip file.");
                }

                using (var s = zf.GetInputStream(ze))
                {
                    TextReader tr = new StreamReader(s);

                    rawHtml = tr.ReadToEnd();
                }
            }

            return ParseHtmlStr(rawHtml);
        }

        private static string extractParametersSignature(IElement fnDefinition, string fnName)
        {
            var clone = fnDefinition.Clone();

            var fc = clone.FirstChild;
            var lc = clone.LastChild;

            clone.RemoveChild(fc);
            clone.RemoveChild(lc);

            return clone.TextContent.Trim().TrimStart(fnName);
        }
    }

    public static class Extension
    {
        public static String GetExclusiveText(this INode node)
        {
            return node.ChildNodes.OfType<IText>().Select(m => m.Text).FirstOrDefault();
        }

        public static string TrimEnd(this string target, string trimChars)
        {
            return target.TrimEnd(trimChars.ToCharArray());
        }

        public static string TrimStart(this string target, string trimChars)
        {
            return target.TrimStart(trimChars.ToCharArray());
        }
    }
}
