using System;
using WorkforceManagement.Services.Services;
using Xunit;

namespace WorkforceManagement.UnitTests.ServicesTests
{
    public class DateServiceTests
    {
        [Fact]
        public void CheckDate_Valid_ReturnsTrue()
        {
            // arrange
            var sut = new DateService();
            var date = new DateTime(2022, 1, 1);

            // act
            var result = sut.CheckDate(date);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void CheckDate_Invalid_ReturnsFalse()
        {
            // arrange
            var sut = new DateService();

            // act
            var result = sut.CheckDate(DateTime.UtcNow);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void CheckDate_Default_ReturnsIntcountOfWorkDays()
        {
            // arrange
            var sut = new DateService();
            var startDate = new DateTime(2021, 12, 24);
            var endDate = startDate.AddDays(15);

            // act
            var result = sut.CheckforWorkingDays(startDate, endDate);

            // assert
            Assert.Equal(10, result);
        }
    }
}
