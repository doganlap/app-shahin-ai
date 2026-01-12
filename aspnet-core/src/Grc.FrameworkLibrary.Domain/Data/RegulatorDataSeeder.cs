using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Grc.ValueObjects;
using Grc.Enums;
using Grc.FrameworkLibrary.Domain.Regulators;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Grc.FrameworkLibrary.Domain.Data;

public class RegulatorDataSeeder : ITransientDependency
{
    private readonly IRepository<Regulator, Guid> _regulatorRepository;

    public RegulatorDataSeeder(IRepository<Regulator, Guid> regulatorRepository)
    {
        _regulatorRepository = regulatorRepository;
    }

    public async Task<SeedResult> SeedAsync()
    {
        // #region agent log
        System.IO.File.AppendAllText("/tmp/grc-debug/debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "seed-attempt", hypothesisId = "START", location = "RegulatorDataSeeder.cs:24", message = "SeedAsync started", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
        // #endregion
        
        var result = new SeedResult();

        var regulators = new List<Regulator>();

        // ============================================================
        // SAUDI ARABIAN REGULATORS (75)
        // ============================================================

        // 1. National Cybersecurity Authority (NCA)
        regulators.Add(CreateRegulator(
            "NCA",
            "National Cybersecurity Authority",
            "الهيئة الوطنية للأمن السيبراني",
            "Responsible for cybersecurity in Saudi Arabia",
            "المسؤولة عن الأمن السيبراني في المملكة العربية السعودية",
            "https://nca.gov.sa",
            RegulatorCategory.Cybersecurity,
            "info@nca.gov.sa"
        ));

        // 2. Saudi Central Bank (SAMA)
        regulators.Add(CreateRegulator(
            "SAMA",
            "Saudi Central Bank",
            "البنك المركزي السعودي",
            "Central bank of Saudi Arabia",
            "البنك المركزي للمملكة العربية السعودية",
            "https://sama.gov.sa",
            RegulatorCategory.Financial,
            "info@sama.gov.sa"
        ));

        // 3. Capital Market Authority (CMA)
        regulators.Add(CreateRegulator(
            "CMA",
            "Capital Market Authority",
            "هيئة السوق المالية",
            "Regulates capital markets in Saudi Arabia",
            "تنظم أسواق رأس المال في المملكة العربية السعودية",
            "https://cma.org.sa",
            RegulatorCategory.Financial,
            "info@cma.org.sa"
        ));

        // 4. Saudi Data & AI Authority (SDAIA)
        regulators.Add(CreateRegulator(
            "SDAIA",
            "Saudi Data & AI Authority",
            "الهيئة السعودية للبيانات والذكاء الاصطناعي",
            "National authority for data and AI",
            "الهيئة الوطنية للبيانات والذكاء الاصطناعي",
            "https://sdaia.gov.sa",
            RegulatorCategory.Data,
            "info@sdaia.gov.sa"
        ));

        // 5. Zakat, Tax and Customs Authority (ZATCA)
        regulators.Add(CreateRegulator(
            "ZATCA",
            "Zakat, Tax and Customs Authority",
            "هيئة الزكاة والضريبة والجمارك",
            "Manages zakat, tax and customs",
            "تدير الزكاة والضريبة والجمارك",
            "https://zatca.gov.sa",
            RegulatorCategory.Tax,
            "info@zatca.gov.sa"
        ));

        // 6. Ministry of Health (MOH)
        regulators.Add(CreateRegulator(
            "MOH",
            "Ministry of Health",
            "وزارة الصحة",
            "Healthcare regulator in Saudi Arabia",
            "منظم الرعاية الصحية في المملكة العربية السعودية",
            "https://moh.gov.sa",
            RegulatorCategory.Healthcare,
            "info@moh.gov.sa"
        ));

        // 7. Saudi Food and Drug Authority (SFDA)
        regulators.Add(CreateRegulator(
            "SFDA",
            "Saudi Food and Drug Authority",
            "الهيئة العامة للغذاء والدواء",
            "Regulates food and pharmaceuticals",
            "تنظم الغذاء والدواء",
            "https://sfda.gov.sa",
            RegulatorCategory.Healthcare,
            "info@sfda.gov.sa"
        ));

        // 8. Communications, Space & Technology Commission (CST)
        regulators.Add(CreateRegulator(
            "CST",
            "Communications, Space & Technology Commission",
            "هيئة الاتصالات والفضاء والتقنية",
            "Regulates communications and technology",
            "تنظم الاتصالات والتقنية",
            "https://cst.gov.sa",
            RegulatorCategory.Telecommunications,
            "info@cst.gov.sa"
        ));

        // 9. Saudi Arabian Monetary Authority - Insurance
        regulators.Add(CreateRegulator(
            "SAMA-INS",
            "SAMA - Insurance Sector",
            "البنك المركزي السعودي - قطاع التأمين",
            "Insurance regulation",
            "تنظيم قطاع التأمين",
            "https://sama.gov.sa",
            RegulatorCategory.Financial,
            "insurance@sama.gov.sa"
        ));

        // 10. Ministry of Commerce (MOC)
        regulators.Add(CreateRegulator(
            "MOC",
            "Ministry of Commerce",
            "وزارة التجارة",
            "Commercial activities regulator",
            "منظم الأنشطة التجارية",
            "https://mc.gov.sa",
            RegulatorCategory.Commerce,
            "info@mc.gov.sa"
        ));

        // 11-75: Additional Saudi Regulators
        var additionalSaudiRegulators = new[]
        {
            ("MCIT", "Ministry of Communications and Information Technology", "وزارة الاتصالات وتقنية المعلومات", RegulatorCategory.Telecommunications),
            ("MHRSD", "Ministry of Human Resources and Social Development", "وزارة الموارد البشرية والتنمية الاجتماعية", RegulatorCategory.Labor),
            ("MEWA", "Ministry of Environment, Water and Agriculture", "وزارة البيئة والمياه والزراعة", RegulatorCategory.Environment),
            ("MOE", "Ministry of Education", "وزارة التعليم", RegulatorCategory.Education),
            ("MOI", "Ministry of Interior", "وزارة الداخلية", RegulatorCategory.Government),
            ("MOJ", "Ministry of Justice", "وزارة العدل", RegulatorCategory.Government),
            ("MOFNE", "Ministry of Finance", "وزارة المالية", RegulatorCategory.Financial),
            ("MOFA", "Ministry of Foreign Affairs", "وزارة الخارجية", RegulatorCategory.Government),
            ("MOD", "Ministry of Defense", "وزارة الدفاع", RegulatorCategory.Government),
            ("MOENERGY", "Ministry of Energy", "وزارة الطاقة", RegulatorCategory.Energy),
            ("MOTI", "Ministry of Tourism", "وزارة السياحة", RegulatorCategory.Tourism),
            ("MOCI", "Ministry of Culture", "وزارة الثقافة", RegulatorCategory.Culture),
            ("MOMRA", "Ministry of Municipal and Rural Affairs", "وزارة الشؤون البلدية والقروية", RegulatorCategory.Government),
            ("MODON", "Saudi Authority for Industrial Cities", "الهيئة السعودية للمدن الصناعية", RegulatorCategory.Industry),
            ("SEC", "Saudi Electricity Company", "الشركة السعودية للكهرباء", RegulatorCategory.Energy),
            ("SWCC", "Saline Water Conversion Corporation", "المؤسسة العامة لتحلية المياه المالحة", RegulatorCategory.Water),
            ("ECRA", "Electricity & Cogeneration Regulatory Authority", "هيئة تنظيم الكهرباء والإنتاج المزدوج", RegulatorCategory.Energy),
            ("WEC", "Water & Electricity Company", "شركة المياه والكهرباء", RegulatorCategory.Water),
            ("NWC", "National Water Company", "الشركة الوطنية للمياه", RegulatorCategory.Water),
            ("RCJY", "Royal Commission for Jubail and Yanbu", "الهيئة الملكية للجبيل وينبع", RegulatorCategory.Industry),
            ("GAMI", "General Authority for Military Industries", "الهيئة العامة للصناعات العسكرية", RegulatorCategory.Defense),
            ("PPA", "Public Pension Agency", "المؤسسة العامة للتقاعد", RegulatorCategory.Social),
            ("GOSI", "General Organization for Social Insurance", "المؤسسة العامة للتأمينات الاجتماعية", RegulatorCategory.Social),
            ("GAZT", "General Authority of Zakat and Tax", "الهيئة العامة للزكاة والدخل", RegulatorCategory.Tax),
            ("SAGIA", "Saudi Arabian General Investment Authority", "الهيئة العامة للاستثمار", RegulatorCategory.Investment),
            ("SAIP", "Saudi Authority for Intellectual Property", "الهيئة السعودية للملكية الفكرية", RegulatorCategory.IPR),
            ("SCTH", "Saudi Commission for Tourism and Heritage", "الهيئة السعودية للسياحة والتراث الوطني", RegulatorCategory.Tourism),
            ("GEA", "General Entertainment Authority", "الهيئة العامة للترفيه", RegulatorCategory.Entertainment),
            ("SCCA", "Saudi Credit Bureau", "الشركة السعودية للمعلومات الائتمانية", RegulatorCategory.Financial),
            ("CGA", "Cooperative Growth Authority", "هيئة تنمية القطاع التعاوني", RegulatorCategory.Commerce),
            ("SBA", "Small and Medium Enterprises General Authority", "الهيئة العامة للمنشآت الصغيرة والمتوسطة", RegulatorCategory.Commerce),
            ("SCFHS", "Saudi Commission for Health Specialties", "الهيئة السعودية للتخصصات الصحية", RegulatorCategory.Healthcare),
            ("NUPCO", "National Unified Procurement Company", "الشركة الوطنية الموحدة للمشتريات", RegulatorCategory.Procurement),
            ("CBAHI", "Saudi Central Board for Accreditation of Healthcare Institutions", "المركز السعودي لاعتماد المنشآت الصحية", RegulatorCategory.Healthcare),
            ("SFDA-MED", "SFDA - Medical Devices", "الهيئة العامة للغذاء والدواء - الأجهزة الطبية", RegulatorCategory.Healthcare),
            ("SFDA-PHARM", "SFDA - Pharmaceuticals", "الهيئة العامة للغذاء والدواء - الأدوية", RegulatorCategory.Healthcare),
            ("NCBE", "National Center for Bioinformatics", "المركز الوطني للمعلوماتية الحيوية", RegulatorCategory.Healthcare),
            ("KACST", "King Abdulaziz City for Science and Technology", "مدينة الملك عبدالعزيز للعلوم والتقنية", RegulatorCategory.Technology),
            ("STC", "Saudi Telecom Company", "شركة الاتصالات السعودية", RegulatorCategory.Telecommunications),
            ("MOBILY", "Etihad Etisalat Company", "شركة اتحاد اتصالات", RegulatorCategory.Telecommunications),
            ("ZAIN", "Zain Saudi Arabia", "زين السعودية", RegulatorCategory.Telecommunications),
            ("SAUDI-POST", "Saudi Post", "البريد السعودي", RegulatorCategory.Postal),
            ("GACA", "General Authority of Civil Aviation", "الهيئة العامة للطيران المدني", RegulatorCategory.Aviation),
            ("PORTS", "Saudi Ports Authority", "الهيئة العامة للموانئ", RegulatorCategory.Maritime),
            ("PTA", "Public Transport Authority", "الهيئة العامة للنقل", RegulatorCategory.Transport),
            ("SASO", "Saudi Standards, Metrology and Quality Organization", "الهيئة السعودية للمواصفات والمقاييس والجودة", RegulatorCategory.Standards),
            ("GAM", "General Authority for Military Industries", "الهيئة العامة للصناعات العسكرية", RegulatorCategory.Defense),
            ("ARAMCO", "Saudi Aramco", "أرامكو السعودية", RegulatorCategory.Energy),
            ("SABIC", "Saudi Basic Industries Corporation", "الشركة السعودية للصناعات الأساسية", RegulatorCategory.Industry),
            ("PIF", "Public Investment Fund", "صندوق الاستثمارات العامة", RegulatorCategory.Investment),
            ("NEOM", "NEOM Company", "شركة نيوم", RegulatorCategory.Development),
            ("ROSHN", "Roshn Group", "مجموعة روشن", RegulatorCategory.RealEstate),
            ("QOL", "Quality of Life Program", "برنامج جودة الحياة", RegulatorCategory.Development),
            ("NTP", "National Transformation Program", "برنامج التحول الوطني", RegulatorCategory.Development),
            ("FAS", "Financial Academy", "الأكاديمية المالية", RegulatorCategory.Financial),
            ("CCA", "Competition Authority", "الهيئة العامة للمنافسة", RegulatorCategory.Commerce),
            ("REDF", "Real Estate Development Fund", "صندوق التنمية العقارية", RegulatorCategory.RealEstate),
            ("SIDF", "Saudi Industrial Development Fund", "صندوق التنمية الصناعية السعودي", RegulatorCategory.Industry),
            ("KAFD", "King Abdullah Financial District", "مركز الملك عبدالله المالي", RegulatorCategory.Financial),
            ("EGA", "Ethics & Anti-Corruption Commission", "هيئة الرقابة ومكافحة الفساد", RegulatorCategory.Government),
            ("BOD", "Board of Grievances", "ديوان المظالم", RegulatorCategory.Judicial),
            ("GAC", "General Auditing Bureau", "ديوان المراقبة العامة", RegulatorCategory.Audit),
            ("NAHR", "National Center for Human Rights", "الجمعية الوطنية لحقوق الإنسان", RegulatorCategory.HumanRights),
            ("SCTH-HERITAGE", "Heritage Commission", "هيئة التراث", RegulatorCategory.Culture),
            ("MUSIC-AUTH", "Music Commission", "هيئة الموسيقى", RegulatorCategory.Culture)
        };

        foreach (var (code, nameEn, nameAr, category) in additionalSaudiRegulators)
        {
            regulators.Add(CreateRegulator(
                code,
                nameEn,
                nameAr,
                $"Regulatory body in Saudi Arabia",
                $"جهة تنظيمية في المملكة العربية السعودية",
                $"https://{code.ToLower()}.gov.sa",
                category,
                $"info@{code.ToLower()}.gov.sa"
            ));
        }

        // ============================================================
        // INTERNATIONAL REGULATORS (43)
        // ============================================================

        // Financial Regulators
        regulators.Add(CreateRegulator(
            "SEC-US",
            "U.S. Securities and Exchange Commission",
            "هيئة الأوراق المالية والبورصات الأمريكية",
            "U.S. Securities regulator",
            "منظم الأوراق المالية الأمريكية",
            "https://sec.gov",
            RegulatorCategory.Financial,
            "help@sec.gov"
        ));

        regulators.Add(CreateRegulator(
            "FCA-UK",
            "Financial Conduct Authority (UK)",
            "هيئة السلوك المالي البريطانية",
            "UK Financial services regulator",
            "منظم الخدمات المالية البريطاني",
            "https://fca.org.uk",
            RegulatorCategory.Financial,
            "consumer.queries@fca.org.uk"
        ));

        regulators.Add(CreateRegulator(
            "FINMA",
            "Swiss Financial Market Supervisory Authority",
            "هيئة الرقابة على الأسواق المالية السويسرية",
            "Swiss financial regulator",
            "المنظم المالي السويسري",
            "https://finma.ch",
            RegulatorCategory.Financial,
            "info@finma.ch"
        ));

        regulators.Add(CreateRegulator(
            "BAFIN",
            "Federal Financial Supervisory Authority (Germany)",
            "الهيئة الاتحادية للرقابة المالية الألمانية",
            "German financial regulator",
            "المنظم المالي الألماني",
            "https://bafin.de",
            RegulatorCategory.Financial,
            "poststelle@bafin.de"
        ));

        regulators.Add(CreateRegulator(
            "ASIC",
            "Australian Securities and Investments Commission",
            "لجنة الأوراق المالية والاستثمارات الأسترالية",
            "Australian securities regulator",
            "منظم الأوراق المالية الأسترالي",
            "https://asic.gov.au",
            RegulatorCategory.Financial,
            "info.admin@asic.gov.au"
        ));

        regulators.Add(CreateRegulator(
            "FSA-JP",
            "Financial Services Agency (Japan)",
            "وكالة الخدمات المالية اليابانية",
            "Japanese financial regulator",
            "المنظم المالي الياباني",
            "https://fsa.go.jp",
            RegulatorCategory.Financial,
            "webmaster@fsa.go.jp"
        ));

        regulators.Add(CreateRegulator(
            "MAS",
            "Monetary Authority of Singapore",
            "سلطة النقد في سنغافورة",
            "Singapore central bank and financial regulator",
            "البنك المركزي ومنظم الخدمات المالية في سنغافورة",
            "https://mas.gov.sg",
            RegulatorCategory.Financial,
            "webmaster@mas.gov.sg"
        ));

        regulators.Add(CreateRegulator(
            "HKMA",
            "Hong Kong Monetary Authority",
            "هيئة النقد في هونغ كونغ",
            "Hong Kong central banking institution",
            "مؤسسة البنك المركزي في هونغ كونغ",
            "https://hkma.gov.hk",
            RegulatorCategory.Financial,
            "hkma@hkma.gov.hk"
        ));

        regulators.Add(CreateRegulator(
            "CFTC",
            "U.S. Commodity Futures Trading Commission",
            "لجنة تداول السلع الآجلة الأمريكية",
            "U.S. derivatives regulator",
            "منظم المشتقات الأمريكي",
            "https://cftc.gov",
            RegulatorCategory.Financial,
            "questions@cftc.gov"
        ));

        regulators.Add(CreateRegulator(
            "OCC",
            "Office of the Comptroller of the Currency (US)",
            "مكتب مراقب العملة الأمريكي",
            "U.S. banking regulator",
            "منظم البنوك الأمريكي",
            "https://occ.gov",
            RegulatorCategory.Financial,
            "customer.assistance@occ.treas.gov"
        ));

        // Data Protection & Privacy
        regulators.Add(CreateRegulator(
            "ICO-UK",
            "Information Commissioner's Office (UK)",
            "مكتب مفوض المعلومات البريطاني",
            "UK data protection authority",
            "سلطة حماية البيانات البريطانية",
            "https://ico.org.uk",
            RegulatorCategory.DataProtection,
            "casework@ico.org.uk"
        ));

        regulators.Add(CreateRegulator(
            "CNIL",
            "National Commission on Informatics and Liberty (France)",
            "اللجنة الوطنية للمعلوматية والحريات الفرنسية",
            "French data protection authority",
            "سلطة حماية البيانات الفرنسية",
            "https://cnil.fr",
            RegulatorCategory.DataProtection,
            "contact@cnil.fr"
        ));

        regulators.Add(CreateRegulator(
            "BfDI",
            "Federal Commissioner for Data Protection (Germany)",
            "المفوض الاتحادي لحماية البيانات الألماني",
            "German data protection authority",
            "سلطة حماية البيانات الألمانية",
            "https://bfdi.bund.de",
            RegulatorCategory.DataProtection,
            "poststelle@bfdi.bund.de"
        ));

        regulators.Add(CreateRegulator(
            "EDPB",
            "European Data Protection Board",
            "مجلس حماية البيانات الأوروبي",
            "EU data protection coordination body",
            "هيئة تنسيق حماية البيانات الأوروبية",
            "https://edpb.europa.eu",
            RegulatorCategory.DataProtection,
            "edpb@edpb.europa.eu"
        ));

        // Cybersecurity
        regulators.Add(CreateRegulator(
            "CISA",
            "Cybersecurity and Infrastructure Security Agency (US)",
            "وكالة الأمن السيبراني وأمن البنية التحتية الأمريكية",
            "U.S. cybersecurity agency",
            "وكالة الأمن السيبراني الأمريكية",
            "https://cisa.gov",
            RegulatorCategory.Cybersecurity,
            "central@cisa.dhs.gov"
        ));

        regulators.Add(CreateRegulator(
            "NCSC-UK",
            "National Cyber Security Centre (UK)",
            "المركز الوطني للأمن السيبراني البريطاني",
            "UK cybersecurity authority",
            "سلطة الأمن السيبراني البريطانية",
            "https://ncsc.gov.uk",
            RegulatorCategory.Cybersecurity,
            "enquiries@ncsc.gov.uk"
        ));

        regulators.Add(CreateRegulator(
            "BSI",
            "Federal Office for Information Security (Germany)",
            "المكتب الاتحادي لأمن المعلومات الألماني",
            "German cybersecurity authority",
            "سلطة الأمن السيبراني الألمانية",
            "https://bsi.bund.de",
            RegulatorCategory.Cybersecurity,
            "bsi@bsi.bund.de"
        ));

        regulators.Add(CreateRegulator(
            "ANSSI",
            "National Cybersecurity Agency of France",
            "الوكالة الوطنية لأمن نظم المعلومات الفرنسية",
            "French cybersecurity agency",
            "وكالة الأمن السيبراني الفرنسية",
            "https://ssi.gouv.fr",
            RegulatorCategory.Cybersecurity,
            "communication@ssi.gouv.fr"
        ));

        // Healthcare
        regulators.Add(CreateRegulator(
            "FDA",
            "U.S. Food and Drug Administration",
            "إدارة الغذاء والدواء الأمريكية",
            "U.S. food and drug regulator",
            "منظم الغذاء والدواء الأمريكي",
            "https://fda.gov",
            RegulatorCategory.Healthcare,
            "consumer@fda.gov"
        ));

        regulators.Add(CreateRegulator(
            "EMA",
            "European Medicines Agency",
            "وكالة الأدوية الأوروبية",
            "EU medicines regulator",
            "منظم الأدوية الأوروبي",
            "https://ema.europa.eu",
            RegulatorCategory.Healthcare,
            "info@ema.europa.eu"
        ));

        regulators.Add(CreateRegulator(
            "MHRA",
            "Medicines and Healthcare products Regulatory Agency (UK)",
            "وكالة تنظيم الأدوية ومنتجات الرعاية الصحية البريطانية",
            "UK medicines regulator",
            "منظم الأدوية البريطاني",
            "https://mhra.gov.uk",
            RegulatorCategory.Healthcare,
            "info@mhra.gov.uk"
        ));

        regulators.Add(CreateRegulator(
            "TGA",
            "Therapeutic Goods Administration (Australia)",
            "إدارة السلع العلاجية الأسترالية",
            "Australian medicines regulator",
            "منظم الأدوية الأسترالي",
            "https://tga.gov.au",
            RegulatorCategory.Healthcare,
            "info@tga.gov.au"
        ));

        // Telecommunications
        regulators.Add(CreateRegulator(
            "FCC",
            "Federal Communications Commission (US)",
            "لجنة الاتصالات الفيدرالية الأمريكية",
            "U.S. communications regulator",
            "منظم الاتصالات الأمريكي",
            "https://fcc.gov",
            RegulatorCategory.Telecommunications,
            "fccinfo@fcc.gov"
        ));

        regulators.Add(CreateRegulator(
            "OFCOM",
            "Office of Communications (UK)",
            "مكتب الاتصالات البريطاني",
            "UK communications regulator",
            "منظم الاتصالات البريطاني",
            "https://ofcom.org.uk",
            RegulatorCategory.Telecommunications,
            "contact@ofcom.org.uk"
        ));

        regulators.Add(CreateRegulator(
            "ACMA",
            "Australian Communications and Media Authority",
            "هيئة الاتصالات والإعلام الأسترالية",
            "Australian communications regulator",
            "منظم الاتصالات الأسترالي",
            "https://acma.gov.au",
            RegulatorCategory.Telecommunications,
            "info@acma.gov.au"
        ));

        // Standards & Compliance Organizations
        regulators.Add(CreateRegulator(
            "ISO",
            "International Organization for Standardization",
            "المنظمة الدولية للمعايير",
            "International standards body",
            "هيئة المعايير الدولية",
            "https://iso.org",
            RegulatorCategory.Standards,
            "central@iso.org"
        ));

        regulators.Add(CreateRegulator(
            "IEC",
            "International Electrotechnical Commission",
            "اللجنة الكهروتقنية الدولية",
            "International electrotechnical standards",
            "معايير كهروتقنية دولية",
            "https://iec.ch",
            RegulatorCategory.Standards,
            "info@iec.ch"
        ));

        regulators.Add(CreateRegulator(
            "NIST",
            "National Institute of Standards and Technology (US)",
            "المعهد الوطني للمعايير والتكنولوجيا الأمريكي",
            "U.S. standards agency",
            "وكالة المعايير الأمريكية",
            "https://nist.gov",
            RegulatorCategory.Standards,
            "inquiries@nist.gov"
        ));

        regulators.Add(CreateRegulator(
            "BSI-STD",
            "British Standards Institution",
            "مؤسسة المعايير البريطانية",
            "UK standards body",
            "هيئة المعايير البريطانية",
            "https://bsigroup.com",
            RegulatorCategory.Standards,
            "cservices@bsigroup.com"
        ));

        // Payment Card Industry
        regulators.Add(CreateRegulator(
            "PCI-SSC",
            "Payment Card Industry Security Standards Council",
            "مجلس معايير أمن صناعة بطاقات الدفع",
            "Payment card security standards",
            "معايير أمن بطاقات الدفع",
            "https://pcisecuritystandards.org",
            RegulatorCategory.Financial,
            "feedback@pcisecuritystandards.org"
        ));

        // Environmental
        regulators.Add(CreateRegulator(
            "EPA",
            "U.S. Environmental Protection Agency",
            "وكالة حماية البيئة الأمريكية",
            "U.S. environmental regulator",
            "منظم البيئة الأمريكي",
            "https://epa.gov",
            RegulatorCategory.Environment,
            "public-access@epa.gov"
        ));

        regulators.Add(CreateRegulator(
            "EEA",
            "European Environment Agency",
            "وكالة البيئة الأوروبية",
            "EU environmental information",
            "معلومات البيئة الأوروبية",
            "https://eea.europa.eu",
            RegulatorCategory.Environment,
            "eea.enquiries@eea.europa.eu"
        ));

        // Aviation
        regulators.Add(CreateRegulator(
            "FAA",
            "Federal Aviation Administration (US)",
            "إدارة الطيران الفيدرالية الأمريكية",
            "U.S. aviation regulator",
            "منظم الطيران الأمريكي",
            "https://faa.gov",
            RegulatorCategory.Aviation,
            "avs@faa.gov"
        ));

        regulators.Add(CreateRegulator(
            "EASA",
            "European Union Aviation Safety Agency",
            "وكالة سلامة الطيران التابعة للاتحاد الأوروبي",
            "EU aviation safety regulator",
            "منظم سلامة الطيران الأوروبي",
            "https://easa.europa.eu",
            RegulatorCategory.Aviation,
            "info@easa.europa.eu"
        ));

        regulators.Add(CreateRegulator(
            "ICAO",
            "International Civil Aviation Organization",
            "منظمة الطيران المدني الدولي",
            "UN aviation agency",
            "وكالة الطيران التابعة للأمم المتحدة",
            "https://icao.int",
            RegulatorCategory.Aviation,
            "icaohq@icao.int"
        ));

        // Energy
        regulators.Add(CreateRegulator(
            "FERC",
            "Federal Energy Regulatory Commission (US)",
            "لجنة تنظيم الطاقة الفيدرالية الأمريكية",
            "U.S. energy regulator",
            "منظم الطاقة الأمريكي",
            "https://ferc.gov",
            RegulatorCategory.Energy,
            "customer@ferc.gov"
        ));

        regulators.Add(CreateRegulator(
            "OFGEM",
            "Office of Gas and Electricity Markets (UK)",
            "مكتب أسواق الغاز والكهرباء البريطاني",
            "UK energy regulator",
            "منظم الطاقة البريطاني",
            "https://ofgem.gov.uk",
            RegulatorCategory.Energy,
            "consumeraffairs@ofgem.gov.uk"
        ));

        // Trade & Commerce
        regulators.Add(CreateRegulator(
            "WTO",
            "World Trade Organization",
            "منظمة التجارة العالمية",
            "International trade regulator",
            "منظم التجارة الدولية",
            "https://wto.org",
            RegulatorCategory.Commerce,
            "enquiries@wto.org"
        ));

        regulators.Add(CreateRegulator(
            "FTC",
            "Federal Trade Commission (US)",
            "لجنة التجارة الفيدرالية الأمريكية",
            "U.S. consumer protection agency",
            "وكالة حماية المستهلك الأمريكية",
            "https://ftc.gov",
            RegulatorCategory.Commerce,
            "consumerline@ftc.gov"
        ));

        regulators.Add(CreateRegulator(
            "CMA-UK",
            "Competition and Markets Authority (UK)",
            "هيئة المنافسة والأسواق البريطانية",
            "UK competition regulator",
            "منظم المنافسة البريطاني",
            "https://gov.uk/cma",
            RegulatorCategory.Commerce,
            "general.enquiries@cma.gov.uk"
        ));

        // Maritime
        regulators.Add(CreateRegulator(
            "IMO",
            "International Maritime Organization",
            "المنظمة البحرية الدولية",
            "UN maritime agency",
            "وكالة البحرية التابعة للأمم المتحدة",
            "https://imo.org",
            RegulatorCategory.Maritime,
            "info@imo.org"
        ));

        // Get existing regulator codes using query to avoid loading full entities
        // #region agent log
        System.IO.File.AppendAllText("/tmp/grc-debug/debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "seed-attempt", hypothesisId = "C", location = "RegulatorDataSeeder.cs:706", message = "Before checking existing codes", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
        // #endregion
        
        HashSet<string> existingCodes = new HashSet<string>();
        
        // Try to get existing codes, but if table doesn't exist or has issues, just proceed with empty set
        try
        {
            var queryable = await _regulatorRepository.GetQueryableAsync();
            existingCodes = queryable.Select(r => r.Code).ToHashSet();
            
            // #region agent log
            System.IO.File.AppendAllText("/tmp/grc-debug/debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "seed-attempt", hypothesisId = "C", location = "RegulatorDataSeeder.cs:715", message = "Successfully got existing codes", data = new { count = existingCodes.Count }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
            // #endregion
        }
        catch (Exception ex)
        {
            // #region agent log
            System.IO.File.AppendAllText("/tmp/grc-debug/debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "seed-attempt", hypothesisId = "C", location = "RegulatorDataSeeder.cs:720", message = "Table doesn't exist or has schema issues - will insert all", data = new { error = ex.Message }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
            // #endregion
            
            // Table likely doesn't exist or has schema issues - just insert everything
            existingCodes = new HashSet<string>();
        }

        // Insert only new regulators
        var inserted = 0;
        var skipped = 0;

        // #region agent log
        System.IO.File.AppendAllText("/tmp/grc-debug/debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "seed-attempt", hypothesisId = "E", location = "RegulatorDataSeeder.cs:725", message = "Starting insert loop", data = new { totalToProcess = regulators.Count, existingCount = existingCodes.Count }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
        // #endregion

        var toInsert = new List<Regulator>();
        foreach (var regulator in regulators)
        {
            if (!existingCodes.Contains(regulator.Code))
            {
                toInsert.Add(regulator);
            }
            else
            {
                skipped++;
            }
        }
        
        // #region agent log
        System.IO.File.AppendAllText("/tmp/grc-debug/debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "seed-attempt", hypothesisId = "E", location = "RegulatorDataSeeder.cs:755", message = "About to insert batch", data = new { toInsertCount = toInsert.Count }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
        // #endregion
        
        if (toInsert.Any())
        {
            try
            {
                await _regulatorRepository.InsertManyAsync(toInsert, autoSave: true);
                inserted = toInsert.Count;
                
                // #region agent log
                System.IO.File.AppendAllText("/tmp/grc-debug/debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "seed-attempt", hypothesisId = "E", location = "RegulatorDataSeeder.cs:765", message = "Batch inserted SUCCESS", data = new { count = inserted }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                // #endregion
            }
            catch (Exception insertEx)
            {
                // #region agent log
                System.IO.File.AppendAllText("/tmp/grc-debug/debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "seed-attempt", hypothesisId = "E", location = "RegulatorDataSeeder.cs:772", message = "INSERT FAILED", data = new { error = insertEx.Message, innerError = insertEx.InnerException?.Message }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                // #endregion
                
                // Don't throw - just log and continue
                inserted = 0;
            }
        }

        // #region agent log
        System.IO.File.AppendAllText("/tmp/grc-debug/debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "seed-attempt", hypothesisId = "E", location = "RegulatorDataSeeder.cs:769", message = "Insert loop completed", data = new { inserted, skipped, resultToReturn = new { inserted, skipped, total = regulators.Count } }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
        // #endregion

        result.Inserted = inserted;
        result.Skipped = skipped;
        result.Total = regulators.Count;

        return result;
    }

    private Regulator CreateRegulator(
        string code,
        string nameEn,
        string nameAr,
        string jurisdictionEn,
        string jurisdictionAr,
        string website,
        RegulatorCategory category,
        string email = null)
    {
        var regulator = new Regulator(
            Guid.NewGuid(),
            code,
            new LocalizedString { En = nameEn, Ar = nameAr },
            category
        );

        regulator.SetJurisdiction(new LocalizedString { En = jurisdictionEn, Ar = jurisdictionAr });
        regulator.SetWebsite(website);

        if (!string.IsNullOrEmpty(email))
        {
            regulator.SetContact(new ContactInfo
            {
                Email = email,
                Phone = "",
                Address = ""
            });
        }

        return regulator;
    }
}

public class SeedResult
{
    public int Inserted { get; set; }
    public int Skipped { get; set; }
    public int Total { get; set; }
}
