# PostgreSQL Database Setup Guide

Bu dokümanda JWT Backend API projesi için PostgreSQL veritabanı kurulumu ve yapılandırması anlatılmaktadır.

## İçindekiler

1. [Gereksinimler](#gereksinimler)
2. [Docker ile Kurulum](#docker-ile-kurulum)
3. [Manuel Kurulum](#manuel-kurulum)
4. [Database Oluşturma](#database-oluşturma)
5. [Entity Framework Migrations](#entity-framework-migrations)
6. [Connection String Yapılandırması](#connection-string-yapılandırması)
7. [Veritabanı Şeması](#veritabanı-şeması)

## Gereksinimler

- PostgreSQL 13+ veya Docker
- .NET 8.0 SDK
- Entity Framework Core Tools

## Docker ile Kurulum (Önerilen)

Docker kullanarak PostgreSQL ve pgAdmin'i hızlıca başlatabilirsiniz.

### 1. Docker Compose ile Başlatma

```bash
# PostgreSQL ve pgAdmin'i başlat
docker-compose up -d

# Logları kontrol et
docker-compose logs -f postgres

# Durdurma
docker-compose down

# Verileri sil (dikkatli kullanın!)
docker-compose down -v
```

### 2. Container Bilgileri

**PostgreSQL:**
- Host: `localhost`
- Port: `5432`
- Database: `jwtbackendapi`
- Username: `postgres`
- Password: `postgres`

**pgAdmin (Web UI):**
- URL: `http://localhost:5050`
- Email: `admin@example.com`
- Password: `admin`

### 3. pgAdmin'de PostgreSQL Bağlantısı

1. `http://localhost:5050` adresine gidin
2. Email ve password ile giriş yapın
3. Sağ tıklayıp "Create > Server" seçin
4. **General** sekmesinde:
   - Name: `JwtBackendAPI`
5. **Connection** sekmesinde:
   - Host: `postgres` (Docker network içinde) veya `host.docker.internal`
   - Port: `5432`
   - Database: `jwtbackendapi`
   - Username: `postgres`
   - Password: `postgres`
6. Save

## Manuel Kurulum

### 1. PostgreSQL Kurulumu

**Ubuntu/Debian:**
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo systemctl enable postgresql
```

**macOS (Homebrew):**
```bash
brew install postgresql@16
brew services start postgresql@16
```

**Windows:**
- [PostgreSQL İndirme Sayfası](https://www.postgresql.org/download/windows/)

### 2. Kullanıcı ve Database Oluşturma

```bash
# PostgreSQL'e bağlan
sudo -u postgres psql

# Database oluştur
CREATE DATABASE jwtbackendapi;

# Kullanıcı oluştur (opsiyonel)
CREATE USER jwtapiuser WITH PASSWORD 'YourSecurePassword';
GRANT ALL PRIVILEGES ON DATABASE jwtbackendapi TO jwtapiuser;

# Çıkış
\q
```

## Database Oluşturma

### Yöntem 1: SQL Script ile

```bash
# SQL scriptini çalıştır
psql -U postgres -d jwtbackendapi -f database/init.sql

# Veya Docker container içinde
docker exec -i jwtbackendapi-postgres psql -U postgres -d jwtbackendapi < database/init.sql
```

### Yöntem 2: Entity Framework Migrations ile

```bash
cd JwtBackendApi

# Migration oluştur
dotnet ef migrations add InitialCreate

# Veritabanını güncelle
dotnet ef database update
```

### Yöntem 3: Uygulama Başlatıldığında Otomatik (DbInitializer)

Uygulama ilk çalıştırıldığında `DbInitializer.Initialize()` metodu veritabanını oluşturur ve seed data ekler.

```bash
cd JwtBackendApi
dotnet run
```

## Connection String Yapılandırması

### Development Ortamı

`appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=jwtbackendapi_dev;Username=postgres;Password=postgres"
  }
}
```

### Production Ortamı

`appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=jwtbackendapi;Username=postgres;Password=postgres"
  }
}
```

### Environment Variables (Önerilen - Production)

```bash
export ConnectionStrings__DefaultConnection="Host=your-server;Port=5432;Database=jwtbackendapi;Username=your-user;Password=your-password;SSL Mode=Require"
```

Docker'da:
```yaml
environment:
  - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=jwtbackendapi;Username=postgres;Password=postgres
```

## Veritabanı Şeması

### Tablolar

1. **users** - Kullanıcı bilgileri
2. **user_groups** - Kullanıcı grupları (roller)
3. **user_group_memberships** - Kullanıcı-grup ilişkileri
4. **screens** - Ekran tanımları
5. **screen_actions** - Ekran aksiyonları
6. **screen_permissions** - Grup-ekran-aksiyon izinleri
7. **menus** - Menü tanımları
8. **menu_items** - Menü öğeleri

### İlişkiler

```
users ─────< user_group_memberships >───── user_groups
                                                │
                                                │
                                                ▼
                                      screen_permissions
                                                │
                                                │
screens ──< screen_actions ◄────────────────────┘
  │
  │
  ▼
menu_items ◄─── menus
```

### Seed Data

Varsayılan veriler:
- **User Groups:** Administrators, Managers, Users
- **Screens:** Dashboard, Users, Settings
- **Screen Actions:** view, create, edit, delete, export
- **Admin User:** username: `admin`, password: `Admin123!`
- **Menus:** MainSidebar, TopHeader

## Entity Framework Komutları

### Migration Oluşturma

```bash
dotnet ef migrations add MigrationName
```

### Veritabanını Güncelleme

```bash
dotnet ef database update
```

### Migration Geri Alma

```bash
dotnet ef database update PreviousMigrationName
```

### Migration Silme

```bash
dotnet ef migrations remove
```

### SQL Script Oluşturma

```bash
dotnet ef migrations script
dotnet ef migrations script InitialCreate AddMenus
```

### Veritabanını Sıfırlama

```bash
dotnet ef database drop
dotnet ef database update
```

## Veritabanı Bakımı

### Backup

```bash
# Backup oluştur
pg_dump -U postgres jwtbackendapi > backup.sql

# Docker ile
docker exec jwtbackendapi-postgres pg_dump -U postgres jwtbackendapi > backup.sql
```

### Restore

```bash
# Restore
psql -U postgres jwtbackendapi < backup.sql

# Docker ile
docker exec -i jwtbackendapi-postgres psql -U postgres jwtbackendapi < backup.sql
```

### Veritabanı Boyutunu Kontrol Etme

```sql
SELECT pg_size_pretty(pg_database_size('jwtbackendapi'));
```

## Performans İyileştirme

### Index Kontrolü

```sql
-- Eksik indexleri tespit et
SELECT schemaname, tablename, indexname
FROM pg_indexes
WHERE schemaname = 'public'
ORDER BY tablename, indexname;
```

### Query Performansı

```sql
-- Slow query log
EXPLAIN ANALYZE SELECT * FROM users WHERE username = 'admin';
```

### Connection Pooling

`appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=jwtbackendapi;Username=postgres;Password=postgres;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100"
  }
}
```

## Sorun Giderme

### Bağlantı Hatası

```bash
# PostgreSQL çalışıyor mu?
sudo systemctl status postgresql

# Port dinleniyor mu?
sudo netstat -plnt | grep 5432

# Docker logları
docker logs jwtbackendapi-postgres
```

### Permission Hataları

```sql
-- Kullanıcıya yetki ver
GRANT ALL PRIVILEGES ON DATABASE jwtbackendapi TO postgres;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO postgres;
```

### Migration Hataları

```bash
# Migration tablosunu sıfırla
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Güvenlik Önerileri

1. **Güçlü Şifreler Kullanın:** Varsayılan `postgres` şifresini değiştirin
2. **SSL/TLS Kullanın:** Production'da SSL bağlantısı zorunlu olmalı
3. **Firewall Kuralları:** PostgreSQL portunu sadece gerekli IP'lere açın
4. **Role-Based Access:** Her uygulama için ayrı kullanıcı oluşturun
5. **Environment Variables:** Connection string'i environment variable'da saklayın
6. **Backup:** Düzenli backup alın
7. **Monitoring:** Veritabanı performansını izleyin

## Faydalı Komutlar

### PostgreSQL CLI

```bash
# Veritabanına bağlan
psql -U postgres -d jwtbackendapi

# Tabloları listele
\dt

# Tablo detayını gör
\d users

# Sorgu çalıştır
SELECT COUNT(*) FROM users;

# Çıkış
\q
```

### Docker Komutları

```bash
# Container'a gir
docker exec -it jwtbackendapi-postgres bash

# PostgreSQL CLI aç
docker exec -it jwtbackendapi-postgres psql -U postgres -d jwtbackendapi

# Container'ı yeniden başlat
docker restart jwtbackendapi-postgres
```

## Kaynaklar

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Npgsql Documentation](https://www.npgsql.org/doc/)
- [Docker PostgreSQL](https://hub.docker.com/_/postgres)
