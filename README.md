# Creación e instalación de un Proyecto Base (API) para Codere.SBGOnline

 - Crear **solución en blanco** en Visual Studio (**VS**)
 - Agregar un nuevo proyecto **API .net framework c# 4.5.2**
 - **Limpiar** el proyecto de carpetas y archivos innecesarios
 - En las propiedades del proyecto->web->Servidores->**iislocal** (con el nombre que elijamos)
 - Instalamos el paquete nuget (Swashbuckle) [**SWAGGER**]
 - En las propiedades del proyecto->Página especifica->**swagger**
 - Instalamos mediante paquetes nuget **Autofac (v. 4.9.4)** y **Autofac.WebApi2 (v. 4.3.1)**
 - Añadimos dentro de la carpeta App_Start la clase static: **AutofacConfig**
 - Notaremos que nos faltan muchar referencias
 - Modificaremos el Web.config añadiendo ***[cadena de conexión al crm de Dynamcs]***
 ```sh
		  <appSettings>
					<add key="CrmConnectionStringIsEncrypted" value="true" />
					<add key="CrmConnectionString" value="y2QjVvD/V8keS9dnOQlqVJmx9GOM8zI0ah1A400kS4Z7pNYSU78LyeC2uRRKJ6cNR6qkicdqJhp/+Gwu82a8KhgcWqtlThw7/LLcH6HOPpw=" />
		  </appSettings>
 ```
  - Modificaremos el Web.config añadiendo ***[para el traceo]***
 ```sh
		  <system.diagnostics>
					<trace autoflush="true" indentsize="3" />
					<sources>
					  <source name="cSBG" switchName="sourceSwitch" switchType="System.Diagnostics.SourceSwitch">
						<listeners>
						  <add name="console" type="System.Diagnostics.ConsoleTraceListener">
							<filter type="System.Diagnostics.EventTypeFilter" initializeData="Error" />
						  </add>
						  <add name="ExceptionListener" />
						  <remove name="Default" />
						</listeners>
					  </source>
					  <source name="cSBGTrace" switchName="sourceSwitch" switchType="System.Diagnostics.SourceSwitch">
						<listeners>
						  <add name="cSBGTraceListener" />
						  <remove name="Default" />
						</listeners>
					  </source>
					</sources>
					<switches>
					  <add name="sourceSwitch" value="Verbose" />
					</switches>
					<sharedListeners>
					  <add name="ExceptionListener" type="Codere.SBGOnline.Common.CodereTraceListener, Codere.SBGOnline.Common" initializeData="C:\cSbg_Logs\userServices_exceptionLog" traceOutputOptions="Callstack">
						<filter type="System.Diagnostics.EventTypeFilter" initializeData="Error" />
					  </add>
					  <add name="cSBGTraceListener" type="Codere.SBGOnline.Common.CodereTraceListener, Codere.SBGOnline.Common" initializeData="C:\cSbg_Logs\userServices_traceLog" traceOutputOptions="ProcessId, DateTime,ThreadId">
						<filter type="System.Diagnostics.EventTypeFilter" initializeData="Verbose" />
					  </add>
					</sharedListeners>
		  </system.diagnostics>
 ```
 - Agregamos en ***AutofacConfig***:
 ```sh
            builder.RegisterType<CodereCryptographyService>().As<ICryptographyService>().SingleInstance();

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
 ```
 - Modificamos el método ***Application_start*** del ***Global.asax*** para que quede de esta manera
 ```sh
         protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(AutofacConfig.Register);
        }
 ```
 
# Instalación de paquetes nuget's necesarios para conectar con el CRM de Dynamics (Base de Datos)
 
 - Instalaremos en ***API*** estos paquetes nugets:
	- **Codere.SBGOnline.Common**
	- **Codere.SBGOnline.Common.Cryptography**
	- **Codere.SBGOnline.Common.DataAccess.Dynamics**
	- **Codere.SBGOnline.DynamicsClasses.CodereId**
	- **Codere.SBGOnline.Common.DataAccess.Core**
- Instalaremos en ***Services*** estos paquetes nugets para tener acceso desde ellos al CRM:
	- **Codere.SBGOnline.Common.DataAccess.Dynamics**
	- **Codere.SBGOnline.DynamicsClasses.CodereId**
	
# Acciones requeridas para conectar con la Sesion de Usuario (Cookie) de .CodereApuetas
- Modificaremos el Web.config añadiendo: 
 ```sh
		<system.web>
					<compilation debug="true" targetFramework="4.5.2" />
					<httpRuntime targetFramework="4.5.2" />
					
					<authentication mode="Forms">
					  <forms name=".CodereApuestas" protection="All" path="/" enableCrossAppRedirects="true" loginUrl="~/Account/Login" timeout="20" />
					</authentication>
					<machineKey validationKey="55D09B86DE964E89C69A12EDAF0B021E698FE8CBC345FB6B5BD0FA1CA4180D12F44F8979EB0E733F5EFCEA3D4D0480555A095AFF6761FFDAC871869E3DECA094" decryptionKey="B60C3188BEE5F493959A62C9884E9EEF61E711BA21CECF0DCBC00A2CAC6FA934" validation="SHA1" decryption="AES" />
					
		</system.web>
```
- Añadiremos el fichero: **FilterConfig.cs** a la carpeta ***App_Start***
- Instalaremos en ***API*** estos paquetes nugets:
	- **Codere.SBGOnline.Common.WebApi**
	- **Codere.SBGOnline.Common.Configuration**
- Limpiamos el fichero ***WebApiConfig.cs*** (App_Start) y lo dejamos de esta manera:
```sh
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
        }
    }
```
- Modificamos en ***AutofacConfig***, añadiendo:
 ```sh
             // Common
            builder.RegisterType<CodereCryptographyService>().As<ICryptographyService>().SingleInstance();
            builder.RegisterType<UniqueKeyProvider>().As<IUniqueKeyProvider>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();

            // Configuration
            builder.RegisterType<CodereIdLicenseConfigProvider>().As<ILicenseConfigProvider>().InstancePerRequest();
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
```
- Instalaremos en ***API*** estos paquetes nugets: 
	- **Codere.SBGOnline.Common.WebApi**
	- **Codere.SBGOnline.Common.Configuration**
	- **Codere.SBGOnline.Common.DataAccess.Interfaces**
	- **Codere.SBGOnline.Common.DataAccess.Core**
	- **Codere.SBGOnline.Common.Logging.TraceSource**
	- **Codere.SBGOnline.Common.CodereId.Configuration**
- Dejar los ***Using*** de ***AutofacConfig.cs*** de la siguiente manera:

 ```sh
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
 ```
 - Modificar el ***Global.asax***:
```sh
	protected void Application_Start()
	{
			GlobalConfiguration.Configure(CodereWebApiConfig.Register);
			GlobalConfiguration.Configure(WebApiConfig.Register);
			GlobalConfiguration.Configure(FilterConfig.RegisterGlobalFilters);
			GlobalConfiguration.Configure(AutofacConfig.Register);
	}
```

# Prueba de un Controller

Construimos un ***Controller*** con un Get de la siguiente forma para probar que accedemos a la Sesión de .CodereApuestas (Cookie):
```sh
     public IHttpActionResult GetContacts()
        {
            try
            {
                var principal = RequestContext.Principal as CoderePrincipal;
				...
```	

# Posibles problemas detectados

- Error en ***Codere.SBGOnline.Base\Codere.SBGOnline.WebAPI\bin\roslyn\csc.exe*** -> Este problema se arregla limpiando la solución y volviendo a ejecutarla
- Error en ***FileWatcher*** -> Deshabilitar este error en las excepciones
