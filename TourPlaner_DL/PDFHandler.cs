using DinkToPdf;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TourPlaner_Models;

namespace TourPlaner_DL
{
    public class PDFHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PDFHandler));
        private static string userPath = ConfigurationManager.ConnectionStrings["userPath"].ConnectionString;

        public async Task CreateSummary(List<TourItem> mockList = null)
        {
            var employeeList = new List<TourItem>();
            if (mockList == null)
            {
                var jsonData = System.IO.File.ReadAllText(userPath + @"TourPlaner_DL\TourJson\TourData.json");
                employeeList = JsonConvert.DeserializeObject<List<TourItem>>(jsonData) ?? new List<TourItem>();
            }
            else
            {
                employeeList = mockList;
            }

            float totalDistance = 0;
            string summaryText = "";

            TimeSpan total = new();
            var totalDisplay = "";
            foreach (var item in employeeList)
            {
                totalDistance += item.TourInfo.Distance;

                if (mockList == null)
                {
                    var split = item.TourInfo.TotalTime.Split(':');
                    total = total.Add(new TimeSpan(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2])));
                    totalDisplay = total.ToString(@"dd\:hh\:mm\:ss");
                }

                summaryText += @$"Tourname: {item.Name} || Start: {item.TourInfo.Start} -> Destination: {item.TourInfo.Goal} || Time: {item.TourInfo.TotalTime} || Distance: {item.TourInfo.Distance}km <br>";
            }

            var converter = new BasicConverter(new PdfTools());

            if (!Directory.Exists(userPath + $@"TourPlaner_DL\TourPDFs\"))
            {
                Directory.CreateDirectory(userPath + @"TourPlaner_DL\TourPDFs\");
            }

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4Plus,
                Margins = new MarginSettings { Top = 10 },
                Out = userPath + @"TourPlaner_DL\TourPDFs\SummaryLog.pdf",
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = @$"<h3>Total Time (DD:HH:MM:SS): {totalDisplay}</h3>
                                         <h3>Total Distance: {totalDistance}km </h3><br>   
                                        " + summaryText,
                        WebSettings = { DefaultEncoding = "utf-8" }, //, UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                        HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                        FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
                    }
                }
            };

            converter.Convert(doc);
        }

        public async Task CreatePDF(string tourName, TourItem mockItem = null)
        {
            var account = new TourItem();
            var employeeList = new List<TourItem>();
            var jsonData = "";
            dynamic dynJson = null;
            if (mockItem == null)
            {
                jsonData = System.IO.File.ReadAllText(userPath + @"TourPlaner_DL\TourJson\TourData.json");
                employeeList = JsonConvert.DeserializeObject<List<TourItem>>(jsonData) ?? new List<TourItem>();
                account = employeeList.FirstOrDefault(p => p.Name == tourName);
                dynJson = JToken.Parse(account.TourInfo.JsonData).ToObject<dynamic>(); //need to find a way of iterating through narrative
            }
            else
                account = mockItem;



            string navigationText = "", logText = "";

            int i = 1;
            foreach (var item in account.TourLogs)
            {
                string test = i.ToString();
                logText += @$"Logs-Nr." + test +
                            "<br><span style='margin-left: 50px;'>Text: " + item.Logs + "</span>" +
                            "<br><span style='margin-left: 50px;'>Weather: " + item.Weather + "</span>" +
                            "<br><span style='margin-left: 50px;'>Rating: " + item.Rating + "</span>" +
                            "<br><span style='margin-left: 50px;'>Date: " + item.ActualTime + "</span>" +
                            "<br>";
                i++;
            }

            if (mockItem == null)
            {
                foreach (var item in dynJson.route.legs[0].maneuvers)
                {
                    navigationText += "<span style='margin-left: 50px; '>" + item.narrative + "</span><br>";
                }
            }

            var converter = new BasicConverter(new PdfTools());
            var travelMethod = "";
            if (account.TourInfo.Method == "fastest")
                travelMethod = "car";

            if (!Directory.Exists(userPath + $@"TourPlaner_DL\TourPDFs\"))
            {
                Directory.CreateDirectory(userPath + @"TourPlaner_DL\TourPDFs\");
            }

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4Plus,
                Margins = new MarginSettings { Top = 10 },
                Out = userPath + @$"TourPlaner_DL\TourPDFs\{tourName}Log.pdf",
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = @$"   Tourname: {tourName} <br> 
                                            Startname: {account.TourInfo.Start} <br> 
                                            Destinationname: {account.TourInfo.Goal} <br>
                                            Method of travelling: {travelMethod}<br> 
                                            Image: <img width=364 height=160 src='file:///{userPath}TourPlaner_DL/TourMaps/{tourName}.png' hspace=12><br>
                                            {logText}
                                            <div style='height: 635px; '></div>
                                            Navigationtext: <br>{navigationText}",
                        WebSettings = { DefaultEncoding = "utf-8" }, //, UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                        HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                        FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
                    }
                }
            };
            try
            {
                converter.Convert(doc);
            }
            catch (Exception e)
            {
                log.Error(e);
            }

        }

    }
}
