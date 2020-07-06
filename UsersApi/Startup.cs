using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using UsersApi.Config;
using UsersApi.Services;
using UsersApi.Storage;

[assembly: FunctionsStartup(typeof(UsersApi.Startup))]

namespace UsersApi
{
    public class Startup : FunctionsStartup
    {
        private readonly IConfigurationSettings _configurationSettings;

        public Startup()
        {
            _configurationSettings = new ConfigurationSettings();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IRepository, Repository>(provider =>
                new Repository(_configurationSettings.AzureWebJobsStorage));

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
        }
    }
}
