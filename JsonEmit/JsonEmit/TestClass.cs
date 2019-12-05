using System;
using System.IO;
using System.Text.Json;

namespace JsonEmit
{
    public sealed class TestObject {
        public string StringProperty { get; set; }
        public int Int32Property { get; set; }
        public double DoubleProperty { get; set; }
        public byte ByteProperty { get; set; }
    }
    public class TestClass
    {
        int count;
        TestObject[] testData;
        public TestClass(int count = 1000000) => this.count = count;

        TestObject[] GetTestData() {
            if(testData != null)
                return testData;
            testData = new TestObject[count];
            for(int i = 0; i < count; i++)
                testData[i] = new TestObject() { ByteProperty = (byte)i, DoubleProperty = i, Int32Property = i, StringProperty = i.ToString() };
            return testData;
        }
        public TimeSpan GetTimeHardCode() {
            var options = new JsonWriterOptions();
            options.SkipValidation = false;
            options.Indented = false;
            var objectArray = GetTestData();
            var now = DateTime.Now;
            var writer = new Utf8JsonWriter(Stream.Null, options);
            var jsonEncodedStringPropertyName = JsonEncodedText.Encode("StringProperty");
            var jsonEncodedInt32Property = JsonEncodedText.Encode("Int32Property");
            var jsonEncodedDoubleProperty = JsonEncodedText.Encode("DoubleProperty");
            var jsonEncodedByteProperty = JsonEncodedText.Encode("ByteProperty");
            writer.WriteStartArray();
            foreach(var obj in objectArray) {
                writer.WriteStartObject();
                writer.WriteString(jsonEncodedStringPropertyName, obj.StringProperty);
                writer.WriteNumber(jsonEncodedInt32Property, obj.Int32Property);
                writer.WriteNumber(jsonEncodedDoubleProperty, obj.DoubleProperty);
                writer.WriteNumber(jsonEncodedByteProperty, obj.ByteProperty);
                writer.WriteEndObject();
                writer.Flush();
            }
            writer.WriteEndArray();
            return DateTime.Now - now;
        }
        public TimeSpan GetTimeJsonText() {
            var objectArray = GetTestData();
            var now = DateTime.Now;
            JsonSerializer.SerializeAsync(Stream.Null, objectArray).GetAwaiter().GetResult();
            return DateTime.Now - now;
        }
    }
}
