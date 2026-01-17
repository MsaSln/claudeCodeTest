# Visual Studio ile Geliştirme Ortamı Kurulumu

Bu döküman, projeyi Visual Studio 2022 ve SQL Server LocalDB ile geliştirmek için gerekli adımları açıklar.

## Gereksinimler

- **Visual Studio 2022** (17.8 veya üzeri önerilir)
  - ASP.NET and web development workload
  - .NET 8.0 SDK
  - SQL Server LocalDB (Visual Studio ile birlikte gelir)

## 1. SQL Server LocalDB

Visual Studio 2022 kurulumu sırasında "ASP.NET and web development" workload'u seçtiyseniz, **SQL Server LocalDB** otomatik olarak yüklenir.

### LocalDB Kurulumu Kontrol

1. Visual Studio Installer'ı açın
2. "Modify" butonuna tıklayın
3. "Individual components" sekmesine gidin
4. "SQL Server Express LocalDB" seçili olmalı

### Alternatif: Manuel LocalDB Kurulumu

LocalDB yüklü değilse:
1. [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) sayfasına gidin
2. "Express" sürümünü indirin
3. Kurulum sırasında "LocalDB" seçeneğini işaretleyin

## 2. Visual Studio ile Projeyi Açma

1. Visual Studio 2022'yi açın
2. "Open a project or solution" seçin
3. `JwtBackendApi.sln` dosyasını seçin
4. Solution Explorer'da projenin yüklendiğini doğrulayın

## 3. NuGet Paketlerini Yükleme

Visual Studio projeyi açtığında NuGet paketlerini otomatik olarak yükler. Eğer yüklenmezse:

1. Solution Explorer'da Solution'a sağ tık
2. "Restore NuGet Packages" seçin

Veya Package Manager Console'da:

```powershell
Update-Package -reinstall
```

## 4. Veritabanı (Otomatik Oluşturma)

Proje **Entity Framework Core** kullanır ve veritabanı ilk çalıştırmada otomatik oluşturulur:

- Development ortamında `JwtBackendApi_Dev` veritabanı oluşur
- `DbInitializer` otomatik olarak seed data ekler

### Veritabanını Elle Oluşturmak İsterseniz

Package Manager Console'da (Tools → NuGet Package Manager → Package Manager Console):

```powershell
# Migration oluştur (zaten varsa atlayın)
Add-Migration InitialCreate

# Veritabanını oluştur/güncelle
Update-Database
```

## 5. Projeyi Çalıştırma

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

## 6. API'yi Test Etme

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

## 7. Veritabanını Görüntüleme

### SQL Server Object Explorer

1. Visual Studio'da View → SQL Server Object Explorer
2. SQL Server → (localdb)\MSSQLLocalDB → Databases → JwtBackendApi_Dev
3. Tables altında tüm tabloları görebilirsiniz

### SSMS (SQL Server Management Studio)

1. SSMS'i açın
2. Server name: `(localdb)\mssqllocaldb`
3. Authentication: Windows Authentication
4. Connect

## Sorun Giderme

### "Cannot connect to LocalDB" hatası

1. Windows Services'te "SQL Server (MSSQLLOCALDB)" servisini kontrol edin
2. Veya komut satırında:
```cmd
sqllocaldb info mssqllocaldb
sqllocaldb start mssqllocaldb
```

### "Database does not exist" hatası

Uygulama ilk çalıştırmada veritabanını otomatik oluşturur. Eğer oluşmadıysa:

```powershell
# Package Manager Console'da
Update-Database
```

### Migration hataları

```powershell
# Tüm migration'ları sil ve yeniden oluştur
Remove-Migration
Add-Migration InitialCreate
Update-Database
```

### LocalDB sürümü uyumsuzluğu

Connection string'deki LocalDB sürümünü kontrol edin:
- Visual Studio 2022: `(localdb)\mssqllocaldb`
- Eski sürümler: `(localdb)\v11.0` veya `(localdb)\ProjectsV13`

## Geliştirme İpuçları

### Hot Reload

Visual Studio 2022'de Hot Reload varsayılan olarak aktiftir. Kod değişikliklerini kaydettikten sonra uygulama otomatik olarak güncellenir.

### Database Seeding

`Data/DbInitializer.cs` dosyasında varsayılan veriler tanımlıdır. Yeni seed data eklemek için bu dosyayı düzenleyebilirsiniz.

### User Secrets (Önerilen)

Production şifrelerini kod tabanında saklamak yerine User Secrets kullanın:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=myserver;Database=JwtBackendApi;User Id=sa;Password=GercekSifre;TrustServerCertificate=true"
```

## Connection String Seçenekleri

### Development (LocalDB - Varsayılan)
```json
"Server=(localdb)\\mssqllocaldb;Database=JwtBackendApi_Dev;Trusted_Connection=true;MultipleActiveResultSets=true"
```

### SQL Server Express
```json
"Server=.\\SQLEXPRESS;Database=JwtBackendApi;Trusted_Connection=true;MultipleActiveResultSets=true"
```

### SQL Server (Windows Authentication)
```json
"Server=localhost;Database=JwtBackendApi;Trusted_Connection=true;TrustServerCertificate=true"
```

### SQL Server (SQL Authentication)
```json
"Server=localhost;Database=JwtBackendApi;User Id=sa;Password=YourPassword;TrustServerCertificate=true"
```

## Klasör Yapısı

```
JwtBackendApi/
├── Controllers/          # API endpoint'leri
├── Data/                 # DbContext ve seed data
├── Models/               # Entity ve DTO'lar
├── Services/             # İş mantığı katmanı
├── Properties/           # Launch ayarları
├── appsettings.json      # Production ayarları (SQL Server)
└── appsettings.Development.json  # Development ayarları (LocalDB)
```
