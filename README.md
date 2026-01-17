# JWT Backend API with Role & Permission Management

.NET Core 8.0 ile geliştirilmiş, JWT (JSON Web Token) authentication ve kapsamlı rol/yetki yönetimi kullanan güvenli bir RESTful Web API projesi.

## Özellikler

### Authentication & Authorization
- JWT Token tabanlı authentication
- Güvenli kullanıcı kaydı ve girişi
- Role-based authorization (Admin, User, vb.)
- Güvenli password hashing (SHA256)

### Rol ve Yetki Yönetimi
- **Kullanıcı Grupları (User Groups):** Esnek rol tanımlama sistemi
- **Çoklu Grup Üyeliği:** Bir kullanıcı birden fazla gruba dahil olabilir
- **Ekran Bazlı Yetkiler:** Her ekran için özel aksiyonlar tanımlayabilme
- **Granüler İzin Kontrolü:** Ekran-aksiyon seviyesinde detaylı yetkilendirme
- **Dinamik Yetki Yönetimi:** Çalışma zamanında yetki atama/kaldırma

### Menü Yönetimi
- **Dinamik Menü Sistemi:** Esnek sidebar ve header menü yapısı
- **Hiyerarşik Menüler:** Grup ve alt menü desteği
- **Yetki Bazlı Görünürlük:** Kullanıcının yetkili olduğu ekranlar otomatik filtrelenir
- **Konum Kontrolü:** Menülerin sidebar veya header'da gösterilmesi
- **Grup ve Ekran Desteği:** Menüye hem grup (kategori) hem de direkt ekran eklenebilir
- **Badge Desteği:** Menü öğelerine badge/etiket eklenebilir

### Teknik Özellikler
- RESTful API best practices
- Comprehensive Swagger/OpenAPI dokümantasyonu
- CORS desteği
- Model validasyonu (DataAnnotations)
- Structured logging
- Dependency Injection

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
│   ├── AuthController.cs           # Kayıt ve giriş işlemleri
│   ├── SecureController.cs         # Örnek korumalı endpoint'ler
│   ├── UserGroupController.cs      # Kullanıcı grubu yönetimi
│   ├── ScreenController.cs         # Ekran ve aksiyon yönetimi
│   ├── PermissionController.cs     # İzin yönetimi ve kontrol
│   └── MenuController.cs           # Menü yönetimi
├── Models/
│   ├── User.cs                     # Kullanıcı modeli
│   ├── UserGroup.cs                # Kullanıcı grubu modeli
│   ├── Screen.cs                   # Ekran modeli
│   ├── ScreenAction.cs             # Ekran aksiyonu modeli
│   ├── ScreenPermission.cs         # İzin modeli
│   ├── UserGroupMembership.cs      # Kullanıcı-grup ilişkisi
│   ├── Menu.cs                     # Menü modeli
│   ├── MenuItem.cs                 # Menü elemanı modeli
│   ├── MenuLocation.cs             # Menü konumu (Sidebar/Header)
│   ├── MenuItemType.cs             # Menü elemanı tipi (Group/Screen)
│   ├── JwtSettings.cs              # JWT ayarları
│   └── DTOs/                       # Data Transfer Objects
│       ├── UserGroupDto.cs
│       ├── ScreenDto.cs
│       ├── ScreenActionDto.cs
│       ├── PermissionDto.cs
│       └── MenuDto.cs
├── Services/
│   ├── IAuthService.cs             # Authentication servis interface
│   ├── AuthService.cs              # Authentication servisi
│   ├── IUserGroupService.cs        # User group servis interface
│   ├── UserGroupService.cs         # User group servisi
│   ├── IScreenService.cs           # Screen servis interface
│   ├── ScreenService.cs            # Screen servisi
│   ├── IPermissionService.cs       # Permission servis interface
│   ├── PermissionService.cs        # Permission servisi
│   ├── IMenuService.cs             # Menu servis interface
│   └── MenuService.cs              # Menu servisi
├── Program.cs                      # Uygulama başlangıç noktası
└── appsettings.json               # Konfigürasyon dosyası
```

## Veri Modeli

### İlişkisel Yapı

```
User (1) ─────< (N) UserGroupMembership (N) >───── (1) UserGroup
                                                           │
                                                           │ (1)
                                                           │
                                                           ▼
