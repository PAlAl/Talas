using Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Talas.Objects
{
    public class EventComparer : IComparer<Event>
    {
        public int Compare(Event x, Event y)
        {
            int result=0;
            if (x.IsNew ^ y.IsNew)
            {
                result = x.IsNew ? -1 : 1;
            }
            else
            {
                result = x.Date > y.Date ? -1 : 1;
            }
            return result;
        }
    }
}