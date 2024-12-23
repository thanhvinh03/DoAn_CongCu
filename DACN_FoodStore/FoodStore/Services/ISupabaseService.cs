using Supabase;

namespace FoodStore.Services
{
    public interface ISupabaseService
    {
        Client GetClient();
    }

    public class SupabaseService : ISupabaseService
    {
        private readonly Client client;
        public SupabaseService(IConfiguration configuration)
        {
            var url = configuration["Supabase:URL"];
            var key = configuration["Supabase:Key"];

            client = new Client(url, key);
            client.InitializeAsync().Wait();
        }
        public Client GetClient()
        {
            return client;
        }
    }
}
