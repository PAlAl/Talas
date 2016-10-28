using System;
using System.ComponentModel.DataAnnotations;

namespace Talas.Models
{
    public class EventModel
    {

        public String MotorName { get; set; }

        public DateTime Date { get; set; }

        public String Message { get; set; }
        public Boolean IsNew { get; set; }
        public EventModel(String name, DateTime date, String text,Boolean isNew)
        {
            MotorName = name;
            Date = date;
            Message = text;
            IsNew = isNew;
        }

        public String DateString
        {
            get
            {
                return Date.ToString("dd.MM.yyyy HH:mm:ss");
            }
        }

    }
}