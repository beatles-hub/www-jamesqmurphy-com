﻿using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JamesQMurphy.Email.Aws
{
    public class SQSEmailService : IEmailService
    {
        public class Options
        {
            public string QueueUrl { get; set; }
        }

        private readonly Options _options;
        private readonly AmazonSQSClient _sqsClient;
        public SQSEmailService(Options options)
        {
            _options = options;

            _sqsClient = new AmazonSQSClient(new AmazonSQSConfig()
            {
                ServiceURL = "http://sqs.us-east-1.amazonaws.com"
            });

        }

        public async Task<EmailResult> SendEmailAsync(EmailMessage emailMessage)
        {
            var sbQueueMessage = new StringBuilder();
            sbQueueMessage.AppendLine(emailMessage.EmailAddress);
            sbQueueMessage.AppendLine(emailMessage.Subject);
            sbQueueMessage.AppendLine(emailMessage.Body);

            var sendMessageRequest = new SendMessageRequest(_options.QueueUrl, sbQueueMessage.ToString());

            var response = await _sqsClient.SendMessageAsync(sendMessageRequest);
            return new EmailResult()
            {
                Success = true,
                Details = response.MessageId
            };
        }
    }
}
