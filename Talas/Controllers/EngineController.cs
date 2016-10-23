using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using Objects;
using Talas.Models;
using Talas.Objects;

namespace Talas.Controllers
{
    public class EngineController : Controller
    {
        private const byte NUMBERS_FOR_GRAPHICS=15;
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
        public ActionResult EngineStates(Int32 id, Byte mode)
        {
            /*НЕТ ЗАЩИТЫ ОТ ЧУЖОГО ИДДВИЖКА*/
            Int32 idEngine = id;
            if (!IsSetCookies()) return RedirectToAction("Login", "Account");
            if (Request.Params["viewType"] == "graph") return PartialView("~/views/Engine/Graph.cshtml", Graph(idEngine));
            List<DateTime> dates = PrepareDatesEngineState(idEngine, Request.Params["dateStart"], Request.Params["dateFinish"]);          
            ViewBag.Mode = mode;
            switch (mode)
            {
                case 1:
                    ViewBag.All = false;
                    return PartialView("~/views/Engine/Event.cshtml", GetListEvent(idEngine));
                case 2:
                    ViewBag.ModeName = "Operation Hours";
                    List<String> workTimes = GetWorkTimes(idEngine);
                    ViewBag.WorkTimes = workTimes[0];
                    ViewBag.NotWorkTimes = workTimes[1];
                    break;
                case 3:
                    ViewBag.ModeName = "Insulation Resistance, kOhm";
                    break;
                case 4:
                    ViewBag.ModeName = "Polarization Index";
                    break;
                case 5:
                    ViewBag.ModeName = "Drying Mode";
                    break;
                case 6:
                    ViewBag.ModeName = "Online";
                    break;

            }
            return PartialView(GetListEngineState(idEngine, dates));
        }

        [Authorize]
        public ActionResult Messages(Byte showAll)
        {
            ActionResult result;
            switch (showAll)
            {
                case 1:
                    ViewBag.All = true;
                    result = View("~/views/Engine/EventAll.cshtml", GetListEvent(-1));
                    break;
                case 2:
                    ViewBag.All = true;
                    result = PartialView("~/views/Engine/Event.cshtml", GetListEvent(-1));
                    break;
                default:
                    ViewBag.All = true;
                    result = View("~/views/Engine/Event.cshtml", GetListEvent(-1));
                    break;

            }
            return result;
        }

