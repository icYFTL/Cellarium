using Cellarium.Commands.Aliases;

namespace Cellarium.Commands.Parameters;

public class BaseParameter : BaseAlias
{
    public override AliasTypeEnum Type => AliasTypeEnum.Other;
    public virtual string? Value { get; init; }
    public virtual bool Optional { get; init; } = true;
    
    public virtual string? Description { get; init; }
    
}