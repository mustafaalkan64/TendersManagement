using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Offers.Services.EquipmentModel
{
    public interface IEquipmentModelService
    {
        Task<IList<Models.EquipmentModel>> GetEquipmentModelsAsync(string searchString, CancellationToken cancellationToken = default);
        Task<List<Models.Company>> GetCompaniesAsync(CancellationToken cancellationToken = default);
        Task AddCompanyAssignmentAsync(int companyId, int equipmentModelId, CancellationToken cancellationToken = default);
        Task RemoveCompanyAssignmentAsync(int companyId, int equipmentModelId, CancellationToken cancellationToken = default);
        // Add more methods as needed (Create, Edit, Delete, etc.)
    }
}
