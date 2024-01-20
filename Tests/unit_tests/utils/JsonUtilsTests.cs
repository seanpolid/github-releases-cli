using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHubReleasesCLI.utils;

namespace Tests.unit_tests.utils
{
    public class JsonUtilsTests
    {
        [Fact]
        public void Serialize_Success()
        {
            // Arrange
            TestObject testObject = new()
            {
                RandomStringField = "test",
                RandomIntField = 1,
            };

            // Act
            string serializedJson = JsonUtils.Serialize(testObject);

            // Assert
            Assert.Contains("random_string_field", serializedJson);
            Assert.Contains("random_int_field", serializedJson);
        }

        [Fact]
        public void Deserialize_Success()
        {
            // Arrange
            string json = "{\r\n  \"random_string_field\": \"test\",\r\n  \"random_int_field\": 1\r\n}";
            TestObject expectedObject = new()
            {
                RandomStringField = "test",
                RandomIntField = 1,
            };

            // Act
            TestObject actualObject = JsonUtils.Deserialize<TestObject>(json);

            // Assert
            Assert.StrictEqual(expectedObject, actualObject);
        }

        class TestObject
        {
            public string? RandomStringField { get; set; }

            public int RandomIntField { get; set; }
            public override bool Equals(object? obj)
            {
                return obj is TestObject @object &&
                       RandomStringField == @object.RandomStringField &&
                       RandomIntField == @object.RandomIntField;
            }
        }
    }
}
