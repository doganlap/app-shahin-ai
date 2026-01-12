using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Assessment.Domain.Issues;
using Grc.ValueObjects;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Grc.Assessment.Domain.Data;

public class IssueDataSeeder : ITransientDependency
{
    private readonly IRepository<Issue, Guid> _issueRepository;

    public IssueDataSeeder(IRepository<Issue, Guid> issueRepository)
    {
        _issueRepository = issueRepository;
    }

    [UnitOfWork]
    public virtual async Task<SeedResult> SeedAsync()
    {
        var result = new SeedResult();
        var existingIssues = (await _issueRepository.GetListAsync()).Select(i => i.IssueCode).ToHashSet();
        var issues = new List<Issue>();

        var issueData = new List<(string Code, string TitleEn, string TitleAr, string DescEn, string DescAr, IssueType Type, IssueSeverity Severity, int DueDays)>
        {
            ("ISS-001", "Outdated SSL/TLS Configuration", "تكوين SSL/TLS قديم", 
             "Web servers are using outdated TLS 1.0 and TLS 1.1 protocols which are vulnerable to attacks",
             "خوادم الويب تستخدم بروتوكولات TLS 1.0 و TLS 1.1 القديمة والمعرضة للهجمات",
             IssueType.Vulnerability, IssueSeverity.High, 30),
             
            ("ISS-002", "Missing Data Retention Policy", "سياسة الاحتفاظ بالبيانات مفقودة",
             "Organization lacks documented data retention and disposal policy required by PDPL",
             "المنظمة تفتقر إلى سياسة موثقة للاحتفاظ بالبيانات والتخلص منها المطلوبة من قبل نظام حماية البيانات",
             IssueType.NonCompliance, IssueSeverity.Medium, 45),
             
            ("ISS-003", "Insufficient Access Control Reviews", "مراجعات التحكم في الوصول غير كافية",
             "User access rights are not reviewed quarterly as required by internal control procedures",
             "حقوق وصول المستخدم لا يتم مراجعتها ربع سنوياً كما هو مطلوب من إجراءات الرقابة الداخلية",
             IssueType.InternalControl, IssueSeverity.Medium, 60),
             
            ("ISS-004", "Unpatched Critical Vulnerabilities", "ثغرات حرجة غير مصححة",
             "15 servers have critical security patches pending installation for over 30 days",
             "15 خادماً لديها تصحيحات أمنية حرجة في انتظار التثبيت لأكثر من 30 يوماً",
             IssueType.Finding, IssueSeverity.Critical, 7),
             
            ("ISS-005", "Business Continuity Plan Not Tested", "خطة استمرارية الأعمال غير مختبرة",
             "Business continuity and disaster recovery plans have not been tested in the past 12 months",
             "خطط استمرارية الأعمال والتعافي من الكوارث لم يتم اختبارها خلال الـ 12 شهراً الماضية",
             IssueType.ProcessGap, IssueSeverity.High, 90),
             
            ("ISS-006", "Weak Password Policy", "ضعف سياسة كلمات المرور",
             "Current password policy does not meet NCA ECC requirements for complexity and rotation",
             "سياسة كلمات المرور الحالية لا تستوفي متطلبات هيئة الأمن السيبراني للتعقيد والتدوير",
             IssueType.NonCompliance, IssueSeverity.High, 30),
             
            ("ISS-007", "Missing Security Awareness Training", "نقص التدريب على التوعية الأمنية",
             "Security awareness training has not been conducted for 40% of employees",
             "لم يتم إجراء تدريب التوعية الأمنية لـ 40٪ من الموظفين",
             IssueType.ProcessGap, IssueSeverity.Medium, 45)
        };

        foreach (var (code, titleEn, titleAr, descEn, descAr, type, severity, dueDays) in issueData)
        {
            if (!existingIssues.Contains(code))
            {
                var issue = new Issue(
                    Guid.NewGuid(),
                    code,
                    new LocalizedString(titleEn, titleAr),
                    type,
                    severity
                );
                issue.SetDescription(new LocalizedString(descEn, descAr));
                issue.SetDueDate(DateTime.UtcNow.AddDays(dueDays));
                issues.Add(issue);
                result.Inserted++;
            }
            else
            {
                result.Skipped++;
            }
            result.Total++;
        }

        if (issues.Any())
        {
            await _issueRepository.InsertManyAsync(issues);
        }
        
        return result;
    }
}
