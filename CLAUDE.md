# TcmbKurDonusturucu

TCMB'nin (Türkiye Cumhuriyet Merkez Bankası) günlük döviz kuru XML servisinden veri çekip PostgreSQL'de önbelleğe alan ve iki para birimi arasında çapraz kur hesaplayan, tek sayfalık bir ASP.NET Core MVC uygulaması.

## Teknoloji Yığını

- **.NET 10** / ASP.NET Core Web MVC — sunucu tarafı render, Razor views (`.cshtml`)
- **Entity Framework Core 10** + **Npgsql** (PostgreSQL provider)
- Frontend: vanilla JS (`fetch` API), vendored Bootstrap/jQuery/jquery-validation (`wwwroot/lib`) — npm/bundler yok, `package.json` yok
- Test projesi yok, lint/format config yok (`.eslintrc`, `.prettierrc`, `.editorconfig` yok)

## Klasör Yapısı

```
Controllers/
  HomeController.cs      # tek controller: Index (GET) ve KurHesapla (POST)
Data/
  AppDbContext.cs         # EF Core DbContext, DbSet<DovizKuru>
Migrations/                # tek migration: InitialCreate (DovizKurlari tablosu)
Models/
  DovizKuru.cs             # EF entity + KurHesaplaSonucu DTO (aynı dosyada)
  ErrorViewModel.cs
Services/
  ITcmbKurServisi.cs / TcmbKurServisi.cs   # TCMB XML + DB cache mantığı
Views/
  Home/Index.cshtml        # döviz hesaplama formu (tek sayfa)
  Shared/_Layout.cshtml
wwwroot/                   # statik dosyalar (css, js, vendored lib)
Program.cs                 # DI, middleware pipeline, uygulama giriş noktası
appsettings.json           # bağlantı dizesi (bkz. Dikkat bölümü)
```

## Mimari / Veri Akışı

1. `GET /` → `Views/Home/Index.cshtml` render edilir (tarih, kaynak/hedef para birimi, miktar formu).
2. `POST /Home/KurHesapla` → `HomeController.KurBul()` → `TcmbKurServisi`:
   - Önce PostgreSQL'de (`DovizKurlari` tablosu, tarihe göre) kayıt aranır.
   - Bulunamazsa TCMB'nin `https://www.tcmb.gov.tr/kurlar/{yyyyMM}/{ddMMyyyy}.xml` adresinden XML çekilir, parse edilir ve sonuç DB'ye yazılır.
   - Her iki para birimi TRY karşılığına çevrilip (`ForexSatis / Birim`, TRY = 1) oranlanarak çapraz kur hesaplanır.
3. Sonuç `KurHesaplaSonucu` DTO'su olarak JSON döner, `wwwroot/js/kur-hesapla.js` formu günceller.

Katmanlama: Controller → Service (`ITcmbKurServisi`, DI ile inject edilen `HttpClient` + `AppDbContext`) → EF Core → PostgreSQL, dış kaynak olarak TCMB XML.

## Çalıştırma / Geliştirme Komutları

```bash
dotnet restore
dotnet build
dotnet run                     # http://localhost:5183, https://localhost:7292
dotnet ef database update      # PostgreSQL "exchange" DB'sine migration uygular
```

PostgreSQL çalışır durumda olmalı — `AppDbContext`, `Program.cs`'de koşulsuz register edilir, DB olmadan uygulama başlamaz.

Test komutu yoktur (test projesi mevcut değil).

## Bilinmesi Gerekenler / Dikkat

- **Şifre commit edilmiş**: `appsettings.json` içindeki `DefaultConnection` bağlantı dizesinde düz metin PostgreSQL şifresi git'e commit edilmiş durumda. Yeni bir ortam kurarken veya paylaşırken bunu `dotnet user-secrets` ya da ortam değişkenine taşımak gerekir.
- **`.gitignore` yok**: Repoda `.gitignore` dosyası bulunmuyor; `bin/` ve `obj/` derleme çıktıları (DLL, PDB, cache dosyaları) git'e commit edilmiş. Standart bir .NET `.gitignore` eklenip bu klasörlerin `git rm -r --cached` ile çıkarılması önerilir.
- **"Resmi tatil kontrolü" gerçekte yok**: `3025c51` commit'i ("Resmi tatil kontrolü eklendi") aslında önceki hafta sonu kaydırma mantığını kaldırıp genel bir try/catch ile değiştirmiş. Şu an kodda gerçek bir resmi tatil takvimi/mantığı yok — tatil/hafta sonu günlerinde TCMB veri döndürmediğinde kullanıcıya genel bir hata mesajı gösteriliyor.
- **Sınırlı para birimi seçimi**: `Views/Home/Index.cshtml` formunda sadece USD/EUR/GBP/TRY seçenekleri hardcoded olarak tanımlı; ancak `TcmbKurServisi`, TCMB'nin yayınladığı tüm para birimlerini parse edebiliyor — genişletmek için sadece view güncellenmesi yeterli.
