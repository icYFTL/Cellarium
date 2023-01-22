namespace Cellarium.Models;

public class CellariumFile
{
    private readonly string _externalDir = null!;
    public string ExternalDir
    {
        get => _externalDir;
        init => _externalDir = value.Replace("\\", "/");
    }

    public string InternalPath { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string FullExternalPath => Path.Combine(ExternalDir, FileName).Replace("\\", "/");
}