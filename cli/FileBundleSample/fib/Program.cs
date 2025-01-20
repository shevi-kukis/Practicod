using System.CommandLine;

var bundleOptionOutput = new Option<FileInfo>(
    "--output",
    "File path and name");


var bundleOptionLanguages = new Option<string[]>(
    "--languages",
    "List of programming languages (e.g., cs, js, py). Use 'all' to include all code files.")
{
    IsRequired = true,
    AllowMultipleArgumentsPerToken = true,
   
};

var bundleOptionNote = new Option<bool>(
    "--note",
    description: "Include source file path and name as a comment in the output bundle.",
    getDefaultValue: () => false);

var bundleOptionSort = new Option<string>(
    "--sort",
    description: "Sort files by 'name' or 'type'. Default is 'name'.",
    getDefaultValue: () => "name");
var bundleOptionRemoveEmptyLines = new Option<bool>(
    "--remove-empty-lines",
    description: "Remove empty lines from the source code before bundling.",
    getDefaultValue: () => false);
var bundleOptipnCreator = new Option<string>(
    "--creator",
    description: "who create the folder",
    getDefaultValue: () => string.Empty
    );
var bundleCommand = new Command("bundle", "Bundle code files to a single file");
bundleCommand.AddOption(bundleOptionOutput);
bundleCommand.AddOption(bundleOptionLanguages);
bundleCommand.AddOption(bundleOptionNote);
bundleCommand.AddOption(bundleOptionSort);
bundleCommand.AddOption(bundleOptionRemoveEmptyLines);
bundleCommand.AddOption(bundleOptipnCreator);
bundleOptionOutput.AddAlias("-o");
bundleOptionLanguages.AddAlias("-l");
bundleOptionNote.AddAlias("-n");
bundleOptionSort.AddAlias("-s");
bundleOptionRemoveEmptyLines.AddAlias("-r");
bundleOptipnCreator.AddAlias("-c");

bundleCommand.SetHandler((FileInfo output, string[] languages, bool note, string sort,bool removeEmptyLines,string creator) =>
{
    try
    {
        
        var currentDirectory = Environment.CurrentDirectory;

        var files = Directory.GetFiles(currentDirectory, "*", SearchOption.AllDirectories);

        if (!languages.Contains("all", StringComparer.OrdinalIgnoreCase))
        {
            var extensions = languages.Select(lang => "." + lang.ToLower()).ToArray();
            files = files.Where(file =>
                extensions.Contains(Path.GetExtension(file).ToLower()) &&
                !file.Contains("\\bin\\") &&
                !file.Contains("\\obj\\") &&
                !Path.GetFileName(file).StartsWith(".")).ToArray();
        }
        else
        {
            files = files.Where(file =>
                !file.Contains("\\bin\\") &&
                !file.Contains("\\obj\\") &&
                !Path.GetFileName(file).StartsWith(".") &&
                IsCodeFile(file)).ToArray();
        }


        Console.WriteLine($"Sort option: {sort}");

        switch (sort.ToLower())
        {
            case "name":
                files = files.OrderBy(Path.GetFileName).ToArray();
                Console.WriteLine("Files sorted by name.");
                break;

            case "type":
                files = files.OrderBy(Path.GetExtension).ThenBy(Path.GetFileName).ToArray();
                Console.WriteLine("Files sorted by extension.");
                break;

            default:
                Console.WriteLine("Invalid sort option. Use 'name' or 'extension'.");
                throw new ArgumentException("Invalid sort option.");
        }



        using var outputFile = new StreamWriter(output.FullName);
        if (!string.IsNullOrEmpty(creator))
        {
            outputFile.WriteLine($"// Creator: {creator}");
        }
        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            if (removeEmptyLines)
            {
               
                content = string.Join(Environment.NewLine, content.Split(Environment.NewLine)
                    .Where(line => !string.IsNullOrWhiteSpace(line)));
            }
            if (note)
            {
                var relativePath = Path.GetRelativePath(currentDirectory, file);
                outputFile.WriteLine($"// --- Start of file: {Path.GetFileName(file)} ---");
                outputFile.WriteLine($"// Source: {relativePath}");
            }
            outputFile.WriteLine(content);

            if (note)
                outputFile.WriteLine($"// --- End of file: {Path.GetFileName(file)} ---");
            outputFile.WriteLine();
        }

        Console.WriteLine($"Bundle {files.Length} files into {output.FullName}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}, bundleOptionOutput, bundleOptionLanguages, bundleOptionNote, bundleOptionSort,bundleOptionRemoveEmptyLines,bundleOptipnCreator);

var rootCommand = new RootCommand("Root command for file Bundler CLI");
rootCommand.AddCommand(bundleCommand);



bool IsCodeFile(string file)
{
    var validExtensions = new[]
    {
        ".cs", ".js", ".py", ".java", ".cpp", ".c", ".h", ".php", ".rb", ".go", ".ts", ".html", ".css",
        ".json", ".xml", ".swift", ".dart", ".scala", ".vb", ".kotlin", ".r", ".lua", ".sh", ".pl", ".clj",
        ".rs", ".groovy", ".aspx", ".jsp", ".yaml", ".md", ".tcl", ".tex", ".dockerfile"
    };

    return validExtensions.Contains(Path.GetExtension(file).ToLower());
}
////יצירת פקודה create-rsp:
var createRspCommand = new Command("create-rsp", "Create a response file for the bundle command");

createRspCommand.SetHandler(() =>
{
    try
    {
        Console.WriteLine("Welcome to the create-rsp command!");

      
        Console.Write("Enter the output file path and name (e.g., output.txt): ");
        string output = Console.ReadLine();

        Console.Write("Enter the list of languages (comma separated, e.g., cs,js): ");
        string languages = Console.ReadLine();

        Console.Write("Include source file path and name as a comment? (yes/no): ");
        bool note = Console.ReadLine().ToLower() == "yes";

        Console.Write("Sort by name or type? (name/type): ");
        string sort = Console.ReadLine();

        Console.Write("Remove empty lines? (yes/no): ");
        bool removeEmptyLines = Console.ReadLine().ToLower() == "yes";

        Console.Write("Enter the creator's name (optional): ");
        string creator = Console.ReadLine();

        var command = "bundle ";
        if (!string.IsNullOrEmpty(output)) command += $"--output {output} ";
        if (!string.IsNullOrEmpty(languages)) command += $"--languages {languages} ";
        if (note) command += "--note ";
        if (!string.IsNullOrEmpty(sort)) command += $"--sort {sort} ";
        if (removeEmptyLines) command += "--remove-empty-lines ";
        if (!string.IsNullOrEmpty(creator)) command += $"--creator {creator} ";

        string rspFileName = "response.rsp";
        File.WriteAllText(rspFileName, command.Trim());

        Console.WriteLine($"Response file created: {rspFileName}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
});



rootCommand.AddCommand(createRspCommand);

await rootCommand.InvokeAsync(args);
