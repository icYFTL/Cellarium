namespace Cellarium.Models;

public class CellariumFile
{
    private readonly string _externalPath = null!;
    public string ExternalPath
    {
        get => _externalPath;
        init => _externalPath = value.Replace("\\", "/");
    }

    public string InternalPath { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string FullExternalPath => Path.Combine(ExternalPath, FileName).Replace("\\", "/");
}