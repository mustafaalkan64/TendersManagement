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

        public async Task<IList<Models.EquipmentModel>> GetEquipmentModelsAsync(string searchString, CancellationToken cancellationToken = default)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var isAdmin = user != null && user.IsInRole("Admin");

            var userRoles = user?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value.Trim())
                .ToList() ?? new List<string>();

            var query = await _context.EquipmentModels
                .Include(em => em.Equipment)
                .Include(em => em.CompanyEquipmentModels)
                    .ThenInclude(cem => cem.Company)
                        .ThenInclude(c => c.CompaniesRoles) // Make sure to include this for filtering
                            .ThenInclude(cr => cr.Role)
                .ToListAsync(cancellationToken);

            if (!isAdmin && userRoles.Any())
            {
                 query = query.Where(em => em.CompanyEquipmentModels.Any(cem => 
                    cem.Company.CompaniesRoles.Any(cr => userRoles.Contains(cr.Role.Name.Trim()))
                 )).ToList();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(em =>
                    em.Equipment.Name.Contains(searchString) ||
                    em.Brand.Contains(searchString) ||
                    em.Capacity.Contains(searchString) ||
                    em.CompanyEquipmentModels.Any(cem => cem.Company.Name.Contains(searchString)) ||
                    em.Model.Contains(searchString)).ToList();
            }

            return query
                .OrderBy(em => em.Equipment.Name)
                .ThenBy(em => em.Brand)
                .ThenBy(em => em.Model)
                .ToList();
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
