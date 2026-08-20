# HARC API Agent Instructions

Bu repository'de çalışırken `.github/copilot-instructions.md` ve `.github/instructions/` altındaki ortak talimatları uygula.

- FastEndpoints + EF Core modüler monolith yapısını koru.
- Kullanıcı kimliğini `harc_user_id` claim'inden al; request body'deki user/owner id'ye güvenme.
- `IdentityDbContext` mapping ve migration'ları model değişiklikleriyle birlikte kontrol et.
- API response casing, enum değerleri ve mevcut endpoint sözleşmelerini koru.
- FormData dosyaları için tekrarlanan `Documents` alan adını ve manuel `Content-Type` yazmama kuralını koru.
- Doğrulama sonrası `dotnet build harc-api.csproj` çalıştır.
