using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data
{
    public static class PrepDb
    {

        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClientService>();
                var platforms = grpcClient.ReturnAllPlatforms();

                SeddData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
            }
        }

        private static void SeddData(ICommandRepo repo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("Seeding new platforms...");
            foreach (Platform platform in platforms)
            {
                if (!repo.ExternalPlatformExist(platform.ExternalId)) 
                    repo.CreatePlatform(platform);
                repo.SaveChanges();
            }
        }
    }
}
