using NUnit.Framework;
using ChessBoom;
using System.IO;

namespace ChessBoom.NUnitTests
{
    public class AppUnitTests
    {
        private App app = null!;

        [SetUp]
        public void Setup()
        {
            app = new App();
        }

        /// <summary>
        /// Test the current working directory
        /// </summary>
        [Test]
        public void WorkingDirectoryTest()
        {
            app.SetWorkingDirectory();
            string directory = new DirectoryInfo(Directory.GetCurrentDirectory()).Name;

            // ChessBoom.NUnitTests and src are at the same hierarchy level
            Assert.AreEqual(directory, "ChessBoom.NUnitTests");
        }
    }
}