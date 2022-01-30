using Cellarium.Commands.Aliases;
using Cellarium.Commands.Parameters;

namespace Cellarium.Commands.Base;

public interface IBaseCommand
{
    public string Description { get; init; }
    public List<BaseAlias> Aliases { get; init; }
    public List<BaseParameter>? Parameters { get; init; }
    public void Run(params BaseParameter [] arguments);
}