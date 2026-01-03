using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Offers.Services.Company
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CompanyService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<IList<Models.Company>> GetCompaniesAsync(CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var userRoles = user?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value.Trim())
                .ToList() ?? new List<string>();

            var companies = await _context.Companies
                .Include(c => c.CompaniesRoles) // Ensure mapped
                .ThenInclude(cr => cr.Role)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync(cancellationToken);
            // Filter companies where any of the company's assigned roles match any of the user's roles
            return companies
                .Where(c => c.CompaniesRoles.Any(cr => userRoles.Contains(cr.Role.Name.Trim())))
                .ToList();
        }

        public async Task CreateCompanyAsync(Models.Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }
        public async Task<Models.Company?> GetCompanyByIdAsync(int id)
        {
            return await _context.Companies.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            }
        }
        // Add more methods as needed (Edit, Delete, Details, etc.)
    }
}
