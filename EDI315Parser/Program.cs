using System;
using System.IO;
using System.Threading.Tasks;
using EDI315Parser.Services;
using EDI315Parser.Models;

namespace EDI315Parser
{
    public class Program
    {
        // Cosmos DB connection details
        private const string EndpointUri = "https://edi-containers.documents.azure.com:443/";
        private const string PrimaryKey = "h";
        private const string DatabaseName = "EDIParserDatabase";
        private const string ContainerName = "EDIParserContainer";

        private static readonly string sourceFolder = "D:/project-docs";  
        private static readonly string processedFolder = "D:/processed-edi";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("EDI 315 Parser Application Starting...");

           
            await CosmosService.InitializeAsync(EndpointUri, PrimaryKey);

         
            if (!Directory.Exists(processedFolder))
            {
                Directory.CreateDirectory(processedFolder);
            }

            
            var ediFiles = Directory.GetFiles(sourceFolder, "*.txt");

            if (ediFiles.Length == 0)
            {
                Console.WriteLine("No EDI files found to process.");
                return;
            }

            foreach (var file in ediFiles)
            {
                Console.WriteLine($"Processing file: {file}");
                await ProcessEdiFile(file);

                string processedFilePath = Path.Combine(processedFolder, Path.GetFileName(file));
                File.Move(file, processedFilePath);
                Console.WriteLine($"File processed and moved to: {processedFilePath}");
            }
        }

        private static async Task ProcessEdiFile(string filePath)
        {
            string[] fileData = await File.ReadAllLinesAsync(filePath);
            MsgData msgData = null;
            R4 r4 = new R4();

            foreach (string line in fileData)
            {
                string[] lineData = line.Split('*');

                switch (lineData[0])
                {
                    case "ST":
                        msgData = new MsgData();
                        msgData.stSegment = SegmentParserService.ParseSTSegment(lineData);
                        break;

                    case "B4":
                        if (msgData != null) msgData.b4Segment = SegmentParserService.ParseB4Segment(lineData);
                        break;

                    case "N9":
                        if (msgData != null)
                        {
                            var n9Segment = SegmentParserService.ParseN9Segment(lineData, msgData);
                            msgData.n9Segments.Add(n9Segment);
                        }
                        break;

                    case "Q2":
                        if (msgData != null) msgData.q2Segment = SegmentParserService.ParseQ2Segment(lineData);
                        break;

                    case "SG":
                        if (msgData != null) msgData.sgSegment = SegmentParserService.ParseSGSegment(lineData);
                        break;

                    case "R4":
                        if (msgData != null)
                        {
                            msgData.r4Segment = SegmentParserService.ParseR4Segment(lineData, r4);
                        }
                        break;

                    case "SE":
                        if (msgData != null)
                        {
                            msgData.seSegment = SegmentParserService.ParseSESegment(lineData);
                            await CosmosService.PushDataToCosmos(msgData);

                            msgData = null;
                        }
                        break;
                }
            }

            Console.WriteLine("All segments processed and data pushed to Cosmos DB.");
        }
    }
}
