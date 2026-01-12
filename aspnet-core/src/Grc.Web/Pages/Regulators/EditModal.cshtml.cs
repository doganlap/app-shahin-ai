using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Enums;
using Grc.FrameworkLibrary.Application.Contracts.Regulators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Grc.Web.Pages.Regulators;

public class EditModalModel : GrcPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateRegulatorDto Regulator { get; set; }

    public List<SelectListItem> CategoryList { get; set; }

    private readonly IRegulatorAppService _regulatorAppService;

    public EditModalModel(IRegulatorAppService regulatorAppService)
    {
        _regulatorAppService = regulatorAppService;
    }

    public async Task OnGetAsync()
    {
        var regulatorDto = await _regulatorAppService.GetAsync(Id);
        Regulator = ObjectMapper.Map<RegulatorDto, CreateUpdateRegulatorDto>(regulatorDto);

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
        await _regulatorAppService.UpdateAsync(Id, Regulator);
        return NoContent();
    }
}

