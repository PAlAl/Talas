using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Talas.Jobs
{
    public class CollectStatisticSheduler
    {
        public static void Start()
        {
            DateTime startTime = DateTime.Today.AddDays(1);
            //startTime.AddHours(startTime.Hour-(startTime.Hour-1));//установка кол-ва часов в 1

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
 
            IJobDetail job = JobBuilder.Create<StatisticCollector>().Build();
 
            ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("stat", "group1")     // идентифицируем триггер с именем и группой
                .StartNow()                            // запуск во время
                .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                    .WithIntervalInHours(24)          // через 24 часа
                    .RepeatForever())                   // бесконечное повторение
                .Build();                               // создаем триггер
 
            scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
        }
    }
}