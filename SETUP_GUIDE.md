# Geliştirici Ortamı Kurulum Kılavuzu

Bu kılavuz, JWT Backend API projesini sıfırdan yeni bir geliştirici ortamına kurmanız için gereken tüm adımları içerir.

## İçindekiler

1. [Sistem Gereksinimleri](#sistem-gereksinimleri)
2. [Kurulum Adımları](#kurulum-adımları)
3. [Database Kurulumu](#database-kurulumu)
4. [Uygulama Konfigürasyonu](#uygulama-konfigürasyonu)
5. [Uygulamayı Çalıştırma](#uygulamayı-çalıştırma)
6. [Test ve Doğrulama](#test-ve-doğrulama)
7. [IDE Kurulumu](#ide-kurulumu)
8. [Sorun Giderme](#sorun-giderme)

---

## Sistem Gereksinimleri

### Zorunlu Yazılımlar

- **.NET 8.0 SDK** (en az 8.0.0)
- **Git** (versiyon kontrolü için)
- **Docker Desktop** (veritabanı için - önerilen) VEYA **PostgreSQL 16+** (manuel kurulum)

### Önerilen Yazılımlar

- **Visual Studio Code** veya **Visual Studio 2022** veya **JetBrains Rider**
- **Postman** veya **Insomnia** (API test için)
- **pgAdmin 4** (database yönetimi için - Docker Compose ile gelir)

### Minimum Donanım

- RAM: 4 GB (8 GB önerilir)
- Disk: 2 GB boş alan
- İşlemci: x64 veya ARM64

---

## Kurulum Adımları

### Adım 1: .NET 8.0 SDK Kurulumu

#### Windows

1. [.NET Download](https://dotnet.microsoft.com/download/dotnet/8.0) sayfasını açın
2. "Download .NET 8.0 SDK" butonuna tıklayın
3. İndirilen dosyayı çalıştırın ve kurulum sihirbazını takip edin
4. Kurulumu doğrulayın:

```powershell
dotnet --version
# Çıktı: 8.0.x gibi bir versiyon görmelisiniz
```

#### macOS

```bash
# Homebrew ile
brew install --cask dotnet-sdk

# Doğrula
dotnet --version
```

#### Linux (Ubuntu/Debian)

```bash
# Microsoft paket deposunu ekle
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# .NET SDK kur
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# Doğrula
dotnet --version
```

### Adım 2: Git Kurulumu

#### Windows

1. [Git for Windows](https://git-scm.com/download/win) indir
2. Kurulum sihirbazını varsayılan ayarlarla tamamla
3. Git Bash veya PowerShell'i aç ve doğrula:

```bash
git --version
```

#### macOS

```bash
# Homebrew ile
brew install git

# Veya Xcode Command Line Tools ile
xcode-select --install
```

#### Linux

```bash
sudo apt-get install git
```

### Adım 3: Docker Desktop Kurulumu (Önerilen)

#### Windows

1. [Docker Desktop for Windows](https://www.docker.com/products/docker-desktop/) indir
2. Kurulumu tamamla ve sistemi yeniden başlat
3. Docker Desktop'ı başlat
4. Doğrula:

```powershell
docker --version
docker-compose --version
```

#### macOS

```bash
# Homebrew ile
brew install --cask docker

# Veya indirme sayfasından
# https://www.docker.com/products/docker-desktop/
```

#### Linux

```bash
# Docker kurulumu
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Docker Compose kurulumu
sudo apt-get install docker-compose-plugin

# Kullanıcıyı docker grubuna ekle
sudo usermod -aG docker $USER
newgrp docker

# Doğrula
docker --version
docker compose version
```

### Adım 4: Projeyi Klonlama

```bash
# Proje dizinine git (örnek)
cd ~/projects

# Projeyi klonla
git clone https://github.com/yourusername/claudeCodeTest.git
# VEYA SSH ile
git clone git@github.com:yourusername/claudeCodeTest.git

# Proje dizinine gir
cd claudeCodeTest

# Branch'i kontrol et
git status
git branch -a
```

---

## Database Kurulumu

### Yöntem 1: Docker ile Kurulum (Önerilen) ⭐

Bu yöntem en hızlı ve kolaydır. PostgreSQL ve pgAdmin otomatik olarak kurulur.

#### 1. Docker Compose ile Başlat

```bash
# Proje ana dizininde
cd claudeCodeTest

# Container'ları başlat (arka planda)
docker-compose up -d

# Logları kontrol et (isteğe bağlı)
docker-compose logs -f postgres

# Container'ların çalıştığını doğrula
docker-compose ps
```

**Çıktı şöyle olmalı:**
```
NAME                      STATUS              PORTS
jwtbackendapi-postgres    Up                  0.0.0.0:5432->5432/tcp
jwtbackendapi-pgadmin     Up                  0.0.0.0:5050->80/tcp
```

#### 2. Database'in Hazır Olduğunu Doğrula

```bash
# PostgreSQL'e bağlan
docker exec -it jwtbackendapi-postgres psql -U postgres -d jwtbackendapi

# Tabloları listele
\dt

# Çıkış
\q
```

**Beklenen çıktı:** 8 tablo görmeli siniz (users, user_groups, screens, vb.)

#### 3. pgAdmin'e Erişim (İsteğe Bağlı)

1. Tarayıcıda `http://localhost:5050` aç
2. Giriş bilgileri:
   - Email: `admin@example.com`
   - Password: `admin`
3. Server ekle:
   - Name: `JwtBackendAPI`
   - Host: `postgres` (Docker network içinde)
   - Port: `5432`
   - Database: `jwtbackendapi`
   - Username: `postgres`
   - Password: `postgres`

### Yöntem 2: Manuel PostgreSQL Kurulumu

Docker kullanmak istemiyorsanız PostgreSQL'i manuel kurabilirsiniz.

#### Windows

1. [PostgreSQL Windows İndirme](https://www.postgresql.org/download/windows/)
2. Kurulum sihirbazını takip edin
3. Şifre belirleyin (örn: `postgres`)
4. Port: `5432` (varsayılan)

#### macOS

```bash
brew install postgresql@16
brew services start postgresql@16

# Şifre belirle (isteğe bağlı)
psql postgres -c "ALTER USER postgres PASSWORD 'postgres';"
```

#### Linux

```bash
sudo apt install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo systemctl enable postgresql

# postgres kullanıcısına şifre ver
sudo -u postgres psql -c "ALTER USER postgres PASSWORD 'postgres';"
```

#### Database ve Tabloları Oluştur

```bash
# PostgreSQL'e bağlan
sudo -u postgres psql
# veya Windows'ta
psql -U postgres

# Database oluştur
CREATE DATABASE jwtbackendapi;

# Bağlan
\c jwtbackendapi

# Çıkış
\q

# SQL scriptini çalıştır
psql -U postgres -d jwtbackendapi -f database/init.sql
```

---

## Uygulama Konfigürasyonu

### Adım 1: NuGet Paketlerini Yükle

```bash
# Proje dizinine git
cd JwtBackendApi

# Paketleri restore et
dotnet restore
```

**Çıktı:** Tüm paketlerin başarıyla yüklendiğini görmelisiniz.

### Adım 2: Connection String'i Kontrol Et

Connection string'ler zaten yapılandırılmış durumda, ancak kontrol edin:

**appsettings.json** (Production):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=jwtbackendapi;Username=postgres;Password=postgres"
  }
}
```

**appsettings.Development.json** (Development):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=jwtbackendapi_dev;Username=postgres;Password=postgres"
  }
}
```

#### Özel PostgreSQL Ayarlarınız Varsa:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=YOUR_HOST;Port=YOUR_PORT;Database=jwtbackendapi;Username=YOUR_USER;Password=YOUR_PASSWORD"
  }
}
```

### Adım 3: JWT Secret Key'i Kontrol Et (Opsiyonel)

Production'da güçlü bir secret key kullanın:

```json
{
  "JwtSettings": {
    "SecretKey": "BURAYA_GÜÇLÜ_BİR_ŞİFRE_GİRİN_EN_AZ_32_KARAKTER",
    "Issuer": "JwtBackendApi",
    "Audience": "JwtBackendApiUsers",
    "ExpirationMinutes": 60
  }
}
```

---

## Uygulamayı Çalıştırma

### Adım 1: Build (Derleme)

```bash
# JwtBackendApi dizininde
dotnet build

# Hata yoksa başarılı çıktı görmelisiniz
```

### Adım 2: Uygulamayı Başlat

```bash
# Development modunda çalıştır
dotnet run

# VEYA Production modunda
dotnet run --environment Production
```

**Beklenen Çıktı:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7xxx
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5xxx
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Adım 3: Swagger UI'ye Erişim

1. Tarayıcıda aşağıdaki URL'yi açın:
   ```
   https://localhost:7xxx/swagger
   ```
   (xxx yerine konsol çıktısındaki port numarasını yazın)

2. Swagger UI sayfasını görmelisiniz

---

## Test ve Doğrulama

### 1. Health Check - Uygulama Çalışıyor mu?

Tarayıcıda veya Postman'de:
```
https://localhost:7xxx/swagger
```

### 2. Admin Kullanıcı ile Giriş Yapma

#### 2.1. Swagger UI Kullanarak

1. Swagger sayfasında `POST /api/auth/login` endpoint'ini açın
2. "Try it out" butonuna tıklayın
3. Request body:
   ```json
   {
     "username": "admin",
     "password": "Admin123!"
   }
   ```
4. "Execute" butonuna tıklayın
5. Response'dan `token` değerini kopyalayın

#### 2.2. Token ile Authenticate Olma

1. Swagger sayfasında sağ üstteki "Authorize" butonuna tıklayın
2. Açılan kutua şu formatta girin:
   ```
   Bearer KOPYALADIGINIZ_TOKEN
   ```
3. "Authorize" butonuna tıklayın

Artık tüm korumalı endpoint'leri test edebilirsiniz!

### 3. Örnek Test Senaryoları

#### Test 1: Kullanıcı Menülerini Getir
```
GET /api/menu/my-menus
```
Admin kullanıcısının erişebildiği tüm menüleri göreceksiniz.

#### Test 2: Kullanıcı İzinlerini Kontrol Et
```
GET /api/permission/my-permissions
```
Admin kullanıcısının tüm izinlerini göreceksiniz.

#### Test 3: Yeni Kullanıcı Kaydı
```
POST /api/auth/register
{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Test123!"
}
```

#### Test 4: User Groups Listesi (Admin Only)
```
GET /api/usergroup
```

### 4. Database Verilerini Kontrol Etme

```bash
# PostgreSQL'e bağlan
docker exec -it jwtbackendapi-postgres psql -U postgres -d jwtbackendapi

# Veya manuel kurulumda
psql -U postgres -d jwtbackendapi

# Kullanıcıları listele
SELECT id, username, email, role FROM users;

# User groups listele
SELECT id, name, description FROM user_groups;

# Menüleri listele
SELECT id, name, display_name, location FROM menus;

# Çıkış
\q
```

---

## IDE Kurulumu

### Visual Studio Code (Önerilen)

#### 1. VS Code Kurulumu

[VS Code İndir](https://code.visualstudio.com/)

#### 2. Gerekli Extension'ları Yükle

VS Code içinde Extensions (Ctrl+Shift+X) açın ve aşağıdakileri yükleyin:

1. **C# Dev Kit** (Microsoft)
2. **C#** (Microsoft)
3. **.NET Extension Pack** (Microsoft)
4. **REST Client** veya **Thunder Client** (API test için)
5. **Docker** (ms-azuretools.vscode-docker)
6. **PostgreSQL** (ckolkman.vscode-postgres)

#### 3. Projeyi Aç

```bash
code .
```

#### 4. Debug Konfigürasyonu

`.vscode/launch.json` dosyası otomatik oluşturulacaktır. Yoksa:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/JwtBackendApi/bin/Debug/net8.0/JwtBackendApi.dll",
      "args": [],
      "cwd": "${workspaceFolder}/JwtBackendApi",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      },
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "sourceFileMap": {
        "/Views": "${workspaceFolder}/Views"
      }
    }
  ]
}
```

#### 5. Debug Başlatma

- F5 tuşuna basın veya
- Run > Start Debugging

### Visual Studio 2022

1. Visual Studio 2022'yi başlatın
2. "Open a project or solution" seçin
3. `JwtBackendApi/JwtBackendApi.csproj` dosyasını seçin
4. F5 ile debug başlatın

### JetBrains Rider

1. Rider'ı başlatın
2. "Open" seçin
3. `JwtBackendApi.sln` veya proje klasörünü seçin
4. Otomatik konfigürasyon yapılacaktır
5. Shift+F10 ile çalıştırın

---

## Sorun Giderme

### 1. "Port already in use" Hatası

**Sorun:** 5432 veya diğer portlar kullanımda

**Çözüm:**
```bash
# Windows
netstat -ano | findstr :5432
taskkill /PID <PID> /F

# Linux/macOS
lsof -i :5432
kill -9 <PID>

# Veya Docker container'ı durdurun
docker-compose down
```

### 2. "Unable to connect to database" Hatası

**Kontrol Listesi:**

1. PostgreSQL çalışıyor mu?
   ```bash
   docker-compose ps
   # veya
   sudo systemctl status postgresql
   ```

2. Connection string doğru mu?
   ```bash
   cat JwtBackendApi/appsettings.Development.json
   ```

3. Database var mı?
   ```bash
   docker exec -it jwtbackendapi-postgres psql -U postgres -l
   ```

### 3. "Package restore failed" Hatası

**Çözüm:**
```bash
# Cache'i temizle
dotnet nuget locals all --clear

# Tekrar restore et
dotnet restore --force
```

### 4. Migration Hataları

**Çözüm:**
```bash
# Mevcut migration'ları sil
rm -rf JwtBackendApi/Migrations

# Database'i sıfırla
dotnet ef database drop --force

# Yeni migration oluştur
dotnet ef migrations add InitialCreate

# Database'i güncelle
dotnet ef database update
```

### 5. "DbInitializer" Seed Data Sorunları

**Kontrol:**
```bash
# Database'i tamamen sıfırla
docker-compose down -v
docker-compose up -d

# Veya
dotnet ef database drop --force
dotnet run
```

### 6. SSL Certificate Hataları (Development)

**Linux/macOS:**
```bash
dotnet dev-certs https --trust
```

**Windows:**
```powershell
dotnet dev-certs https --trust
```

### 7. Docker Permission Hataları (Linux)

```bash
sudo usermod -aG docker $USER
newgrp docker
```

---

## Ek Kaynaklar

### Faydalı Komutlar

```bash
# Tüm NuGet paketlerini güncelle
dotnet list package --outdated
dotnet add package <PackageName>

# Solution build
dotnet build

# Testleri çalıştır (varsa)
dotnet test

# Production build
dotnet publish -c Release

# Database migration
dotnet ef migrations add MigrationName
dotnet ef database update

# Docker logları
docker-compose logs -f

# Docker durumu
docker-compose ps
docker-compose down
docker-compose up -d --build
```

### Önemli URL'ler

- Swagger UI: `https://localhost:7xxx/swagger`
- pgAdmin: `http://localhost:5050`
- API Base URL: `https://localhost:7xxx/api`

### Default Credentials

**Admin User:**
- Username: `admin`
- Password: `Admin123!`

**PostgreSQL:**
- Host: `localhost`
- Port: `5432`
- Database: `jwtbackendapi`
- Username: `postgres`
- Password: `postgres`

**pgAdmin:**
- Email: `admin@example.com`
- Password: `admin`

---

## Kurulum Sonrası Yapılacaklar

### 1. Güvenlik Ayarları (Production)

- [ ] JWT SecretKey değiştir (appsettings.json)
- [ ] PostgreSQL şifresi değiştir
- [ ] CORS policy güncelle
- [ ] HTTPS'i zorunlu kıl
- [ ] Environment variables kullan

### 2. Git Konfigürasyonu

```bash
git config user.name "Your Name"
git config user.email "your.email@example.com"
```

### 3. Branch Stratejisi

```bash
# Development branch'ine geç
git checkout -b development

# Feature branch oluştur
git checkout -b feature/your-feature-name
```

### 4. Code Style (Opsiyonel)

`.editorconfig` dosyası projeye eklenebilir.

---

## Yardım ve Destek

### Dokümantasyon

- [README.md](README.md) - Proje genel bakış
- [DATABASE_SETUP.md](DATABASE_SETUP.md) - Database detaylı kurulum

### Loglar

```bash
# Uygulama logları
dotnet run --verbosity detailed

# Docker logları
docker-compose logs -f
```

### Hata Raporlama

Sorun yaşarsanız:
1. Logları kontrol edin
2. Database bağlantısını doğrulayın
3. NuGet paketlerini kontrol edin
4. Issue açın veya ekip liderinize bildirin

---

## Özet Checklist

Kurulumu başarıyla tamamladıysanız:

- [x] .NET 8.0 SDK kurulu ve çalışıyor
- [x] Git kurulu ve konfigüre edilmiş
- [x] Docker Desktop kurulu ve çalışıyor (veya PostgreSQL manuel kurulu)
- [x] Proje klonlanmış
- [x] Database ayakta ve tablolar oluşturulmuş
- [x] NuGet paketleri restore edilmiş
- [x] Uygulama başarıyla çalışıyor
- [x] Swagger UI erişilebiliyor
- [x] Admin kullanıcı ile giriş yapılabiliyor
- [x] API endpoint'leri test edilebiliyor
- [x] IDE kurulumu tamamlanmış

**Tebrikler! Geliştirme ortamınız hazır! 🎉**
