using Aiursoft.Warp.Services.FileStorage;

namespace Aiursoft.Warp.Tests;

[TestClass]
public class FileAccessSecurityTest
{
    private StorageService _storageService = null!;
    private string _tempPath = null!;

    [TestInitialize]
    public void Initialize()
    {
        _tempPath = Path.Combine(Path.GetTempPath(), "AiursoftTemplateTest_" + Guid.NewGuid());
        Directory.CreateDirectory(_tempPath);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Storage:Path", _tempPath }
            })
            .Build();

        _storageService = new StorageService(config);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_tempPath))
        {
            Directory.Delete(_tempPath, true);
        }
    }

    [TestMethod]
    public void TestGetFilePhysicalPath_NormalAccess()
    {
        const string relativePath = "test.txt";
        var physicalPath = _storageService.GetFilePhysicalPath(relativePath);

        Assert.StartsWith(_tempPath, physicalPath);
        Assert.EndsWith(relativePath, physicalPath);
    }

    [TestMethod]
    [DataRow("../secret.txt")]
    [DataRow("../../etc/passwd")]
    [DataRow("/etc/passwd")]
    public void TestGetFilePhysicalPath_PathTraversal(string maliciousPath)
    {
        try
        {
            _storageService.GetFilePhysicalPath(maliciousPath);
            Assert.Fail("Expected ArgumentException was not thrown.");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task TestSave_NormalAccess()
    {
        const string content = "Hello World";
        const string fileName = "test_upload.txt";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        await writer.WriteAsync(content);
        await writer.FlushAsync(TestContext.CancellationToken);
        ms.Position = 0;

        var formFile = new FormFile(ms, 0, ms.Length, "file", fileName);

        var savedPath = await _storageService.Save("uploads/" + fileName, formFile);

        Assert.Contains("uploads", savedPath);
        Assert.Contains(fileName, savedPath);
    }

    [TestMethod]
    [DataRow("../malicious.txt")]
    [DataRow("../../malicious.txt")]
    [DataRow("/absolute/path/malicious.txt")]
    public async Task TestSave_PathTraversal(string maliciousPath)
    {
        var ms = new MemoryStream();
        var formFile = new FormFile(ms, 0, 0, "file", "dummy.txt");

        try
        {
            await _storageService.Save(maliciousPath, formFile);
            Assert.Fail("Expected ArgumentException was not thrown.");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    public TestContext TestContext { get; set; }
}
