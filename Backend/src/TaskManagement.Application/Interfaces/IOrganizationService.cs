using System.Threading.Tasks;

namespace TaskManagement.Application.Interfaces
{
    public interface IOrganizationService
    {
        Task ClaimDomainUsersAsync(string organizationId);
    }
}
