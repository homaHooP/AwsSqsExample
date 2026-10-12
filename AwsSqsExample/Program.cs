using Amazon;

using Amazon.SQS;
using Amazon.SQS.Model;
using AwsSqsExample.models;
using System.Text.Json;

DotNetEnv.Env.TraversePath().Load();

var queueUrl = Environment.GetEnvironmentVariable("SQS_QUEUE_URL");
if (string.IsNullOrEmpty(queueUrl))
{
    throw new InvalidOperationException("SQS_QUEUE_URL is missing.");
}

var sqs = new AmazonSQSClient();

async Task SendReq(SupportRequest request)
{
    var json = JsonSerializer.Serialize(request);
    await sqs.SendMessageAsync(queueUrl, json);
}

async Task<List<SupportRequest>> ReceiveReqs()
{
    var requests = new List<SupportRequest>();

    ReceiveMessageResponse resp = new ReceiveMessageResponse();
    int count = 0;
    do
    {
        resp = await sqs.ReceiveMessageAsync(new ReceiveMessageRequest()
        {
            QueueUrl = queueUrl,
            MaxNumberOfMessages = 10,
            WaitTimeSeconds = 5
        });
        if (resp.Messages == null)
        {
            Console.WriteLine("End of messages.");
            break;
        }
        foreach (var message in resp.Messages)
        {
            var receivedOrder = JsonSerializer.Deserialize<SupportRequest>(message.Body);
            requests.Add(receivedOrder);
            await sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
            count++;
        }
    } while (resp.Messages.Count > 0);
    if (count != 0)
    {
        Console.WriteLine($"Total messages received: {count}");
    }
    else
    {
        Console.WriteLine("No messages received.");
    }
    return requests;
}

async Task<int> EstimateReqCount()
{
    var attrs = await sqs.GetQueueAttributesAsync(queueUrl, ["ApproximateNumberOfMessages"]);
    return attrs.ApproximateNumberOfMessages;
}


string choice = "";

while (choice != "4")
{
    Console.WriteLine("Choose an action:");
    Console.WriteLine("1. Send request");
    Console.WriteLine("2. Receive requests");
    Console.WriteLine("3. Estimate requests count");
    Console.WriteLine("4. Exit: ");
    choice = Console.ReadLine();
    switch (choice)
    {
        case "1":
            string UserName = "";
            string Topic = "";
            string Description = "";
            int priority = 0;
            Console.Write("Enter your name: "); UserName = Console.ReadLine();
            Console.Write("Enter the topic: "); Topic = Console.ReadLine();
            Console.Write("Enter the description: "); Description = Console.ReadLine();
            Console.Write("Enter the priority level (0 - low, 1 - medium, 2 - high): "); int.TryParse(Console.ReadLine(), out priority);
            if (priority < 0 || priority > 2)
            {
                Console.WriteLine("Invalid priority level. Setting to low.");
                priority = 0;
            }
            var request = new SupportRequest()
            {
                Id = Guid.NewGuid(),
                UserName = UserName,
                Topic = Topic,
                Description = Description,
                Priority = (PriorityLevel)priority,
                CreatedAt = DateTime.UtcNow
            };
            await SendReq(request);
            Console.WriteLine("Request sent.");
            break;
        case "2":
            var requests = await ReceiveReqs();
            foreach (var req in requests)
            {
                Console.WriteLine($"Received request from user {req.UserName}: Order id: {req.Id}, Topic: {req.Topic}, Description: {req.Description}, Priority: {req.Priority}, Created at: {req.CreatedAt}");
            }
            break;
        case "3":
            int count = await EstimateReqCount();
            Console.WriteLine($"Estimated messages count: {count}");
            break;
        case "4":
            Console.WriteLine("Exiting...");
            break;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }
}