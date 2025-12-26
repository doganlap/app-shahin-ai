using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Enums;
using Grc.FrameworkLibrary.Application.Contracts.Regulators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Grc.Web.Pages.Regulators;

public class CreateModalModel : GrcPageModel
{
    [BindProperty]
    public CreateUpdateRegulatorDto Regulator { get; set; }

    public List<SelectListItem> CategoryList { get; set; }

    private readonly IRegulatorAppService _regulatorAppService;

    public CreateModalModel(IRegulatorAppService regulatorAppService)
    {
        _regulatorAppService = regulatorAppService;
    }

    public void OnGet()
    {
        Regulator = new CreateUpdateRegulatorDto();
        CategoryList = System.Enum.GetValues(typeof(RegulatorCategory))
            .Cast<RegulatorCategory>()
            .Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = L[$"Enum:RegulatorCategory:{(int)e}"].Value
            }).ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _regulatorAppService.CreateAsync(Regulator);
        return NoContent();
    }
}

