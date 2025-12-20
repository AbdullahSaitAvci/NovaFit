# 🏋️‍♂️ NovaFit - Spor Salonu Yönetim ve AI Koç Sistemi

**NovaFit**, spor salonu süreçlerini dijitalleştiren, üyelerin antrenörlerden randevu alabildiği, yöneticilerin sistemi tam yetkiyle yönetebildiği ve **Google Gemini Yapay Zeka** destekli kişisel koçluk hizmeti sunan kapsamlı bir **ASP.NET Core MVC** projesidir.

---

## 👨‍🎓 Proje Sahibi
* **Ad Soyad:** Abdullah Sait AVCI
* **Öğrenci No:** g231210035
* **Ders:** Web Programlama (2024-2025 Güz)

---

## 🚀 Projenin Temel Özellikleri

Bu proje, dönem ödevi isterlerinin **tamamını** karşılamaktadır:

### ✅ 1. Mimari ve Teknoloji
* **ASP.NET Core 8.0 MVC** mimarisi.
* **Entity Framework Core (Code-First)** ile veritabanı yönetimi.
* **MS SQL Server** veritabanı.
* **LINQ** sorguları ile veri işleme.
* **Bootstrap 5** ile responsive (mobil uyumlu) ve **Karanlık/Aydınlık (Dark Mode)** destekli modern arayüz.

### ✅ 2. Kullanıcı ve Rol Yönetimi (Identity)
* **Rol Bazlı Yetkilendirme (RBAC):** Admin, Trainer ve Member rolleri.
* **Güvenli Giriş:** Kullanıcı Kayıt/Giriş ve Şifreleme işlemleri.
* **Authorization:** Sayfalara ve aksiyonlara yetkisiz erişim engeli.

### ✅ 3. Yönetim Paneli (CRUD)
* **Antrenör Yönetimi:** Ekleme, Silme, Güncelleme, Resim Yükleme.
* **Hizmet Yönetimi:** Spor hizmetlerinin ve fiyatlarının yönetimi.
* **Randevu Onayı:** Gelen randevuların Admin tarafından onaylanması/reddedilmesi.

### ✅ 4. Yapay Zeka (AI) Entegrasyonu 🤖
* **Google Gemini API** kullanılarak geliştirilen "AI Koç" modülü.
* Kullanıcının fiziksel verilerine (Yaş, Kilo, Boy, Hedef) göre **kişiye özel beslenme ve antrenman programı** oluşturur.
* Hedeflenen vücut tipini **yapay zeka ile görselleştirir**.

### ✅ 5. API Hizmeti 🔌
* **RESTful API:** Eğitmen istatistiklerini ve uzmanlık alanlarını JSON formatında dış dünyaya sunan servis.
* **Endpoint:** `/api/ApiReports/TrainerStats`

---

## 🛠️ Kurulum ve Çalıştırma

1. **Projeyi İndirin:** Bu repoyu klonlayın veya ZIP olarak indirin.
2. **Veritabanı Bağlantısı:** `appsettings.json` dosyasındaki `ConnectionStrings` alanını kendi SQL Server'ınıza göre düzenleyin (Gerekirse).
3. **Veritabanını Oluşturun:**
   * **Seçenek A (Migration):** Package Manager Console'u açın ve `Update-Database` komutunu çalıştırın.
   * **Seçenek B (SQL Script):** Proje içindeki `Database/NovaFit_DbBackup.sql` dosyasını SQL Server'da çalıştırın.
4. **Çalıştırın:** Projeyi başlatın.

### 🔑 Varsayılan Yönetici (Admin) Hesabı
Veritabanı oluşturulduğunda otomatik olarak tanımlanan yönetici hesabı:
* **E-Posta:** `g231210035@sakarya.edu.tr`
* **Şifre:** `sau`

---

## 📸 Ekran Görüntüleri

Proje; Ana Sayfa, Admin Paneli, Randevu Sistemi ve AI Koç modülleriyle uçtan uca bir deneyim sunar.

*(Not: Bu README dosyası proje teslimi için hazırlanmıştır.)*