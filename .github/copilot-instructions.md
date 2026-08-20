# HARC API Instructions

Bu repository HARC'ın .NET 10 FastEndpoints API'sidir. Ortak kurallar `.github/instructions/` altındaki coding, architecture, security ve documentation dosyalarındadır.

## Proje kuralları

- Yeni özellikleri uygun `Modules/<Context>` altında dikey dilim olarak ekle.
- Endpoint, request ve response tiplerini feature klasöründe tut.
- Kullanıcı kimliğini authenticated `harc_user_id` claim'inden al.
- `IdentityDbContext` mapping'ini ve son migration'ı incelemeden veri modeli değiştirme.
- Model değişikliğinde migration oluştur ve ilgili build/manual endpoint kontrollerini çalıştır.
- Secret, connection string veya OAuth credential'larını source control'e yazma.
