## A simple test of C# Lambda serving an SQS queue.

This is a simple test to establish whether SQS servicing works in dotnet6/C#.

The API project declares an SQS queue, and a lambda function (HTTP) that, when you call it using GET, simply creates a guid and puts it on the queue.

The SQS project declares a Dynamo DB, and a lambda that services the SQS queue declared in the API project.  The lambda function will write the guid entry (with its own guid as a key) to
the database, and then attempt to remove the message from the queue, using the SQSClient.

So far, I haven't been able to make the servicing lambda work, because any instantiations of it fail at any async calls - sometimes when writing to the database, and
sometimes when removing the message from the queue.  I've tried batch removing the messages, and removing them one at a time.  Either way, as we can see from what
goes in to the database, the same messages are being processed again and again.

Any help in establishing what I am doing wrong would be gratefully received.

To run, there are two serverless.template files, one per project, that depend on each other.  The command to run them is in the Readme.md of the respective projects.
Move to the API project first and create the stack, then once complete, move to the SQS project and create the stack.  The API stack will display the URL to call,
which if you call it in a browser, will add a message to the queue.  You can then use the AWS console to see what is happening.

So far, I cannot get the SQS servicing lambda to create any CloudWatch logs????

