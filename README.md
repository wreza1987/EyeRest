# 👀 EyeRest - ابزار سبک یادآور استراحت چشم

یک برنامه فوق‌سبک، پرتابل و بدون نیاز به پیش‌نیاز برای ویندوز که در فواصل زمانی مشخص، تمام مانیتورها را به مدت چند ثانیه تاریک می‌کند تا به چشمان خود استراحت دهید.

An ultra-lightweight, standalone, zero-dependency Windows utility designed to prevent eye strain by periodically blanking your screens for a short break.

---

## ✨ ویژگی‌ها (Features)

* **فوق‌سبک و بدون پیش‌نیاز (Zero-Dependency & Portable):** اجرا به صورت یک فایل `.exe` مستقل (حدود ۲۰ کیلوبایت در نسخه C#) بدون نیاز به نصب هیچ برنامه یا کتابخانه جانبی.
* **پشتیبانی کامل از چند مانیتور (Multi-Monitor & High-DPI Support):** پوشش هم‌زمان و دقیق تمام مانیتورهای متصل به سیستم با هر رزولوشن و مقیاس تصویر (Scaling).
* **اجرا در پس‌زمینه (System Tray Integration):** قرارگیری در کنار ساعت ویندوز (System Tray) بدون اشغال فضای تسک‌بار، همراه با نمایش زمان باقی‌مانده با نگه‌داشتن ماوس روی آیکون.
* **قابلیت شخصی‌سازی (Customizable):** امکان تغییر فاصله زمانی کار (دقیقه)، مدت زمان استراحت (ثانیه) و رنگ صفحه استراحت.
* **تنظیم هوشمند کنتراست (Smart Contrast):** تغییر خودکار رنگ فونت شمارنده متناسب با تیرگی یا روشنی رنگ پس‌زمینه انتخاب‌شده.
* **لغو اضطراری (Instant Skip):** امکان بستن فوری صفحه استراحت در مواقع ضروری با فشردن کلید `Esc`.
* **ذخیره خودکار تنظیمات (Persistent Config):** ذخیره‌سازی تنظیمات در پوشه `AppData` ویندوز بدون ایجاد فایل اضافی در کنار فایل اجرایی.

---

## 🚀 نحوه استفاده (How to Use)

1. فایل `EyeRest.exe` را اجرا کنید.
2. فاصله زمانی (مثلاً ۳۰ دقیقه)، مدت استراحت (مثلاً ۳۰ ثانیه) و رنگ دلخواه خود را تنظیم کنید.
3. روی دکمه **«ذخیره و مخفی شدن»** کلیک کنید تا برنامه به کنار ساعت ویندوز (System Tray) منتقل شود.
4. هر زمان که صفحه استراحت ظاهر شد، در صورت نیاز فوری به سیستم می‌توانید با زدن کلید `Esc` آن را رد کنید.

---

## 🛠️ ساخت فایل اجرایی از سورس‌کد (Build from Source)

این پروژه به زبان **C#** (نسخه بومی و فوق‌سبک ویندوز) پیاده‌سازی شده است.

تمامی نسخه‌های ویندوز دارای کامپایلر توکار C# هستند. کافی است در `CMD` دستور زیر را در پوشه پروژه اجرا کنید:

```cmd
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:EyeRest.exe EyeRest.cs
```
خروجی یک فایل `EyeRest.exe` با حجم حدود **۲۰ کیلوبایت** خواهد بود.

---

## 🇬🇧 English Overview

**EyeRest** is a minimalist Windows utility that helps reduce digital eye strain. Every *N* minutes (default: 30), it covers all connected monitors with a customizable solid-color overlay for *M* seconds (default: 30) while displaying a clean countdown timer in the corner.

### Highlights:
- **Single-file `.exe`** (~20 KB in C#) — runs out of the box on any Windows PC.
- **Multi-monitor & DPI aware** — seamlessly covers all displays.
- **System Tray support** — runs silently in the background; hover over the tray icon to see the remaining time.
- **Quick Cancel** — press `Esc` at any time during a break to dismiss the overlay immediately.

---

## 📄 مجوز (License)

این پروژه تحت مجوز **MIT** منتشر شده است و استفاده، ویرایش و انتشار آن کاملاً آزاد است.
