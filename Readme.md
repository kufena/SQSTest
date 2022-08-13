## A simple test of AWS Lambda serving an SQS queue, C#/Dotnet6.

This is a simple test to establish whether SQS servicing works in dotnet6/C#.

The API project declares an SQS queue, and a lambda function (HTTP) that, when you call it using GET, simply creates a guid and puts it on the queue.

The SQS project declares a Dynamo DB, and a lambda that services the SQS queue declared in the API project.  The lambda function will write the guid entry (with its own guid as a key) to
the database, and then attempt to remove the message from the queue, using the SQSClient.

NOTE: this project was initially created because I couldn't get the SQS lambda function to work.  It looked like it was timing out because of the await calls, but in fact, it was because
I hadn't given it a long enough time out.  Also, I couldn't get either function to create any CloudWatch logs, but adding a policy to the Role in both templates fixed that.

To run, there are two serverless.template files, one per project, that depend on each other.  The command to run them is in the Readme.md of the respective projects.
Move to the API project first and create the stack, then once complete, move to the SQS project and create the stack.  The API stack will display the URL to call,
which if you call it in a browser, will add a message to the queue.  You can then use the AWS console to see what is happening.

