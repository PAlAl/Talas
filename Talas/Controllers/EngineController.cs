using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using Objects;
using Talas.Models;


namespace Talas.Controllers
{
    public class EngineController : Controller
    {
        private const byte NUMBERS_FOR_GRAPHICS=10;
       [Authorize]
        public ActionResult Index(int Id)
        {
            if (!IsSetCookies()) return RedirectToAction("Login", "Account");
            Engine engine = null;
            using (AppContext db = new AppContext())
            {
                engine = db.Engines.FirstOrDefault(e => e.Id == Id);
            }
            return View(engine);
        }
        [Authorize]
        public ActionResult EngineStates()
        {
            if (!IsSetCookies()) return RedirectToAction("Login", "Account");
            int id = int.Parse(Request.Params["idEngine"]);
            List<DateTime> dates = PrepareDatesEngineState(id, Request.Params["dateStart"], Request.Params["dateFinish"]);
            return PartialView(GetListEngineState(id, dates));
        }
        [Authorize]
        public ActionResult Graph()
        {
            int id = int.Parse(Request.Params["idEngine"]);
            DateTime date = Request.Params["calendar"] != "" ? DateTime.Parse(Request.Params["calendar"] + "-01") : new DateTime();
            Dictionary<string, string> data = PrepareDataGraph(date, id);
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart")
            .SetTitle(new Title
            {
                Text = "Data"
            })
            .SetXAxis(new XAxis
            {
                Categories = data.Keys.ToArray()
            })
            .SetYAxis(new YAxis
            {
                Title = new YAxisTitle() { Text = "Insulation Resistance,kOhm" },
                Min = 0,
                GridLineWidth = 0,
                AlternateGridColor = null,
                PlotBands = new[] { new YAxisPlotBands { From=0,To=500, Color = Color.Red},
                                    new YAxisPlotBands { From = 500, To = 1100, Color=Color.Orange},
                                    new YAxisPlotBands { From = 1100, To = 10000, Color= Color.Green}}
            })
            .SetSeries(new Series
            {
                Name = "Insulation Resistance",
                Data = new Data(Array.ConvertAll(data.Values.ToArray(), element => (object)element))
            })
            .SetTooltip(new Tooltip
            {
                //Formatter =  "<b>{point.y:,.0f}%</b>"

                /*formatter: function() {
                                    return '<b>'+this.series.name +'</b><br/>'+
                                    this.x+': '+ this.y +' kOhm';
                }*/
            });
            return View(chart);
        }



        [Authorize]
        public FileResult Download(string Id, string DateStart, string DateFinish)
        {
            if (!IsSetCookies()) return null;
            FileResult result = null;
            if (Id != "" && Id != null)
            {
                int id = int.Parse(Id);
                List<DateTime> dates = PrepareDatesEngineState(id, DateStart, DateFinish);
                List<EngineState> EngineStates = GetListEngineState(id, dates);
                result = PrepareFileDownload(Id, EngineStates);
            }
            return result;
        }

        private Dictionary<string, string> PrepareDataGraph(DateTime Date, int IdEngine)
        {          
            Dictionary<string, string > result;
            using (AppContext db = new AppContext())
            {
                if (Date != DateTime.MinValue)
                {
                    int month = Date.Month;
                    result = db.Statistics.Where(st => st.EngineId == IdEngine && st.Date.Month == month).ToDictionary(st => ConvertDate(st.Date), st => st.Value.ToString());
                }
                else
                {
                    result = db.Statistics.Where(st => st.EngineId == IdEngine).OrderByDescending(st=>st.Id).Take(NUMBERS_FOR_GRAPHICS).OrderBy(st => st.Id).ToDictionary(st => ConvertDate(st.Date), st => st.Value.ToString());
                } 
            }
            return result;
        }

        private string ConvertDate(DateTime Date)
        {
            return Date.Day.ToString() + "." + Date.ToString().Substring(3,2);
        }

        private bool IsSetCookies()
        {
            return (HttpContext.Request.Cookies["Talas"] != null);
        }
        private FileResult PrepareFileDownload(string Id ,List<EngineState> EngineStates)
        {
            string file_path = null, file_type = null, file_name = null;
            file_path = Server.MapPath("~/Content/Files/" + Id + ".txt");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(file_path, false))
            {
                foreach (EngineState en in EngineStates)
                {
                    file.WriteLine(EngineStateToString(en));
                }
            }
            file_type = "application/plain";
            file_name = Id + "_" + DateTime.Today.Date.ToString() + ".txt";
            return file_path != null ? File(file_path, file_type, file_name) : null;
        }

        private string EngineStateToString(EngineState EnState)
        {
            return EnState.Date.ToString() + " " + EnState.Id.ToString() + " " + EnState.Value.ToString() + " ";
        }

        private List<DateTime> PrepareDatesEngineState(int Id, string DateStart, string DateFinish)
        {
            List <DateTime> result = new List<DateTime>();       
            DateTime dateLast;
            using (AppContext db = new AppContext())
            {
                 dateLast = db.EngineStates.Where(es => es.EngineId == Id).Max(es => es.Date).Date;
            }
            DateTime dateFirst = DateStart != ""  && DateStart != null ? DateTime.Parse(DateStart) : dateLast;
            DateTime dateSecond = DateFinish != "" && DateFinish != null ? DateTime.Parse(DateFinish) : dateLast.AddDays(1);
            result.Add(dateFirst);
            result.Add(dateSecond);
            return result;
        }

        private List<EngineState> GetListEngineState(int Id, List<DateTime> Dates)
        {
            List<EngineState> list = null;
            DateTime dateFisrt = Dates[0], dateSecond = Dates[1];
            using (AppContext db = new AppContext())
            {
                list = db.EngineStates.Where(es => es.EngineId == Id && es.Date >= dateFisrt && es.Date < dateSecond).ToList();
            }
            return list;
        }
    }
}