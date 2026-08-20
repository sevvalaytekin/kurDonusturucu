# 11. Gün Tekrarı — Bugün Sorduğun Sorulara Göre Kavramlar

Bu dosya, 3. haftayı baştan gözden geçirirken 11. gün (React Native/Expo temelleri) hakkında sorduğun tüm soruların cevaplarını, düzenli bir şekilde topluyor.

## npm ve "kütüphane" ekosistemi

**Kütüphane (paket) nedir:** Başka birinin yazıp herkesle paylaştığı, senin sıfırdan yazmak zorunda kalmayacağın hazır kod parçası.

**npm nedir:** Bu hazır kod parçalarını internetten indirip projene ekleyen araç. `dotnet restore`'un NuGet paketleri için yaptığını, `npm install` JavaScript paketleri için yapıyor.

**Üç dosya/klasörün farkı:**
- **`package.json`** — sadece **senin doğrudan istediğin** kütüphanelerin kısa, okunabilir listesi (örn. `react`, `expo`).
- **`node_modules`** — indirilen her şeyin (doğrudan istediklerin + onların kendi ihtiyaç duyduğu dolaylı kütüphaneler) fiziksel olarak durduğu klasör, gerçekte diskte olan her şey.
- **`package-lock.json`** — hangi kütüphanenin tam olarak hangi versiyonunun indiği, dolaylı olanlar dahil, kesin kayıt defteri.

**Kütüphaneler nasıl kullanılır:** `import` satırıyla, kendi dosyanın içine çağırarak:
```ts
import { View, TextInput } from "react-native";
```
Bu, C#'taki `using System;` ile aynı mantık — "bu dosyada şu kütüphanenin içindekilerini kullanacağım" demek.

**Not:** Her şey `import` ile kullanılmaz. `npm`, `expo` (CLI olarak), `watchman` gibi araçlar terminale komut olarak yazılır, hiçbir dosyanın içine `import` edilmezler — onlar birer program/araç, kod parçası değil.

## Node.js

JavaScript normalde sadece tarayıcı içinde çalışan bir dil. Node.js, bu sınırı aşıp JavaScript'i doğrudan bilgisayarında (tarayıcı olmadan) çalıştırmayı sağlıyor.

**Neden lazım:** Expo, Metro, npm'in kendisi gibi tüm mobil geliştirme araçları JavaScript ile yazılmış — Node.js olmadan hiçbiri çalışmaz. Node.js, bunlara "hayat veren" temel motor.

## Homebrew ve .dmg

**Homebrew:** Mac'te, özellikle **komut satırı araçlarını** (Watchman gibi küçük programları) kurmak için kullanılan bir paket yöneticisi.

**Büyük GUI programları (Android Studio, Chrome) neden Homebrew ile değil:** Onlar genelde doğrudan siteden indirilen **`.dmg`** dosyalarıyla kurulur. `.dmg` (Disk Image), Mac'e özel kurulum dosya formatı — Windows'taki `.exe`/`.msi`'nin karşılığı. Çift tıklayınca açılan pencereden uygulama simgesini "Applications" klasörüne sürükleyerek kurulum tamamlanır.

## Watchman vs Metro — ikisi de "değişiklik" ile ilgili ama farklı işler

- **Watchman = hareket sensörü.** Tek işi: dosyalarda değişiklik olup olmadığını anında fark etmek. JavaScript'ten, paketlemeden hiçbir şey anlamıyor, sadece "şu dosya değişti" diye haber veriyor.
- **Metro = gerçek işi yapan işçi.** Watchman'den haberi alınca (ya da ilk açılışta), `.tsx` dosyalarını gerçekten okuyup telefonun/emulatörün çalıştırabileceği tek bir pakete dönüştürüyor ve gönderiyor.

