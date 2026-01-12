using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Grc.FrameworkLibrary.Domain.Frameworks;
using Grc.FrameworkLibrary.Domain.Regulators;
using Grc.ValueObjects;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;

namespace Grc.FrameworkLibrary.Application.DataImport;

/// <summary>
/// Service to import framework data from CSV files
/// </summary>
public class FrameworkDataImporter
{
    private readonly IRegulatorRepository _regulatorRepository;
    private readonly IFrameworkRepository _frameworkRepository;
    private readonly IControlRepository _controlRepository;
    private readonly ILogger<FrameworkDataImporter> _logger;

    public FrameworkDataImporter(
        IRegulatorRepository regulatorRepository,
        IFrameworkRepository frameworkRepository,
        IControlRepository controlRepository,
        ILogger<FrameworkDataImporter> logger)
    {
        _regulatorRepository = regulatorRepository;
        _frameworkRepository = frameworkRepository;
        _controlRepository = controlRepository;
        _logger = logger;
    }

    /// <summary>
    /// Import frameworks and controls from CSV files
    /// Expected CSV format:
    /// RegulatorCode,FrameworkCode,FrameworkVersion,ControlNumber,DomainCode,TitleEn,TitleAr,RequirementEn,RequirementAr,ControlType,Priority
    /// </summary>
    public async Task<ImportResult> ImportFromCsvAsync(string csvFilePath)
    {
        var result = new ImportResult();

        try
        {
            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            });

            var records = csv.GetRecords<FrameworkImportRecord>().ToList();

            // Group by regulator and framework
            var frameworkGroups = records.GroupBy(r => new { r.RegulatorCode, r.FrameworkCode, r.FrameworkVersion });

            foreach (var group in frameworkGroups)
            {
                try
                {
                    // Get or create regulator
                    var regulator = await GetOrCreateRegulatorAsync(group.Key.RegulatorCode);
                    
                    // Get or create framework
                    var framework = await GetOrCreateFrameworkAsync(
                        regulator.Id,
                        group.Key.FrameworkCode,
                        group.Key.FrameworkVersion);

                    // Import controls
                    foreach (var record in group)
                    {
                        await ImportControlAsync(framework.Id, record);
                        result.ControlsImported++;
                    }

                    result.FrameworksImported++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error importing framework {FrameworkCode}", group.Key.FrameworkCode);
                    result.Errors.Add($"Framework {group.Key.FrameworkCode}: {ex.Message}");
                }
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing framework data");
            result.Success = false;
            result.Errors.Add($"Import failed: {ex.Message}");
        }

        return result;
    }

    private async Task<Regulator> GetOrCreateRegulatorAsync(string code)
    {
        var existing = await _regulatorRepository.FindAsync(r => r.Code == code);
        if (existing != null) return existing;

        // Create new regulator
        var regulator = new Regulator(
            Guid.NewGuid(),
            code,
            new LocalizedString { En = code, Ar = code },
            Enums.RegulatorCategory.Other);

        await _regulatorRepository.InsertAsync(regulator);
        return regulator;
    }

    private async Task<Framework> GetOrCreateFrameworkAsync(Guid regulatorId, string code, string version)
    {
        var existing = await _frameworkRepository.FindAsync(f => 
            f.RegulatorId == regulatorId && 
            f.Code == code && 
            f.Version == version);
        
        if (existing != null) return existing;

        // Create new framework
        var framework = new Framework(
            Guid.NewGuid(),
            regulatorId,
            code,
            version,
            new LocalizedString { En = code, Ar = code },
            Enums.FrameworkCategory.Other,
            DateTime.UtcNow);

        await _frameworkRepository.InsertAsync(framework);
        return framework;
    }

    private async Task ImportControlAsync(Guid frameworkId, FrameworkImportRecord record)
    {
        var existing = await _controlRepository.FindAsync(c => 
            c.FrameworkId == frameworkId && 
            c.ControlNumber == record.ControlNumber);
        
        if (existing != null) return;

        var control = new Control(
            Guid.NewGuid(),
            frameworkId,
            record.ControlNumber,
            record.DomainCode,
            new LocalizedString { En = record.TitleEn, Ar = record.TitleAr },
            new LocalizedString { En = record.RequirementEn, Ar = record.RequirementAr },
            Enum.Parse<Enums.ControlType>(record.ControlType ?? "Preventive"));

        if (!string.IsNullOrEmpty(record.Priority))
        {
            control.SetPriority(Enum.Parse<Enums.Priority>(record.Priority));
        }

        await _controlRepository.InsertAsync(control);
    }
}

/// <summary>
/// CSV import record
/// </summary>
public class FrameworkImportRecord
{
    public string RegulatorCode { get; set; }
    public string FrameworkCode { get; set; }
    public string FrameworkVersion { get; set; }
    public string ControlNumber { get; set; }
    public string DomainCode { get; set; }
    public string TitleEn { get; set; }
    public string TitleAr { get; set; }
    public string RequirementEn { get; set; }
    public string RequirementAr { get; set; }
    public string ControlType { get; set; }
    public string Priority { get; set; }
}

/// <summary>
/// Import result
/// </summary>
public class ImportResult
{
    public bool Success { get; set; }
    public int FrameworksImported { get; set; }
    public int ControlsImported { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}

