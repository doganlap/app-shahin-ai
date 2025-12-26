using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Assessment.Domain.Teams;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Grc.Assessment.Domain.Data;

public class TeamDataSeeder : ITransientDependency
{
    private readonly IRepository<Team, Guid> _teamRepository;

    public TeamDataSeeder(IRepository<Team, Guid> teamRepository)
    {
        _teamRepository = teamRepository;
    }

    [UnitOfWork]
    public virtual async Task<SeedResult> SeedAsync()
    {
        var result = new SeedResult();
        var existingTeams = (await _teamRepository.GetListAsync()).Select(t => t.Name).ToHashSet();
        var teams = new List<Team>();

        var teamData = new List<(string Name, TeamType Type, string Description)>
        {
            ("Security Assessment Team", TeamType.Security, "Responsible for conducting security assessments and vulnerability management"),
            ("Compliance Assessment Team", TeamType.Compliance, "Handles regulatory compliance assessments and audits"),
            ("Risk Management Team", TeamType.Risk, "Manages enterprise risk assessment and mitigation"),
            ("Internal Audit Team", TeamType.Audit, "Conducts internal audits and control assessments"),
            ("Data Protection Team", TeamType.Privacy, "Ensures data privacy and protection compliance"),
            ("IT Operations Team", TeamType.IT, "Manages IT infrastructure and operations assessments"),
            ("Business Operations Team", TeamType.Operations, "Handles business process assessments")
        };

        foreach (var (name, type, description) in teamData)
        {
            if (!existingTeams.Contains(name))
            {
                var team = new Team(Guid.NewGuid(), name, type, description);
                teams.Add(team);
                result.Inserted++;
            }
            else
            {
                result.Skipped++;
            }
            result.Total++;
        }

        if (teams.Any())
        {
            await _teamRepository.InsertManyAsync(teams);
        }
        
        return result;
    }
}
