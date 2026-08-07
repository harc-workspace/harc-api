# HARC API

`harc-api`, HARC’ın .NET 10 tabanlı backend uygulamasıdır. FastEndpoints ile HTTP endpoint’leri, Entity Framework Core/Npgsql ile PostgreSQL erişimini ve Google JWT Bearer authentication’ı sağlar.

## Mimari

- Tek ASP.NET Core host üzerinde modüler monolith.
- İş alanları `Modules/` altında tutulur.
- Feature klasörleri endpoint, request ve response tiplerini birlikte barındırır.
- Tüm entity’ler şu anda `IdentityDbContext` üzerinden yönetilir.
- OpenAPI/Scalar yalnızca Development ortamında yayınlanır.

## Modüller

- `Identity`: kullanıcı, rol, takım, unvan ve claims transformation.
- `Leave`: izin oluşturma, takvim ve izin bakiyesi.
- `Document`: dosya yükleme ve metadata kaydı.
- `Common`: audit alanları, tatil entity’si ve ortak exception tipi.

## Endpoint’ler

### `GET /api/identity/me`

Geçerli Bearer token’dan kullanıcı email/rol claim’lerini alır; veritabanından kullanıcı, takım, unvan ve yönetici bilgilerini ekler.

### `POST /api/leave`

`multipart/form-data` ile izin oluşturur. Alanlar: `StartDate`, `EndDate`, `LeaveType`, `Description`, tekrar eden `Documents`. Kullanıcı ID’si request’ten değil `harc_user_id` claim’inden alınır.

Mevcut handler yalnızca kaydı `Pending` olarak oluşturur ve gün sayısını takvim günüyle hesaplar. İş kuralı doğrulamaları henüz uygulanmamıştır.

### `GET /api/leave/calendar?year=YYYY&month=M`

Kişinin izinlerini, aynı takımın diğer izinlerini ve ilgili ayın tatillerini döndürür.

### `GET /api/leave/my-balance`

Onaylanmış yıllık izinleri kullanılmış gün olarak toplar ve `LeaveSettings` değerleriyle toplam/remaining bakiye ile bir sonraki hakedişi hesaplar.

## Authentication akışı

Frontend Google ID token’ı `Authorization: Bearer` header’ı ile gönderir. API issuer, audience ve lifetime doğrulaması yapar. Ardından `ClaimsTransformation`:

1. `email` claim’ini bulur.
2. `identity.Users` içinde email arar.
3. Kullanıcı yoksa `ERR_USER_NOT_FOUND` döndürür; otomatik kayıt oluşturmaz.
4. Terminated kullanıcıyı `ERR_ACCOUNT_TERMINATED` ile reddeder.
5. `role` ve `harc_user_id` claim’lerini ekler.

Bu nedenle yeni endpoint’ler kullanıcı kimliğini frontend’den gelen body alanına göre belirlememelidir.

## Veri modeli

Varsayılan şema `identity`’dir. Açık mapping’ler:

- `identity.Users`, `identity.Roles`, `identity.Teams`, `identity.Titles`
- `leave.Leaves`, `leave.LeaveSettings`
- `document.Documents`
- `common.Holidays`

`BaseEntity` audit alanları içerir. `IdentityDbContext`, insert/update bilgilerini HTTP kullanıcısından doldurur ve delete işlemlerini `DeletedAt` ile soft delete’e çevirir. Migration’lar `Modules/Identity/Data/Migrations` altındadır.

## Yerel çalıştırma

```bash
docker compose up -d
dotnet run
```

Compose PostgreSQL’i `localhost:5433` üzerinde çalıştırır. Connection string ve Google Client ID’yi Development config veya user secrets ile verin; gerçek secret’ları source control’e koymayın.

API portu `Properties/launchSettings.json` ile belirlenir. Gateway standalone ayarında backend varsayılan olarak `http://localhost:5100` adresindedir.

## Dosya yükleme

`DocumentManager`, dosyaları `wwwroot/uploads/documents` altına GUID isimle kaydeder ve `document.Documents` tablosuna metadata yazar. Dosya boyutu/türü, virüs taraması ve transaction rollback politikası mevcut kodda kapsamlı değildir.

## Bilinen eksikler

- `login-google` endpoint’i yoktur; eski dokümantasyondaki bu açıklama geçersizdir.
- Kullanıcı onboarding/otomatik kayıt akışı yoktur.
- İzin oluşturma doğrulamaları yorum seviyesindedir.
- Otomatik test projesi yoktur.
- `Microsoft.OpenApi` dependency’si build sırasında güvenlik uyarısı üretebilir.

Ayrıntılı sistem rehberi için [kök AI_PROJECT_GUIDE.md](../AI_PROJECT_GUIDE.md) dosyasına bakın.
