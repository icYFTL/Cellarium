namespace Cellarium.Commands.Aliases;

public class BaseAlias
{
    public virtual AliasTypeEnum Type { get; init; }
    public string Content { get; init; }
    public string AsArgument => Type == AliasTypeEnum.Abbreviation ? $"-{Content}" : $"--{Content}";
}