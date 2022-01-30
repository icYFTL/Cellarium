namespace Cellarium.Models;

public class CellariumFile
{
    private readonly string _externalPath;
    public string ExternalPath
    {
        get => _externalPath;
        init => _externalPath = value.Replace("\\", "/");
    }
    public string InternalPath { get; init; }
    public string FileName { get; init; }
    public string FullExternalPath => Path.Combine(ExternalPath, FileName).Replace("\\", "/");
}