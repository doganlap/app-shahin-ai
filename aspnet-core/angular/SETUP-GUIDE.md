# دليل التثبيت والتشغيل السريع
# Quick Installation and Setup Guide

## 🚀 خطوات التثبيت | Installation Steps

### 1. تثبيت Node.js و npm

تأكد من تثبيت Node.js (الإصدار 18 أو أحدث):

```bash
# التحقق من الإصدار
node --version
npm --version
```

إذا لم يكن مثبتاً:
```bash
# Ubuntu/Debian
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs

# أو تحميل من الموقع الرسمي
# https://nodejs.org/
```

### 2. تثبيت Angular CLI

```bash
npm install -g @angular/cli@18
ng version
```

### 3. تثبيت حزم المشروع

```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/angular
npm install
```

### 4. تشغيل التطبيق

```bash
# تشغيل في وضع التطوير
npm start

# أو
ng serve --host 0.0.0.0 --port 4200
```

### 5. الوصول للتطبيق

افتح المتصفح وانتقل إلى:
```
http://localhost:4200
```

## 🏗️ البناء للإنتاج | Build for Production

### بناء التطبيق

```bash
npm run build:prod
```

سيتم إنشاء الملفات في مجلد `dist/grc-platform/`

### نشر التطبيق

```bash
# نسخ الملفات إلى خادم الويب
sudo cp -r dist/grc-platform/* /var/www/grc-platform/

# تعيين الصلاحيات
sudo chmod -R 755 /var/www/grc-platform/
sudo chown -R www-data:www-data /var/www/grc-platform/

# إعادة تحميل nginx
sudo systemctl reload nginx
```

## 🔧 إعداد Nginx

أنشئ ملف إعداد nginx:

```bash
sudo nano /etc/nginx/sites-available/grc-dashboard
```

أضف المحتوى التالي:

```nginx
server {
    listen 80;
    server_name 37.27.139.173;
    root /var/www/grc-platform;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

تفعيل الإعداد:

```bash
sudo ln -s /etc/nginx/sites-available/grc-dashboard /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

## 🐳 التشغيل باستخدام Docker (اختياري)

### إنشاء Dockerfile

```dockerfile
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build:prod

FROM nginx:alpine
COPY --from=build /app/dist/grc-platform /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### بناء وتشغيل

```bash
docker build -t grc-dashboard .
docker run -d -p 8080:80 grc-dashboard
```

## 🔍 استكشاف الأخطاء | Troubleshooting

### مشكلة: لا يمكن تثبيت الحزم

```bash
# حذف المجلدات القديمة
rm -rf node_modules package-lock.json
npm cache clean --force
npm install
```

### مشكلة: خطأ في البناء

```bash
# التحقق من إصدار Angular CLI
ng version

# تحديث Angular CLI
npm install -g @angular/cli@latest
```

### مشكلة: الخطوط العربية لا تظهر

تأكد من وجود اتصال بالإنترنت لتحميل خط Cairo من Google Fonts، أو قم بتنزيل الخط محلياً.

### مشكلة: API لا يستجيب

تحقق من:
1. تشغيل Backend API على المنفذ 5000
2. إعدادات CORS في Backend
3. إعدادات البروكسي في nginx

## 📊 المتغيرات البيئية | Environment Variables

عدّل الملفات في `src/environments/`:

**environment.ts** (Development):
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000',
  apiBaseUrl: '/api'
};
```

**environment.prod.ts** (Production):
```typescript
export const environment = {
  production: true,
  apiUrl: 'http://37.27.139.173:5000',
  apiBaseUrl: '/api'
};
```

## 🧪 الاختبار | Testing

```bash
# تشغيل الاختبارات الوحدوية
ng test

# تشغيل اختبارات E2E
ng e2e
```

## 📈 المراقبة والأداء | Monitoring

### تحليل حجم الحزمة

```bash
npm run build:prod -- --stats-json
npx webpack-bundle-analyzer dist/grc-platform/stats.json
```

### تحسين الأداء

1. تفعيل Lazy Loading للمودل
2. استخدام AOT Compilation
3. تفعيل Service Workers للـ PWA
4. ضغط الملفات في nginx

## 🔄 التحديثات | Updates

```bash
# تحديث Angular
ng update @angular/cli @angular/core

# تحديث الحزم
npm update

# التحقق من التحديثات المتاحة
npm outdated
```

## 📞 الدعم الفني | Technical Support

إذا واجهت أي مشاكل:

1. تحقق من السجلات (Logs):
   ```bash
   # Browser Console
   F12 -> Console
   
   # Terminal Logs
   npm start
   ```

2. راجع الوثائق:
   - Angular: https://angular.io/docs
   - Bootstrap RTL: https://getbootstrap.com/
   - Font Awesome: https://fontawesome.com/

3. تواصل مع فريق الدعم

---

✅ **جاهز للاستخدام!**
✅ **Ready to Use!**