        private DotNet.Highcharts.Highcharts Graph(Int32 idEngine)
        {
            DateTime dateStart = Request.Params["dateStart"] != "" && Request.Params["dateStart"] != null ? DateTime.Parse(Request.Params["dateStart"]) : new DateTime();
            DateTime dateFinish = Request.Params["dateFinish"] != "" && Request.Params["dateFinish"] != null ? DateTime.Parse(Request.Params["dateFinish"]) : new DateTime();
            if (dateStart > dateFinish) return null;
            Dictionary<String, String> data = PrepareDataGraph(dateStart, dateFinish, idEngine);
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart")
            .InitChart(new Chart { BackgroundColor = new BackColorOrGradient(System.Drawing.Color.Transparent) })
            .SetTitle(new Title
            {
                Text = ""
            })
            .SetXAxis(new XAxis
            {
                Categories = data.Keys.ToArray(),
                LineColor = Color.Black,
                TickColor = Color.Black,
                GridLineColor = Color.Black,
                Labels = new XAxisLabels { Style = "color: '#000000'" },

            })
            .SetYAxis(new YAxis
            {
                Title = new YAxisTitle() { Text = "Insulation Resistance,kOhm" , Style = "color: '#000000'" },
                Min = 0,
                GridLineWidth = 0,
                AlternateGridColor = null,
                LineColor = Color.Black,
                PlotBands = new[] { new YAxisPlotBands { From=0,To=500, Color = Color.FromArgb(100,Color.Red)},
                                    new YAxisPlotBands { From = 500, To = 1100, Color=Color.FromArgb(100,Color.Orange)},
                                    new YAxisPlotBands { From = 1100, To = 10000, Color= Color.FromArgb(100,Color.Green)}},
                GridLineColor = Color.Black,
                TickColor = Color.Black,
                Labels = new YAxisLabels{ Style= "color: '#000000'" }
        })
            .SetSeries(new Series
            {
                PlotOptionsBar = new PlotOptionsBar { ShowInLegend=false},
                Name = "Insulation Resistance",
                Data = new Data(Array.ConvertAll(data.Values.ToArray(), element => (object)element)),
                Color = Color.FromArgb(209,209,209),
                PlotOptionsSeries = new PlotOptionsSeries
                {
                    Marker = new PlotOptionsSeriesMarker
                    {
                        FillColor = Color.Gray,
                        LineWidth = 2,
                        LineColor = Color.White
                    }
                },             
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
        public FileResult Download()
        {
            if (!IsSetCookies()) return null;
            String idEngine = Request.Params["id"];
            String dateReportStart = Request.Params["dateStart"];
            String dateReportFinish = Request.Params["dateFinish"];
            FileResult result = null;
            if (idEngine != "" && idEngine != null)
            {
                List<DateTime> dates = PrepareDatesEngineState(Int32.Parse(idEngine), dateReportStart, dateReportFinish);
                List<EngineState> EngineStates = GetListEngineState(Int32.Parse(idEngine), dates);
                result = PrepareFileDownload(idEngine, EngineStates);
            }
            return result;
        }

        #region Graph
        private Dictionary<String, String> PrepareDataGraph(DateTime dateStart, DateTime dateFinish, Int32 idEngine)
        {
            Dictionary<String, String> result;
            using (AppContext db = new AppContext())
            {
                if (dateStart == DateTime.MinValue || dateFinish == DateTime.MinValue)
                    result = db.Statistics.Where(st => st.EngineId == idEngine).OrderByDescending(st => st.Id).Take(NUMBERS_FOR_GRAPHICS).OrderBy(st => st.Id).ToDictionary(st => ConvertDate(st.Date), st => st.Value.ToString());
                else
                    result = db.Statistics.Where(st => st.EngineId == idEngine && st.Date <= dateFinish && st.Date >= dateStart).ToDictionary(st => ConvertDate(st.Date), st => st.Value.ToString());                             
            }
            if (result.Count > NUMBERS_FOR_GRAPHICS)
            {
                int i = 0, d = 0;
                Dictionary<String, String> localDictionary = new Dictionary<string, string>();
                d = result.Count / NUMBERS_FOR_GRAPHICS;
                while (localDictionary.Count < NUMBERS_FOR_GRAPHICS)
                {
                    localDictionary.Add(result.ElementAt(i).Key, result.ElementAt(i).Value);
                    i += d;
                }
                result.Clear();
                result = localDictionary;
            }

            return result;
        }
        private Dictionary<String, String> PrepareDataGraphMonth(DateTime date, Int32 idEngine)
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
            string day = date.Day>9? date.Day.ToString(): "0"+date.Day.ToString();
            string month = date.Month > 9 ? date.Month.ToString() : "0" + date.Month.ToString();
            return day + "." + month;
        }

        #endregion

        #region ReportDownload
        
        private FileResult PrepareFileDownload(String idEngine, List<EngineState> engineStates)
        {
            String filePath = null, fileType = null, fileName = null;
            filePath = Server.MapPath("~/Content/Files/" + idEngine + ".txt");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false))
            {
                file.WriteLine(
                "R – Instant value of insolation Resistance\r\n" +
                "R30 - insolation Resistance after 30 sec\r\n" +
                "R60 - insolation Resistance after 60 sec\r\n" +
                "DAR - Dielectric Absorption Ratio (R60 / R30)\r\n" +
                "Ravg – Average insolation Resistance for 60 sec\r\n" +
                "Talas Dryer – connected / disconnected\r\n" +
                "Motor – Ready / not ready to start\r\n" +
                "Drying – on / off\r\n" +
                "Motor – on / off\r\n" +
                "Motor Start - ok / failure") ;
 
                 file.WriteLine("        Date       |  R  | R30 | R60 | DAR |Ravg|Talas Dryer|Motor|Drying|Motor|Motor Start");
                foreach (EngineState engineState in engineStates)
                {
                    file.WriteLine(EngineStateToString(engineState));
                }
            }
            fileType = "application/plain";
            fileName = idEngine + "_" + DateTime.Today.Date.ToString() + ".txt";
            return filePath != null ? File(filePath, fileType, fileName) : null;
        }

        private String EngineStateToString(EngineState engineState)
        {
            return engineState.DateString + "|" +
                AddSpacing(engineState.Value.ToString(), 5) + "|" +
                AddSpacing(engineState.R30.ToString(), 5) + "|" +
                AddSpacing(engineState.R60.ToString(), 5) + "|" +
                (engineState.R60 == null || engineState.R30 == null ? "     " : AddSpacing(((float)engineState.R60 / (float)engineState.R30).ToString(), 5)) + "|" +
                (AddSpacing(engineState.Ravg.ToString(), 4)) + "|" +
                AddSpacing((bool)engineState.MainCont?"on":"off", 11) + "|" +
                AddSpacing((bool)engineState.LowR ? "on" : "off", 5) + "|" +
                AddSpacing((bool)engineState.Status ? "on" : "off", 6) + "|" +
                AddSpacing((bool)engineState.Work ? "on" : "off", 5) + "|" +               
                AddSpacing((bool)engineState.NoStart ? "on" : "off", 11);
        }

