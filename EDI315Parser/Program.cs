using System;
using System.IO;
using System.Threading.Tasks;
using EDI315Parser.Services;
using EDI315Parser.Models;
using Azure.Messaging.ServiceBus;

namespace EDI315Parser
{
    public class Program
    {
        // Cosmos DB connection details
        private const string EndpointUri = "https://edi-containers.documents.azure.com:443/";
        private const string PrimaryKey = "hJstdxjcKIiBlqWxJ0UMSr2qib5YAxDDnuuV9G8EKwGzekmSz142dhpxz3idI2Te5yNUdsSXa5U2ACDbkj1tuw==";
        private const string DatabaseName = "EDIParserDatabase";
        private const string ContainerName = "EDIParserContainer";

        // Azure Service Bus configuration
        private const string ServiceBusConnectionString = "Endpoint=sb://ahmdtraining.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=VZ5ATwLeF1/6jaD1GJcH/z5nSWfAxaPNa+ASbG/W8Fk=";
        private const string TopicName = "vishant"; // Topic name
        private const string SubscriptionName = "cosmosSub"; // Subscription name

        private static readonly string sourceFolder = "D:/project-docs";  // Folder to watch
        private static readonly string processedFolder = "D:/processed-edi"; // Folder for processed files

        public static async Task Main(string[] args)
        {
            Console.WriteLine("EDI 315 Parser Application Starting...");

            // Initialize Cosmos DB
            await CosmosService.InitializeAsync(EndpointUri, PrimaryKey);

            // Initialize Service Bus Client
            var serviceBusClient = new ServiceBusClient(ServiceBusConnectionString);
            var sender = serviceBusClient.CreateSender(TopicName);

            // Ensure the processed folder exists
            if (!Directory.Exists(processedFolder))
            {
                Directory.CreateDirectory(processedFolder);
            }

            // Get all .txt files in the project-docs folder
            var ediFiles = Directory.GetFiles(sourceFolder, "*.txt");

            if (ediFiles.Length == 0)
            {
                Console.WriteLine("No EDI files found to process.");
                return;
            }

            // Process each file in the folder
            foreach (var file in ediFiles)
            {
                Console.WriteLine($"Processing file: {file}");
                await ProcessEdiFile(file, sender);

                // Move the processed file to the 'processed-edi' folder
                // string processedFilePath = Path.Combine(processedFolder, Path.GetFileName(file));
                // File.Move(file, processedFilePath);
                // Console.WriteLine($"File processed and moved to: {processedFilePath}");
            }

            Console.WriteLine("All files processed and moved to the processed-edi folder.");
        }

        private static async Task ProcessEdiFile(string filePath, ServiceBusSender sender)
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

                            // Send data to Azure Service Bus Topic
                            var message = new ServiceBusMessage
                            {
                                ContentType = "application/json",
                                Body = new BinaryData(new
                                {
                                    ContainerNumber = msgData.b4Segment.ContainerNumber,
                                    TotalDemurrageFees = msgData.TotalDemurrageFees,
                                    OtherPayments = msgData.OtherPayments,
                                    FeeStatus = msgData.FeeStatus
                                })
                            };
                            await sender.SendMessageAsync(message);
                            Console.WriteLine("Message sent to Service Bus.");

                            msgData = null;
                        }
                        break;
                }
            }

            Console.WriteLine("All segments processed and data pushed to Cosmos DB.");
        }
    }
}
