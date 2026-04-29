using ApproveFlow.Application.Abstractions;
using ApproveFlow.Application.Services;
using ApproveFlow.Infrastructure.Notifications;
using ApproveFlow.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ApproveFlow.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApproveFlow(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryStores>();

        services.AddScoped<ILeaveRequestRepository, InMemoryLeaveRequestRepository>();
        services.AddScoped<IApprovalRouteRepository, InMemoryApprovalRouteRepository>();
        services.AddScoped<ILeaveBalanceRepository, InMemoryLeaveBalanceRepository>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

        services.AddScoped<INotificationService, LoggingNotificationService>();
        services.AddSingleton<IClock, SystemClock>();

        services.AddScoped<ILeaveRequestService, LeaveRequestService>();
        services.AddScoped<IApprovalRouteService, ApprovalRouteService>();
        services.AddScoped<ILeaveBalanceService, LeaveBalanceService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