        private String AddSpacing (String st, Byte count)
        {
            for (int i= st.Length; i<count; i++)
            {
                if (i%2==0)
                    st += " ";
                else
                    st=" "+st;
            }
            return st;
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

        private List<String> GetWorkTimes(Int32 idEngine)
        {
            List<String> result = new List<String>();
            TimeSpan workTime=TimeSpan.Zero, notWorkTime= TimeSpan.Zero,changeTime = TimeSpan.Zero;
            Boolean mode=false;
            DateTime lastDate=new DateTime();
            using (AppContext db = new AppContext())
            {
                var workTimes = db.EngineStates.Where(es => es.EngineId == idEngine).OrderBy(es => es.Date).Select(es=>new {IsWork = es.Work,Date = es.Date });
                foreach (var work in workTimes)
                {
                    if (lastDate == DateTime.MinValue)
                    {       lastDate = work.Date;
                            mode = (bool)work.IsWork;
                            continue;
                    }
                    else
                    {
                        if (work.IsWork==mode)
                        {
                            if (mode)
                                workTime += work.Date-lastDate;
                            else
                                notWorkTime += work.Date - lastDate;


                        }
                        else
                        {
                            changeTime += work.Date - lastDate;
                        }
                    }
                    lastDate = work.Date;
                    mode = (bool)work.IsWork;
                }                  
            }
            workTime += TimeSpan.FromTicks(changeTime.Ticks/2);
            notWorkTime += TimeSpan.FromTicks(changeTime.Ticks / 2);
            result.Add(workTime.Days+"d."+workTime.Hours+"h."+workTime.Minutes+"m.");
            result.Add(notWorkTime.Days + "d." + notWorkTime.Hours + "h." + notWorkTime.Minutes + "m.");
            return result;
        }


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

        private List<DateTime>  PrepareDatesMessage()
        {
            List<DateTime> result = new List<DateTime>();
            DateTime dateFirst = Request.Params["dateStart"] != "" && Request.Params["dateStart"] != null ? DateTime.Parse(Request.Params["dateStart"]) : DateTime.Today.AddDays(-NUMBERS_FOR_GRAPHICS);
            DateTime dateSecond = Request.Params["dateFinish"] != "" && Request.Params["dateFinish"] != null ? DateTime.Parse(Request.Params["dateFinish"]) : DateTime.Today;
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
                listStates = db.EngineStates.Where(es => es.EngineId == idEngine && es.Date >= dateFisrt && es.Date < dateSecond).OrderByDescending(es => es.Date).ToList();
            }
            return listStates;
        }

        private List<EventModel> GetListEvent(Int32 idEngine)
        {
            List<Event> events = null;
            List<EventModel> listEventModels = new List<EventModel>();
            List<Int32> listEngineId = new List<int>();
            List<DateTime> dates = PrepareDatesMessage();
            DateTime dateFirst= dates[0], dateSecond= dates[1];
            using (AppContext db = new AppContext())
            {
                EventComparer eventComparer = new EventComparer();
                if (idEngine > 0)
                    listEngineId.Add(idEngine);
                else
                {
                    Int32 idUser = Int32.Parse(HttpContext.Request.Cookies["Talas"].Value);
                    listEngineId = db.Engines.Where(e => e.UserId == idUser).Select(e=>e.Id).ToList();
                }

                events = db.Events
                                          .Include("Message")
                                          .Include("EngineState")
                                          .Include("EngineState.Engine")
                                          .Where(e => listEngineId.Contains(e.EngineState.EngineId) && e.Date >= dateFirst && e.Date <= dateSecond).ToList();
                                          
                                          //.OrderBy(e => e, eventComparer)
                                          //.OrderByDescending(e => e.IsNew)                                                                                 
                       //  select e;
                events.Sort(eventComparer);
                foreach (Event ev in events)
                {                   
                    listEventModels.Add(new EventModel(ev.EngineState.Engine.Name, ev.Date, ev.Message.Text,ev.IsNew));
                    if (idEngine==-1 && ev.IsNew)
                    {
                        db.Entry(ev).Entity.IsNew = false;
                        //db.Entry(player).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            
            return listEventModels;
        }

    }
}