using System;
using System.Threading.Tasks;

namespace PlatformWellSync
{
    class TestDummy
    {
        public static async Task TestDummyData()
        {
            Console.WriteLine("\n=== Testing Dummy Data (Missing/Extra Keys) ===\n");

            var apiService = new ApiService();
            
            Console.WriteLine("Logging in...");
            var loginSuccess = await apiService.LoginAsync();
            
            if (!loginSuccess) return;

            Console.WriteLine("\nFetching dummy data...");
            var data = await apiService.GetPlatformWellDummyAsync();

            if (data != null)
            {
                Console.WriteLine("\nSyncing dummy data...");
                using (var context = new ApplicationDbContext())
                {
                    var syncService = new DataSyncService(context);
                    try
                    {
                        syncService.SyncData(data);
                        Console.WriteLine("✓ Application handled missing/extra keys gracefully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ Failed: {ex.Message}");
                    }
                }
            }
        }
    }
}