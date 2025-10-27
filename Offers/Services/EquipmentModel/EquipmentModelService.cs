using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Offers.Services.EquipmentModel
{
    public class EquipmentModelService : IEquipmentModelService
    {
        private readonly ApplicationDbContext _context;
        public EquipmentModelService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Models.EquipmentModel>> GetEquipmentModelsAsync(string searchString, CancellationToken cancellationToken = default)
        {
            var query = _context.EquipmentModels
                .Include(em => em.Equipment)
                .Include(em => em.CompanyEquipmentModels)
                    .ThenInclude(cem => cem.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(em =>
                    em.Equipment.Name.Contains(searchString) ||
                    em.Brand.Contains(searchString) ||
                    em.Capacity.Contains(searchString) ||
                    em.CompanyEquipmentModels.Any(cem => cem.Company.Name.Contains(searchString)) ||
                    em.Model.Contains(searchString));
            }

            return await query
                .OrderBy(em => em.Equipment.Name)
                .ThenBy(em => em.Brand)
                .ThenBy(em => em.Model)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Models.Company>> GetCompaniesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Companies.OrderBy(c => c.Name).ToListAsync(cancellationToken);
        }

        public async Task AddCompanyAssignmentAsync(int companyId, int equipmentModelId, CancellationToken cancellationToken = default)
        {
            var exists = await _context.CompanyEquipmentModels.AnyAsync(cem => cem.CompanyId == companyId && cem.EquipmentModelId == equipmentModelId, cancellationToken);
            if (!exists)
            {
                _context.CompanyEquipmentModels.Add(new CompanyEquipmentModel
                {
                    CompanyId = companyId,
                    EquipmentModelId = equipmentModelId
                });
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        public async Task RemoveCompanyAssignmentAsync(int companyId, int equipmentModelId, CancellationToken cancellationToken = default)
        {
            var companyEquipmentModel = await _context.CompanyEquipmentModels.FirstOrDefaultAsync(cem => cem.CompanyId == companyId && cem.EquipmentModelId == equipmentModelId, cancellationToken);
            if (companyEquipmentModel != null)
            {
                _context.CompanyEquipmentModels.Remove(companyEquipmentModel);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        // Add more methods as needed (Create, Edit, Delete, etc.)
    }
}
