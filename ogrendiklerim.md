# Öğrendiklerim

Bu dosya, staj boyunca öğrendiğim kavramları ve "neden"lerini kısa notlar hâlinde tutmak için.

## 6. Gün — Claude Code Kurulumu, CLAUDE.md, Oturum Açma

- **Claude Code'un 3 modu var:**
  - **Plan** → sadece plan sunar, hiçbir dosyaya dokunmaz. Yeni/karmaşık bir işe başlarken veya güvenlik içeren (login gibi) konularda kullanılır.
  - **Manual** → her adımda izin ister. Riskli/emin olmadığın işlerde güvenlik ağı gibi.
  - **Edit automatically** → onay sormadan direkt yazar. Plan zaten onaylandıktan sonra hız kazanmak için kullanılır.

- **CLAUDE.md nedir:** Claude'un her yeni oturumda otomatik okuduğu bir "proje hafızası" dosyası. İçine teknoloji yığını, klasör yapısı, mimari yazılır — böylece her seferinde projeyi baştan anlatmak gerekmez.

- **Şifreler asla düz metin saklanmaz.** `PasswordHasher<T>` (PBKDF2-HMAC-SHA256) gibi bir hash fonksiyonuyla saklanır. Veritabanı bir şekilde sızsa bile şifreler doğrudan okunamaz.

- **Cookie tabanlı authentication:** Kullanıcı giriş yapınca tarayıcıya bir "kimlik" çerezi yazılır; sonraki her istekte bu çerez kontrol edilerek kullanıcının giriş yapmış olduğu anlaşılır.

- **"Address already in use" hatası:** `dotnet run` çalıştırırken bu hata, o portu (örn. 5183) başka bir sürecin hâlâ kullanıyor olmasından kaynaklanır.
  - `lsof -i :PORT` → o portu kullanan process'i (PID) bulur.
  - `kill -9 PID` → o process'i sonlandırır.
  - Sonra `dotnet run` tekrar denenir.

- **Kurulum sorunlarını teşhis etme yöntemi:** Bir şey (VS Code gibi) internete bağlanamıyorsa, aynı adresi tarayıcıda denemek "genel internet sorunu mu, yoksa uygulamaya özel bir sorun mu" ayrımını hızlıca yapmayı sağlar. Tarayıcı çalışıp uygulama çalışmıyorsa, sorun o uygulamanın kendi ayarında/motorundadır.

## 7. Gün — Google ile Oturum Açma (OAuth 2.0)

- **OAuth 2.0 mantığı:** Google ile giriş yaptığımızda, kullanıcının kimliğini biz değil Google doğruluyor. Google bize sadece "bu kullanıcı gerçekten var, işte kimlik bilgisi (sub, isim, email)" diyor. Biz kullanıcının Google şifresini hiçbir zaman görmüyoruz.

- **Client ID / Client Secret:** Google'a "ben bu uygulamayım" demek için kullanılan kimlik bilgileri. Google Cloud Console'da bir proje açıp OAuth Client oluşturarak alınır — kodla değil, panelden yapılan bir kurulum adımı.

- **Redirect URI (yönlendirme adresi):** Google, girişten sonra kullanıcıyı SADECE önceden Google Console'da tanımladığımız adreslere geri gönderir (örn. `/signin-google`). Bu, bir güvenlik önlemi — biri kimlik bilgilerini çalıp farklı bir siteye yönlendiremesin diye. Adres kayıtlı değilse `redirect_uri_mismatch` hatası alınır.

- **dotnet user-secrets:** Client ID/Secret gibi hassas bilgileri `appsettings.json`'a yazıp git'e commit etmek yerine, bilgisayara özel/gizli bir depoya kaydetme yöntemi. `dotnet user-secrets set "Key" "Value"` ile eklenir, kod içinde normal ayar gibi okunur ama repoya asla girmez.

- **"Testing" modundaki Google uygulaması:** Google OAuth uygulaması yayınlanmadan (Testing aşamasında) sadece Google Console'da "Test users" listesine eklenen hesaplar giriş yapabilir. Gerçek kullanıcılar için uygulamanın "Published/Verified" olması gerekir.

- **Nullable şifre alanı:** Google ile giriş yapan bir kullanıcının bizim veritabanımızda ayrı bir şifresi olmaz (kimlik doğrulamayı Google yapıyor). Bu yüzden `SifreHash` alanı nullable yapıldı, kullanıcıyı ayırt etmek için `GoogleId` (Google'ın verdiği benzersiz `sub` değeri) eklendi.

- **Find-or-create mantığı:** Google ile giriş yapan kullanıcı, `GoogleId`'sine göre veritabanında aranır; varsa doğrudan giriş yaptırılır, yoksa otomatik (şifresiz) yeni bir kayıt oluşturulur.
