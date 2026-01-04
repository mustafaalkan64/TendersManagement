using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Models;

namespace Offers.Services.EquipmentModel
{
    public class EquipmentModelService : IEquipmentModelService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EquipmentModelService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedList<Models.EquipmentModel>> GetEquipmentModelsAsync(string searchString, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var isAdmin = user != null && user.IsInRole("Admin");

            var userRoles = user?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value.Trim())
                .ToList() ?? new List<string>();

            var query = _context.EquipmentModels
                .Include(em => em.Equipment)
                .Include(em => em.CompanyEquipmentModels)
                    .ThenInclude(cem => cem.Company)
                        .ThenInclude(c => c.CompaniesRoles)
                            .ThenInclude(cr => cr.Role)
                .AsNoTracking();

            var filteredList = await query.ToListAsync(cancellationToken);

            if (!isAdmin && userRoles.Any())
            {
                 filteredList = filteredList.Where(em => em.CompanyEquipmentModels.Any(cem => 
                    cem.Company.CompaniesRoles.Any(cr => userRoles.Contains(cr.Role.Name.Trim()))
                 )).ToList();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                filteredList = filteredList.Where(em =>
                    em.Equipment.Name.Contains(searchString, System.StringComparison.OrdinalIgnoreCase) ||
                    em.Brand.Contains(searchString, System.StringComparison.OrdinalIgnoreCase) ||
                    em.Capacity.Contains(searchString, System.StringComparison.OrdinalIgnoreCase) ||
                    em.CompanyEquipmentModels.Any(cem => cem.Company.Name.Contains(searchString, System.StringComparison.OrdinalIgnoreCase)) ||
                    em.Model.Contains(searchString, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var orderedList = filteredList
                .OrderBy(em => em.Equipment.Name)
                .ThenBy(em => em.Brand)
                .ThenBy(em => em.Model);

            return PaginatedList<Models.EquipmentModel>.Create(orderedList, pageIndex, pageSize);
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
