using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using VismaResourceShortage.Models;
using VismaResourceShortage.Services;

namespace VismaResourceShortage.Tests
{
    [TestFixture]
    public class JsonServiceTests
    {
        private JsonService jsonService;
        private string testFilePath = "test_shortages.json";

        [SetUp]
        public void Setup()
        {
            jsonService = new JsonService();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }

        [Test]
        public void TestLoadShortages()
        {
            var shortages = jsonService.LoadShortages();
            Assert.IsNotNull(shortages, "Shortages should not be null");
        }

        [Test]
        public void TestSaveShortages()
        {
            var shortages = new List<Shortage>
            {
                new Shortage { Title = "Test", Name = "User", Room = "Meeting room", Category = "Electronics", Priority = 5, CreatedOn = DateTime.Now }
            };

            jsonService.SaveShortages(shortages);
            var loadedShortages = jsonService.LoadShortages();
            Assert.AreEqual(1, loadedShortages.Count, "There should be one shortage saved");
            Assert.AreEqual("Test", loadedShortages[0].Title, "The title of the saved shortage should be 'Test'");
        }

        [Test]
        public void TestSaveAndLoadMultipleShortages()
        {
            var shortages = new List<Shortage>
            {
                new Shortage { Title = "Test1", Name = "User1", Room = "Meeting room", Category = "Electronics", Priority = 5, CreatedOn = DateTime.Now },
                new Shortage { Title = "Test2", Name = "User2", Room = "Kitchen", Category = "Food", Priority = 7, CreatedOn = DateTime.Now }
            };

            jsonService.SaveShortages(shortages);
            var loadedShortages = jsonService.LoadShortages();
            Assert.AreEqual(2, loadedShortages.Count, "There should be two shortages saved");
        }

        [Test]
        public void TestUpdateShortageWithHigherPriority()
        {
            var shortages = new List<Shortage>
            {
                new Shortage { Title = "Test", Name = "User", Room = "Meeting room", Category = "Electronics", Priority = 5, CreatedOn = DateTime.Now }
            };

            jsonService.SaveShortages(shortages);

            var newShortage = new Shortage { Title = "Test", Name = "User", Room = "Meeting room", Category = "Food", Priority = 7, CreatedOn = DateTime.Now };
            var existingShortage = shortages.FirstOrDefault(s => s.Title == newShortage.Title && s.Room == newShortage.Room);

            if (existingShortage != null)
            {
                if (newShortage.Priority > existingShortage.Priority)
                {
                    existingShortage.Name = newShortage.Name;
                    existingShortage.Category = newShortage.Category;
                    existingShortage.Priority = newShortage.Priority;
                    existingShortage.CreatedOn = newShortage.CreatedOn;
                }
            }

            jsonService.SaveShortages(shortages);
            var loadedShortages = jsonService.LoadShortages();
            Assert.AreEqual(1, loadedShortages.Count, "There should still be one shortage");
            Assert.AreEqual("Food", loadedShortages[0].Category, "The category should be updated to 'Food'");
            Assert.AreEqual(7, loadedShortages[0].Priority, "The priority should be updated to 7");
        }
    }
}
