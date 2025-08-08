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

        public async Task<IList<Models.Equipment>> GetEquipmentListAsync()
        {
            return await _context.Equipment
                .OrderBy(e => e.Name)
                .ToListAsync();
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
