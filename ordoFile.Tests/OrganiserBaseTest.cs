using ordoFile.Models.Organisers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ordoFile.Models;
using System.Collections.ObjectModel;

namespace ordoFile.Tests
{
    
    
    /// <summary>
    ///This is a test class for OrganiserBaseTest and is intended
    ///to contain all OrganiserBaseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OrganiserBaseTest
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


        internal virtual OrganiserBase CreateOrganiserBase()
        {
            // TODO: Instantiate an appropriate concrete class.
            OrganiserBase target = null;
            return target;
        }

        /// <summary>
        ///A test for ClearLists
        ///</summary>
        [TestMethod()]
        public void ClearListsTest()
        {
            OrganiserBase target = CreateOrganiserBase(); // TODO: Initialize to an appropriate value
            target.ClearLists();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        internal virtual OrganiserBase_Accessor CreateOrganiserBase_Accessor()
        {
            // TODO: Instantiate an appropriate concrete class.
            OrganiserBase_Accessor target = null;
            return target;
        }

        /// <summary>
        ///A test for CreateFileObject
        ///</summary>
        [TestMethod()]
        [DeploymentItem("ordoFile.exe")]
        public void CreateFileObjectTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            OrganiserBase_Accessor target = new OrganiserBase_Accessor(param0); // TODO: Initialize to an appropriate value
            string filePath = string.Empty; // TODO: Initialize to an appropriate value
            string fileType = string.Empty; // TODO: Initialize to an appropriate value
            long totalFileSize = 0; // TODO: Initialize to an appropriate value
            long totalFileSizeExpected = 0; // TODO: Initialize to an appropriate value
            target.CreateFileObject(filePath, fileType, ref totalFileSize);
            Assert.AreEqual(totalFileSizeExpected, totalFileSize);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetFileObjects
        ///</summary>
        [TestMethod()]
        [DeploymentItem("ordoFile.exe")]
        public void GetFileObjectsTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            OrganiserBase_Accessor target = new OrganiserBase_Accessor(param0); // TODO: Initialize to an appropriate value
            DirectoryModel directory = null; // TODO: Initialize to an appropriate value
            long totalFileSize = 0; // TODO: Initialize to an appropriate value
            long totalFileSizeExpected = 0; // TODO: Initialize to an appropriate value
            target.GetFileObjects(directory, ref totalFileSize);
            Assert.AreEqual(totalFileSizeExpected, totalFileSize);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Organise
        ///</summary>
        [TestMethod()]
        public void OrganiseTest()
        {
            OrganiserBase target = CreateOrganiserBase(); // TODO: Initialize to an appropriate value
            target.Organise();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for PopulateFileTypes
        ///</summary>
        [TestMethod()]
        public void PopulateFileTypesTest()
        {
            string[] files = null; // TODO: Initialize to an appropriate value
            ObservableCollection<string> availableFileTypes = null; // TODO: Initialize to an appropriate value
            OrganiserBase.PopulateFileTypes(files, availableFileTypes);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Rename
        ///</summary>
        [TestMethod()]
        [DeploymentItem("ordoFile.exe")]
        public void RenameTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            OrganiserBase_Accessor target = new OrganiserBase_Accessor(param0); // TODO: Initialize to an appropriate value
            string destDir = string.Empty; // TODO: Initialize to an appropriate value
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            string fileType = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Rename(destDir, fileName, fileType);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for TraverseDirectories
        ///</summary>
        [TestMethod()]
        public void TraverseDirectoriesTest()
        {
            DirectoryModel directory = null; // TODO: Initialize to an appropriate value
            ObservableCollection<string> availableFileTypes = null; // TODO: Initialize to an appropriate value
            OrganiserBase.TraverseDirectories(directory, availableFileTypes);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
