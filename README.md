# NovaFit - Spor Salonu Yönetim ve AI Koç Sistemi

Bu proje, Sakarya Üniversitesi Bilgisayar Mühendisliği bölümü Web Programlama dersi dönem projesi olarak geliştirilmiştir. Üyelerin antrenörlerden randevu alabildiği, yöneticilerin sistemi kontrol edebildiği ve yapay zeka destekli kişisel koçluk hizmeti sunan kapsamlı bir web uygulamasıdır.

## 👨‍🎓 Öğrenci Bilgileri

* **Ad Soyad:** [Adını Soyadını Buraya Yaz]
* **Öğrenci No:** g231210035
* **Ders Grubu:** [Varsa Grubun]

## 🚀 Projenin Özellikleri (Beklenenler Listesi Uyumu)

Proje, dönem ödevi isterlerinin tamamını karşılamaktadır:

* **Front-End:** Bootstrap 5 ile modern, responsive ve karanlık/aydınlık mod destekli arayüz.
* **CRUD İşlemleri:** Antrenörler, Hizmetler ve Randevular için tam Ekleme, Okuma, Güncelleme ve Silme yeteneği.
* **Identity & Auth:** Kullanıcı Kayıt/Giriş işlemleri ve Rol Bazlı Yetkilendirme (Admin, Trainer, Member).
* **Admin Paneli:** Sadece yöneticilerin erişebildiği Dashboard ve yönetim sayfaları.
* **AI Entegrasyonu:** Google Gemini API kullanılarak kişiye özel beslenme ve antrenman programı hazırlayan yapay zeka asistanı.
* **API Hizmeti:** Eğitmen istatistiklerini JSON formatında sunan LINQ destekli RESTful API (`/api/ApiReports/TrainerStats`).
* **Validation:** Hem sunucu (Server-side) hem istemci (Client-side) tarafında veri doğrulama.

## 🛠 Kullanılan Teknolojiler

* **Framework:** ASP.NET Core 8.0 MVC
* **Veritabanı:** MS SQL Server
* **ORM:** Entity Framework Core (Code-First)
* **Yapay Zeka:** Google Gemini Generative AI
* **Tasarım:** HTML5, CSS3, Bootstrap 5, JavaScript

## ⚙️ Kurulum ve Çalıştırma

1.  Projeyi klonlayın veya indirin.
2.  `appsettings.json` dosyasındaki Connection String'i kendi SQL Server yapınıza göre düzenleyin (Gerekirse).
3.  **Package Manager Console**'u açın ve veritabanını oluşturmak için şu komutu girin:
    ```powershell
    Update-Database
    ```
4.  Projeyi çalıştırın.

### 🔑 Varsayılan Admin Hesabı
Veritabanı oluştuğunda otomatik olarak şu yönetici hesabı tanımlanır:
* **Email:** g231210035@sakarya.edu.tr
* **Şifre:** sau

---