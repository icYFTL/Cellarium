using Cellarium.Commands.Aliases;

namespace Cellarium.Commands.Parameters;

public class BaseParameter : BaseAlias
{
    public override AliasTypeEnum Type => AliasTypeEnum.Other;
    public string Value { get; init; }
    public bool Optional { get; init; } = true;
    
    public string Description { get; init; }
    
}