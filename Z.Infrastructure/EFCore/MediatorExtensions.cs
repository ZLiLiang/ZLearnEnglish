﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Z.DomainCommons.Models;

namespace Z.Infrastructure.EFCore
{
    public static class MediatorExtensions
    {
        /// <summary>
        /// 把rootAssembly及直接或者间接引用的程序集（排除系统程序集）中的MediatR 相关类进行注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rootAssembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddMediatR(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            return services.AddMediatR(assemblies.ToArray());
        }

        /// <summary>
        /// 异步分发域事件
        /// </summary>
        /// <param name="mediator">中介者</param>
        /// <param name="context">数据库上下文</param>
        /// <returns></returns>
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext context)
        {
            var domainEntities = context
                .ChangeTracker
                .Entries<IDomainEvents>()
                .Where(x => x.Entity.GetDomainEvents().Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.GetDomainEvents())
                .ToList();//加ToList()是为立即加载，否则会延迟执行，到foreach的时候已经被ClearDomainEvents()了

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}
