using System;
using System.IO;
using WorkforceManagement.Common;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Services.Services;
using Xunit;

namespace WorkforceManagement.UnitTests.ServicesTests
{
    public class MailServiceTests
    {
        private MailService _service;

        public MailServiceTests()
        {
            _service = new MailService();
        }

        [Fact]
        public void SendEmail_WithValidInput_ShouldReturnTrue()
        {
            // arrange
            Directory.CreateDirectory(GlobalConstants.MailsDirectory);

            var user = new User()
            {
                Email = "user@demo.com"
            };

            var request = new TimeOffRequest()
            {
                Reason = "Testing"
            };

            // act
            var result = _service.SendEmail(user, "test@demo.com", request);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void SendEmail_WithNullUser_ShouldReturnFalse()
        {
            // arrange
            var request = new TimeOffRequest()
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
                Reason = "Testing"
            };

            // act
            var result = _service.SendEmail(null, "test@demo.com", request);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void SendEmail_WithNullRequest_ShouldReturnFalse()
        {
            // arrange
            var user = new User()
            {
                Email = "user@demo.com"
            };

            // act
            var result = _service.SendEmail(user, "test@demo.com", null);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void SendEmail_WithInvalidAddress_ShouldReturnFalse()
        {
            // arrange
            var user = new User()
            {
                Email = "user@demo.com"
            };

            var request = new TimeOffRequest()
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
                Reason = "Testing"
            };

            // act
            var firstResult = _service.SendEmail(user, "testdemo.com", request);
            var secondResult = _service.SendEmail(user, "testdemocom", request);

            // assert
            Assert.False(firstResult);
            Assert.False(secondResult);
        }

        [Fact]
        public void IsValidEmail_WithValidEmail_ShouldReturnTrue()
        {
            var result = _service.IsValidEmail("test@demo.com");

            Assert.True(result);
        }

        [Fact]
        public void IsValidRequest_WithNullRequest_ShouldReturnFalse()
        {
            var result = _service.IsValidRequest(null);

            Assert.False(result);
        }

        [Fact]
        public void IsValidUser_WithNullUser_ShouldReturnFalse()
        {
            var result = _service.IsValidUser(null);

            Assert.False(result);
        }

        [Fact]
        public void IsValidUser_WithInvalidEmail_ShouldReturnFalse()
        {
            var user = new User()
            {
                Email = "userdemo.com"
            };

            var result = _service.IsValidUser(user);

            Assert.False(result);
        }

        [Fact]
        public void IsValidUser_WithValidEmail_ShouldReturnTrue()
        {
            var user = new User()
            {
                Email = "user@demo.com"
            };

            var result = _service.IsValidUser(user);

            Assert.True(result);
        }

        [Fact]
        public void IsValidRequest_WithNullReason_ShouldReturnFalse()
        {
            var request = new TimeOffRequest();

            var result = _service.IsValidRequest(request);

            Assert.False(result);
        }

        [Fact]
        public void IsValidRequest_WithValidReason_ShouldReturnTrue()
        {
            var request = new TimeOffRequest();
            request.Reason = "Test";

            var result = _service.IsValidRequest(request);

            Assert.True(result);
        }
    }
}