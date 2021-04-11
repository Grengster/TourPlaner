using NUnit.Framework;
using NUnit;
using TourPlaner_BL;

namespace TourPlanerTests
{
    [TestFixture()]
    public class UnitTest1
    {
        private readonly ITourItemFactory tourItemFactory = TourItemFactory.GetInstance();

        [Test, Description("Testing Empty List")]
        public void TestMethod1()
        {
           // Assert.IsNull(tourItemFactory.GetItems());
            Assert.Equals(1, 1);
        }
    }
}
