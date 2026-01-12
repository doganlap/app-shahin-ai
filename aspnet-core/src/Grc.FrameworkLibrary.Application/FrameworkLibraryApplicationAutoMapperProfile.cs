using AutoMapper;
using Grc.FrameworkLibrary.Application.Contracts.Frameworks;
using Grc.FrameworkLibrary.Application.Contracts.Regulators;
using Grc.FrameworkLibrary.Domain.Frameworks;
using Grc.FrameworkLibrary.Domain.Regulators;

namespace Grc.FrameworkLibrary.Application;

public class FrameworkLibraryApplicationAutoMapperProfile : Profile
{
    public FrameworkLibraryApplicationAutoMapperProfile()
    {
        // Regulator mappings
        CreateMap<Regulator, RegulatorDto>();
        CreateMap<CreateUpdateRegulatorDto, Regulator>();

        // Framework mappings
        CreateMap<Framework, FrameworkDto>();

        // Control mappings
        CreateMap<Control, ControlDto>();
    }
}

