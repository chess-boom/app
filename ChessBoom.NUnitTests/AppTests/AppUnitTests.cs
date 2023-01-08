using NUnit.Framework;

namespace ChessBoom.NUnitTests.AppTests;

public class AppUnitTests
{
    private App _app = null!;

    [SetUp]
    public void Setup()
    {
        _app = new App();
    }

    /// <summary>
    /// Test the current working directory
    /// </summary>
    [Test]
    public void WorkingDirectoryTest()
    {
        App.SetWorkingDirectory();
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory()).Name;

        // ChessBoom.NUnitTests and src are at the same hierarchy level
        Assert.AreEqual("ChessBoom.NUnitTests", directory);
    }
}