# XSD to XML Generator

This is a simple .NET tool to generate an example XML file from an XSD schema.

## How to Install

1.  Navigate to the `XsdToXmlGenerator` directory.
2.  Pack the project to create a NuGet package:
    ```bash
    dotnet pack
    ```
3.  Install the tool globally from the local package:
    ```bash
    dotnet tool install --global --add-source XsdToXmlGenerator/nupkg/ XsdToXmlGenerator
    ```

## How to Use

Once the tool is installed, you can use it from anywhere on your system.

```bash
xsd2xml <path_to_your_xsd_file>
```

### Example

```bash
xsd2xml xsd/basic_User_Data_documents.xsd
```

This will print the generated XML to the console.
