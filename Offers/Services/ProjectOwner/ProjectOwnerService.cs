using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Offers.Services.ProjectOwner
{
    public class ProjectOwnerService : IProjectOwnerService
    {
        private readonly ApplicationDbContext _context;
        public ProjectOwnerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Models.ProjectOwner>> GetProjectOwnersAsync(string searchString = null)
        {
            var query = _context.ProjectOwners.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchTerm = searchString.ToLower();
                query = query.Where(po => po.Name.ToLower().Contains(searchTerm));
            }
            return await query.OrderBy(po => po.Name).ToListAsync();
        }

        public async Task<Models.ProjectOwner?> GetProjectOwnerByIdAsync(int id)
        {
            return await _context.ProjectOwners.FirstOrDefaultAsync(po => po.Id == id);
        }

        public async Task CreateProjectOwnerAsync(Models.ProjectOwner projectOwner)
        {
            _context.ProjectOwners.Add(projectOwner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProjectOwnerAsync(Models.ProjectOwner projectOwner)
        {
            _context.ProjectOwners.Update(projectOwner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProjectOwnerAsync(int id)
        {
            var owner = await _context.ProjectOwners.FindAsync(id);
            if (owner != null)
            {
                _context.ProjectOwners.Remove(owner);
                await _context.SaveChangesAsync();
            }
        }
    }
}
