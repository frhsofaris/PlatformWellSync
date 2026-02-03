using System;
using System.Threading.Tasks;

namespace PlatformWellSync
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Platform Well Data Sync ===\n");

            using (var context = new ApplicationDbContext())
            {
                Console.WriteLine("Creating database...");
                context.Database.EnsureCreated();
                Console.WriteLine("✓ Database ready\n");
            }

            var apiService = new ApiService();
            
            Console.WriteLine("Logging in...");
            var loginSuccess = await apiService.LoginAsync();
            
            if (!loginSuccess)
            {
                Console.WriteLine("Failed to login. Exiting...");
                return;
            }

            Console.WriteLine("\nFetching platform and well data...");
            var data = await apiService.GetPlatformWellActualAsync();

            if (data != null && data.Count > 0)
            {
                Console.WriteLine("\n=== DEBUG: First Platform JSON Structure ===");
                Console.WriteLine(data[0].ToString());
                Console.WriteLine("============================================\n");

                Console.WriteLine("Syncing data to database...");
                using (var context = new ApplicationDbContext())
                {
                    var syncService = new DataSyncService(context);
                    syncService.SyncData(data);
                }
                
                Console.WriteLine("\n✓ Sync completed successfully!");
            }
            else
            {
                Console.WriteLine("\n✗ No data to sync");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}