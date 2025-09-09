
using System;
using System.IO;
using NUnit.Framework;

namespace FolderSync.Tests;


[TestFixture]
public class ArgParserTests
{
    // Tests for valid that all arguments have valid paths and a positive integer for the sync interval
    // Args 0,1,2 are valid paths, arg 3 is a positive integer

    // Tests with more and less than 4 arguments
    // Test with non-existing source directory (arg 0)
    private string _tempRoot = default!;
    private string _sourceDir = default!;
    private string _replicaDir = default!;
    private string _logFile = default!;

    [SetUp]
    public void SetUp()
    {
        // Create a unique temp root for this test run
        _tempRoot = Path.Combine(Path.GetTempPath(), "FolderSyncTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_tempRoot);

        // Create a valid source directory
        _sourceDir = Path.Combine(_tempRoot, "source");
        Directory.CreateDirectory(_sourceDir);

        // Create a valid replica directory (we won't always use it; replica may not exist by design)
        _replicaDir = Path.Combine(_tempRoot, "replica");
        Directory.CreateDirectory(_replicaDir);

        // Create a valid log file (touch an empty file)
        _logFile = Path.Combine(_tempRoot, "sync.log");
        File.WriteAllText(_logFile, string.Empty);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up any files/directories created for the tests
        if (Directory.Exists(_tempRoot))
            Directory.Delete(_tempRoot, recursive: true);
    }

    [Test]
    public void ParseArgs_HappyPath_ReturnsConfig()
    // Test that all 4 args valid
    {
        
        string[] args = [_sourceDir, _replicaDir, _logFile, "10"];

        // Act
        var config = ArgParser.ParseArgs(args);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(config.SourceFolder, Is.EqualTo(_sourceDir), "Source folder should match input.");
            Assert.That(config.ReplicaFolder, Is.EqualTo(_replicaDir), "Replica folder should match input.");
            Assert.That(config.LogFilePath, Is.EqualTo(_logFile), "Log file path should match input.");
            Assert.That(config.SyncInterval, Is.EqualTo(10), "Sync interval should match parsed integer.");
        });
    }

    [Test]
    public void ParseArgs_LessThanFourArguments_Throws()
    {
        // Test failure when less than 4 args provided
        
        string[] args = [_sourceDir, _replicaDir, _logFile]; // 3 args

        Assert.Throws<ArgumentException>(() => ArgParser.ParseArgs(args));
    }

    [Test]
    public void ParseArgs_MoreThanFourArguments_Throws()
    {
        // Test failure when more than 4 args provided
        
        string[] args = [_sourceDir, _replicaDir, _logFile, "10", "extra"]; // 5 args

        Assert.Throws<ArgumentException>(() => ArgParser.ParseArgs(args));
    }

    [Test]
    public void ParseArgs_SourceDirectoryDoesNotExist_Throws()
    {
        // Test failure when source directory does not exist
        string missingSource = Path.Combine(_tempRoot, "missingSource"); // does not exist
        string[] args = [missingSource, _replicaDir, _logFile, "10"];

        Assert.Throws<ArgumentException>(() => ArgParser.ParseArgs(args), "Source directory must exist.");
    }


    [Test]
    public void ParseArgs_InvalidPathString_Throws()
    {
        // Test failure when one of the path strings is invalid (contains illegal characters)
        string badPath = "bad|path";
        string[] args = [badPath, _replicaDir, _logFile, "10"];

        Assert.Throws<ArgumentException>(() => ArgParser.ParseArgs(args), "Invalid path format should throw.");
    }

    [TestCase("0")]
    [TestCase("-1")]
    [TestCase("abc")]
    public void ParseArgs_InvalidInterval_Throws(string interval)
    {
        
        string[] args = [_sourceDir, _replicaDir, _logFile, interval];

        Assert.Throws<ArgumentException>(() => ArgParser.ParseArgs(args), "Interval must be a positive integer.");
    }
}


