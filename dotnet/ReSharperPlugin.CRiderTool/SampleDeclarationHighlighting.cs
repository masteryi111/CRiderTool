using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

[RegisterConfigurableSeverity(
    SeverityId,
    CompoundItemName: null,
    Group: HighlightingGroupIds.CodeSmell,
    Title: Message,
    Description: Description,
    DefaultSeverity: Severity.WARNING)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    OverlapResolve = OverlapResolveKind.ERROR,
    OverloadResolvePriority = 0,
    // Appears in solution-wide analysis
    ToolTipFormatString = Message)]
public class SampleDeclarationHighlighting : IHighlighting
{
    // Appears in suppression comments
    public const string SeverityId = "SampleDeclarationInspection";

    // Appears in settings
    public const string Message = $"ReSharper SDK: {nameof(SampleDeclarationHighlighting)}.{nameof(Message)}";
    public const string Description = $"ReSharper SDK: {nameof(SampleDeclarationHighlighting)}.{nameof(Description)}";

    public SampleDeclarationHighlighting(ICSharpDeclaration declaration)
    {
        Declaration = declaration;
    }

    // Used for IsValid and quick-fixes
    public ICSharpDeclaration Declaration { get; }

    public bool IsValid() => Declaration.IsValid();
    public DocumentRange CalculateRange() => Declaration.NameIdentifier.NotNull().GetHighlightingRange();

    // Appears in editor
    public string ToolTip => $"ReSharper SDK: {nameof(SampleDeclarationHighlighting)}.{nameof(Message)}";

    // Appears in scrollbar
    public string ErrorStripeToolTip => $"ReSharper SDK: {nameof(SampleDeclarationHighlighting)}.{nameof(ErrorStripeToolTip)}";
}