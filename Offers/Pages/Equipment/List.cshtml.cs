using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;
using Offers.Services.Equipment;
using Offers.Helpers;

[Authorize(Policy = "CanListEquipment")]
public class EquipmentListModel : PageModel
{
    private readonly IEquipmentService _equipmentService;
    public EquipmentListModel(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [BindProperty(SupportsGet = true)]
    public string SearchString { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    public PaginatedList<Equipment> Equipment { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        if (PageIndex < 1) PageIndex = 1;
        Equipment = await _equipmentService.GetEquipmentListAsync(SearchString, PageIndex, 10);
    }
}