Screen (1) ──< (N) ScreenAction (1)            ScreenPermission
     │                  ▲                             │
     │                  │                             │
     └──────────────────┴─────────────────────────────┘
```

### Temel Modeller

- **User:** Kullanıcı bilgileri
- **UserGroup:** Rol/grup tanımları (Administrators, Managers, Users, vb.)
- **UserGroupMembership:** Kullanıcı-grup ilişkileri (many-to-many)
- **Screen:** Ekran/sayfa tanımları (Dashboard, Users, Settings, vb.)
- **ScreenAction:** Ekran aksiyonları (view, create, edit, delete, export, vb.)
- **ScreenPermission:** Grup-ekran-aksiyon izinleri

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

### 1. Authentication (Public)

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

#### Kullanıcı Girişi
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}
```

### 2. User Group Management (Admin Only)

#### Tüm Grupları Listele
```http
GET /api/usergroup
Authorization: Bearer {token}
```

#### Yeni Grup Oluştur
```http
POST /api/usergroup
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Developers",
  "description": "Development team members",
  "isActive": true
}
```

#### Grup Güncelle
```http
PUT /api/usergroup/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Senior Developers",
  "description": "Senior development team",
  "isActive": true
}
```

#### Grup Sil
```http
DELETE /api/usergroup/{id}
Authorization: Bearer {token}
```

### 3. Screen Management (Admin Only)

#### Tüm Ekranları Listele
```http
GET /api/screen
Authorization: Bearer {token}
```

#### Yeni Ekran Oluştur
```http
POST /api/screen
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Reports",
  "displayName": "Raporlar",
  "description": "Rapor ekranları",
  "route": "/reports",
  "icon": "chart",
  "order": 4,
  "isActive": true
}
```

#### Ekrana Aksiyon Ekle
```http
POST /api/screen/actions
Authorization: Bearer {token}
Content-Type: application/json

{
  "screenId": 1,
  "actionName": "export",
  "displayName": "Dışa Aktar",
  "description": "Verileri dışa aktarma yetkisi",
  "isActive": true
}
```

### 4. Permission Management (Admin Only)

#### Gruba İzin Ata
```http
POST /api/permission/assign
Authorization: Bearer {token}
Content-Type: application/json

{
  "userGroupId": 2,
  "screenId": 1,
  "screenActionIds": [1, 2, 3],
  "isGranted": true
}
```

#### Kullanıcıyı Gruplara Ata
```http
POST /api/permission/user/assign-groups
Authorization: Bearer {token}
Content-Type: application/json

{
  "userId": 1,
  "userGroupIds": [2, 3]
}
```

#### Kullanıcının İzinlerini Görüntüle
```http
GET /api/permission/user/{userId}/screens
Authorization: Bearer {token}
```

**Response:**
```json
{
  "userId": 1,
  "username": "testuser",
  "screens": [
    {
      "screenId": 1,
      "screenName": "Dashboard",
      "displayName": "Ana Sayfa",
      "route": "/dashboard",
      "icon": "dashboard",
      "allowedActions": ["view", "export"]
    },
    {
      "screenId": 2,
      "screenName": "Users",
      "displayName": "Kullanıcılar",
      "route": "/users",
      "icon": "people",
      "allowedActions": ["view", "create", "edit"]
    }
  ]
}
```

#### İzin Kontrolü
```http
POST /api/permission/check
Authorization: Bearer {token}
Content-Type: application/json

{
  "userId": 1,
  "screenName": "Users",
  "actionName": "create"
}
```

**Response:**
```json
{
  "hasPermission": true,
  "message": "User has permission to create on Users",
  "userGroups": ["Managers", "Users"]
}
```

#### Mevcut Kullanıcının İzinlerini Getir
```http
GET /api/permission/my-permissions
Authorization: Bearer {token}
```

