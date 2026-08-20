# 2. Hafta, 1. Gün — Claude Code Kurulumu, CLAUDE.md, Kullanıcı Girişi

Bu ve sonraki 2. hafta notları, gerçekten senin yaptığın oturumlardan derlendi — tahmini değil, doğrudan gözlemlenmiş.

## Bugün Ne Yapıldı

- Claude Code'un VS Code eklentisi kuruldu (kurulum sırasında yaşanan sorunlar: marketplace bağlantı hatası, yanlış platforma ait VSIX dosyası, `claudeProcessWrapper` ayarıyla çözüldü).
- Claude Code'un üç çalışma modu (**Plan**, **Manual/Default**, **Edit automatically**) tanındı.
- Proje köküne **CLAUDE.md** dosyası oluşturuldu — teknoloji yığını, klasör yapısı, mimari ve bilinen sorunlar (şifre commit'i, `.gitignore` eksikliği, sahte "tatil kontrolü") belgelendi.
- Kullanıcı adı/şifre ile giriş özelliği geliştirildi: `PasswordHasher<T>` (PBKDF2-HMAC-SHA256) ile şifre hash'leme, cookie tabanlı oturum yönetimi.

## Kavramlar — Basitçe

### Claude Code'un üç modu
- **Plan:** sadece plan sunar, dosyaya dokunmaz — riskli/yeni işlerde kullanılır.
- **Manual:** her adımda izin ister — güvenlik ağı gibi.
- **Edit automatically:** onay sormadan direkt uygular — plan onaylandıktan sonra hız için.

### CLAUDE.md nedir
Claude Code'un her oturumda otomatik okuduğu, projenin "hafızası" — teknoloji yığını, mimari, bilinen sorunlar buraya yazılır, böylece her seferinde projeyi baştan anlatmak gerekmez.

### Şifre neden hash'lenir
Veritabanı sızsa bile gerçek şifrelerin okunamaması için. `PasswordHasher<T>`, PBKDF2 gibi kasıtlı olarak yavaş çalışan bir algoritma kullanıyor — bu, brute-force saldırılarını zorlaştırıyor.

### Cookie tabanlı authentication
HTTP doğası gereği "stateless" (durumsuz) olduğu için, sunucu art arda gelen istekleri aynı kullanıcıdan geldiğini normalde bilemez. Giriş yapınca tarayıcıya bir "kimlik" cookie'si veriliyor, her istekte bu geri gönderiliyor.

## Görev Durumu

✅ "AI Agent Ortamı & Markdown Dosyaları" (0.5 gün) ve "Oturum Açma Özelliğinin Eklenmesi" (0.5 gün) görevleri tamamlandı.
