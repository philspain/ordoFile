using ordoFile.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ordoFile.Tests
{
    
    
    /// <summary>
    ///This is a test class for PresetFiltersTest and is intended
    ///to contain all PresetFiltersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PresetFiltersTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for AddPreset
        ///</summary>
        [TestMethod()]
        public void AddPresetTest()
        {
            bool condition = false;

            PresetFilters target = new PresetFilters();
            string presetName = null;
            List<string> types = null;

            try
            {
                target.AddPreset(presetName, types);
            }
            catch (ArgumentException AEx)
            {
                condition = true;
            }

            Assert.IsTrue(condition);

            condition = true;

            presetName = "MockPreset";
            types = new List<string>() { "mp3" };

            try
            {
                target.AddPreset(presetName, types);
            }
            catch (ArgumentException AEx)
            {
                condition = false;
            }

            Assert.IsTrue(condition);
        }

        /// <summary>
        ///A test for AddPresetType
        ///</summary>
        [TestMethod()]
        public void AddPresetTypeTest()
        {
            PresetFilters target = new PresetFilters();

            string presetName = "MockPreset";
            string type = string.Empty; //Empty string should throw exception

            bool shouldThrowException = false;

            try
            {
                target.AddPresetType(presetName, type);
            }
            catch (ArgumentException AEx)
            {
                shouldThrowException = true;
            }

            Assert.IsTrue(shouldThrowException);


            type = "mp4"; // valid filetype, should not throw exception

            bool shouldNotThrowException = true;

            try
            {
                target.AddPresetType(presetName, type);
            }
            catch (ArgumentException AEx)
            {
                shouldNotThrowException = false;
            }

            Assert.IsTrue(shouldNotThrowException);
        }

        /// <summary>
        ///A test for EditPresets
        ///</summary>
        [TestMethod()]
        public void EditPresetsTest()
        {
            PresetFilters target = new PresetFilters();
            
            string presetName = "MockedUp";
            List<string> types = new List<string>() { "Mocking" };
            target.AddPreset(presetName, types);

            List<string> newTypes = new List<string>() { "Mark" };
            string newName = "Marking";
            target.EditPresets(presetName, newName, newTypes);
            
            Assert.IsTrue(target.PresetExists("Marking"), "Preset name has not been changed");
            Assert.IsTrue(target.PresetTypeExists("Marking", "Mark"), "New types have not been added");
        }

        /// <summary>
        ///A test for GetPresetTypes
        ///</summary>
        [TestMethod()]
        public void GetPresetTypesTest()
        {
            PresetFilters target = new PresetFilters();
            string presetName = "MockedOut";
            List<string> types = new List<string>() { "Mocking" };

            target.AddPreset(presetName, types);

            List<string> returnTypes = target.GetPresetTypes(presetName);

            Assert.IsTrue(returnTypes.Contains("Mocking"), "Added type has not been found");
        }

        /// <summary>
        ///A test for PresetExists
        ///</summary>
        [TestMethod()]
        public void PresetExistsTest()
        {
            PresetFilters target = new PresetFilters();
            string presetName = "MockedYouOut";
            List<string> types = new List<string>() { "Mocking" };

            target.AddPreset(presetName, types);

            Assert.IsTrue(target.PresetExists(presetName), "Added preset does not appear to exist");
        }

        /// <summary>
        ///A test for PresetTypeExists
        ///</summary>
        [TestMethod()]
        public void PresetTypeExistsTest()
        {
            PresetFilters target = new PresetFilters();
            string presetName = "AreYouMockingMe";
            List<string> types = new List<string>() { "Yes" };

            target.AddPreset(presetName, types);

            Assert.IsTrue(target.PresetTypeExists(presetName, "Yes"), "Added type does not appear to exist");
        }

        /// <summary>
        ///A test for RemovePresetType
        ///</summary>
        [TestMethod()]
        public void RemovePresetTypeTest()
        {
            PresetFilters target = new PresetFilters();
            string presetName = "MockingBird";
            List<string> types = new List<string>() { "MockING" };

            target.AddPreset(presetName, types);
            target.RemovePresetType(presetName, "MockING");

            Assert.IsFalse(target.PresetTypeExists(presetName, "MockING"), "Added type does not appear to exist");
        }
    }
}
