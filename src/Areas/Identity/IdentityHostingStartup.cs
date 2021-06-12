using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(PartShop.Areas.Identity.IdentityHostingStartup))]
namespace PartShop.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}