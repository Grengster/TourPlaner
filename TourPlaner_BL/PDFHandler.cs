﻿using DinkToPdf;
using DinkToPdf.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using TourPlaner_Models;

namespace TourPlaner_BL
{



    public class PDFHandler
    {

        public static async Task CreateSummary()
        {
            var jsonData = System.IO.File.ReadAllText(@"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourJson\TourData.json");
            var employeeList = JsonConvert.DeserializeObject<List<TourItem>>(jsonData) ?? new List<TourItem>();
            float totalDistance = 0;
            int hours = 0, minutes = 0, seconds = 0;
            //dynamic dynJson = JToken.Parse(account.TourInfo.JsonData).ToObject<dynamic>(); //need to find a way of iterating through narrative
            string summaryText = "";

            TimeSpan total = new TimeSpan();
            var totalDisplay = "";
            foreach (var item in employeeList)
            {
                totalDistance += item.TourInfo.Distance;

                
                
                var split = item.TourInfo.TotalTime.Split(':');
                total = total.Add(new TimeSpan(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2])));
                totalDisplay = total.ToString(@"dd\:hh\:mm\:ss");

                summaryText += @$"Tourname: {item.Name} || Start: {item.TourInfo.Start} -> Destination: {item.TourInfo.Goal} || Time: {item.TourInfo.TotalTime} || Distance: {item.TourInfo.Distance}km <br>";
            }

            var converter = new BasicConverter(new PdfTools());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4Plus,
                Margins = new MarginSettings { Top = 10 },
                Out = @$"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourPDFs\SummaryLog.pdf",
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


        public static async Task CreatePDF(string tourName)
        {
            var jsonData = System.IO.File.ReadAllText(@"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourJson\TourData.json");
            var employeeList = JsonConvert.DeserializeObject<List<TourItem>>(jsonData) ?? new List<TourItem>();
            var account = employeeList.FirstOrDefault(p => p.Name == tourName);

            dynamic dynJson = JToken.Parse(account.TourInfo.JsonData).ToObject<dynamic>(); //need to find a way of iterating through narrative
            string navigationText = "", logText = "";

            int i = 1;
            foreach (var item in account.TourLogs)
            {
                string test = i.ToString();
                logText +=  @$"Logs-Nr." + test + 
                            "<br><span style='margin-left: 50px;'> Text: " + item.Logs + "</span>" +
                            "<br><span style='margin-left: 50px;'>Weather: " + item.Description + "</span>" +
                            "<br><span style='margin-left: 50px;'>Rating: " + item.Rating + "</span>" +
                            "<br><span style='margin-left: 50px;'>Date: " + item.ActualTime + "</span>" +
                            "<br>";
                i++;
            }

            foreach (var item in dynJson.route.legs[0].maneuvers)
            {
                navigationText += "<span style='margin-left: 50px; '>" + item.narrative + "</span><br>";
            }

            var converter = new BasicConverter(new PdfTools());
            var travelMethod = "";
            if (account.TourInfo.Method == "fastest")
                travelMethod = "car";

                var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4Plus,
                Margins = new MarginSettings { Top = 10 },
                Out = @$"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourPDFs\{tourName}Log.pdf",
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = @$"   Tourname: {tourName} <br> 
                                            Startname: {account.TourInfo.Start} <br> 
                                            Destinationname: {account.TourInfo.Goal} <br>
                                            Method of travelling: {travelMethod}<br> 
                                            Image: <img width=364 height=160 src='file:///C:/Users/Gregor/source/repos/TourPlaner/TourPlaner_DL/TourMaps/{tourName}.png' hspace=12><br>
                                            {logText}
                                            <div style='height: 635px; '></div>
                                            Navigationtext: <br>{navigationText}",
                        WebSettings = { DefaultEncoding = "utf-8" }, //, UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                        HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                        FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
                    }
                }
            };

            converter.Convert(doc);

            //byte[] pdf = converter.Convert(doc);

            //await SaveDoc(doc);
        }

        private static async Task SaveDoc(HtmlToPdfDocument doc)
        {
            var converter = new SynchronizedConverter(new PdfTools());


            doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings() { Top = 10 },
                Out = @"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourPDFs\TestPDF.pdf",
                },
                Objects = {
                    new ObjectSettings()
                    {
                        Page = "http://google.com/",
                    },
                }
            };


            converter.Convert(doc);
        }
    }
}
