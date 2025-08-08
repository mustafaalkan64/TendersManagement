using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Offers.Services.Company
{
    public interface ICompanyService
    {
        Task<IList<Models.Company>> GetCompaniesAsync();
        Task CreateCompanyAsync(Models.Company company);
        Task<Models.Company?> GetCompanyByIdAsync(int id);
        Task DeleteCompanyAsync(int id);
        // Add more methods as needed (Edit, Delete, Details, etc.)
    }
}
