using System;
using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Feedler.Tests.Extensions
{
    /// <summary>
    /// Host class that wraps corresponding <typeparamref name="TStartup"/>
    /// and allows to remove/add/replace services before/after <c>Configure</c>/<c>ConfigureServices</c> methods. <para/>
    /// Also provides <see cref="Client"/> for executing queries against emulated server
    /// for testing whole request pipeline instead of controller methods only.
    /// </summary>
    /// <typeparam name="TStartup">Type of Startup class for project.</typeparam>
    public class TestHost<TStartup> where TStartup: class
    {
        private IServiceCollection _services = new ServiceCollection();
        private IServiceProvider _provider;

        /// <summary>
        /// This class is used to wrap corresponding <see cref="TStartup"/> class and call <see cref="TestHost{TStartup}"/>
        /// <c>Before*</c> and <c>After*</c> methods when appropriate.
        /// </summary>
        private sealed class TestHostStartupWrapper: IStartup
        {
            private readonly TestHost<TStartup> _testHost;
            private readonly StartupMethods _startupMethods;

            public TestHostStartupWrapper(TestHost<TStartup> testHost, StartupMethods startupMethods)
            {
                _testHost = testHost;
                _startupMethods = startupMethods;
            }

            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                _testHost._services = services;

                _testHost.BeforeConfigureServices(services);
                _startupMethods.ConfigureServicesDelegate(services);
                _testHost.AfterConfigureServices(services);

                return _testHost._provider = services.BuildServiceProvider();
            }

            public void Configure(IApplicationBuilder app)
            {
                _testHost.BeforeConfigure(app);
                _startupMethods.ConfigureDelegate(app);
                _testHost.AfterConfigure(app);
            }
        }

        protected IServiceProvider Provider => _provider ?? (_provider = _services.BuildServiceProvider());

        /// <summary>
        /// Tries to get service from host's registered service collection.
        /// Returns <c>null</c> if service is not registered.
        /// </summary>
        protected T GetService<T>() => Provider.GetService<T>();

        /// <summary>
        /// Gets service from host's registered service collection.
        /// Throws <see cref="InvalidOperationException"/> if service is not registered.
        /// </summary>
        protected T GetRequiredService<T>() => Provider.GetRequiredService<T>();

        public TestServer Server { get; private set; }

        public HttpClient Client { get; private set; }

        /// <summary>
        /// This method is called before <see cref="TStartup"/>.ConfigureServices.
        /// </summary>
        protected virtual void BeforeConfigureServices(IServiceCollection services) { }

        /// <summary>
        /// This method is called after <see cref="TStartup"/>.ConfigureServices.
        /// </summary>
        protected virtual void AfterConfigureServices(IServiceCollection services) { }

        /// <summary>
        /// This method is called before <see cref="TStartup"/>.Configure.
        /// </summary>
        protected virtual void BeforeConfigure(IApplicationBuilder app) { }

        /// <summary>
        /// This method is called after <see cref="TStartup"/>.Configure.
        /// </summary>
        protected virtual void AfterConfigure(IApplicationBuilder app) { }

        private void InitEnvironment()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Local");
        }

        private void InitServerAndClient()
        {
            Server = new TestServer(WebHost.CreateDefaultBuilder()
                .ConfigureServices((context, services) => services.AddSingleton((IConfigurationRoot)context.Configuration))
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IStartup>(sp =>
                    {
                        var startupMethods = StartupLoader.LoadMethods(sp, typeof(TStartup),
                            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
                        return new TestHostStartupWrapper(this, startupMethods);
                    });
                })
                // Todo: remove once fixed (see https://github.com/aspnet/Hosting/issues/1137)
                .UseSetting(WebHostDefaults.ApplicationKey, typeof(TStartup).Assembly.FullName)
            );

            Client = Server.CreateClient();
        }

        /// <summary>
        /// This method should be called once before all tests.
        /// </summary>
        /// <remarks><see cref="Init"/> method is used instead of constructor to support different testing frameworks.</remarks>
        public virtual void Init()
        {
            InitEnvironment();
            InitServerAndClient();
        }
    }
}