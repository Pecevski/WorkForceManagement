using System;
using System.Net.Mail;
using WorkforceManagement.Common;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using WorkforceManagement.Data.Entities.Enums;

namespace WorkforceManagement.Services.Services
{
    public class MailService : IMailService, IValidatableEmail, IDisposable
    {
        private readonly SmtpClient _smtp;

        public MailService()
        {
            _smtp = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = GlobalConstants.MailsDirectory,
                Timeout = 30000,
            };
        }
        public bool SendEmail(User sender, string toAddress, TimeOffRequest requestToBeSend, EmailType type = EmailType.Default)
        {
            if (IsValidUser(sender) == false ||
                IsValidRequest(requestToBeSend) == false ||
                IsValidEmail(toAddress) == false
                )
            {
                return false;
            }

            bool result = true;

            string senderID = sender.Email;

            string subject = "";
            string body = "";
            switch (type)
            {
                case EmailType.Default:
                    subject = "Time off from " + requestToBeSend.StartDate + " to " + requestToBeSend.EndDate;
                    body = requestToBeSend.Reason;
                    break;
                case EmailType.Approved:
                    subject = "[Approved] time off request";
                    body = "Your time off request is approved";
                    break;
                case EmailType.Rejected:
                    subject = "[Rejected] time off request";
                    body = "Your time off request is rejected";
                    break;
                default:
                    break;
            }

            try 
            { 

                MailMessage message = new MailMessage(senderID, toAddress, subject, body);
                _smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            return result;
        }

        public bool IsValidUser(User user)
        {
            if (user == null)
            {
                return false;
            }

            return IsValidEmail(user.Email);
        }

        public bool IsValidRequest(TimeOffRequest request)
        {
            if (request == null)
            {
                return false;
            }

            return request.Reason != null;
        }

        public bool IsValidEmail(string address)
        {
            return new EmailAddressAttribute().IsValid(address);
        }

        public void Dispose()
        {
            _smtp.Dispose();
        }
    }
}