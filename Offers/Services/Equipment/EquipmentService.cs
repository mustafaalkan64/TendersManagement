using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Offers.Services.Equipment
{
    public class EquipmentService : IEquipmentService
    {
        private readonly ApplicationDbContext _context;
        public EquipmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Models.Equipment>> GetEquipmentListAsync(string searchString, int pageIndex, int pageSize)
        {
            var query = _context.Equipment.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(e => e.Name.Contains(searchString) || (e.Description != null && e.Description.Contains(searchString)));
            }

            query = query.OrderBy(e => e.Name);
            return await PaginatedList<Models.Equipment>.CreateAsync(query.AsNoTracking(), pageIndex, pageSize);
        }

        public async Task<Models.Equipment?> GetEquipmentByIdAsync(int id)
        {
            return await _context.Equipment.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task CreateEquipmentAsync(Models.Equipment equipment)
        {
            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEquipmentAsync(Models.Equipment equipment)
        {
            _context.Equipment.Update(equipment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEquipmentAsync(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment != null)
            {
                _context.Equipment.Remove(equipment);
                await _context.SaveChangesAsync();
            }
        }
        // Add more methods as needed
    }
}
