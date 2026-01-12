using System.Threading.Tasks;
using Grc.Assessments;
using HotChocolate;

namespace Grc.GraphQL;

/// <summary>
/// GraphQL Mutation root
/// </summary>
public class Mutation
{
    public async Task<AssessmentDto> CreateAssessment(
        CreateAssessmentInput input,
        [Service] IAssessmentAppService appService)
    {
        return await appService.CreateAsync(input);
    }

    public async Task<AssessmentDto> GenerateAssessment(
        GenerateAssessmentInput input,
        [Service] IAssessmentAppService appService)
    {
        return await appService.GenerateAsync(input);
    }

    public async Task<ControlAssessmentDto> AssignControl(
        System.Guid id,
        AssignControlInput input,
        [Service] IControlAssessmentAppService appService)
    {
        return await appService.AssignAsync(id, input);
    }

    public async Task<ControlAssessmentDto> VerifyControl(
        System.Guid id,
        VerifyControlInput input,
        [Service] IControlAssessmentAppService appService)
    {
        return await appService.VerifyAsync(id, input);
    }
}

