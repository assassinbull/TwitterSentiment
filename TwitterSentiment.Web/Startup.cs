using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TwitterSentiment.Web.Startup))]
namespace TwitterSentiment.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
