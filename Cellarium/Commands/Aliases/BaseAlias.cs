namespace Cellarium.Commands.Aliases;

public class BaseAlias
{
    public virtual AliasTypeEnum Type { get; init; }
    public string Content { get; init; } = null!;
    public string AsArgument => Type == AliasTypeEnum.Abbreviation ? $"-{Content}" : $"--{Content}";
    public string AsParameter => Content;
}