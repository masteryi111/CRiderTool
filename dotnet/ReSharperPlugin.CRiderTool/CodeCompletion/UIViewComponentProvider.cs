using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.RdBackend.Common.Features.Documents;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems.Impl;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.Match;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Features.Intellisense.CodeCompletion.CSharp.Rules;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Resources;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.UI.Icons;
using JetBrains.Util;
using NuGet;

namespace ReSharperPlugin.CRiderTool
{
    [Language(typeof(CSharpLanguage))]
    public class UIViewComponentProvider : CSharpItemsProviderBase<CSharpCodeCompletionContext>
    {
        struct UIComponentData
        {
            public string key;
            public string type;
        }
        
        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            return context.BasicContext.CodeCompletionType == CodeCompletionType.BasicCompletion;
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, IItemsCollector collector)
        {
            var ranges = context.CompletionRanges;
            List<UIComponentData> datas = null;

            if (CompletionUtils.IsSpecificArgumentInSpecificMethod(context, out var argumentLiteral,
                    ExpressionReferenceUtils.IsLayerMaskNameToLayerMethod,
                    CompletionUtils.IsCorrespondingArgument("name")))
            {
                RiderDocument doc = context.BasicContext.Document as RiderDocument;
                if (doc != null && doc.FullPath.EndsWith("View.cs"))
                {
                    string assetPath = doc.FullPath.Substring(0, doc.FullPath.LastIndexOf("Assets"));
                    string parentDirName = Path.GetFileName(Path.GetDirectoryName(doc.FullPath));
                    string fileName = Path.GetFileNameWithoutExtension(doc.FullPath).Replace("View","");
                    string prefabPath = $"{assetPath}/Assets/UIRes/prefabs/{parentDirName}/{fileName}.prefab";
                    if (File.Exists(prefabPath))
                    {
                        datas = new List<UIComponentData>();

                        string[] lines = File.ReadAllLines(prefabPath);
                        bool start = false;
                        string key = "";
                        string value = "";
                        foreach (var line in lines)
                        {
                            if (line.Contains("bindObjs"))
                            {
                                start = true;
                                continue;
                            }

                            if (start)
                            {
                                if (line.Contains("- Key:"))
                                {
                                    key = line.Trim().Replace("- Key:","").Trim();
                                }
                                else if (line.Contains("type: "))
                                {
                                    value = line.Trim().Replace("type: ","").Trim();
                                    //记录
                                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                                    {
                                        datas.Add(new UIComponentData()
                                        {
                                            key = key,
                                            type = value,
                                        });
                                    }
                                    key = null;
                                    value = null;
                                }
                                else if (line.Contains("m_ScriptableObject"))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            bool any = false;
            if (datas != null)
            {
                any = datas.Count > 0;
                foreach (var data in datas)
                {
                    var item = new CompletionItem($"{data.key}",$"{data.type}");
                    item.InitializeRanges(ranges, context.BasicContext);
                    collector.Add(item);
                }
            }

            return any;
        }

        private sealed class CompletionItem : TextLookupItemBase, IMLSortingAwareItem
        {
            public CompletionItem([NotNull] string text,[NotNull] string type)
            {
                Text = $"\"{text}\"";
                DisplayTypeName = $"类型 - {type}";
                // LookupUtil.AddInformationText(DisplayName, extension);
            }

            public override IconId Image => PsiSymbolsThemedIcons.Const.Id;
        
            public override MatchingResult Match(PrefixMatcher prefixMatcher)
            {
                var matchingResult = prefixMatcher.Match(Text);
                if (matchingResult == null)
                    return null;
                return new MatchingResult(matchingResult.MatchedIndices, matchingResult.AdjustedScore - 100,
                    matchingResult.OriginalScore);
            }
        
            public override void Accept(
                ITextControl textControl, DocumentRange nameRange, LookupItemInsertType insertType,
                Suffix suffix, ISolution solution, bool keepCaretStill)
            {
                base.Accept(textControl, nameRange, LookupItemInsertType.Replace, suffix, solution, keepCaretStill);
            }
        
            public bool UseMLSort() => false;
        }
    }
}
