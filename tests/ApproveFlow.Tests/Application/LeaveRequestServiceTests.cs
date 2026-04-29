using ApproveFlow.Application.Abstractions;
using ApproveFlow.Application.Models;
using ApproveFlow.Application.Services;
using ApproveFlow.Domain.Entities;
using ApproveFlow.Domain.Enums;
using ApproveFlow.Infrastructure.Persistence;

namespace ApproveFlow.Tests.Application;

public sealed class LeaveRequestServiceTests
{
    [Fact]
    public async Task Create_And_Approve_To_Final_Consumes_Balance()
    {
        var fixture = new Fixture();
        await fixture.RouteService.SetAsync(new SetRouteInput("u1", new[]
        {
            new RouteStepInput(1, "a1", "Approver1", "a1@example.com", ""),
            new RouteStepInput(2, "a2", "Approver2", "a2@example.com", ""),
            new RouteStepInput(3, "a3", "Approver3", "a3@example.com", ""),
            new RouteStepInput(4, "a4", "Approver4", "a4@example.com", "")
        }), default);

        var request = await fixture.Service.CreateAsync(new CreateLeaveRequestInput(
            "u1", "User One", "Annual", UnitType.Day,
            new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 1, 7, 45, 0, TimeSpan.Zero),
            "vacation", null, true), default);

        await fixture.Service.ApproveAsync(new ActionInput(request.Id, "a1", null), default);
        await fixture.Service.ApproveAsync(new ActionInput(request.Id, "a2", null), default);
        await fixture.Service.ApproveAsync(new ActionInput(request.Id, "a3", null), default);
        var done = await fixture.Service.ApproveAsync(new ActionInput(request.Id, "a4", null), default);

        Assert.Equal(RequestStatus.Approved, done.Status);
        var balance = await fixture.BalanceService.GetAsync("u1", default);
        Assert.True(balance.UsedDays >= 1.0m);
    }

    [Fact]
    public async Task Create_With_Invalid_15Min_Unit_Throws()
    {
        var fixture = new Fixture();
        await fixture.RouteService.SetAsync(new SetRouteInput("u1", new[]
        {
            new RouteStepInput(1, "a1", "Approver1", "a1@example.com", ""),
            new RouteStepInput(2, "a2", "Approver2", "a2@example.com", ""),
            new RouteStepInput(3, "a3", "Approver3", "a3@example.com", ""),
            new RouteStepInput(4, "a4", "Approver4", "a4@example.com", "")
        }), default);

        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.CreateAsync(new CreateLeaveRequestInput(
            "u1", "User One", "Annual", UnitType.Hour,
            new DateTimeOffset(2026, 4, 1, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 1, 9, 10, 0, TimeSpan.Zero),
            "invalid", null, true), default));
    }

    private sealed class Fixture
    {
        public ILeaveRequestService Service { get; }
        public IApprovalRouteService RouteService { get; }
        public ILeaveBalanceService BalanceService { get; }

        public Fixture()
        {
            var stores = new InMemoryStores();
            var req = new InMemoryLeaveRequestRepository(stores);
            var route = new InMemoryApprovalRouteRepository(stores);
            var balance = new InMemoryLeaveBalanceRepository(stores);
            var uow = new InMemoryUnitOfWork();
            var clock = new FixedClock();

            RouteService = new ApprovalRouteService(route, uow);
            BalanceService = new LeaveBalanceService(balance);
            Service = new LeaveRequestService(req, route, balance, new NoopNotificationService(), uow, clock);
        }
    }

    private sealed class NoopNotificationService : INotificationService
    {
        public Task NotifyAsync(string to, string subject, string body, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class FixedClock : IClock
    {
        public DateTimeOffset UtcNow => new(2026, 4, 1, 0, 0, 0, TimeSpan.Zero);
    }
}
