namespace Nexile.Desktop.Core.Internal;

public interface INexileInformationService
{
    string NexileVersion { get; }
    string NexilePlatform { get; }
}