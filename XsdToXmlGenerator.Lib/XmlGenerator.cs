using System.Xml;
using System.Xml.Schema;
using System.Text;
using Fare;

namespace XsdToXmlGenerator.Lib
{
	public class XmlGenerator
	{
		public string Generate(string xsdPath)
		{
			string fullXsdPath = Path.GetFullPath(xsdPath);
			var schemaSet = new XmlSchemaSet();
			string? xsdDirectory = Path.GetDirectoryName(fullXsdPath);

			if (xsdDirectory != null)
			{
				foreach (string file in Directory.GetFiles(xsdDirectory, "*.xsd", SearchOption.AllDirectories))
				{
					using var reader = new XmlTextReader(file);
					XmlSchema? schema = XmlSchema.Read(reader, null);
					if (schema != null)
					{
						schemaSet.Add(schema);
					}
				}
			}

			schemaSet.Compile();

			XmlSchema? mainSchema = null;
			foreach (XmlSchema schema in schemaSet.Schemas())
			{
				if (schema.SourceUri != null && new Uri(schema.SourceUri).LocalPath == fullXsdPath)
				{
					mainSchema = schema;
					break;
				}
			}

			if (mainSchema == null)
			{
				using var reader = new XmlTextReader(xsdPath);
				mainSchema = XmlSchema.Read(reader, null);
				if (mainSchema != null)
				{
					if (!schemaSet.Contains(mainSchema.TargetNamespace))
					{
						schemaSet.Add(mainSchema);
						schemaSet.Compile();
					}
				}
				else
				{
					throw new Exception("Could not read the main schema from the provided XSD file.");
				}
			}


			var sb = new StringBuilder();
			var settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  "
			};

			using (var writer = XmlWriter.Create(sb, settings))
			{
				foreach (XmlSchemaElement element in mainSchema.Elements.Values)
				{
					if (element.Name != null)
					{
						writer.WriteStartElement(element.Name, mainSchema.TargetNamespace);
						GenerateElement(writer, element);
						writer.WriteEndElement();
					}
				}
			}

			return sb.ToString();
		}

		private void GenerateElement(XmlWriter writer, XmlSchemaElement element)
		{
			if (element.ElementSchemaType is XmlSchemaComplexType complexType)
			{
				if (complexType.Particle is XmlSchemaSequence sequence)
				{
					foreach (XmlSchemaObject item in sequence.Items)
					{
						if (item is XmlSchemaElement childElement && childElement.Name != null)
						{
							writer.WriteStartElement(childElement.Name);
							GenerateElement(writer, childElement);
							writer.WriteEndElement();
						}
					}
				}
			}
			else if (element.ElementSchemaType is XmlSchemaSimpleType simpleType)
			{
				writer.WriteString(GenerateSimpleTypeValue(element, simpleType));
			}
			else if (element.Name != null)
			{
				writer.WriteString($"Sample_{element.Name}");
			}
		}

		private static string GenerateStringFromRegex(string pattern)
		{
			try
			{
				var xeger = new Xeger(pattern);
				return xeger.Generate();
			}
			catch
			{
				return pattern;
			}
		}

		private string GenerateSimpleTypeValue(XmlSchemaElement element, XmlSchemaSimpleType simpleType)
		{
			if (simpleType.Content is XmlSchemaSimpleTypeRestriction restriction)
			{
				var enums = restriction.Facets.OfType<XmlSchemaEnumerationFacet>().ToList();
				if (enums.Count != 0)
				{
					return enums[0].Value ?? string.Empty;
				}

				var patternFacet = restriction.Facets.OfType<XmlSchemaPatternFacet>().FirstOrDefault();
				if (patternFacet?.Value != null)
				{
					return GenerateStringFromRegex(patternFacet.Value);
				}

				var lengthFacet = restriction.Facets.OfType<XmlSchemaLengthFacet>().FirstOrDefault();
				if (lengthFacet?.Value != null)
				{
					return new string('A', int.Parse(lengthFacet.Value));
				}

				var minLengthFacet = restriction.Facets.OfType<XmlSchemaMinLengthFacet>().FirstOrDefault();
				if (minLengthFacet?.Value != null)
				{
					return new string('A', int.Parse(minLengthFacet.Value));
				}
			}

			switch (simpleType.TypeCode)
			{
				case XmlTypeCode.String:
					return $"Sample_{element.Name}";
				case XmlTypeCode.Int:
				case XmlTypeCode.Integer:
				case XmlTypeCode.NegativeInteger:
				case XmlTypeCode.NonNegativeInteger:
				case XmlTypeCode.NonPositiveInteger:
				case XmlTypeCode.PositiveInteger:
				case XmlTypeCode.UnsignedInt:
					return "123";
				case XmlTypeCode.Decimal:
					return "123.45";
				case XmlTypeCode.Boolean:
					return "true";
				case XmlTypeCode.Date:
					return DateTime.Now.ToString("yyyy-MM-dd");
				case XmlTypeCode.Time:
					return DateTime.Now.ToString("HH:mm:ss");
				case XmlTypeCode.DateTime:
					return DateTime.Now.ToString("o");
				default:
					return $"Sample_{element.Name}";
			}
		}
	}
}