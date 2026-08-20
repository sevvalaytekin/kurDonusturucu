# 2. Hafta, 4. Gün — TypeScript ve Playwright E2E Testleri

## Bugün Ne Yapıldı

- `wwwroot/js/kur-hesapla.js` dosyası TypeScript'e (`kur-hesapla.ts`) taşındı — backend'den dönen veri için `KurHesaplaSonucu` interface'i, DOM elemanları için tipli castler eklendi.
- `tsconfig.json` (strict mode, ES2020+DOM) ve `package.json` (`npm run build` → `tsc`) eklendi — TypeScript derlemesi, `dotnet build`'den bağımsız, ayrı bir script olarak kuruldu.
- Playwright E2E test ortamı kuruldu (`playwright.config.ts`) — `webServer` ayarı, testten önce `dotnet run`'ı otomatik başlatıp bitince kapatıyor.
- Bir E2E test senaryosu yazıldı: sayfa açılıyor, USD/EUR seçiliyor, miktar giriliyor, "Hesapla" tıklanıyor, sonuç mesajı doğrulanıyor. Test geçti (`1 passed`).

## Kavramlar — Basitçe

### TypeScript nedir, neden kullanıldı
JavaScript'in üzerine tip sistemi ekleyen bir dil. Backend'in döndürdüğü verinin şekli değişirse, bunu kod **yazarken** (derleme anında) fark ediyorsun — kullanıcı hata görene kadar beklemene gerek kalmıyor.

### Neden bir derleme adımı gerekiyor
Tarayıcılar sadece JavaScript anlıyor, TypeScript'i değil. `tsc` (TypeScript compiler), `.ts` dosyalarını `.js`'e çeviriyor (transpile).

### E2E (uçtan uca) test, unit testten farkı
Unit test izole bir fonksiyonu test eder. E2E test, **gerçek bir tarayıcıyı** açıp kullanıcı gibi davranarak sistemin bütün olarak (sayfa + backend + veritabanı) çalıştığını doğruluyor.

### webServer ayarı neden önemli
Test öncesi `dotnet run`'ı elle başlatıp sonra kapatmak yerine, bunu otomatikleştiriyor — hem unutma riskini ortadan kaldırıyor hem de otomatik test sistemlerinde (CI/CD) insan müdahalesi gerektirmiyor.

## Görev Durumu

✅ "TypeScript ile Çalışmak" (0.5 gün) ve "E2E Playwright Test Ortamı Kurulması" (0.5 gün) görevleri tamamlandı.
