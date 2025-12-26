# حل مشكلة الخطأ 500 (Error 500 Fix)

## المشكلة (Problem):
```
[500] خطأ!
حدث خطأ داخلي أثناء طلبك!
```

## السبب (Root Cause):
الوحدات (Evidence, FrameworkLibrary, Risks) تعطي خطأ 500 لأن:
1. جداول قاعدة البيانات لم يتم إنشاؤها بعد
2. التطبيق يحاول الوصول إلى جداول غير موجودة

## الحل (Solution):

### 1. التحقق من الجداول الموجودة:
```bash
export PGPASSWORD="sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ"
psql -h mainline.proxy.rlwy.net -p 46662 -U postgres -d railway -c "\dt"
```

النتيجة: **0 جداول GRC** (لا توجد جداول Evidences, Frameworks, Risks, etc.)

### 2. إنشاء الجداول:

**الخيار الأول - تشغيل التطبيق لأول مرة (التلقائي):**
التطبيق سينشئ الجداول تلقائياً عند أول تشغيل إذا كان مُعد بشكل صحيح.

**الخيار الثاني - تشغيل DbMigrator يدوياً:**
```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.DbMigrator
dotnet run --configuration Release
```

**الخيار الثالث - تشغيل Migrations يدوياً:**
```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.EntityFrameworkCore
dotnet ef database update --startup-project ../Grc.Web --connection "Host=mainline.proxy.rlwy.net;Port=46662;Database=railway;Username=postgres;Password=sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ;SSL Mode=Require;Trust Server Certificate=true"
```

### 3. الجداول المطلوبة:
- `Evidences` أو `grc_evidences`
- `Frameworks` أو `grc_frameworks`
- `Controls` أو `grc_controls`
- `Regulators` أو `grc_regulators`
- `Risks` أو `grc_risks`
- `RiskTreatments` أو `grc_risk_treatments`

## الحالة الحالية (Current Status):

✅ التطبيق يعمل (الصفحة الرئيسية - HTTP 200)  
✅ API يعمل (HTTP 302 → swagger)  
❌ صفحات الوحدات (HTTP 500 - تحتاج جداول)  

## الخطوة التالية (Next Step):

سنقوم بإنشاء الجداول الآن...

---

**التاريخ:** 21 ديسمبر 2025  
**الحالة:** قيد الإصلاح



