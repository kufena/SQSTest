using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.DynamoDBv2;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.DynamoDBv2.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SQS;

public class Function
{
    private string QueueUrl;
    private AmazonSQSClient SqsClient;
    private AmazonDynamoDBClient DbClient;
    private string TableName;

    public Function()
    {
        Console.WriteLine("Starting Function");
        try
        {
            var s = Environment.GetEnvironmentVariable("QueueUrl");
            if (s == null)
                QueueUrl = "wibblywobbly"; // throw new Exception("No QueueUrl found in environment.");
            else
                QueueUrl = s;
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.ToString());
            QueueUrl = "WobblyWrongDude";
        }

        try
        {
            var s = Environment.GetEnvironmentVariable("TableName");
            if (s == null)
                TableName = "flimflam"; // throw new Exception("No Table Name found in environment.");
            else
                TableName = s;
        }
        catch (Exception xe)
        {
            //Console.WriteLine(xe.ToString());
            TableName = "FlobaLobbaWrongDude";
        }

        SqsClient = new AmazonSQSClient();
        DbClient = new AmazonDynamoDBClient();
        //Console.WriteLine($"{TableName} {QueueUrl}");
        //Console.WriteLine("Done constructor - now work.");
    }

    /// <summary>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent input, ILambdaContext context)
    {
        context.Logger.LogInformation($"TableName = {TableName} and QueueUrl = {QueueUrl}");

        var allenv = Environment.GetEnvironmentVariables();
        if (allenv == null) {
            context.Logger.LogInformation("Null environment variable list.  so there.");
        }
        else {
            var str = JsonSerializer.Serialize(allenv);
            context.Logger.LogInformation($"Variable {str}");
            
        }

        if (input == null)
        {
            context.Logger.LogInformation("Passed a NULL event - oh no!");
            return;
        }

        if (input.Records == null)
        {
            context.Logger.LogInformation("No Records given by the event.  Woops!");
            return;
        }

        List<DeleteMessageBatchRequestEntry> deletes = new List<DeleteMessageBatchRequestEntry>();

        foreach (var message in input.Records)
        {
            string guid = message.Body;
            context.Logger.LogInformation($"Given guid {guid}");
            var myguid = Guid.NewGuid();

            Dictionary<string, AttributeValue> keyValuePairs = new Dictionary<string, AttributeValue>();
            keyValuePairs.Add("Key", new AttributeValue(myguid.ToString()));
            keyValuePairs.Add("Guid", new AttributeValue(guid.ToString()));

            context.Logger.LogInformation($"Pushing Item to DB with Key {myguid} and value {guid}");
            var dbresp = await DbClient.PutItemAsync(TableName, keyValuePairs);
            context.Logger.LogInformation($"Pushed Item to DB with response http code of {dbresp.HttpStatusCode}");

            if (dbresp.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                context.Logger.LogInformation($"Adding {guid} to messages to delete.");
                deletes.Add(new DeleteMessageBatchRequestEntry(myguid.ToString(), message.ReceiptHandle));
            }
        }

        context.Logger.LogInformation($"Deleting {deletes.Count} messages from the queue.");
        var sqsresp = await SqsClient.DeleteMessageBatchAsync(QueueUrl, deletes);
        context.Logger.LogInformation($"Batch delete returned http code {sqsresp.HttpStatusCode}");
        context.Logger.LogInformation("Done.");
    }
}
