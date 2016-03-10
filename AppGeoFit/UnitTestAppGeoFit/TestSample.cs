using System;
using NUnit.Framework;
using Xamarin.Forms;
using AppGeoFit.BusinessLayer.Managers;


namespace UnitTestAppGeoFit
{
    [TestFixture]
    public class TestsSample
    {
        PlayerManager playerManager;

       [SetUp]
        public void Setup() {
            playerManager = new PlayerManager(true);
        }


        [TearDown]
        public void Tear() { }

        [Test]
        public void Pass()
        {
            Console.WriteLine("test1");
            Assert.True(true);
        }

        [Test]
        public void Fail()
        {
            Assert.False(true);
        }

        [Test]
        [Ignore("another time")]
        public void Ignore()
        {
            Assert.True(false);
        }

        [Test]
        public void Inconclusive()
        {
            Assert.Inconclusive("Inconclusive");
        }
    }
}