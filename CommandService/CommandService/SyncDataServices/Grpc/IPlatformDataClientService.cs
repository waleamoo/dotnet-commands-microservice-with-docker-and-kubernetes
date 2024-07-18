using CommandService.Models;

namespace CommandService.SyncDataServices.Grpc
{
    public interface IPlatformDataClientService
    {
        IEnumerable<Platform> ReturnAllPlatforms();
    }
}
 