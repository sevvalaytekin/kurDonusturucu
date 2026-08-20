# 2. Hafta, 2. Gün — Google ile Oturum Açma (OAuth 2.0)

## Bugün Ne Yapıldı

- Google Cloud Console üzerinden bir OAuth Client (Client ID / Client Secret) oluşturuldu.
- Client ID/Secret, `dotnet user-secrets` ile (appsettings.json'a değil) güvenli şekilde saklandı.
- Redirect URI (`/signin-google`) Google Console'da tanımlandı.
- `GoogleGiris()` ve `GoogleGirisCallback()` action'ları eklendi: kullanıcı `GoogleId`'ye göre veritabanında aranıyor, varsa giriş yaptırılıyor, yoksa otomatik (şifresiz) kayıt oluşturuluyor.
- `Kullanici` modeline nullable `SifreHash` ve `GoogleId` alanı eklendi.

## Kavramlar — Basitçe

### OAuth 2.0 mantığı
Kullanıcının kimliğini biz değil Google doğruluyor. Google bize sadece "bu kullanıcı gerçek, işte bilgisi" diyor — kullanıcının Google şifresini biz hiçbir zaman görmüyoruz.

### Client ID / Client Secret
Google'a "ben bu uygulamayım" demek için kullanılan kimlik bilgileri — Google Cloud Console'da panelden oluşturuluyor, kodla değil.

### Redirect URI neden güvenlik önlemi
Google, girişten sonra kullanıcıyı **sadece önceden tanımlanmış adreslere** geri gönderiyor. Bu olmasaydı, biri Client ID'yi kullanarak kullanıcıyı kendi sahte sitesine yönlendirebilirdi.

### dotnet user-secrets
Hassas bilgileri appsettings.json'a (ve dolayısıyla git'e) yazmak yerine, sadece kendi bilgisayarında, proje dışında saklayan bir mekanizma. **Neden bu tercih edildi:** 1. haftada appsettings.json'a yazılan veritabanı şifresi git'e commit edilmişti — bu hatayı tekrarlamamak için.

### Nullable şifre / GoogleId / find-or-create
Google ile giriş yapan bir kullanıcının bizim sistemimizde ayrı bir şifresi yok, o yüzden `SifreHash` nullable yapıldı. `GoogleId` (Google'ın verdiği `sub` değeri) ile kullanıcı bulunuyor; yoksa otomatik oluşturuluyor.

## Görev Durumu

✅ "Google İle Oturum Açma Entegrasyonu" (1 gün) görevi tamamlandı.
