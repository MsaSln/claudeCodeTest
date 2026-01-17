# JWT Backend API

.NET Core 8.0 ile geliştirilmiş, JWT (JSON Web Token) authentication kullanan güvenli bir RESTful Web API projesi.

## Özellikler

- JWT Token tabanlı authentication
- Güvenli kullanıcı kaydı ve girişi
- Role-based authorization (Admin, User)
- RESTful API best practices
- Swagger/OpenAPI dokümantasyonu
- CORS desteği
- Model validasyonu
- Güvenli password hashing (SHA256)

## Teknolojiler

- .NET 8.0
- ASP.NET Core Web API
- JWT Bearer Authentication
- Swagger/OpenAPI
- Microsoft.AspNetCore.Authentication.JwtBearer
- System.IdentityModel.Tokens.Jwt

## Proje Yapısı

```
JwtBackendApi/
├── Controllers/
│   ├── AuthController.cs       # Kayıt ve giriş işlemleri
│   └── SecureController.cs     # Korumalı endpoint'ler
├── Models/
│   ├── User.cs                 # Kullanıcı modeli
│   ├── LoginDto.cs            # Giriş DTO
│   ├── RegisterDto.cs         # Kayıt DTO
│   ├── AuthResponse.cs        # Authentication cevap modeli
│   └── JwtSettings.cs         # JWT ayarları modeli
├── Services/
│   ├── IAuthService.cs        # Authentication servis interface
│   └── AuthService.cs         # Authentication servis implementasyonu
├── Program.cs                  # Uygulama başlangıç noktası
└── appsettings.json           # Konfigürasyon dosyası
```

## Kurulum

1. .NET 8.0 SDK'nın yüklü olduğundan emin olun:
```bash
dotnet --version
```

2. Bağımlılıkları yükleyin:
```bash
cd JwtBackendApi
dotnet restore
```

3. Uygulamayı çalıştırın:
```bash
dotnet run
```

Uygulama varsayılan olarak `https://localhost:7xxx` ve `http://localhost:5xxx` adreslerinde çalışacaktır.

## API Endpoints

### Authentication Endpoints (Public)

#### Kullanıcı Kaydı
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "User registered successfully",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-01-17T08:24:00Z",
  "user": {
    "id": 1,
    "username": "testuser",
    "email": "test@example.com",
    "role": "User"
  }
}
```

#### Kullanıcı Girişi
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-01-17T08:24:00Z",
  "user": {
    "id": 1,
    "username": "testuser",
    "email": "test@example.com",
    "role": "User"
  }
}
```

### Secure Endpoints (Authentication Required)

Tüm güvenli endpoint'ler için Authorization header'ında Bearer token gönderilmelidir:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Kullanıcı Profili
```http
GET /api/secure/profile
Authorization: Bearer {token}
```

**Response:**
```json
{
  "userId": "1",
  "username": "testuser",
  "email": "test@example.com",
  "role": "User",
  "message": "This is a secure endpoint. You are authenticated!"
}
```

#### Korumalı Veri
```http
GET /api/secure/data
Authorization: Bearer {token}
```

**Response:**
```json
{
  "message": "This is protected data",
  "data": [
    {
      "id": 1,
      "title": "Secure Item 1",
      "description": "This data requires authentication"
    }
  ],
  "accessedBy": "testuser",
  "accessedAt": "2026-01-17T07:24:00Z"
}
```

#### Admin Endpoint (Admin Role Required)
```http
GET /api/secure/admin
Authorization: Bearer {token}
```

**Response (Admin only):**
```json
{
  "message": "This is admin-only data",
  "adminInfo": "Only users with Admin role can access this endpoint",
  "accessedBy": "adminuser"
}
```

## JWT Konfigürasyonu

`appsettings.json` dosyasında JWT ayarlarını yapılandırabilirsiniz:

```json
{
  "JwtSettings": {
    "SecretKey": "ThisIsASecretKeyForJwtTokenGeneration12345678",
    "Issuer": "JwtBackendApi",
    "Audience": "JwtBackendApiUsers",
    "ExpirationMinutes": 60
  }
}
```

**Önemli:** Production ortamında:
- `SecretKey` değerini güçlü ve benzersiz bir değerle değiştirin
- Environment variables veya Azure Key Vault gibi güvenli bir yerde saklayın
- `RequireHttpsMetadata` ayarını `true` yapın

## Swagger UI

Uygulama çalıştığında Swagger UI'ye şu adresten erişebilirsiniz:
```
https://localhost:7xxx/swagger
```

Swagger UI'de:
1. `/api/auth/register` veya `/api/auth/login` endpoint'ini kullanarak token alın
2. Sağ üst köşedeki "Authorize" butonuna tıklayın
3. `Bearer {token}` formatında token'ı girin
4. Artık korumalı endpoint'leri test edebilirsiniz

## Güvenlik Özellikleri

- **JWT Token Authentication:** Her istek için token doğrulaması
- **Password Hashing:** Şifreler SHA256 ile hash'lenerek saklanır
- **Model Validation:** Gelen veriler DataAnnotations ile doğrulanır
- **Role-Based Authorization:** Farklı roller için farklı erişim seviyeleri
- **CORS Policy:** Cross-origin requests için yapılandırılabilir
- **HTTPS Redirection:** Güvenli iletişim için HTTP'den HTTPS'e yönlendirme

## HTTP Status Kodları

API aşağıdaki HTTP status kodlarını kullanır:

- `200 OK` - İstek başarılı
- `400 Bad Request` - Geçersiz istek verisi
- `401 Unauthorized` - Authentication gerekli veya token geçersiz
- `403 Forbidden` - Yetkisiz erişim (role problemi)
- `500 Internal Server Error` - Sunucu hatası

## Best Practices

Bu proje aşağıdaki industry best practices'i takip eder:

1. **RESTful Design:** Resource-based URL yapısı
2. **DTO Pattern:** Data Transfer Objects ile güvenli veri transferi
3. **Dependency Injection:** Loosely coupled mimari
4. **Interface Segregation:** Servisler için interface kullanımı
5. **Proper HTTP Methods:** GET, POST, PUT, DELETE doğru kullanımı
6. **Proper Status Codes:** Anlamlı HTTP status kodları
7. **API Documentation:** Swagger/OpenAPI ile detaylı dokümantasyon
8. **Validation:** Model state validation
9. **Logging:** Structured logging ile önemli olayların kaydı
10. **Error Handling:** Consistent error response formatı

## Notlar

- Bu proje demo amaçlıdır ve in-memory user storage kullanır
- Production ortamında Entity Framework Core ile gerçek bir veritabanı kullanılmalıdır
- Password hashing için BCrypt veya PBKDF2 tercih edilmelidir
- Refresh token mekanizması eklenebilir
- Rate limiting implementasyonu önerilir

## Lisans

Bu proje eğitim amaçlı oluşturulmuştur.
