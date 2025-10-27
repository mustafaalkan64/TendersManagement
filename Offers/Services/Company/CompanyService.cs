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

        public async Task<IList<Models.Company>> GetCompaniesAsync()
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
                .Select(c => c.Value.ToLower().Trim()) // rolleri lowercase yapýyoruz
                .ToList() ?? new List<string>();

            return await _context.Companies
                .Where(x => roles.Contains(x.Name.ToLower())) // company name de lowercase karþýlaþtýrma
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
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
