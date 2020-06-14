﻿using Bawbee.Application.Interfaces;
using Bawbee.Application.Services;
using Bawbee.Domain.Commands;
using Bawbee.Domain.Core.Bus;
using Bawbee.Domain.Core.Commands;
using Bawbee.Domain.Core.Events;
using Bawbee.Domain.Core.Notifications;
using Bawbee.Domain.Interfaces;
using Bawbee.Domain.Queries.Users.Queries;
using Bawbee.Infra.CrossCutting.Bus;
using Bawbee.Infra.Data.EventSource;
using Bawbee.Infra.Data.RavenDB.EventHandlers;
using Bawbee.Infra.Data.ReadRepositories;
using Bawbee.Infra.Data.WriteRepositories;
using Bawbee.Infra.Data.WriteRepositories.Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bawbee.Infra.CrossCutting.IoC
{
    public class BawbeeInjectorBootstrapper
    {
        public static void RegisterDependencies(IServiceCollection services, IConfiguration configuration)
        {
            RegisterAssembliesForMediatr(services);

            services.RegisterJwt(configuration);

            // Domain
            services.AddScoped<IMediatorHandler, InMemoryBus>();
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // Application
            services.AddScoped<IUserApplication, UserApplication>();

            services.RegisterRavenDB(configuration);

            // Repositories
            services.AddScoped<IUserWriteRepository, UserWriteRepository>();
            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<IDapperConnection, DapperConnection>();

            // EventSource
            services.AddScoped<IEventStore, RavenDBEventStore>();

            services.RegisterSwagger();
        }

        private static void RegisterAssembliesForMediatr(IServiceCollection services)
        {
            // Bawbee.Domain.Core
            services.AddMediatR(typeof(Command).GetTypeInfo().Assembly);

            // Bawbee.Domain
            services.AddMediatR(typeof(BaseCommandHandler).GetTypeInfo().Assembly);

            // Bawbee.Domain.Queries
            services.AddMediatR(typeof(GetAllUsersQuery).GetTypeInfo().Assembly);

            // Bawbee.Infra.Data
            services.AddMediatR(typeof(UserRavenDBHandler).GetTypeInfo().Assembly);
        }
    }
}