#### Basit İzin Kontrolü
```http
GET /api/permission/can-access/Users/create
Authorization: Bearer {token}
```

**Response:**
```json
{
  "screenName": "Users",
  "actionName": "create",
  "hasPermission": true,
  "message": "Access granted"
}
```

### 5. Menu Management (Admin Only)

#### Tüm Menüleri Listele
```http
GET /api/menu
Authorization: Bearer {token}
```

#### Yeni Menü Oluştur
```http
POST /api/menu
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "AdminMenu",
  "displayName": "Admin Menüsü",
  "description": "Yönetici menüsü",
  "location": 1,  // 1: Sidebar, 2: Header
  "order": 1,
  "isActive": true
}
```

#### Menüye Ekran Ekle
```http
POST /api/menu/items
Authorization: Bearer {token}
Content-Type: application/json

{
  "menuId": 1,
  "itemType": 2,  // 1: Group, 2: Screen
  "screenId": 2,
  "displayName": "Kullanıcılar",
  "icon": "people",
  "route": "/users",
  "order": 1,
  "isActive": true
}
```

#### Menüye Grup (Kategori) Ekle
```http
POST /api/menu/items
Authorization: Bearer {token}
Content-Type: application/json

{
  "menuId": 1,
  "itemType": 1,  // Group
  "displayName": "Yönetim",
  "icon": "settings",
  "order": 2,
  "isActive": true
}
```

#### Grup Altına Ekran Ekle
```http
POST /api/menu/items
Authorization: Bearer {token}
Content-Type: application/json

{
  "menuId": 1,
  "parentMenuItemId": 2,  // Yönetim grubunun ID'si
  "itemType": 2,  // Screen
  "screenId": 3,
  "displayName": "Ayarlar",
  "icon": "settings",
  "route": "/settings",
  "order": 1,
  "isActive": true
}
```

#### Kullanıcının Menülerini Getir (Yetki Filtreli)
```http
GET /api/menu/my-menus
Authorization: Bearer {token}
```

**Response:**
```json
{
  "sidebarMenus": [
    {
      "id": 1,
      "name": "MainSidebar",
      "displayName": "Ana Menü",
      "location": 1,
      "order": 1,
      "items": [
        {
          "id": 1,
          "displayName": "Ana Sayfa",
          "icon": "dashboard",
          "route": "/dashboard",
          "itemType": 2,
          "screenId": 1,
          "hasPermission": true,
          "children": []
        },
        {
          "id": 2,
          "displayName": "Yönetim",
          "icon": "settings",
          "itemType": 1,
          "hasPermission": true,
          "children": [
            {
              "id": 3,
              "displayName": "Kullanıcılar",
              "icon": "people",
              "route": "/users",
              "screenId": 2,
              "hasPermission": true
            }
          ]
        }
      ]
    }
  ],
  "headerMenus": []
}
```

## Kullanım Senaryoları

### Senaryo 1: Yeni Bir Rol Oluşturma ve Yetkilendirme

```bash
# 1. Yeni bir grup oluştur
POST /api/usergroup
{
  "name": "Content Editors",
  "description": "İçerik düzenleme ekibi"
}

# 2. Gruba ekran yetkilerini ata
POST /api/permission/assign
{
  "userGroupId": 4,
  "screenId": 5,  # Blog ekranı
  "screenActionIds": [1, 2, 3],  # view, create, edit
  "isGranted": true
}

# 3. Kullanıcıyı bu gruba ekle
POST /api/permission/user/assign-groups
{
  "userId": 10,
  "userGroupIds": [4]
}
```

### Senaryo 2: Dinamik Menü Oluşturma

```javascript
// Frontend tarafında kullanıcının erişebileceği menüleri getir
const response = await fetch('/api/permission/my-permissions', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});

const data = await response.json();

// data.screens ile dinamik menü oluştur
const menu = data.screens.map(screen => ({
  name: screen.displayName,
  route: screen.route,
  icon: screen.icon,
  actions: screen.allowedActions
}));
```

