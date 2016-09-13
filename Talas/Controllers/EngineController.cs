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
        public ActionResult Index(Int32 id)
        {
            if (!IsSetCookies()) return RedirectToAction("Login", "Account");
            Engine engine = null;
            using (AppContext db = new AppContext())
            {
                engine = db.Engines.FirstOrDefault(e => e.Id == id);             
            }
            return PartialView("~/views/Engine/EngineInfo.cshtml", engine);
        }
        [Authorize]
        public ActionResult EngineStates(Int32 id,Byte mode)
        {
            /*НЕТ ЗАЩИТЫ ОТ ЧУЖОГО ИДДВИЖКА*/
            if (!IsSetCookies()) return RedirectToAction("Login", "Account");
            List<DateTime> dates = PrepareDatesEngineState(id, Request.Params["dateStart"], Request.Params["dateFinish"]);
            if (Request.Params["viewType"] == "graph") return PartialView("~/views/Engine/Graph.cshtml", Graph(id));
            ViewBag.Mode = mode;
            switch (mode)
            {
                case 2:
                    ViewBag.ModeName = "Operation Hours";
                    break;
                case 3:
                    ViewBag.ModeName = "Insulation Resistance, kOhm";
                    break;
                case 5:
                    ViewBag.ModeName = "Drying Mode";
                    break;

            }
            return PartialView(GetListEngineState(id, dates));
        }

        [Authorize]
        public DotNet.Highcharts.Highcharts Graph(Int32 id)
        {
            //if (!IsSetCookies()) return RedirectToAction("Login", "Account");
            DateTime date = DateTime.Parse(Request.Params["dateStart"]);
           // DateTime date = Request.Params["calendar"] != "" ? DateTime.Parse(Request.Params["calendar"] + "-01") : new DateTime();
            Dictionary<String, String> data = PrepareDataGraph(date, id);
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
            return chart;
        }

        [Authorize]
        public FileResult Download(String idEngine, String dateReportStart, String dateReportFinish)
        {
            if (!IsSetCookies()) return null;
            FileResult result = null;
            if (idEngine != "" && idEngine != null)
            {
                //Int32 id = Int32.Parse(id);
                List<DateTime> dates = PrepareDatesEngineState(Int32.Parse(idEngine), dateReportStart, dateReportFinish);
                List<EngineState> EngineStates = GetListEngineState(Int32.Parse(idEngine), dates);
                result = PrepareFileDownload(idEngine, EngineStates);
            }
            return result;
        }

        #region Graph
        private Dictionary<String, String> PrepareDataGraph(DateTime date, Int32 idEngine)
        {
            Dictionary<String, String> result;
            using (AppContext db = new AppContext())
            {
                if (date != DateTime.MinValue)
                {
                    Int32 month = date.Month;
                    result = db.Statistics.Where(st => st.EngineId == idEngine && st.Date.Month == month).ToDictionary(st => ConvertDate(st.Date), st => st.Value.ToString());
                }
                else
                {
                    result = db.Statistics.Where(st => st.EngineId == idEngine).OrderByDescending(st => st.Id).Take(NUMBERS_FOR_GRAPHICS).OrderBy(st => st.Id).ToDictionary(st => ConvertDate(st.Date), st => st.Value.ToString());
                }
            }
            return result;
        }

        private String ConvertDate(DateTime date)
        {
            return date.Day.ToString() + "." + date.ToString().Substring(3, 2);
        }

        #endregion

        #region ReportDownload
        
        private FileResult PrepareFileDownload(String idEngine, List<EngineState> engineStates)
        {
            String filePath = null, fileType = null, fileName = null;
            filePath = Server.MapPath("~/Content/Files/" + idEngine + ".txt");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false))
            {
                foreach (EngineState engine in engineStates)
                {
                    file.WriteLine(EngineStateToString(engine));
                }
            }
            fileType = "application/plain";
            fileName = idEngine + "_" + DateTime.Today.Date.ToString() + ".txt";
            return filePath != null ? File(filePath, fileType, fileName) : null;
        }

        private String EngineStateToString(EngineState engineState)
        {
            return engineState.Date.ToString() + " " + engineState.Id.ToString() + " " + engineState.Value.ToString() + " ";
        }

        #endregion

        /*private Boolean EngineUserCheck(Int32 idEngine)
        {
            User user = null;
            Engine engine = null;
            Int32 id = Int32.Parse(HttpContext.Request.Cookies["Talas"].Value);
            using (AppContext db = new AppContext())
            {
                user = db.Users.FirstOrDefault(u => u.Id == id);  
                            
            }
            var a = user.Engines.First(e => e.Id == idEngine);
            return true;
        }*/

        private bool IsSetCookies()
        {
            return (HttpContext.Request.Cookies["Talas"] != null);
        }
          
        private List<DateTime> PrepareDatesEngineState(Int32 idEngine, String dateReportStart, String dateReportFinish)
        {
            List <DateTime> result = new List<DateTime>();       
            DateTime dateLastState;
            using (AppContext db = new AppContext())
            {
                 dateLastState = db.EngineStates.Where(es => es.EngineId == idEngine).Max(es => es.Date).Date;
            }
            DateTime dateFirst = dateReportStart != ""  && dateReportStart != null ? DateTime.Parse(dateReportStart) : dateLastState;
            DateTime dateSecond = dateReportFinish != "" && dateReportFinish != null ? DateTime.Parse(dateReportFinish) : dateLastState.AddDays(1);
            result.Add(dateFirst);
            result.Add(dateSecond);
            return result;
        }

        private List<EngineState> GetListEngineState(Int32 idEngine, List<DateTime> datesStates)
        {
            List<EngineState> listStates = null;
            DateTime dateFisrt = datesStates[0], dateSecond = datesStates[1];
            using (AppContext db = new AppContext())
            {
                listStates = db.EngineStates.Where(es => es.EngineId == idEngine && es.Date >= dateFisrt && es.Date < dateSecond).ToList();
            }
            return listStates;
        }
    }
}