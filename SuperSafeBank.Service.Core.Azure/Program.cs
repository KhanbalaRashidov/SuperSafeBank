using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperSafeBank.Domain.Services;
using SuperSafeBank.Persistence.Azure;
using SuperSafeBank.Service.Core.Azure.Common.Persistence;
using SuperSafeBank.Service.Core.Azure.QueryHandlers;
using SuperSafeBank.Service.Core.Azure.Services;
using SuperSafeBank.Service.Core.Common;

var builder = new HostBuilder();
await builder.ConfigureFunctionsWorkerDefaults()
        .ConfigureServices((ctx, services) =>
        {
            var eventsRepositoryConfig = new EventsRepositoryConfig(ctx.Configuration["EventsStorage"], ctx.Configuration["EventTablesPrefix"]);

            services.AddScoped<ServiceFactory>(ctx => ctx.GetRequiredService)
                .AddScoped<IMediator, Mediator>()
                .Scan(scan =>
                {
                    scan.FromAssembliesOf(typeof(CustomerByIdHandler))
                        .RegisterHandlers(typeof(IRequestHandler<>))
                        .RegisterHandlers(typeof(IRequestHandler<,>))
                        .RegisterHandlers(typeof(INotificationHandler<>));
                })
                .AddTransient<ICustomerEmailsService, CustomerEmailsService>()
                .AddSingleton<IViewsContext>(provider =>
                {
                    var connStr = ctx.Configuration["QueryModelsStorage"];
                    var tablesPrefix = ctx.Configuration["QueryModelsTablePrefix"];
                    return new ViewsContext(connStr, tablesPrefix);
                }).AddAzurePersistence(eventsRepositoryConfig);
        })
        .Build()
        .RunAsync();