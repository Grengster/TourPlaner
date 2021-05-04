using DinkToPdf;
using DinkToPdf.Contracts;
using Newtonsoft.Json;
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
    /*
    public sealed class SynchronizedConverter
    {
        private SynchronizedConverter(ITools tools) { }

        private static SynchronizedConverter converter = null;

        public static SynchronizedConverter Instance()
        {
            if (converter == null)
            {
                converter = new SynchronizedConverter(new PdfTools());
            }
            return converter;
        }
    } */



    public class PDFHandler
    {
        
        public static async Task CreatePDF(string tourName)
        {

            var jsonData = System.IO.File.ReadAllText(@"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourJson\TourData.json");
            var employeeList = JsonConvert.DeserializeObject<List<TourItem>>(jsonData) ?? new List<TourItem>();
            var account = employeeList.FirstOrDefault(p => p.Name == tourName);
            //SynchronizedConverter converter = SynchronizedConverter.Instance();
            //var converter = new SynchronizedConverter(new PdfTools());
            //Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll");

            dynamic dynJson = JsonConvert.DeserializeObject(account.TourInfo.JsonData); //need to find a way of iterating through narrative
            string navigationText = "";
            foreach (var item in dynJson)
            {
                navigationText += item.narrative + "<br>";
            }

            var converter = new BasicConverter(new PdfTools());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4Plus,
                Out = @$"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourPDFs\{tourName}Log.pdf",
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = @$"   Tourname: {tourName} <br> 
                                            Startname: {account.TourInfo.Start} <br> 
                                            Destinationname: {account.TourInfo.Goal} <br>
                                            Navigationtext: {navigationText}
                                            Method of travelling: {account.TourInfo.Method}km <br> 
                                            Image: <img width=364 height=160 src='file:///C:/Users/Gregor/source/repos/TourPlaner/TourPlaner_DL/TourMaps/{tourName}.png' hspace=12>",
                        WebSettings = { DefaultEncoding = "utf-8" },
                        HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
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
