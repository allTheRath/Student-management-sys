using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Student_management_.Startup))]
namespace Student_management_
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
