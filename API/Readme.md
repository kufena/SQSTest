
To create the stack you can run this:

dotnet lambda deploy-serverless --stack-name SQSTestAPI -sb <upload bucket> --region eu-west-2 --template .\serverless.template

obviously, use your own S3 bucket and region.
