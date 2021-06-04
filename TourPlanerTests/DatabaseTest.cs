using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using TourPlaner_BL;
using TourPlaner_Models;
using Assert = NUnit.Framework.Assert;
using DescriptionAttribute = NUnit.Framework.DescriptionAttribute;

namespace TourPlanerTests
{
    [TestClass]
    public class DatabaseTest
    {
        private readonly ITourItemFactory tourItemFactory = TourItemFactory.GetInstance(true);
        private readonly FileWrapper wrapper = new();

        private TourItem GetTour(string findName)
        {
            foreach (var item in tourItemFactory.GetItems())
            {
                if (item.Name == findName)
                {
                    var tempTour = item;
                    return tempTour;
                }
            }
            return null;
        }

        [Test, Description("Testing List Return")]
        public async Task TourItems_NameCheckSuccess()
        {
            await tourItemFactory.AddTourAsync("TestExample1", "Wien", "Graz", DateTime.Today, "fastest");
            await tourItemFactory.AddTourAsync("TestExample2", "Wien", "Graz", DateTime.Today, "fastest");
            foreach (var item in tourItemFactory.GetItems())
            {
                Assert.IsNotEmpty(item.Name);
            }
        }

        [Test, Description("Testing Search List")]
        public async Task Tour_SearchSuccess()
        {
            await tourItemFactory.AddTourAsync("TourFrankfurtSearch", "Wien", "Graz", DateTime.Today, "fastest");
            Assert.IsNotNull(tourItemFactory.Search("Frankfurt"));
        }

        [Test, Description("Testing Search List")]
        public async Task Tour_SearchFail()
        {
            await tourItemFactory.AddTourAsync("TourFrankfurtSearchFail", "Wien", "Graz", DateTime.Today, "fastest");
            Assert.IsEmpty(tourItemFactory.Search("Foo"));
        }

        [Test, Description("Testing Add Tour")]
        public async Task AddTourAsync_SuccessAdd()
        {
            var tempVar = await tourItemFactory.AddTourAsync("TourNewExample", "Wien", "Graz", DateTime.Today, "fastest");
            var tour = GetTour("TourNewExample");
            Assert.IsNotNull(tempVar);
            Assert.AreEqual(tour.Name, "TourNewExample");
        }

        [Test, Description("Testing Duplicate Add Tour")]
        public async Task AddTourAsync_FailAdd()
        {
            var tempVar = await tourItemFactory.AddTourAsync("TourNameExample", "Wien", "Graz", DateTime.Today, "fastest");
            Assert.IsNotNull(tempVar);

            var tempVarDup = await tourItemFactory.AddTourAsync("TourNameExample", "Wien", "Graz", DateTime.Today, "fastest");
            Assert.IsNull(tempVarDup); //Null because couldnt add to list
        }

        [Test, Description("Testing Edit Tour")]
        public async Task TourAsync_SuccessEdit()
        {
            await tourItemFactory.AddTourAsync("UneditedTour", "Wien", "Graz", DateTime.Today, "fastest");
            var tempTour = GetTour("UneditedTour");
            Assert.AreEqual(tempTour.Name, "UneditedTour");
            await tourItemFactory.EditTour("UneditedTour", "EditedTourName", "Wien", "Graz", DateTime.Today, "fastest");
            var editedTour = GetTour("EditedTourName");
            Assert.IsNull(GetTour("UneditedTour"));
            Assert.AreEqual(editedTour.Name, "EditedTourName");
        }        

        [Test, Description("Testing Fail Edit")]
        public async Task TourAsync_FailEdit()
        {
            await tourItemFactory.AddTourAsync("UneditedTour", "Wien", "Graz", DateTime.Today, "fastest");
            var tempTour = GetTour("UneditedTour");
            Assert.AreEqual(tempTour.Name, "UneditedTour");
            Assert.That(() => tourItemFactory.EditTour("FaileditedTour", "EditedTourName", "Wien", "Graz", DateTime.Today, "fastest"), Throws.TypeOf<ArgumentException>());
        }

        [Test, Description("Testing Values in Tour")]
        public async Task AddTourAsync_CheckValues()
        {
            var tempTour = new TourItem();
            var tempVar = await tourItemFactory.AddTourAsync("TourValuesExample", "Wien", "Graz", DateTime.Today, "fastest");
            Assert.IsNotNull(tempVar);
            tempTour = GetTour("TourValuesExample");
            Assert.AreEqual(tempTour.Name, "TourValuesExample");
        }

        [Test, Description("Testing Successful Add Log")]
        public async Task AddLogs_Success()
        {
            await tourItemFactory.AddTourAsync("TourLogSuccessExample", "Wien", "Graz", DateTime.Today, "fastest");
            var tempVar = tourItemFactory.AddLogs("TourLogSuccessExample", "This is a log", 5, 2, "Test Description", DateTime.Today);
            Assert.IsNotNull(tempVar);
        }

        [Test, Description("Testing Failed Add Log Null")]
        public void AddLogs_NullTourFail()
        {
            var ex = tourItemFactory.AddLogs("TourLogSuccessExample", "This is a log", 5, 2, "Test Description", DateTime.Today);
            Assert.IsNull(ex);
        }

