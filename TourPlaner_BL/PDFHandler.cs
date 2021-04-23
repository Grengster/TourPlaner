using DinkToPdf;
using DinkToPdf.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

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
        
        public static async Task CreatePDF(string tourName, string start, string goal, int distance)
        {
            //SynchronizedConverter converter = SynchronizedConverter.Instance();
            //var converter = new SynchronizedConverter(new PdfTools());
            //Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll");

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
                                            Startname: {start} <br> 
                                            Destinationname: {goal} <br> 
                                            Distance: {distance}km <br> 
                                            Image: <img width=364 height=160 src='file:///C:/Users/Gregor/source/repos/TourPlaner/TourPlaner_DL/TourMaps/{tourName}.png' hspace=12>",
                        WebSettings = { DefaultEncoding = "utf-8" },
                        HeaderSettings = { FontSize = 20, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
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
