using System;
using System.Threading.Tasks;

namespace TaskManagement.Application.Interfaces
{
    public interface IUserService
    {
        Task DisableUserAsync(Guid userId);
    }
}
