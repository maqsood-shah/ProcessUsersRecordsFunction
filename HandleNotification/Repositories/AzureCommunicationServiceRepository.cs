using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using HandleNotification.Model;

namespace HandleNotification.Repositories
{
    public class AzureCommunicationServiceRepository : IAzureCommunicationServiceRepository
    {
        IConfiguration _configuration;
        public AzureCommunicationServiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailNotificatioinAsync(UserRecords userRecord)
        {
            // Send email using Azure Communication Services.

            string? connectionString = _configuration["EmailCommunicationServiceConnectionString"];
            EmailClient emailClient = new EmailClient(connectionString);


            string subject = "Notification from azure servicebustrigger function";
            var htmlContent = "<html>" +
                                    "<body><h3>UserId : " + userRecord.UserId +
                                            "<br/>UserName : " + userRecord.UserName +
                                            "<br/>DataValue : " + userRecord.DataValue + "</h3><br/>" +
                                           "<h4>This email message is sent from Azure Communication Service Email.</h4>" +
                                    "</body>  " +
                               "</html>";

            var sender = _configuration["SenderEmail"];

            var emailRecipients = userRecord.UserEmail;

            try
            {
                EmailSendOperation emailSendOperation = await emailClient.SendAsync(
                    Azure.WaitUntil.Completed,
                    sender,
                    emailRecipients,
                    subject,
                    htmlContent);

                EmailSendResult statusMonitor = emailSendOperation.Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
