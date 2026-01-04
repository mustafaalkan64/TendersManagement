using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Offers.Services.Equipment
{
    public interface IEquipmentService
    {
        Task<PaginatedList<Models.Equipment>> GetEquipmentListAsync(string searchString, int pageIndex, int pageSize);
        Task<Models.Equipment?> GetEquipmentByIdAsync(int id);
        Task CreateEquipmentAsync(Models.Equipment equipment);
        Task UpdateEquipmentAsync(Models.Equipment equipment);
        Task DeleteEquipmentAsync(int id);
        // Add more methods as needed (search, assign, etc.)
    }
}
