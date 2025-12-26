# منصة الحوكمة والمخاطر والامتثال (GRC)
# Saudi GRC Compliance Platform

## 🎯 نظرة عامة | Overview

منصة شاملة لإدارة الحوكمة والمخاطر والامتثال التنظيمي في المملكة العربية السعودية.
A comprehensive platform for Governance, Risk, and Compliance (GRC) management in Saudi Arabia.

## ✨ المميزات | Features

### لوحة التحكم المحسّنة | Enhanced Dashboard

- 📊 **بطاقات الإحصائيات**: عرض سريع للمقاييس الرئيسية
  - **Statistics Cards**: Quick overview of key metrics
  
- 📈 **مخططات التقدم**: تتبع التقدم حسب الإطار التنظيمي
  - **Progress Charts**: Track progress by regulatory framework
  
- ✅ **الضوابط المسندة**: إدارة المهام والضوابط المخصصة
  - **Assigned Controls**: Manage tasks and assigned controls
  
- ⏰ **المواعيد النهائية**: تنبيهات بالمواعيد القادمة
  - **Upcoming Deadlines**: Alerts for upcoming deadlines
  
- 🌍 **دعم اللغة العربية**: واجهة كاملة بالعربية مع دعم RTL
  - **Arabic Support**: Full Arabic interface with RTL support

### الأطر التنظيمية | Regulatory Frameworks

- SAMA (مؤسسة النقد العربي السعودي)
- NCA ECC (الهيئة الوطنية للأمن السيبراني)
- PDPL (نظام حماية البيانات الشخصية)
- وأكثر...

## 🚀 البدء | Getting Started

### المتطلبات | Prerequisites

```bash
Node.js >= 18.x
npm >= 9.x
Angular CLI >= 18.x
```

### التثبيت | Installation

```bash
# تثبيت الحزم
# Install packages
npm install

# تشغيل التطبيق
# Run the application
npm start

# بناء النسخة الإنتاجية
# Build for production
npm run build:prod
```

## 🌐 الوصول للتطبيق | Access

```
🌐 التطبيق | App:     http://37.27.139.173
📡 API:                http://37.27.139.173:5000
📚 Swagger API:        http://37.27.139.173:5000/swagger
```

### بيانات الدخول | Login Credentials

```
Username: admin
Password: 1q2w3E*
```

## 📁 هيكل المشروع | Project Structure

```
src/
├── app/
│   ├── features/
│   │   ├── dashboard/          # لوحة التحكم | Dashboard
│   │   ├── products/           # الباقات | Products
│   │   └── subscriptions/      # الاشتراكات | Subscriptions
│   ├── core/                   # الخدمات الأساسية | Core services
│   ├── shared/                 # المكونات المشتركة | Shared components
│   └── environments/           # البيئات | Environments
├── assets/                     # الملفات الثابتة | Static files
└── styles.scss                 # الأنماط العامة | Global styles
```

## 🎨 التصميم | Design

- **Framework**: Bootstrap 5 RTL
- **Icons**: Font Awesome 6
- **Font**: Cairo (Arabic)
- **Colors**: Modern gradient palette
- **Responsive**: Mobile-first design

## 🔧 التطوير | Development

### إضافة ميزة جديدة | Adding a New Feature

```bash
# إنشاء مكون جديد
# Create a new component
ng generate component features/my-feature

# إنشاء خدمة جديدة
# Create a new service
ng generate service features/my-feature/my-feature
```

### الأوامر المتاحة | Available Commands

```bash
npm start              # تشغيل التطبيق | Run development server
npm run build          # بناء التطبيق | Build the app
npm run build:prod     # بناء نسخة إنتاجية | Build for production
npm test               # تشغيل الاختبارات | Run tests
```

## 📊 لوحة التحكم | Dashboard Features

### 1. بطاقات الإحصائيات | Statistics Cards

- التقييمات النشطة | Active Assessments
- إجمالي الضوابط | Total Controls
- الضوابط المكتملة | Completed Controls
- الضوابط المتأخرة | Overdue Controls

### 2. مستوى الامتثال | Compliance Score

عرض دائري يوضح نسبة الامتثال الإجمالية مع مؤشر اللون:
- أخضر: 80% فأكثر | Green: 80%+
- أصفر: 60-79% | Yellow: 60-79%
- أحمر: أقل من 60% | Red: Below 60%

### 3. التقدم حسب الإطار | Framework Progress

مخططات شريطية ملونة تعرض:
- الضوابط المكتملة (أخضر) | Completed (Green)
- الضوابط قيد التنفيذ (أصفر) | In Progress (Yellow)
- الضوابط غير المبدوءة (رمادي) | Not Started (Gray)

### 4. الضوابط المسندة إليّ | My Assigned Controls

جدول تفاعلي يعرض:
- اسم الضابط | Control Name
- الإطار التنظيمي | Framework
- الحالة | Status
- تاريخ الاستحقاق | Due Date
- الإجراءات | Actions

### 5. المواعيد النهائية القادمة | Upcoming Deadlines

قائمة بالمواعيد مع:
- أيقونات ملونة حسب الأولوية | Color-coded icons by priority
- عداد الأيام المتبقية | Days remaining counter
- التاريخ بالتقويم الهجري/الميلادي | Hijri/Gregorian dates

## 🔐 الأمان | Security

- مصادقة JWT | JWT Authentication
- تشفير HTTPS | HTTPS Encryption
- حماية CORS | CORS Protection
- التحقق من الصلاحيات | Role-based Authorization

## 📱 التوافق | Compatibility

- ✅ Chrome, Edge, Firefox, Safari
- ✅ Desktop, Tablet, Mobile
- ✅ RTL (Right-to-Left) Support
- ✅ Arabic & English Languages

## 🤝 المساهمة | Contributing

نرحب بمساهماتكم! يرجى اتباع الخطوات التالية:
We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📄 الترخيص | License

© 2025 منصة الحوكمة والمخاطر والامتثال - المملكة العربية السعودية
© 2025 Saudi GRC Compliance Platform - Kingdom of Saudi Arabia

## 📞 الدعم | Support

للدعم والاستفسارات:
For support and inquiries:

- 📧 Email: support@grc-platform.sa
- 🌐 Website: http://37.27.139.173
- 📚 Documentation: http://37.27.139.173:5000/swagger

---

**Built with ❤️ for Saudi Arabia 🇸🇦**

