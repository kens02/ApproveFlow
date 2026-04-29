using ApproveFlow.Domain.Entities;

namespace ApproveFlow.Tests.Domain;

public sealed class ApprovalRouteTests
{
    [Fact]
    public void Validate_Throws_When_Less_Than_Four_Steps()
    {
        var route = new ApprovalRoute
        {
            ApplicantId = "u1",
            Steps = new List<ApprovalRouteStep>
            {
                new() { StepOrder = 1, ApproverId = "a1", ApproverName = "A1", NotificationEmail = "a1@example.com" }
            }
        };

        Assert.Throws<InvalidOperationException>(() => route.Validate());
    }
}
