using MediatR;
using RealEstateApp.Application.ViewModels.Admin;

namespace RealEstateApp.Application.Features.Admin.Queries.GetDashboardStats;

public class GetDashboardStatsQuery : IRequest<DashboardViewModel>
{
}
