using System.Linq;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

[ElementProblemAnalyzer(
    typeof(ICSharpDeclaration),
    // Allows to disable the problem analyzer if code inspection is disabled
    HighlightingTypes = new[] { typeof(SampleDeclarationHighlighting) })]
public class SampleDeclarationProblemAnalyzer : ElementProblemAnalyzer<ICSharpDeclaration>
{
    protected override void Run(
        ICSharpDeclaration element,
        ElementProblemAnalyzerData data,
        IHighlightingConsumer consumer)
    {
        if (element.NameIdentifier?.Name.All(char.IsUpper) ?? true)
            return;

        consumer.AddHighlighting(new SampleDeclarationHighlighting(element));
    }
}