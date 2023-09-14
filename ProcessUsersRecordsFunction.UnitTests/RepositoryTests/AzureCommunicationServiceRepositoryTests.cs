using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;
using Newtonsoft.Json;
using NUnit.Framework;
namespace ProcessUsersRecordsFunction.UnitTests.RepositoryTests
{
    public class AzureCommunicationServiceRepositoryTests
    {
        string MockConnectionString = "Endpoint=sb://mock-servicebus.servicebus.windows.net/;SharedAccessKeyName=mock-key-name;SharedAccessKey=mock-key-value";
        string QueueName = "mock_queue_name";

        [Test]
        public void Constructor_InvalidParamsThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailClient(null));
            Assert.Throws<ArgumentException>(() => new EmailClient(string.Empty));
            Assert.Throws<InvalidOperationException>(() => new EmailClient(" "));
            Assert.Throws<InvalidOperationException>(() => new EmailClient("blablabla"));
        }

        [Test]
        public void SendEmail_InvalidParams_Throws()
        {
            EmailClient emailClient = new EmailClient(MockConnectionString);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await emailClient.SendAsync(WaitUntil.Started, null));

        }
    }
}
