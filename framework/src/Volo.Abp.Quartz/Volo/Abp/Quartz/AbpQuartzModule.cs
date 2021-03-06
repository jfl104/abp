﻿using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Volo.Abp.Quartz
{
    public class AbpQuartzModule : AbpModule
    {
        private IScheduler _scheduler;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var options = context.Services.ExecutePreConfiguredActions<AbpQuartzPreOptions>();
            context.Services.AddSingleton(AsyncHelper.RunSync(() => new StdSchedulerFactory(options.Properties).GetScheduler()));
            context.Services.AddSingleton(typeof(IJobFactory), typeof(AbpQuartzJobFactory));
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            _scheduler = context.ServiceProvider.GetService<IScheduler>();
            _scheduler.JobFactory = context.ServiceProvider.GetService<IJobFactory>();
            AsyncHelper.RunSync(() => _scheduler.Start());
        }

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            //TODO: ABP may provide two methods for application shutdown: OnPreApplicationShutdown & OnApplicationShutdown
            AsyncHelper.RunSync(() => _scheduler.Shutdown());
        }
    }
}
