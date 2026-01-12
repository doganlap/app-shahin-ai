using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.ValueObjects;
using Grc.Workflow;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Grc.Data;

public class WorkflowDefinitionSeeder : ITransientDependency
{
    private readonly IRepository<WorkflowDefinition, Guid> _workflowDefinitionRepository;
    private readonly IDataSeeder _dataSeeder;

    public WorkflowDefinitionSeeder(
        IRepository<WorkflowDefinition, Guid> workflowDefinitionRepository,
        IDataSeeder dataSeeder)
    {
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _dataSeeder = dataSeeder;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _workflowDefinitionRepository.GetCountAsync() > 0)
        {
            return;
        }

        var workflows = new List<WorkflowDefinition>
        {
            // NCA Essential Cybersecurity Controls Assessment
            new WorkflowDefinition(
                Guid.NewGuid(),
                new LocalizedString("NCA Essential Cybersecurity Controls Assessment", "تقييم ضوابط الأمن السيبراني الأساسية - هيئة الاتصالات"),
                new LocalizedString(
                    "Comprehensive assessment workflow for NCA Essential Cybersecurity Controls (ECC) framework compliance. Includes gap analysis, risk evaluation, and remediation planning for Saudi telecommunications and IT service providers.",
                    "سير عمل شامل لتقييم الامتثال لإطار ضوابط الأمن السيبراني الأساسية لهيئة الاتصالات. يشمل تحليل الثغرات وتقييم المخاطر وتخطيط المعالجة لمقدمي خدمات الاتصالات وتقنية المعلومات السعوديين."
                ),
                "v2.1",
                @"<?xml version='1.0' encoding='UTF-8'?>
<definitions xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' 
             xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
             id='nca-ecc-assessment'>
  <process id='ncaAssessment' name='NCA ECC Assessment' isExecutable='true'>
    <startEvent id='start' name='بدء التقييم/Start Assessment'/>
    <userTask id='scopeDefinition' name='تحديد النطاق/Define Scope'/>
    <userTask id='controlAssessment' name='تقييم الضوابط/Assess Controls'/>
    <userTask id='gapAnalysis' name='تحليل الثغرات/Gap Analysis'/>
    <userTask id='riskEvaluation' name='تقييم المخاطر/Risk Evaluation'/>
    <userTask id='remediation' name='خطة المعالجة/Remediation Plan'/>
    <userTask id='report' name='تقرير الامتثال/Compliance Report'/>
    <endEvent id='end' name='اكتمال التقييم/Assessment Complete'/>
    <sequenceFlow id='flow1' sourceRef='start' targetRef='scopeDefinition'/>
    <sequenceFlow id='flow2' sourceRef='scopeDefinition' targetRef='controlAssessment'/>
    <sequenceFlow id='flow3' sourceRef='controlAssessment' targetRef='gapAnalysis'/>
    <sequenceFlow id='flow4' sourceRef='gapAnalysis' targetRef='riskEvaluation'/>
    <sequenceFlow id='flow5' sourceRef='riskEvaluation' targetRef='remediation'/>
    <sequenceFlow id='flow6' sourceRef='remediation' targetRef='report'/>
    <sequenceFlow id='flow7' sourceRef='report' targetRef='end'/>
  </process>
</definitions>",
                "Compliance"
            ),

            // SAMA Cybersecurity Framework
            new WorkflowDefinition(
                Guid.NewGuid(),
                new LocalizedString("SAMA Cybersecurity Framework Assessment", "تقييم إطار الأمن السيبراني لمؤسسة النقد العربي السعودي"),
                new LocalizedString(
                    "Banking sector cybersecurity compliance assessment based on SAMA Cybersecurity Framework. Covers governance, risk management, incident response, and operational resilience for financial institutions.",
                    "تقييم امتثال الأمن السيبراني للقطاع المصرفي بناءً على إطار الأمن السيبراني لمؤسسة النقد العربي السعودي. يغطي الحوكمة وإدارة المخاطر والاستجابة للحوادث والمرونة التشغيلية للمؤسسات المالية."
                ),
                "v3.0",
                @"<?xml version='1.0' encoding='UTF-8'?>
<definitions xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' id='sama-csf-assessment'>
  <process id='samaAssessment' name='SAMA CSF Assessment' isExecutable='true'>
    <startEvent id='start' name='بدء تقييم الأمن السيبراني/Start Cyber Assessment'/>
    <userTask id='governance' name='تقييم الحوكمة/Governance Assessment'/>
    <userTask id='riskMgmt' name='إدارة المخاطر/Risk Management'/>
    <userTask id='incidentResponse' name='الاستجابة للحوادث/Incident Response'/>
    <userTask id='resilience' name='المرونة التشغيلية/Operational Resilience'/>
    <userTask id='compliance' name='تقرير الامتثال/Compliance Reporting'/>
    <endEvent id='end' name='اكتمال التقييم/Assessment Complete'/>
    <sequenceFlow id='flow1' sourceRef='start' targetRef='governance'/>
    <sequenceFlow id='flow2' sourceRef='governance' targetRef='riskMgmt'/>
    <sequenceFlow id='flow3' sourceRef='riskMgmt' targetRef='incidentResponse'/>
    <sequenceFlow id='flow4' sourceRef='incidentResponse' targetRef='resilience'/>
    <sequenceFlow id='flow5' sourceRef='resilience' targetRef='compliance'/>
    <sequenceFlow id='flow6' sourceRef='compliance' targetRef='end'/>
  </process>
</definitions>",
                "Banking"
            ),

            // PDPL Privacy Impact Assessment
            new WorkflowDefinition(
                Guid.NewGuid(),
                new LocalizedString("Personal Data Protection Law (PDPL) Privacy Impact Assessment", "تقييم أثر الخصوصية وفقاً لنظام حماية البيانات الشخصية"),
                new LocalizedString(
                    "Comprehensive privacy impact assessment workflow for PDPL compliance. Evaluates data processing activities, identifies privacy risks, and ensures compliance with Saudi Arabia's Personal Data Protection Law.",
                    "سير عمل شامل لتقييم أثر الخصوصية للامتثال لنظام حماية البيانات الشخصية. يقيم أنشطة معالجة البيانات ويحدد مخاطر الخصوصية ويضمن الامتثال لنظام حماية البيانات الشخصية في المملكة العربية السعودية."
                ),
                "v1.3",
                @"<?xml version='1.0' encoding='UTF-8'?>
<definitions xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' id='pdpl-pia-workflow'>
  <process id='pdplAssessment' name='PDPL Privacy Impact Assessment' isExecutable='true'>
    <startEvent id='start' name='بدء تقييم الخصوصية/Start Privacy Assessment'/>
    <userTask id='dataMapping' name='رسم خريطة البيانات/Data Mapping'/>
    <userTask id='legalBasis' name='الأساس القانوني/Legal Basis'/>
    <userTask id='riskAssessment' name='تقييم مخاطر الخصوصية/Privacy Risk Assessment'/>
    <userTask id='safeguards' name='الضمانات الوقائية/Safeguards'/>
    <userTask id='consentMgmt' name='إدارة الموافقة/Consent Management'/>
    <userTask id='rightsManagement' name='إدارة حقوق الأفراد/Rights Management'/>
    <userTask id='documentation' name='التوثيق والسجلات/Documentation'/>
    <endEvent id='end' name='اكتمال تقييم الخصوصية/Privacy Assessment Complete'/>
    <sequenceFlow id='flow1' sourceRef='start' targetRef='dataMapping'/>
    <sequenceFlow id='flow2' sourceRef='dataMapping' targetRef='legalBasis'/>
    <sequenceFlow id='flow3' sourceRef='legalBasis' targetRef='riskAssessment'/>
    <sequenceFlow id='flow4' sourceRef='riskAssessment' targetRef='safeguards'/>
    <sequenceFlow id='flow5' sourceRef='safeguards' targetRef='consentMgmt'/>
    <sequenceFlow id='flow6' sourceRef='consentMgmt' targetRef='rightsManagement'/>
    <sequenceFlow id='flow7' sourceRef='rightsManagement' targetRef='documentation'/>
    <sequenceFlow id='flow8' sourceRef='documentation' targetRef='end'/>
  </process>
</definitions>",
                "Privacy"
            ),

            // Risk Management Assessment
            new WorkflowDefinition(
                Guid.NewGuid(),
                new LocalizedString("Enterprise Risk Management Assessment", "تقييم إدارة المخاطر المؤسسية"),
                new LocalizedString(
                    "Comprehensive enterprise risk management assessment workflow following ISO 31000 standards. Includes risk identification, analysis, evaluation, treatment, and continuous monitoring.",
                    "سير عمل شامل لتقييم إدارة المخاطر المؤسسية وفقاً لمعايير أيزو 31000. يشمل تحديد المخاطر والتحليل والتقييم والمعالجة والمراقبة المستمرة."
                ),
                "v2.0",
                @"<?xml version='1.0' encoding='UTF-8'?>
<definitions xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' id='risk-management-assessment'>
  <process id='riskAssessment' name='Risk Management Assessment' isExecutable='true'>
    <startEvent id='start' name='بدء تقييم المخاطر/Start Risk Assessment'/>
    <userTask id='identification' name='تحديد المخاطر/Risk Identification'/>
    <userTask id='analysis' name='تحليل المخاطر/Risk Analysis'/>
    <userTask id='evaluation' name='تقييم المخاطر/Risk Evaluation'/>
    <userTask id='treatment' name='معالجة المخاطر/Risk Treatment'/>
    <userTask id='monitoring' name='المراقبة والمتابعة/Monitoring & Review'/>
    <endEvent id='end' name='اكتمال تقييم المخاطر/Risk Assessment Complete'/>
    <sequenceFlow id='flow1' sourceRef='start' targetRef='identification'/>
    <sequenceFlow id='flow2' sourceRef='identification' targetRef='analysis'/>
    <sequenceFlow id='flow3' sourceRef='analysis' targetRef='evaluation'/>
    <sequenceFlow id='flow4' sourceRef='evaluation' targetRef='treatment'/>
    <sequenceFlow id='flow5' sourceRef='treatment' targetRef='monitoring'/>
    <sequenceFlow id='flow6' sourceRef='monitoring' targetRef='end'/>
  </process>
</definitions>",
                "Risk"
            ),

            // CITC Telecommunications Compliance
            new WorkflowDefinition(
                Guid.NewGuid(),
                new LocalizedString("CITC Telecommunications Regulatory Compliance", "الامتثال التنظيمي لقطاع الاتصالات - هيئة الاتصالات"),
                new LocalizedString(
                    "Telecommunications regulatory compliance assessment for CITC requirements. Covers service quality, network security, consumer protection, and regulatory reporting obligations.",
                    "تقييم الامتثال التنظيمي لقطاع الاتصالات لمتطلبات هيئة الاتصالات. يغطي جودة الخدمة وأمن الشبكة وحماية المستهلك والتزامات التقارير التنظيمية."
                ),
                "v1.5",
                @"<?xml version='1.0' encoding='UTF-8'?>
<definitions xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' id='citc-compliance'>
  <process id='citcCompliance' name='CITC Compliance Assessment' isExecutable='true'>
    <startEvent id='start' name='بدء تقييم الامتثال/Start Compliance Assessment'/>
    <userTask id='serviceQuality' name='تقييم جودة الخدمة/Service Quality Assessment'/>
    <userTask id='networkSecurity' name='أمن الشبكة/Network Security'/>
    <userTask id='consumerProtection' name='حماية المستهلك/Consumer Protection'/>
    <userTask id='regulatoryReporting' name='التقارير التنظيمية/Regulatory Reporting'/>
    <endEvent id='end' name='اكتمال التقييم/Compliance Complete'/>
    <sequenceFlow id='flow1' sourceRef='start' targetRef='serviceQuality'/>
    <sequenceFlow id='flow2' sourceRef='serviceQuality' targetRef='networkSecurity'/>
    <sequenceFlow id='flow3' sourceRef='networkSecurity' targetRef='consumerProtection'/>
    <sequenceFlow id='flow4' sourceRef='consumerProtection' targetRef='regulatoryReporting'/>
    <sequenceFlow id='flow5' sourceRef='regulatoryReporting' targetRef='end'/>
  </process>
</definitions>",
                "Telecommunications"
            ),

            // NDMO Data Management
            new WorkflowDefinition(
                Guid.NewGuid(),
                new LocalizedString("National Data Management Office (NDMO) Data Governance Assessment", "تقييم حوكمة البيانات - المكتب الوطني لإدارة البيانات"),
                new LocalizedString(
                    "Comprehensive data governance assessment aligned with NDMO guidelines and Saudi Arabia's National Data Management Framework. Covers data quality, security, privacy, and lifecycle management.",
                    "تقييم شامل لحوكمة البيانات متماشياً مع إرشادات المكتب الوطني لإدارة البيانات وإطار إدارة البيانات الوطني في المملكة العربية السعودية. يغطي جودة البيانات والأمن والخصوصية وإدارة دورة الحياة."
                ),
                "v1.2",
                @"<?xml version='1.0' encoding='UTF-8'?>
<definitions xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' id='ndmo-data-governance'>
  <process id='ndmoAssessment' name='NDMO Data Governance Assessment' isExecutable='true'>
    <startEvent id='start' name='بدء تقييم حوكمة البيانات/Start Data Governance Assessment'/>
    <userTask id='dataInventory' name='جرد الأصول البياناتية/Data Asset Inventory'/>
    <userTask id='dataQuality' name='تقييم جودة البيانات/Data Quality Assessment'/>
    <userTask id='dataSecurity' name='أمن البيانات/Data Security'/>
    <userTask id='dataPrivacy' name='خصوصية البيانات/Data Privacy'/>
    <userTask id='dataLifecycle' name='إدارة دورة حياة البيانات/Data Lifecycle Management'/>
    <userTask id='dataSharing' name='مشاركة البيانات/Data Sharing'/>
    <endEvent id='end' name='اكتمال تقييم الحوكمة/Governance Assessment Complete'/>
    <sequenceFlow id='flow1' sourceRef='start' targetRef='dataInventory'/>
    <sequenceFlow id='flow2' sourceRef='dataInventory' targetRef='dataQuality'/>
    <sequenceFlow id='flow3' sourceRef='dataQuality' targetRef='dataSecurity'/>
    <sequenceFlow id='flow4' sourceRef='dataSecurity' targetRef='dataPrivacy'/>
    <sequenceFlow id='flow5' sourceRef='dataPrivacy' targetRef='dataLifecycle'/>
    <sequenceFlow id='flow6' sourceRef='dataLifecycle' targetRef='dataSharing'/>
    <sequenceFlow id='flow7' sourceRef='dataSharing' targetRef='end'/>
  </process>
</definitions>",
                "DataGovernance"
            ),

            // SDAIA AI Ethics
            new WorkflowDefinition(
                Guid.NewGuid(),
                new LocalizedString("SDAIA AI Ethics and Responsible AI Assessment", "تقييم أخلاقيات الذكاء الاصطناعي المسؤول - الهيئة السعودية للبيانات والذكاء الاصطناعي"),
                new LocalizedString(
                    "AI ethics compliance assessment based on SDAIA guidelines and international AI ethics principles. Evaluates fairness, accountability, transparency, and human rights considerations in AI systems.",
                    "تقييم امتثال أخلاقيات الذكاء الاصطناعي بناءً على إرشادات الهيئة السعودية للبيانات والذكاء الاصطناعي ومبادئ أخلاقيات الذكاء الاصطناعي الدولية. يقيم العدالة والمساءلة والشفافية واعتبارات حقوق الإنسان في أنظمة الذكاء الاصطناعي."
                ),
                "v1.1",
                @"<?xml version='1.0' encoding='UTF-8'?>
<definitions xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' id='sdaia-ai-ethics'>
  <process id='aiEthicsAssessment' name='AI Ethics Assessment' isExecutable='true'>
    <startEvent id='start' name='بدء تقييم أخلاقيات الذكاء الاصطناعي/Start AI Ethics Assessment'/>
    <userTask id='systemMapping' name='رسم خريطة أنظمة الذكاء الاصطناعي/AI System Mapping'/>
    <userTask id='fairnessAssessment' name='تقييم العدالة/Fairness Assessment'/>
    <userTask id='accountabilityFramework' name='إطار المساءلة/Accountability Framework'/>
    <userTask id='transparencyAudit' name='مراجعة الشفافية/Transparency Audit'/>
    <userTask id='humanRightsImpact' name='تقييم أثر حقوق الإنسان/Human Rights Impact Assessment'/>
    <userTask id='biasDetection' name='كشف التحيز/Bias Detection'/>
    <userTask id='ethicsDocumentation' name='توثيق الأخلاقيات/Ethics Documentation'/>
    <endEvent id='end' name='اكتمال تقييم الأخلاقيات/Ethics Assessment Complete'/>
    <sequenceFlow id='flow1' sourceRef='start' targetRef='systemMapping'/>
    <sequenceFlow id='flow2' sourceRef='systemMapping' targetRef='fairnessAssessment'/>
    <sequenceFlow id='flow3' sourceRef='fairnessAssessment' targetRef='accountabilityFramework'/>
    <sequenceFlow id='flow4' sourceRef='accountabilityFramework' targetRef='transparencyAudit'/>
    <sequenceFlow id='flow5' sourceRef='transparencyAudit' targetRef='humanRightsImpact'/>
    <sequenceFlow id='flow6' sourceRef='humanRightsImpact' targetRef='biasDetection'/>
    <sequenceFlow id='flow7' sourceRef='biasDetection' targetRef='ethicsDocumentation'/>
    <sequenceFlow id='flow8' sourceRef='ethicsDocumentation' targetRef='end'/>
  </process>
</definitions>",
                "AI"
            ),

            // MOH Healthcare Information Security
            new WorkflowDefinition(
                Guid.NewGuid(),
                new LocalizedString("MOH Healthcare Information Security Assessment", "تقييم أمن المعلومات الصحية - وزارة الصحة"),
                new LocalizedString(
                    "Healthcare sector information security assessment based on MOH Healthcare Information Security standards. Covers patient data protection, medical device security, and healthcare-specific compliance requirements.",
                    "تقييم أمن المعلومات لقطاع الرعاية الصحية بناءً على معايير أمن المعلومات الصحية لوزارة الصحة. يغطي حماية بيانات المرضى وأمن الأجهزة الطبية ومتطلبات الامتثال الخاصة بالرعاية الصحية."
                ),
                "v2.2",
                @"<?xml version='1.0' encoding='UTF-8'?>
<definitions xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' id='moh-his-assessment'>
  <process id='mohAssessment' name='MOH Healthcare Security Assessment' isExecutable='true'>
    <startEvent id='start' name='بدء تقييم أمن المعلومات الصحية/Start Healthcare Security Assessment'/>
    <userTask id='patientDataProtection' name='حماية بيانات المرضى/Patient Data Protection'/>
    <userTask id='medicalDeviceSecurity' name='أمن الأجهزة الطبية/Medical Device Security'/>
    <userTask id='accessControl' name='ضوابط الوصول/Access Control'/>
    <userTask id='dataEncryption' name='تشفير البيانات/Data Encryption'/>
    <userTask id='auditLogging' name='تسجيل المراجعة/Audit Logging'/>
    <userTask id='incidentResponse' name='الاستجابة للحوادث الصحية/Healthcare Incident Response'/>
    <endEvent id='end' name='اكتمال تقييم الأمن الصحي/Healthcare Security Complete'/>
    <sequenceFlow id='flow1' sourceRef='start' targetRef='patientDataProtection'/>
    <sequenceFlow id='flow2' sourceRef='patientDataProtection' targetRef='medicalDeviceSecurity'/>
    <sequenceFlow id='flow3' sourceRef='medicalDeviceSecurity' targetRef='accessControl'/>
    <sequenceFlow id='flow4' sourceRef='accessControl' targetRef='dataEncryption'/>
    <sequenceFlow id='flow5' sourceRef='dataEncryption' targetRef='auditLogging'/>
    <sequenceFlow id='flow6' sourceRef='auditLogging' targetRef='incidentResponse'/>
    <sequenceFlow id='flow7' sourceRef='incidentResponse' targetRef='end'/>
  </process>
</definitions>",
                "Healthcare"
            ),

            // Vendor Risk Assessment
            new WorkflowDefinition(
                Guid.NewGuid(),
                new LocalizedString("Third-Party Vendor Risk Assessment", "تقييم مخاطر الموردين من الأطراف الثالثة"),
                new LocalizedString(
                    "Comprehensive third-party vendor risk assessment workflow. Evaluates security, compliance, operational, and financial risks associated with vendor relationships and supply chain dependencies.",
                    "سير عمل شامل لتقييم مخاطر الموردين من الأطراف الثالثة. يقيم المخاطر الأمنية والامتثال والتشغيلية والمالية المرتبطة بعلاقات الموردين وتبعيات سلسلة التوريد."
                ),
                "v1.4",
                @"<?xml version='1.0' encoding='UTF-8'?>
<definitions xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' id='vendor-risk-assessment'>
  <process id='vendorRisk' name='Vendor Risk Assessment' isExecutable='true'>
    <startEvent id='start' name='بدء تقييم مخاطر الموردين/Start Vendor Risk Assessment'/>
    <userTask id='vendorProfiling' name='تحليل الملف الشخصي للمورد/Vendor Profiling'/>
    <userTask id='securityAssessment' name='التقييم الأمني/Security Assessment'/>
    <userTask id='complianceReview' name='مراجعة الامتثال/Compliance Review'/>
    <userTask id='operationalRisk' name='تقييم المخاطر التشغيلية/Operational Risk Assessment'/>
    <userTask id='financialRisk' name='تقييم المخاطر المالية/Financial Risk Assessment'/>
    <userTask id='contractualReview' name='مراجعة التعاقد/Contractual Review'/>
    <userTask id='riskMitigation' name='تخفيف المخاطر/Risk Mitigation'/>
    <endEvent id='end' name='اكتمال تقييم مخاطر الموردين/Vendor Risk Complete'/>
    <sequenceFlow id='flow1' sourceRef='start' targetRef='vendorProfiling'/>
    <sequenceFlow id='flow2' sourceRef='vendorProfiling' targetRef='securityAssessment'/>
    <sequenceFlow id='flow3' sourceRef='securityAssessment' targetRef='complianceReview'/>
    <sequenceFlow id='flow4' sourceRef='complianceReview' targetRef='operationalRisk'/>
    <sequenceFlow id='flow5' sourceRef='operationalRisk' targetRef='financialRisk'/>
    <sequenceFlow id='flow6' sourceRef='financialRisk' targetRef='contractualReview'/>
    <sequenceFlow id='flow7' sourceRef='contractualReview' targetRef='riskMitigation'/>
    <sequenceFlow id='flow8' sourceRef='riskMitigation' targetRef='end'/>
  </process>
</definitions>",
                "Vendor"
            ),

            // Internal Audit Program
            new WorkflowDefinition(
                Guid.NewGuid(),
                new LocalizedString("Internal Audit Program Management", "إدارة برنامج التدقيق الداخلي"),
                new LocalizedString(
                    "Comprehensive internal audit program management workflow following international auditing standards. Includes audit planning, execution, reporting, and follow-up activities with integrated quality assurance.",
                    "سير عمل شامل لإدارة برنامج التدقيق الداخلي وفقاً للمعايير الدولية للتدقيق. يشمل تخطيط التدقيق والتنفيذ وإعداد التقارير وأنشطة المتابعة مع ضمان الجودة المتكامل."
                ),
                "v3.1",
                @"<?xml version='1.0' encoding='UTF-8'?>
<definitions xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' id='internal-audit-program'>
  <process id='auditProgram' name='Internal Audit Program' isExecutable='true'>
    <startEvent id='start' name='بدء برنامج التدقيق/Start Audit Program'/>
    <userTask id='auditPlanning' name='تخطيط التدقيق/Audit Planning'/>
    <userTask id='riskBasedSelection' name='الاختيار المبني على المخاطر/Risk-Based Selection'/>
    <userTask id='auditExecution' name='تنفيذ التدقيق/Audit Execution'/>
    <userTask id='findingsDocumentation' name='توثيق النتائج/Findings Documentation'/>
    <userTask id='auditReporting' name='إعداد تقرير التدقيق/Audit Reporting'/>
    <userTask id='managementResponse' name='رد الإدارة/Management Response'/>
    <userTask id='followUp' name='المتابعة/Follow-up'/>
    <userTask id='qualityAssurance' name='ضمان الجودة/Quality Assurance'/>
    <endEvent id='end' name='اكتمال برنامج التدقيق/Audit Program Complete'/>
    <sequenceFlow id='flow1' sourceRef='start' targetRef='auditPlanning'/>
    <sequenceFlow id='flow2' sourceRef='auditPlanning' targetRef='riskBasedSelection'/>
    <sequenceFlow id='flow3' sourceRef='riskBasedSelection' targetRef='auditExecution'/>
    <sequenceFlow id='flow4' sourceRef='auditExecution' targetRef='findingsDocumentation'/>
    <sequenceFlow id='flow5' sourceRef='findingsDocumentation' targetRef='auditReporting'/>
    <sequenceFlow id='flow6' sourceRef='auditReporting' targetRef='managementResponse'/>
    <sequenceFlow id='flow7' sourceRef='managementResponse' targetRef='followUp'/>
    <sequenceFlow id='flow8' sourceRef='followUp' targetRef='qualityAssurance'/>
    <sequenceFlow id='flow9' sourceRef='qualityAssurance' targetRef='end'/>
  </process>
</definitions>",
                "Audit"
            )
        };

        foreach (var workflow in workflows)
        {
            await _workflowDefinitionRepository.InsertAsync(workflow, autoSave: true);
        }
    }
}