using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class EngineState
    {
        [Key]
        public Int32 Id { get; set; }
        public Int16 Value { get; set; }
        public DateTime Date { get; set; }
        public Boolean? Status { get; set; }
        public Boolean? Status_M { get; set; }
        public Int16? Quality { get; set; }
        public Boolean? Work { get; set; }
        public Boolean? NoStart { get; set; }
        public Boolean? LowR { get; set; }
        public Boolean? MainCont { get; set; }
        public Boolean? SecondMeas { get; set; }
        public Boolean? Test { get; set; }
        public Int16? Ravg { get; set; }
        public Int16? R30 { get; set; }
        public Int16? R60 { get; set; }
        public Int32 EngineId { get; set; }
        public Engine Engine { get; set; }

        public String DateString
        {
            get
            {
                return Date.ToString("dd.MM.yyyy HH:mm:ss");
            }
        }

        public Double LeakageCurrent
        {
            get
            {
                Double result=0;
                if (R60.HasValue && R30.HasValue && Ravg.HasValue)
                    switch (R30)
                    {
                        case 11:
                            result = (Double)R60.Value / 10 + (Double)Ravg.Value / 1000;
                            break;
                        case 10:
                            result = ((Double)R60.Value / 10 + (Double)Ravg.Value / 1000) / 1000;
                            break;
                        case 2:
                            result = ((Double)R60.Value + ((Double)Ravg.Value / 100)) / 1000;
                            break;
                        case 4:
                            result = ((Double)R60.Value * 10 + (Double)Ravg.Value / 10) / 1000;
                            break;
                    }

                return result;
            }
        }

        public ICollection<Event> Events { get; set; }
    }
}