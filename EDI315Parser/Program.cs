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
        private const string EndpointUri = "https://localhost:8081";
        private const string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private const string DatabaseName = "EDIParserDatabase";
        private const string ContainerName = "EDIParserContainer";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("EDI 315 Parser Application Starting...");

            // Initialize Cosmos DB
            await CosmosService.InitializeAsync(EndpointUri, PrimaryKey);

            // Specify the EDI file path
            var ediFilePath = "D:/project-docs/Sample.txt";

            if (!File.Exists(ediFilePath))
            {
                Console.WriteLine("EDI file not found. Please check the file path.");
                return;
            }

            // Read and parse the EDI file
            await ProcessEdiFile(ediFilePath);

            Console.WriteLine("EDI 315 Parser Application Completed.");
        }

        private static async Task ProcessEdiFile(string filePath)
        {
            string[] fileData = await File.ReadAllLinesAsync(filePath);
            MsgData msgData = null;

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
                        if (msgData != null) msgData.n9Segment = SegmentParserService.ParseN9Segment(lineData);
                        break;

                    case "Q2":
                        if (msgData != null) msgData.q2Segment = SegmentParserService.ParseQ2Segment(lineData);
                        break;

                    case "SG":
                        if (msgData != null) msgData.sgSegment = SegmentParserService.ParseSGSegment(lineData);
                        break;

                    case "R4":
                        if (msgData != null) msgData.r4Segment = SegmentParserService.ParseR4Segment(lineData);
                        break;

                    case "SE":
                        if (msgData != null)
                        {
                            msgData.seSegment = SegmentParserService.ParseSESegment(lineData);
                            await CosmosService.PushDataToCosmos(msgData); // Push the complete MsgData object to Cosmos DB
                            msgData = null;
                        }
                        break;

                    case "GE":
                        if (msgData != null) msgData.geSegment = SegmentParserService.ParseGESegment(lineData);
                        break;

                    case "IEA":
                        if (msgData != null) msgData.ieaSegment = SegmentParserService.ParseIEASegment(lineData);
                        break;
                }
            }

            Console.WriteLine("All segments processed and data pushed to Cosmos DB.");
        }

        private static async Task SaveSegmentToDatabase<T>(string segmentType, T segment)
        {
            try
            {
                var msgData = new MsgData
                {
                    id = Guid.NewGuid().ToString(),
                    SegmentType = segmentType,
                    SegmentData = segment
                };

                await CosmosService.PushDataToCosmos(msgData);  // Push MsgData to Cosmos DB
                Console.WriteLine($"Segment {segmentType} saved to database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving segment to database: {ex.Message}");
            }
        }
    }
}
