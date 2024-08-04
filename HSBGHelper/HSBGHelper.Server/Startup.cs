namespace HSBGHelper.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLettuceEncrypt();
        }
    }
}
