using ordoFile.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for ConfigsTest and is intended
    ///to contain all ConfigsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConfigsTest
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
        ///A test for Configs Constructor
        ///</summary>
        [TestMethod()]
        public void ConfigsConstructorTest()
        {
            Configs target = new Configs();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for CheckFileExists
        ///</summary>
        [TestMethod()]
        [DeploymentItem("ordoFile.exe")]
        public void CheckFileExistsTest()
        {
            Configs_Accessor target = new Configs_Accessor(); // TODO: Initialize to an appropriate value
            target.CheckFileExists();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CreateConfigs
        ///</summary>
        [TestMethod()]
        [DeploymentItem("ordoFile.exe")]
        public void CreateConfigsTest()
        {
            Configs_Accessor target = new Configs_Accessor(); // TODO: Initialize to an appropriate value
            target.CreateConfigs();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetBGSetting
        ///</summary>
        [TestMethod()]
        [DeploymentItem("ordoFile.exe")]
        public void GetBGSettingTest()
        {
            Configs_Accessor target = new Configs_Accessor(); // TODO: Initialize to an appropriate value
            string setting = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetBGSetting(setting);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SaveConfigs
        ///</summary>
        [TestMethod()]
        public void SaveConfigsTest()
        {
            Configs target = new Configs(); // TODO: Initialize to an appropriate value
            target.SaveConfigs();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SetBGSetting
        ///</summary>
        [TestMethod()]
        [DeploymentItem("ordoFile.exe")]
        public void SetBGSettingTest()
        {
            Configs_Accessor target = new Configs_Accessor(); // TODO: Initialize to an appropriate value
            string setting = string.Empty; // TODO: Initialize to an appropriate value
            string value = string.Empty; // TODO: Initialize to an appropriate value
            target.SetBGSetting(setting, value);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SetConfigs
        ///</summary>
        [TestMethod()]
        [DeploymentItem("ordoFile.exe")]
        public void SetConfigsTest()
        {
            Configs_Accessor target = new Configs_Accessor(); // TODO: Initialize to an appropriate value
            target.SetConfigs();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for BGDirectory
        ///</summary>
        [TestMethod()]
        public void BGDirectoryTest()
        {
            Configs target = new Configs(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.BGDirectory = expected;
            actual = target.BGDirectory;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for BGDirectoryExists
        ///</summary>
        [TestMethod()]
        public void BGDirectoryExistsTest()
        {
            Configs target = new Configs(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.BGDirectoryExists;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OrganiseSubDirectories
        ///</summary>
        [TestMethod()]
        public void OrganiseSubDirectoriesTest()
        {
            Configs target = new Configs(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.OrganiseSubDirectories = expected;
            actual = target.OrganiseSubDirectories;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for StartupEnabled
        ///</summary>
        [TestMethod()]
        public void StartupEnabledTest()
        {
            Configs target = new Configs(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.StartupEnabled = expected;
            actual = target.StartupEnabled;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
