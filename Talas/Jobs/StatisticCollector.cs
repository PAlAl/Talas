﻿using Objects;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using Talas.Models;

namespace Talas.Jobs
{
    public class StatisticCollector : IJob
    {
        private List<Int32> listEnginesId;
        private Double averageValue;
        private List<Int32> listValues;
        
        public void Execute(IJobExecutionContext context)
        {
            DateTime date = DateTime.Today.AddDays(-1);
            using (AppContext db = new AppContext())
            {
                listEnginesId = db.Engines.Where(e=>!e.IsDelete).Select(e=>e.Id).ToList();
                foreach (Int32 enId in listEnginesId)
                {
                    if (!db.Statistics.Any(x => x.EngineId == enId && x.Date == date))
                    {
                        listValues = db.EngineStates.Where(es => es.EngineId == enId && es.Date >= date && es.Date < DateTime.Today).Select(es => es.Value).ToList();
                        if (listValues.Count != 0)
                        {
                            averageValue = listValues.Average(x => x);
                            db.Statistics.Add(new Statistic(date, (Int32)averageValue, enId));
                            db.SaveChanges();
                        }
                    }                   
                }
                
            }
        }
    }
}