**Bu neden .NET'ten farklı:** `.NET`'te bir kod satırı değiştirince `dotnet run`'ı durdurup yeniden başlatman gerekiyordu. Burada `npx expo start`'ı bir kere başlatman yeterli — sonrasında her kayıtta Watchman fark edip Metro'ya haber veriyor, uygulama kendiliğinden güncelleniyor ("hot reload"). Fark, "hiç çalıştırmama gerek yok" değil, **"ilk çalıştırdıktan sonra her değişiklikte yeniden başlatmama gerek yok."**

## React Native vs Expo

**Küçük düzeltme:** React Native bir "dil" değil — projenin dili TypeScript/JavaScript. React Native, o dille yazılan kodu gerçek Android/iOS ekran elemanlarına çeviren bir **kütüphane/teknoloji**.

**Araba benzetmesi:**
- **React Native = arabanın motoru.** Kodunu gerçek native ekran elemanına çeviren asıl mekanizma. Ama sadece motorla bir yere gidemezsin — kaportayı, elektrik tesisatını elle kurman gerekir (native ayarları elle yapman gerekir).
- **Expo = motor zaten monte edilmiş, sürüşe hazır araba.** React Native'i değiştirmiyor, sadece etrafındaki karmaşık kurulumu senin yerine hazırlıyor.

**Somut kanıt:** `npx create-expo-app TcmbKurMobil` komutu saniyeler içinde çalışmaya hazır bir proje çıkardı — Expo olmasaydı, native Android/iOS proje dosyalarını elle kurman saatler sürebilirdi. Ayrıca `convert.tsx`'te hâlâ `import ... from "react-native"` yazıyoruz — çünkü Expo, React Native'in **üzerine** kurulu, onu **değiştirmiyor**.

**Expo Router'ın verdiği ekstra kolaylık:** `src/app/` klasörüne bir dosya (örn. `convert.tsx`) koymak, otomatik olarak `/convert` ekranını oluşturuyor — React Native'in kendisinde böyle bir otomatik sistem yok, bu tamamen Expo'nun sağladığı bir kolaylık.

## Framework vs Kütüphane (Library)