### Senaryo 3: Aksiyon Bazlı Buton Kontrolü

```javascript
// Belirli bir aksiyona erişim kontrolü
const canCreate = await fetch(
  '/api/permission/can-access/Users/create',
  {
    headers: { 'Authorization': `Bearer ${token}` }
  }
);

const result = await canCreate.json();

if (result.hasPermission) {
  // "Yeni Kullanıcı Ekle" butonunu göster
}
```

### Senaryo 4: Dinamik Menü Sistemi Kurulumu

```bash
# 1. Yeni bir menü oluştur (Sidebar)
POST /api/menu
{
  "name": "MainSidebar",
  "displayName": "Ana Menü",
  "location": 1,  # Sidebar
  "order": 1
}

# 2. Ana menüye dashboard ekranı ekle
POST /api/menu/items
{
  "menuId": 1,
  "itemType": 2,  # Screen
  "screenId": 1,
  "displayName": "Ana Sayfa",
  "icon": "dashboard",
  "order": 1
}

# 3. "Yönetim" grubu oluştur
POST /api/menu/items
{
  "menuId": 1,
  "itemType": 1,  # Group
  "displayName": "Yönetim",
  "icon": "settings",
  "order": 2
}

# 4. Yönetim grubu altına ekranlar ekle
POST /api/menu/items
{
  "menuId": 1,
  "parentMenuItemId": 2,  # Yönetim grubu
  "itemType": 2,
  "screenId": 2,
  "displayName": "Kullanıcılar",
  "order": 1
}

# 5. Frontend'de kullanıcının menüsünü al
GET /api/menu/my-menus
# Kullanıcının yetkili olduğu ekranlar otomatik filtrelenir
```

### Senaryo 5: Frontend Menü Entegrasyonu

```javascript
// Kullanıcının menülerini al (yetki filtreli)
const response = await fetch('/api/menu/my-menus', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});

const menuData = await response.json();

// Sidebar menüsünü render et
const sidebarMenu = menuData.sidebarMenus[0];

function renderMenu(items) {
  return items.map(item => {
    if (item.itemType === 1) { // Group
      return {
        label: item.displayName,
        icon: item.icon,
        items: renderMenu(item.children) // Alt öğeleri render et
      };
    } else { // Screen
      return {
        label: item.displayName,
        icon: item.icon,
        to: item.route,
        badge: item.badgeText,
        badgeColor: item.badgeColor
      };
    }
  });
}

const menuStructure = renderMenu(sidebarMenu.items);
```

## Varsayılan Veriler

Sistem şu varsayılan verilerle başlar:

### User Groups
1. **Administrators** - Tam sistem erişimi
2. **Managers** - Yönetici seviyesi erişim
3. **Users** - Standart kullanıcı erişimi

### Screens
1. **Dashboard** - Ana sayfa
2. **Users** - Kullanıcı yönetimi
3. **Settings** - Sistem ayarları

### Screen Actions
- **view** - Görüntüleme
- **create** - Oluşturma
- **edit** - Düzenleme
- **delete** - Silme
- **export** - Dışa aktarma

### Menus
1. **MainSidebar** - Sol taraf ana menü (Sidebar)
2. **TopHeader** - Üst header menüsü (Header)

### Menu Items (MainSidebar)
- Ana Sayfa (Screen - Dashboard)
- Yönetim (Group)
  - Kullanıcılar (Screen - Users)
  - Ayarlar (Screen - Settings)

## Güvenlik Özellikleri

- **JWT Token Authentication:** Her istek için token doğrulaması
- **Password Hashing:** Şifreler SHA256 ile hash'lenerek saklanır
- **Role-Based Authorization:** Controller seviyesinde rol kontrolü
- **Granular Permissions:** Ekran-aksiyon bazında detaylı yetkilendirme
- **Model Validation:** DataAnnotations ile veri doğrulama
- **CORS Policy:** Yapılandırılabilir CORS politikası
- **HTTPS Redirection:** Güvenli iletişim

