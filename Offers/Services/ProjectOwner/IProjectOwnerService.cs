using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Offers.Services.ProjectOwner
{
    public interface IProjectOwnerService
    {
        Task<IList<Models.ProjectOwner>> GetProjectOwnersAsync(string searchString = null);
        Task<Models.ProjectOwner?> GetProjectOwnerByIdAsync(int id);
        Task CreateProjectOwnerAsync(Models.ProjectOwner projectOwner);
        Task UpdateProjectOwnerAsync(Models.ProjectOwner projectOwner);
        Task DeleteProjectOwnerAsync(int id);
    }
}
