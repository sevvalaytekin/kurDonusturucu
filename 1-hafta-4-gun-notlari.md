# 1. Hafta, 4. Gün — PostgreSQL ve Entity Framework Core Kurulumu

**Not:** Aynı dürüstlük payı geçerli — gün numarası tahmini.

## Bugün Ne Yapıldı (tahmini)

- PostgreSQL veritabanı bağlantı bilgisi `appsettings.json`'a eklendi (o dönemde şifre düz metin olarak commit edildi — bu ileride, 7. günde `dotnet user-secrets`'a geçiş kararını doğrudan etkiledi).
- `AppDbContext` oluşturuldu, `DovizKuru` modeli `DbSet` olarak bağlandı.
- İlk migration (`InitialCreate`) oluşturuldu.
- Git commit geçmişinde "backend tekrar yazıldı" ifadesi geçiyor — ilk deneme yeterince iyi oturmayınca servis katmanının bir kısmı yeniden yazıldı (bu normal ve sağlıklı bir süreç, refactoring).

## Kavramlar — Basitçe

### Entity Framework Core (ORM) nedir
C# nesnelerini (örn. `DovizKuru` sınıfı) veritabanı tablolarıyla eşleştiren bir araç — ham SQL yazmak yerine C# kodu yazarak veritabanıyla konuşmanı sağlıyor.

### Migration nedir
Veritabanı şemasını (hangi tablo, hangi kolonlar) C# model sınıflarından otomatik türeten, git'e commit edilebilen bir değişim kaydı. `dotnet ef database update` ile veritabanına uygulanıyor.

### appsettings.json'da şifre saklamanın riski
appsettings.json git'e commit edildiğinde, içindeki her şey (şifre dahil) herkese açık hale gelebilir. Bu, CLAUDE.md'de bilinen bir sorun olarak işaretlendi ve 7. günde Google Client Secret'ı saklarken aynı hatayı tekrarlamamak için `dotnet user-secrets` tercih edildi.

## Görev Durumu

"Veritabanı Desteğinin Eklenmesi" görevinin ilk yarısı: PostgreSQL bağlantısı ve temel EF Core altyapısı kuruldu, henüz uçtan uca test edilmedi.
