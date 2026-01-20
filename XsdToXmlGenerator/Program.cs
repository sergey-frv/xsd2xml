using XsdToXmlGenerator.Lib;

namespace XsdToXmlGenerator
{
	static class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Usage: xsd2xml <path_to_xsd_file>");
				return;
			}

			string xsdPath = args[0];
			if (!File.Exists(xsdPath))
			{
				Console.WriteLine($"Error: File not found at '{xsdPath}'");
				return;
			}

			try
			{
				var generator = new XmlGenerator();
				string xml = generator.Generate(xsdPath);
				Console.WriteLine(xml);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
				Console.WriteLine(ex.StackTrace);
			}
		}
	}
}