## HTTP Status Kodları

- `200 OK` - İstek başarılı
- `201 Created` - Kaynak oluşturuldu
- `400 Bad Request` - Geçersiz istek
- `401 Unauthorized` - Authentication gerekli
- `403 Forbidden` - Yetkisiz erişim
- `404 Not Found` - Kaynak bulunamadı
- `500 Internal Server Error` - Sunucu hatası

## Swagger UI

Uygulama çalıştığında Swagger UI'ye şu adresten erişebilirsiniz:
```
https://localhost:7xxx/swagger
```

Swagger UI kullanımı:
1. `/api/auth/register` veya `/api/auth/login` ile token alın
2. Sağ üst köşedeki "Authorize" butonuna tıklayın
3. `Bearer {token}` formatında token'ı girin
4. Artık tüm endpoint'leri test edebilirsiniz

## Best Practices

Bu proje aşağıdaki best practices'i takip eder:

1. **RESTful Design** - Resource-based URL yapısı
2. **DTO Pattern** - Data Transfer Objects ile güvenli veri transferi
3. **Dependency Injection** - Loosely coupled mimari
4. **Interface Segregation** - Servisler için interface kullanımı
5. **Single Responsibility** - Her sınıf tek bir sorumluluk
6. **Proper HTTP Methods** - GET, POST, PUT, DELETE doğru kullanımı
7. **Proper Status Codes** - Anlamlı HTTP status kodları
8. **Comprehensive Documentation** - Swagger/OpenAPI ile detaylı dokümantasyon
9. **Validation** - Model state validation
10. **Structured Logging** - Önemli olayların kaydı
11. **Error Handling** - Consistent error response formatı
12. **Separation of Concerns** - Controller, Service, Model ayrımı

## Gelişmiş Özellikler

### Hiyerarşik Ekranlar
Ekranlar parent-child ilişkisi ile hiyerarşik yapıda düzenlenebilir:

```json
{
  "name": "UserDetail",
  "displayName": "Kullanıcı Detayı",
  "parentScreenId": 2,  // Users ekranının child'ı
  "route": "/users/:id"
}
```

### Özel Aksiyonlar
Her ekran için özel aksiyonlar tanımlanabilir:

```json
{
  "screenId": 2,
  "actionName": "reset-password",
  "displayName": "Şifre Sıfırla"
}
```

### Çoklu Grup Üyeliği
Bir kullanıcı birden fazla gruba dahil olabilir:

```json
{
  "userId": 1,
  "userGroupIds": [1, 2, 3]  // Admin, Manager ve User
}
```

İzinler birleştirilerek (union) değerlendirilir.

## Production Notları

Production ortamında aşağıdaki değişiklikleri yapın:

1. **JWT SecretKey:** Güçlü ve benzersiz bir key kullanın
2. **Environment Variables:** Hassas bilgileri environment variables'da saklayın
3. **HTTPS:** `RequireHttpsMetadata = true` yapın
4. **CORS:** Sadece güvenilir origin'lere izin verin
5. **Database:** Entity Framework Core ile gerçek veritabanı kullanın
6. **Password Hashing:** BCrypt veya PBKDF2 kullanın
7. **Rate Limiting:** API rate limiting ekleyin
8. **Logging:** Application Insights veya Serilog kullanın
9. **Caching:** Redis gibi distributed cache kullanın
10. **Refresh Tokens:** Token yenileme mekanizması ekleyin

## Gelecek Geliştirmeler

- [ ] Entity Framework Core entegrasyonu
- [ ] Refresh token mekanizması
- [ ] Audit logging (kim, ne zaman, ne yaptı)
- [ ] İzin değişiklik geçmişi
- [ ] Bulk permission assignment
- [ ] Permission templates
- [ ] Time-based permissions (geçici yetkiler)
- [ ] IP-based access control
- [ ] Two-factor authentication
- [ ] Rate limiting

## Lisans

Bu proje eğitim amaçlı oluşturulmuştur.

## Destek

Sorularınız için issue açabilirsiniz.
