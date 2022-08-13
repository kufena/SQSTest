using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SQS;
using System;
using Amazon.SQS.Model;
using System.Net;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace API;

public class Function
{
    AmazonSQSClient Client;
    string SQSUrl;

    public Function()
    {
        Client = new AmazonSQSClient();
        var s = Environment.GetEnvironmentVariable("SQSUrl");
        if (s != null)
            SQSUrl = s;
        else
            throw new Exception("No SQSUrl environment variable found.");
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<APIGatewayProxyResponse> Get(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var guid = Guid.NewGuid();
        var sqsresponse = await Client.SendMessageAsync(SQSUrl, $"{{\"guid\" = \"{guid.ToString()}\" }}");
        if (sqsresponse.HttpStatusCode != HttpStatusCode.OK)
        {
            var errresponse = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = $"{{\"error\"=\"{sqsresponse.HttpStatusCode}\"}}",
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
            return errresponse;
        }

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = $"{{\"guid\"=\"{guid.ToString()}\"}}",
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
        return response; 
    }
}
