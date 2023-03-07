using FluentAssertions;
using PhonePresenceBot.Core.PhonePresence;

namespace PhonePresenceBot.Tests;

public class PhonePresenceRequestTests
{
    public static IEnumerable<object[]> ValidRequests = new[]
    {
        new object[] { new PhonePresenceRequest() { PhoneName = "df" }, true },
        new object[] { new PhonePresenceRequest() { PhoneMac = "df" }, true },

        new object[] { new PhonePresenceRequest(), false },
        new object[] { new PhonePresenceRequest() { PhoneName = "df", PhoneMac = "df" }, false }
    };

    [Theory]
    [MemberData(nameof(ValidRequests))]
    public void PhonePresenceRequest_IsValidEqualToExpected(PhonePresenceRequest request, bool isValidExpected)
    {
        // Arrange

        // Act
        var isValid = request.IsValid();

        // Assert
        isValid.Should().Be(isValidExpected);
    }
}