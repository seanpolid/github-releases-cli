using GitHubReleasesCLI.models;

namespace Tests.unit_tests.models
{
    public class ParsedArgsTests
    {
        [Fact]
        public void FormatZipName_ContainsZipExtension_Success()
        {
            // Arrange
            ParsedArgs parsedArgs = new()
            {
                ZipName = "file.zip"
            };

            // Act
            parsedArgs.FormatZipName();

            // Assert
            Assert.Equal("file", parsedArgs.ZipName);
        }

        [Fact]
        public void FormatZipName_NoZipExtension_Success()
        {
            // Arrange
            ParsedArgs parsedArgs = new()
            {
                ZipName = "file"
            };

            // Act
            parsedArgs.FormatZipName();

            // Assert
            Assert.Equal("file", parsedArgs.ZipName);
        }
    }
}
