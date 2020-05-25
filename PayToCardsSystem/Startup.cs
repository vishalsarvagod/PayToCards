using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PayToCardsSystem.Startup))]
namespace PayToCardsSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