        [Test, Description("Testing Failed Add Log")]
        public async Task AddLogs_Fail()
        {
            var tempTour = await tourItemFactory.AddTourAsync("TourLogFailExample", "Wien", "Graz", DateTime.Today, "fastest");
            Assert.IsNotNull(tempTour);
            var ex = Assert.Throws<ArgumentNullException>(() => tourItemFactory.AddLogs("", "", 0, 0, "", DateTime.Today));
            Assert.That(ex.ParamName, Is.EqualTo("Empty Arguments"));
        }

        [Test, Description("Testing Creating The PDF")]
        public async Task CreatePDF_Success()
        {
            var tempTour = new TourItem();
            await tourItemFactory.AddTourAsync("TourPDFExample", "Wien", "Graz", DateTime.Today, "fastest");
            tempTour = GetTour("TourPDFExample");
            await tourItemFactory.CreatePDF("TourPDFExample", tempTour);
            Assert.IsTrue(wrapper.Exists(@$"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourPDFs\TourPDFExampleLog.pdf"));
        }

        [Test, Description("Testing Creating The Summary PDF")]
        public async Task CreatePDF_Summary()
        {
            await tourItemFactory.AddTourAsync("TourList1", "Wien", "Graz", DateTime.Today, "fastest");
            await tourItemFactory.AddTourAsync("TourList2", "Wien", "Graz", DateTime.Today, "fastest");
            await tourItemFactory.CreateSummary((System.Collections.Generic.List<TourItem>)tourItemFactory.GetItems());
            Assert.IsTrue(wrapper.Exists(@$"C:\Users\Gregor\source\repos\TourPlaner\TourPlaner_DL\TourPDFs\SummaryLog.pdf"));
        }

        [Test, Description("Testing Deleting Tour")]
        public async Task RemoveTour_Success()
        {
            await tourItemFactory.AddTourAsync("RemoveList", "Wien", "Graz", DateTime.Today, "fastest");
            tourItemFactory.RemoveTour("RemoveList", true);
            Assert.IsNull(tourItemFactory.GetItems());
        }

        [Test, Description("Testing Fail Deleting Tour")]
        public async Task RemoveTour_Fail()
        {
            await tourItemFactory.AddTourAsync("RemoveListFail", "Wien", "Graz", DateTime.Today, "fastest");
            tourItemFactory.RemoveTour("RemoveList", true);
            Assert.IsNotNull(tourItemFactory.GetItems());
        }

        [Test, Description("Testing Successful Edit Log")]
        public async Task EditLog_Success()
        {
            var tempTour = new TourItem();
            await tourItemFactory.AddTourAsync("EditLogTestSuccess", "Wien", "Graz", DateTime.Today, "fastest");
            tourItemFactory.AddLogs("EditLogTestSuccess", "This is a log", 5, 2, "Test Description", DateTime.Today);
            foreach (var item in tourItemFactory.GetItems())
            {
                if (item.Name == "EditLogTestSuccess")
                    tempTour = item;
            }
            Assert.AreEqual(tempTour.TourLogs[0].Logs, "This is a log");
            tourItemFactory.EditLogs("EditLogTestSuccess", "This is a log", "New Log Entry", 2, 3, "New Description", DateTime.Today);
            Assert.AreEqual(tempTour.TourLogs[0].Logs, "New Log Entry");
        }

        [Test, Description("Testing Fail Edit Log")]
        public async Task EditLog_Fail()
        {
            var tempTour = new TourItem();
            await tourItemFactory.AddTourAsync("EditLogTestFail", "Wien", "Graz", DateTime.Today, "fastest");
            tourItemFactory.AddLogs("EditLogTestFail", "This is a log", 5, 2, "Test Description", DateTime.Today);
            tempTour = GetTour("EditLogTestFail");
            Assert.Throws<NullReferenceException>(() => tourItemFactory.EditLogs("EditLogTestFail", "Failure Input", "New Log Entry", 2, 3, "New Description", DateTime.Today));
        }


        [Test, Description("Testing Deleting Log")]
        public async Task RemoveLog_Success()
        {
            var tempTour = new TourItem();
            await tourItemFactory.AddTourAsync("RemoveLogTest", "Wien", "Graz", DateTime.Today, "fastest");
            tourItemFactory.AddLogs("RemoveLogTest", "This is a log", 5, 2, "Test Description", DateTime.Today);
            tempTour = GetTour("RemoveLogTest");
            Assert.AreEqual(tempTour.TourLogs[0].Logs, "This is a log");
            tourItemFactory.EditLogs("RemoveLogTest", "This is a log", "New Log Entry", 2, 3, "New Description", DateTime.Today, true);
            Assert.IsEmpty(tempTour.TourLogs);
        }

        [Test, Description("Testing Fail Deleting Log")]
        public async Task RemoveLog_Fail()
        {
            await tourItemFactory.AddTourAsync("RemoveLogTestFail", "Wien", "Graz", DateTime.Today, "fastest");
            tourItemFactory.RemoveTour("RemoveList", true);
            Assert.IsNotNull(tourItemFactory.GetItems());
        }

        [Test, Description("Testing Instance")]
        public void Check_Instance()
        {
            ITourItemFactory testInstance = TourItemFactory.GetInstance(true);
            Assert.AreEqual(testInstance, tourItemFactory);
        }

    }


    public interface IFileWrapper
    {
        bool Exists(string path);
    }

    public class FileWrapper : IFileWrapper
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
}
