using Xunit;
using System.IO;
using XsdToXmlGenerator.Lib;
using System.Text.RegularExpressions;

namespace XsdToXmlGenerator.Tests
{
    public class XmlGeneratorTests
    {
        [Fact]
        public void Generate_SimpleXsd_ReturnsNotEmptyXml()
        {
            // Arrange
            var generator = new XmlGenerator();
            string xsdContent = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""root"">
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""child"" type=""xs:string"" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";
            string xsdPath = "test_simple.xsd";
            File.WriteAllText(xsdPath, xsdContent);

            // Act
            string xml = generator.Generate(xsdPath);

            // Assert
            Assert.NotNull(xml);
            Assert.Contains("<root>", xml);
            Assert.Contains("<child>Sample_child</child>", xml);

            // Clean up
            File.Delete(xsdPath);
        }

        [Fact]
        public void Generate_XsdWithEnum_ReturnsXmlWithEnumValue()
        {
            // Arrange
            var generator = new XmlGenerator();
            string xsdContent = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""root"">
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""color"">
            <xs:simpleType>
                <xs:restriction base=""xs:string"">
                    <xs:enumeration value=""red""/>
                    <xs:enumeration value=""green""/>
                    <xs:enumeration value=""blue""/>
                </xs:restriction>
            </xs:simpleType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";
            string xsdPath = "test_enum.xsd";
            File.WriteAllText(xsdPath, xsdContent);

            // Act
            string xml = generator.Generate(xsdPath);

            // Assert
            Assert.NotNull(xml);
            Assert.Contains("<color>red</color>", xml);

            // Clean up
            File.Delete(xsdPath);
        }

        [Fact]
        public void Generate_XsdWithPattern_ReturnsXmlWithMatchingValue()
        {
            // Arrange
            var generator = new XmlGenerator();
            string pattern = "[a-z]{5}";
            string xsdContent = $@"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""root"">
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""word"">
            <xs:simpleType>
                <xs:restriction base=""xs:string"">
                    <xs:pattern value=""{pattern}""/>
                </xs:restriction>
            </xs:simpleType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";
            string xsdPath = "test_pattern.xsd";
            File.WriteAllText(xsdPath, xsdContent);

            // Act
            string xml = generator.Generate(xsdPath);

            // Assert
            Assert.NotNull(xml);
            var match = Regex.Match(xml, @"<word>(.*)</word>");
            Assert.True(match.Success);
            string value = match.Groups[1].Value;
            Assert.Matches(pattern, value);

            // Clean up
            File.Delete(xsdPath);
        }
    }
}
