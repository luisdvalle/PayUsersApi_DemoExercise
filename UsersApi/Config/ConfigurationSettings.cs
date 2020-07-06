using System;

namespace UsersApi.Config
{
    public class ConfigurationSettings : IConfigurationSettings
    {
        public string AzureWebJobsStorage => Environment.GetEnvironmentVariable("AzureWebJobsStorage");
    }
}
