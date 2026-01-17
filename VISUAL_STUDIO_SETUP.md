# Visual Studio ile Geliştirme Ortamı Kurulumu

Bu döküman, projeyi Docker kullanmadan Visual Studio ile geliştirmek için gerekli adımları açıklar.

## Gereksinimler

- **Visual Studio 2022** (17.8 veya üzeri önerilir)
  - ASP.NET and web development workload
  - .NET 8.0 SDK
- **PostgreSQL 16** (veya uyumlu bir sürüm)

## 1. PostgreSQL Kurulumu

### Windows için PostgreSQL Kurulumu

1. [PostgreSQL İndirme Sayfası](https://www.postgresql.org/download/windows/)'na gidin
2. "Download the installer" butonuna tıklayın
3. En son PostgreSQL 16.x sürümünü indirin
4. Kurulum sihirbazını başlatın:
   - Kurulum dizinini seçin (varsayılan: `C:\Program Files\PostgreSQL\16`)
   - Veri dizinini seçin (varsayılan: `C:\Program Files\PostgreSQL\16\data`)
   - **Superuser şifresi** belirleyin (bu şifreyi unutmayın!)
   - Port: `5432` (varsayılan)
   - Locale: Turkish, Turkey veya Default locale
5. Stack Builder'ı atlayabilirsiniz (isteğe bağlı araçlar)

### pgAdmin 4 (Opsiyonel)

PostgreSQL kurulumu ile birlikte pgAdmin 4 de yüklenir. Bu araç ile veritabanınızı görsel olarak yönetebilirsiniz.

## 2. Veritabanı Oluşturma

### pgAdmin ile:

1. pgAdmin 4'ü açın
2. Sol panelde "Servers" > "PostgreSQL 16" > sağ tık > "Connect Server"
3. Şifrenizi girin
4. "Databases" > sağ tık > "Create" > "Database"
5. Database name: `jwtbackendapi_dev` (Development için)
6. "Save" butonuna tıklayın

### psql ile (Komut Satırı):

```bash
# PostgreSQL'e bağlan
psql -U postgres

# Veritabanını oluştur
CREATE DATABASE jwtbackendapi_dev;

# Çıkış
\q
```

## 3. Connection String Yapılandırması

`JwtBackendApi/appsettings.Development.json` dosyasını açın ve PostgreSQL şifrenizi güncelleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=jwtbackendapi_dev;Username=postgres;Password=SizinSifreniz"
  }
}
```

**NOT:** `YourPasswordHere` yerine PostgreSQL kurulumunda belirlediğiniz şifreyi yazın.

## 4. Visual Studio ile Projeyi Açma

1. Visual Studio 2022'yi açın
2. "Open a project or solution" seçin
3. `JwtBackendApi.sln` dosyasını seçin
4. Solution Explorer'da projenin yüklendiğini doğrulayın

## 5. NuGet Paketlerini Yükleme

Visual Studio projeyi açtığında NuGet paketlerini otomatik olarak yükler. Eğer yüklenmezse:

1. Solution Explorer'da Solution'a sağ tık
2. "Restore NuGet Packages" seçin

Veya Package Manager Console'da:

```powershell
Update-Package -reinstall
```

## 6. Veritabanı Migration (İlk Çalıştırma)

Proje ilk çalıştırıldığında `DbInitializer` otomatik olarak:
- Tüm tabloları oluşturur
- Varsayılan verileri (seed data) ekler

Eğer manuel migration yapmak isterseniz, Package Manager Console'da:

```powershell
# Migration oluştur
Add-Migration InitialCreate

# Veritabanını güncelle
Update-Database
```

## 7. Projeyi Çalıştırma

### Visual Studio ile:

1. Üst menüden launch profile seçin:
   - **JwtBackendApi** (HTTPS - önerilen)
   - **JwtBackendApi (HTTP)** (sadece HTTP)
   - **IIS Express**
2. F5 tuşuna basın veya yeşil "Start" butonuna tıklayın
3. Tarayıcı otomatik olarak Swagger UI'ı açacaktır

### Komut Satırı ile:

```bash
cd JwtBackendApi
dotnet run
```

## 8. API'yi Test Etme

Uygulama başlatıldığında:

- **Swagger UI:** https://localhost:7262/swagger (veya http://localhost:5035/swagger)
- **API Base URL:** https://localhost:7262/api

### İlk Test - Kullanıcı Kaydı:

```http
POST https://localhost:7262/api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Test123!"
}
```

### Giriş:

```http
POST https://localhost:7262/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!"
}
```

## Varsayılan Kullanıcı

Sistem otomatik olarak bir admin kullanıcısı oluşturur:

| Alan | Değer |
|------|-------|
| Kullanıcı Adı | admin |
| E-posta | admin@example.com |
| Şifre | Admin123! |

## Sorun Giderme

### "Connection refused" hatası

- PostgreSQL servisinin çalıştığından emin olun:
  - Windows Services'te "postgresql-x64-16" servisini kontrol edin
  - Servis durmuşsa başlatın

### "Password authentication failed" hatası

- `appsettings.Development.json` dosyasındaki şifrenin doğru olduğundan emin olun
- PostgreSQL şifresini sıfırlamanız gerekebilir

### "Database does not exist" hatası

- `jwtbackendapi_dev` veritabanını oluşturduğunuzdan emin olun
- pgAdmin veya psql ile kontrol edin

### Port çakışması

Eğer 5432 portu kullanımdaysa, `appsettings.Development.json` dosyasında farklı bir port belirtin ve PostgreSQL'i de aynı porta yapılandırın.

## Geliştirme İpuçları

### Hot Reload

Visual Studio 2022'de Hot Reload varsayılan olarak aktiftir. Kod değişikliklerini kaydettikten sonra uygulama otomatik olarak güncellenir.

### Database Seeding

`Data/DbInitializer.cs` dosyasında varsayılan veriler tanımlıdır. Yeni seed data eklemek için bu dosyayı düzenleyebilirsiniz.

### User Secrets (Önerilen)

Production şifrelerini kod tabanında saklamak yerine User Secrets kullanın:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=jwtbackendapi_dev;Username=postgres;Password=GercekSifre"
```

## Klasör Yapısı

```
JwtBackendApi/
├── Controllers/          # API endpoint'leri
├── Data/                 # DbContext ve seed data
├── Models/               # Entity ve DTO'lar
├── Services/             # İş mantığı katmanı
├── Properties/           # Launch ayarları
├── appsettings.json      # Production ayarları
└── appsettings.Development.json  # Development ayarları
```
