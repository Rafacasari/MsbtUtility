using AeonSake.NintendoTools.Compression.Zstd;
using AeonSake.NintendoTools.FileFormats.Msbt;
using AeonSake.NintendoTools.FileFormats.Sarc;
using MsbtUtility;
using System.CommandLine;

internal class Program
{
    public static ZstdDecompressor Decompressor { get; } = new();
    public static ZstdCompressor Compressor { get; } = new();

    public static MsbtFileReader Reader { get; } = new();

    public static MsbtFileWriter Writer { get; } = new();

    public static SarcFileReader SarcFileReader { get; set; } = new SarcFileReader();

    private static int Main(string[] args)
    {
        if (ConfigManager.CurrentConfig != null)
            Reader.TagFormatter = ConfigManager.CurrentConfig.Msbt.TagFormatter;


        // OPTIONS
        var inputOption = new Option<FileInfo>(name: "--input", "-i")
        {
            Required = true,
            Description = "Input file, expects a SARC file with .msbt inside."
        };

        inputOption.Validators.Add(result =>
        {
            var file = result.GetValue(inputOption);

            if (file is null || !file.Exists)
                result.AddError("Input file does not exist.");

            // Validate if it is a SARC file
        });

        var outputOption = new Option<string>(name: "--output", "-o")
        {
            DefaultValueFactory = _ => "output",
            Description = "Output directory where the .json files will be saved.",
        };


        var toJsonCommand = new Command("to-json", "Convert MSBT to JSON")
        {
            inputOption,
            outputOption
        };


        toJsonCommand.SetAction(h =>
        {
            var input = h.GetValue(inputOption);
            var output = h.GetValue(outputOption);

            if (input != null && output != null)
                ConvertToJson(input, output);

            return 0;
        });


        var rootCommand = new RootCommand("Nintendo MSBT CLI Tool")
        {
            toJsonCommand
        };

        return rootCommand.Parse(args).Invoke();
    }

    private static void ConvertToJson(FileInfo input, string output)
    {
        using var fileStream = input.OpenRead();
        using var decompressedStream = new MemoryStream();

        if (Decompressor.CanDecompress(fileStream))
            Decompressor.Decompress(fileStream, decompressedStream);
        else
            fileStream.CopyTo(decompressedStream);

        decompressedStream.Position = 0;

        if (SarcFileReader.CanRead(decompressedStream))
        {
            var sarc = SarcFileReader.Read(decompressedStream);

            var fileName = input.Name;
            var path = Path.Combine(output, fileName);
            Directory.CreateDirectory(output);

            foreach (var file in sarc.Files)
            {
                Console.WriteLine($"Saving {file.Name}");

                using var dataStream = new MemoryStream(file.Data);

                if (!Reader.CanRead(dataStream))
                    continue;

                var msbtFile = Reader.Read(dataStream);


                var outputPath = Path.Combine(path, file.Name);

                var dir = Path.GetDirectoryName(outputPath);
                
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                var (meta, text) = Converter.Serialize(msbtFile);

                var metaPath = outputPath + ".meta.json";
                var textPath = outputPath + ".text.json";

                File.WriteAllText(metaPath, meta);
                File.WriteAllText(textPath, text);
            }
        }    
    }
}