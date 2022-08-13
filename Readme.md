## A simple test of C# Lambda serving an SQS queue.

This is a simple test to establish whether SQS servicing works in dotnet6/C#.

The API project declares an SQS queue, and a lambda function (HTTP) that, when you call it using GET, simply creates a guid and puts it on the queue.

The SQS project declares a Dynamo DB, and a lambda that services the SQS queue declared in the API project.  The lambda function will write the guid entry (with its own guid as a key) to
the database, and then attempt to remove the message from the queue, using the SQSClient.

So far, I haven't been able to make the servicing lambda work, because any instantiations of it fail at any async calls - sometimes when writing to the database, and
sometimes when removing the message from the queue.  I've tried batch removing the messages, and removing them one at a time.  Either way, as we can see from what
goes in to the database, the same messages are being processed again and again.

Any help in establishing what I am doing wrong would be gratefully received.
