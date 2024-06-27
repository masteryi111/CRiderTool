using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReSharperPlugin.CRiderTool
{
    [ZoneDefinition]
    [ZoneDefinitionConfigurableFeature("Title111", "Description", IsInProductSection: false)]
    public interface ICRiderToolZone : IZone
    {
    }
}