- **Kütüphane:** Sen onu çağırırsın, kontrol sende. İhtiyacın olduğunda import edip kullanırsın (örn. `useState`).
- **Framework:** Tam tersi — o seni çağırır, kontrol onda. Genel yapıyı/kuralları o belirler, sen boşlukları doldurursun (örn. Expo Router'ın "her dosya bir ekran olur" kuralı — bunu sen belirlemiyorsun, framework dayatıyor).

Expo hem framework (proje yapısı, router) hem kütüphane (`expo-image` gibi import edilen paketler) tarafına sahip — bu yüzden "araç seti" demek daha doğru.

## Projedeki dosyalar nereden geldi

- **`npx create-expo-app` ile otomatik geldi:** `node_modules`, `assets`, `scripts`, `src/app/index.tsx` ve `explore.tsx`, `package.json`, `package-lock.json`, `app.json`, `README.md`, `LICENSE`, `.gitignore`.
- **Template'in içinden geldi (ilginç detay):** `.claude/settings.json`, `.vscode/settings.json`, `AGENTS.md`, `CLAUDE.md` — Expo'nun kendi şablonu artık resmi bir Claude Code eklentisiyle (`expo@claude-plugins-official`) birlikte geliyor, biz elle oluşturmadık.
- **Sonradan eklendi:** `.env` (12. gün, API anahtarı için), `.idea` (Android Studio projeyi açtığında otomatik oluşturdu), `.expo` (ilk `expo start` çalıştığında otomatik oluşan cache klasörü).

## İki Uçtan Uca Senaryo — Her Aracın Tam Olarak Ne Zaman Devreye Girdiği

**Senaryo A — Projeyi sıfırdan başlatmak (`npm run android`):**
1. Komutu çalıştırmak için **Node.js** gerekli.
2. **npm**, `node_modules`'teki kütüphaneleri hazırlar.
3. **Expo** (CLI) derlemeyi başlatır.
4. **Android SDK**, kodu Android paketine (APK) çevirir.
5. **Watchman** arka planda izlemeye başlar.
6. **Metro**, `.tsx` dosyalarını pakete dönüştürür.
7. **Emulatör** (Device Manager ile açılmış Pixel 7) paketi çalıştırır.
8. **React Native**, kodundaki bileşenleri gerçek Android ekran elemanlarına çevirmiş olarak ekranda görünür.

**Senaryo B — Küçük bir değişiklik yapmak (bir dosyayı kaydetmek):**
1. Dosyayı kaydedersin.
2. **Watchman** anında fark eder.
3. **Metro**'ya haber verir.
4. **Metro**, sadece o dosyayı yeniden paketleyip emulatöre gönderir.
5. **Emulatördeki uygulama**, sen dokunmadan güncellenir.

## npm ile npx farkı, versiyon numaraları

**Versiyon numaraları neye göre iniyor:** `package.json`'daki her numara üç parçadan oluşuyor: **BÜYÜK.ORTA.KÜÇÜK** (major.minor.patch). Başındaki işaret, hangi güncellemelere izin verildiğini belirliyor:
- **`~57.0.14`** (tilde) → sadece en sondaki (patch/küçük hata düzeltmesi) numarada güncellemeye izin var, `57.0.x` aralığında kal.
- **İşaretsiz, `19.2.3`** → tam olarak bu versiyon.
- (Yaygın ama projede olmayan bir üçüncüsü: `^` → orta numarada da güncellemeye izin verir.)

`npm install` çalıştığında npm, bu kurala uyan **en güncel** versiyonu indiriyor. Ama tam olarak hangi versiyonun indiği kesin olarak `package-lock.json`'da kayıtlı — böylece herkes aynı projeyi indirdiğinde birebir aynı versiyonları kullanmış olur.

**`npx` ile `npm` farkı:** `npx`, bir aracı **kalıcı kurmadan, bir kereliğine çalıştırmak** için kullanılıyor. `create-expo-app`'i sürekli kullanmayacağımız için (proje sadece bir kere oluşturuluyor), `npm install -g` ile kalıcı kurmak yerine `npx` ile "bunu bir kere çalıştır, sonra unut" diyoruz.

## İki Komutu Satır Satır Açmak — Hangi Kavram Nerede Devreye Giriyor

```
cd ~/Desktop
npx create-expo-app TcmbKurMobil
```
- **`npx`** — aracı kalıcı kurmadan bir kere çalıştırıyor.
- **Node.js** — bu JS komutunun çalışabilmesi için arka planda gerekli.
- **Expo** — `create-expo-app`, Expo'nun proje oluşturma aracı; bu satır tam olarak Expo'nun "hazır başlangıç kiti" işini yaptığı an.
- **npm** — Expo'nun şablonu için gereken kütüphaneleri (react, react-native...) indirip `node_modules`'e koyuyor.
- Bu satırda **henüz** Watchman, Metro, Android SDK, emulatör devreye girmiyor — sadece dosyalar oluşuyor, hiçbir şey "çalıştırılmıyor".

```
cd TcmbKurMobil
npm run android
```
- **npm** — `package.json`'daki `"android": "expo start --android"` kısayoluna bakıp onu çalıştırıyor.
- **Expo (CLI)** — gerçek derleme işini başlatıyor.
- **Watchman** — bu noktada arka planda başlayıp dosyaları izlemeye koyuluyor.
- **Metro** — `.tsx` dosyalarını pakete dönüştürmeye başlıyor.
- **Android SDK** — paketi, emulatörün çalıştırabileceği bir Android paketine (APK) çeviriyor.
- **Emulatör** (önceden Device Manager ile açılmış Pixel 7) — paketi alıp çalıştırıyor.
- **React Native** — ekranda gördüğün her şeyi gerçek Android arayüz elemanlarına çevirerek oluşturan taraf.

**Özet:** 1. komut = "malzemeleri hazırla" (kurulum), 2. komut = "makineyi çalıştır" (derleme + çalıştırma).

## ✅ 11. Gün Durumu

Bu gözden geçirme ile 11. günün (React Native/Expo temelleri) kavramları pekiştirildi. Sırada: 12. gün (Ekran 1) gözden geçirmesi.
