using Objects;
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
        private List<short> listValues;
        
        public void Execute(IJobExecutionContext context)
        {
            DateTime date = DateTime.Today.AddDays(-1);
            using (AppContext db = new AppContext())
            {
                listEnginesId = db.Engines.Select(e=>e.Id).ToList();
                foreach (Int32 enId in listEnginesId)
                {
                    listValues = db.EngineStates.Where(es => es.EngineId==enId && es.Date>=date && es.Date<DateTime.Today).Select(es=>es.Value).ToList();
                    if (listValues.Count != 0)
                    {
                        averageValue = listValues.Average(x => x);
                        db.Statistics.Add(new Statistic(date, (short)averageValue, enId));
                        db.SaveChanges();
                    }
                    
                }
                
            }
        }
    }
}