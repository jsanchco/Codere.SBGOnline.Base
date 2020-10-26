namespace Codere.SBGOnline.WebAPI
{
    #region Using

    using Autofac;
    using Autofac.Integration.WebApi;
    using Codere.SBGOnline.Common.CodereId.Configuration;
    using Codere.SBGOnline.Common.Configuration;
    using Codere.SBGOnline.Common.Configuration.Interfaces;
    using Codere.SBGOnline.Common.Cryptography;
    using Codere.SBGOnline.Common.Cryptography.Interfaces;
    using Codere.SBGOnline.Common.DataAccess.Core;
    using Codere.SBGOnline.Common.DataAccess.Dynamics.Repositories;
    using Codere.SBGOnline.Common.DataAccess.Interfaces;
    using Codere.SBGOnline.Common.DataAccess.Interfaces.Dynamics.Repositories;
    using Codere.SBGOnline.Common.Logging.Interfaces;
    using Codere.SBGOnline.Common.Logging.TraceSource;
    using Codere.SBGOnline.Services;
    using Codere.SBGOnline.Services.Interfaces;
    using Microsoft.Xrm.Client;
    using Microsoft.Xrm.Client.Services;
    using Microsoft.Xrm.Sdk;
    using System.Configuration;
    using System.Reflection;
    using System.Web.Http;

    #endregion

    public static class AutofacConfig
    {
        public static void Register(HttpConfiguration config)
        {
            ContainerBuilder builder = new ContainerBuilder();

            // Get your HttpConfiguration.

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            RegisterServices(builder);

            // Set the dependency resolver to be Autofac.
            IContainer container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        public static void RegisterServices(ContainerBuilder builder)
        {
            // Common
            builder.RegisterType<CodereCryptographyService>().As<ICryptographyService>().SingleInstance();
            builder.RegisterType<UniqueKeyProvider>().As<IUniqueKeyProvider>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();

            // Configuration
            //builder.RegisterType<CodereIdLicenseConfigProvider>().As<ILicenseConfigProvider>().InstancePerRequest();
            builder.RegisterType<LicenseConfig>().As<ILicenseConfig>().SingleInstance();

            // Dynamics
            builder
                .Register(x =>
                {
                    var connectionString = ConfigurationManager.AppSettings["CrmConnectionString"];
                    if (ConfigurationManager.AppSettings["CrmConnectionStringIsEncrypted"] == "true")
                    {
                        ICryptographyService cryptographyService = x.Resolve<ICryptographyService>();
                        connectionString = cryptographyService.Decrypt(connectionString);
                    }
                    CrmConnection connection = CrmConnection.Parse(connectionString);
                    return new OrganizationService(connection);
                })
                .As<IOrganizationService>()
                .InstancePerRequest();
            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IDynamicsRepository<>)).InstancePerRequest();

            // Services
            builder.RegisterType<ServiceTest>().As<IServiceTest>().InstancePerRequest();
        }
    }
}