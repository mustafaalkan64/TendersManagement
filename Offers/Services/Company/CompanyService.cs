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
            if(user.IsInRole("Admin"))
            {
                return await _context.Companies
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            }

            var roles = user?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList() ?? new List<string>();

            var companies = await _context.Companies.ToListAsync(cancellationToken);

            return companies.Where(x => roles.Any(r => r.ToLower().Contains(x.Name.ToLower())))
                .OrderByDescending(x => x.CreatedDate).ToList();
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
