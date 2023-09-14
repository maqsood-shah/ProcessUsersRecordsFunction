# ProcessUsersRecordsFunction
Sending Messages with Time Triggered Azure Function to Azure Service Bus and Processing Messages from Azure Service Bus using Service Bus Triggered Azure Functions and sending notification to users using Azure communication Service in .NET 6

This documentation will guide you to setup this project in .NET6. The Azure Functions has been developed using .net isolated environment.

## Prerequisites

Before you begin, make sure you have the following:

Azure subscription for creating Azure resources.
Azure CLI or Azure Portal for resource management.
Visual Studio 2022 or Visual Studio Code for developing Azure Functions.
.NET 6 SDK for building and running .NET 6 functions.
Sql server 19.1 to create a database locally alternatively you can also use azure sql managed instance and azure sql database
Microsoft.NET.Sdk.Functions 4.x If you are not using Visual Studio 2022 (Microsoft.NET.Sdk.Functions 4.x)

## Step 1: Create Required Azure Resources
+ Sign in to the Azure portal
+ Create Azure Service Bus Namespace and Queues 
+ Follow the steps on microsoft website [Service Bus Messaging ](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quickstart-portal)
+ Create Azure Email Communication Service and Azure Communication Service and setup subdomain and connect to send email notification.
+ Follow the steps on microsoft website [Azure Email Communication Service](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/create-email-communication-resource)
+ [Connect to Email Domain with Azure Communication Service](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/connect-email-communication-resource?pivots=azure-portal)


## Step 2: Configure Environment Variables
Before you proceed, configure the following environment variables on your local development machine. These variables will be used to 

+ set values in the local.settings.json file for **SendNotification**:

Windows:

set AzureWebJobsServiceBus "your-service-bus-connection-string-from-azure-portal"
set QueueName "process-user-records-from-azure-portal"
set SqlServerConnectionString "sqlserver-connection-string"

+ Modify local.settings.json
Open your local.settings.json file in **SendNotification** Azure Functions project and configure it to read values from environment variables. Here's an example:

json
Copy code
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "SqlServerConnectionString": "%SqlServerConnectionString%",
    "AzureWebJobsServiceBus": "%AzureWebJobsServiceBus%",
    "ServiceBusQueueName": "%ServiceBusQueueName%"
  }
}
The %AzureWebJobsServiceBus% , %ServiceBusQueueName% and %SqlServerConnectionString% placeholders will be replaced with the values from the environment variables at runtime.

+ set values in the local.settings.json file for **HandleNotification**:

Windows:

set EmailCommunicationServiceConnectionString "your-email-communication-service-connection-string-from-azure-portal"
set SenderEmail "sender-email-from-email-communication-service-azure"
set SqlServerConnectionString "sqlserver-connection-string"

+ Modify local.settings.json
Open your local.settings.json file in **HandleNotification** Azure Functions project and configure it to read values from environment variables. Here's an example:

json
Copy code
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsServiceBus": "%AzureWebJobsServiceBus%",
    "ServiceBusQueueName": "%ServiceBusQueueName%",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "EmailCommunicationServiceConnectionString": "%EmailCommunicationServiceConnectionString%",
    "SenderEmail": "%SenderEmail%"
  }
}

+ Sometime it is required to restart the machine after setting the environment variable.

# Step 3: Open an Azure Function Projects solution
Open Visual Studio 2022 or Visual Studio Code.
Open Azure Functions project using the Azure Functions template for .NET 6.
In your project solution, restore the required NuGet packages or just build the .net solution

+ The **SendNotification** Azure Functions created to get the records from Sql database using stored procedure and to send a message to the Service Bus. This is atime triggered azure functin which runs at every 15 minutes of interval:

+ The **HandleNotification** Azure Functions created to  to process messages from azure service bus queue and send 
notification to user.

## Step 4: Run the Sql Migration Script or dotnet ef command to sql create sql tables and stored procedure.

*Using dotnet ef command*
+ In visual studio open Package Manager Console change directory to the **SendNotification** Azure Functions project directory
for example cd Path_To_The_Project_Direcory\SendNotification\
+ Assuming you already have sql database ready and created and have provided the connectionstring in environment variable run the below command
+ Check if you already have dotnet tool installed to run this command otherwise run and intall using: dotnet tool install
+ Then run the command: dotnet ef database update
+ It will create all the required tables and stored procedure in the database.

*By running Sql Script in query editor*
+ Alternatively you can also create sql tables and stored procedure in the database by running the sql the script inside '20230904115735_SqlServer.cs' file in Migration folder of **SendNotification** Azure Functions project. You can also find the stored procedure to return records inside this script.

# Make sure to execute the stored procedure named 'dbo.spGetRecentUsersRecords' once before starting to run or debug the functions.
+ The stored procedure is inside the script : 20230905183940_SqlScript.cs under Migrations folder of SendNotificaton function project.
+ Once you execute the stored procedure manually it will insert the current last execution time ion the ExecutionLog table and based on this execution time the the next recrdset is returned from database
+ To test the functionality it is also required to insert some dummy records in 'Record' table with the creation time.

That's it! You've successfully set up Azure Functions project and it is ready to run locally (development environment).

You can deploy these Azure Functions project to Azure. You can use Visual Studio's Publish feature or Azure Functions CLI for this.

