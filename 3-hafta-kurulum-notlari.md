# 3. Hafta — Mobil Geliştirme Ortamı Kurulumu Notları

Basit tutuyorum, sadece şu ana kadar yaptıklarımızı ve öğrendiğin kavramları içeriyor.

## Buraya Kadar Ne Yaptık (sırayla)

1. **Node.js ve npm kontrol edildi.** İkisi de zaten kuruluydu (Node v22.22.2). Kurulum yapmamıza gerek kalmadı.
2. **Watchman kontrol edildi.** Kurulu değildi.
3. **Homebrew kontrol edildi.** Kuruluydu (Homebrew, Mac'te program kurmak için kullanılan bir araç).
4. **Watchman kuruldu.** `brew install watchman` ile kurulum tamamlandı, `watchman -v` ile doğrulandı (sürüm: 2026.07.27.00).
5. **Android Studio kontrol edildi.** Zaten kurulu çıktı.
6. **Device Manager kontrol edildi.** Android Studio'nun içindeki "Virtual Device Manager" açıldı, orada zaten hazır bir sanal telefon (emulatör) olduğu görüldü: **Pixel 7, Android 16.0, API 36**. Yeni bir emulatör oluşturmamıza gerek kalmadı.
7. **Emulatör test edildi.** Pixel 7 emulatörü çalıştırıldı (▶ butonuyla), başarıyla açıldı. Bu, "Android emulator kurulumu" görevinin tamamlandığı anlamına geliyor.
8. **İlk Expo projesi oluşturuldu.** `npx create-expo-app TcmbKurMobil` komutu ile masaüstünde `TcmbKurMobil` adlı boş bir mobil proje iskeleti oluşturuldu, gerekli paketler `npm install` ile otomatik indirildi. "✅ Your project is ready!" mesajıyla tamamlandı.
9. **Proje emulatörde çalıştırıldı.** `npm run android` komutu ile proje derlendi ve Pixel 7 emulatörüne yüklendi. Emulatörde Expo'nun varsayılan "Welcome to Expo" ekranı başarıyla göründü. **Bu adımla "React Native İle Mobil Geliştirme Temelleri" görevi tamamlandı.**

## Kavramlar — Basitçe

### Kütüphane / Paket nedir
Başka birinin yazıp herkesle paylaştığı, senin sıfırdan yazmak zorunda kalmayacağın hazır kod parçası.

### npm nedir
Bu hazır kod parçalarını internetten indirip projene ekleyen araç.

**Zaten bildiğin bir şeye benziyor:** `dotnet restore` dediğimizde projenin ihtiyaç duyduğu NuGet paketlerini indiriyorduk. `npm install` de aynı işi JavaScript paketleri için yapıyor.

### Node.js nedir
JavaScript, normalde sadece tarayıcı (Chrome, Safari) içinde çalışan bir dil. Node.js, JavaScript kodunu tarayıcı olmadan, doğrudan senin bilgisayarında çalıştırmayı sağlayan bir araç.

**Neden lazım:** Kuracağımız mobil geliştirme araçları JavaScript ile yazılmış. Onları senin Mac'inde çalıştırabilmek için Node.js'e ihtiyaç var.

### Android Studio nedir
Google'ın resmi olarak sunduğu, Android uygulaması geliştirmek için kullanılan program. Bir kod editörü, Android SDK ve emulatör yönetim araçlarını (Device Manager) tek bir programda bir araya getiriyor. Visual Studio'nun/VS Code'un Android dünyasındaki karşılığı gibi düşünebilirsin — biz kendi kodumuzu Android Studio'nun kod editöründe yazmayacağız (React Native/Expo projemizi VS Code'da yazacağız), ama Android Studio'nun içindeki SDK ve emulatör altyapısını kullanacağız.

### Android SDK nedir
**SDK = "Software Development Kit"**, yani "yazılım geliştirme kiti". Android SDK, bir Android uygulaması yazıp çalıştırabilmek için gereken araçların, kütüphanelerin ve Android işletim sisteminin farklı sürümlerine ait dosyaların bir araya toplanmış hali.

**Zaten bildiğin bir şeye benziyor:** Bu, .NET SDK'nın yaptığı işe çok benziyor — .NET SDK, yazdığın C# kodunu çalıştırılabilir hale getiriyordu; Android SDK de senin (React Native aracılığıyla üretilen) kodunu, bir Android cihazın/emulatörün anlayıp çalıştırabileceği hale getiriyor. Android SDK, Android Studio kurulurken kurulumun bir parçası olarak otomatik geliyor — biz ayrıca elle kurmadık.

### Watchman nedir
Projendeki dosyalarda bir değişiklik olduğunda bunu anında fark eden bir "gözcü" programı.

**Neden lazım — zaten yaşadığın bir sorunla bağlantılı:** `.NET` projesinde bir kod satırı değiştirdiğinde, değişikliği görebilmek için `dotnet run`'ı durdurup yeniden başlatman gerekiyordu. Mobil geliştirmede bu böyle olmuyor — kodu kaydettiğinde, uygulamayı sen durdurmadan, ekran kendiliğinden güncelleniyor. Bunun çalışabilmesi için birinin sürekli "bir dosya değişti mi?" diye bakması lazım — işte bu işi Watchman yapıyor, ve bunu işletim sisteminin kendi (daha yavaş) yönteminden daha hızlı ve verimli yapıyor.

### Metro bundler nedir (kısaca)
Yazdığın JavaScript dosyalarını, telefonun/emulatörün çalıştırabileceği tek bir pakete dönüştüren araç. Watchman bir değişiklik fark edince Metro'ya haber veriyor, Metro da hızlıca güncelliyor.

### React Native nedir
JavaScript ile mobil uygulama yazmanı sağlayan bir teknoloji — tek bir kod tabanıyla hem Android hem iOS için uygulama üretebiliyorsun (ikisini ayrı ayrı, ayrı dillerde yazmak zorunda kalmıyorsun). Meta (Facebook'un sahibi olduğu şirket) tarafından geliştiriliyor — tıpkı Watchman gibi.

### Expo (proje) nedir
Expo, React Native'in üzerine inşa edilmiş, işi kolaylaştıran bir araç seti. React Native ile normalde uğraşman gereken bir sürü karmaşık native (Android/iOS'a özel) ayarı, Expo senin yerine hazır bir şekilde sunuyor — böylece sen doğrudan uygulamanın ekranlarını/mantığını yazmaya odaklanabiliyorsun.

`npx create-expo-app TcmbKurMobil` komutuyla oluşturduğumuz proje, işte bu hazır Expo şablonunu kullanan, çalışmaya hazır bir React Native projesi. Bu yüzden proje klasöründe göreceğin dosyalar "React Native dosyaları" ama kurulumu/başlangıcı Expo sayesinde çok daha kolay oldu.

### Emulatör (sanal cihaz) nedir
Gerçek bir Android telefonun bilgisayarında çalışan bir simülasyonu — ekranı, tuşları, hatta uygulamalarıyla gerçek bir telefon gibi davranıyor. **Neden lazım:** Mobil uygulamayı test etmek için normalde gerçek bir Android telefona ihtiyacın olurdu; emulatör sayesinde telefon olmadan, doğrudan bilgisayarında test edebiliyorsun.

### Device Manager (Virtual Device Manager) nedir
Android Studio'nun içindeki, emulatörleri oluşturup yönetmene yarayan bölüm — "hangi telefon modelini simüle edeceğim, hangi Android sürümüyle" gibi ayarları buradan yapıyorsun. Bizim durumumuzda, daha önceden oluşturulmuş bir emulatör (Pixel 7) zaten hazır çıktı.

### "22 vulnerabilities" uyarısı ne anlama geliyor, endişelenmeli miyiz
`npm install` bittiğinde npm, kullandığımız paketlerin bağımlılıklarında bilinen güvenlik açıkları olabileceğini bildiriyor. Bu, hemen hemen her yeni JavaScript projesinde çıkan, standart bir bilgilendirme mesajı — bir hata değil. `npm audit fix` gibi komutlarla "düzeltmeye" çalışmak bazen paket sürümlerini beklenmedik şekilde değiştirip projeyi bozabiliyor, o yüzden şimdilik dokunmadık; proje ilerleyince ayrıca bakılabilir.

## Terminale Yazdığımız Komutlar (sırayla)

```
node -v
npm -v
```
Node.js ve npm zaten kurulu mu diye kontrol ettik. → İkisi de kuruluydu.

```
watchman -v
```
Watchman kurulu mu diye kontrol ettik. → Kurulu değildi (`command not found`).

```
ls /Applications | grep -i "Android Studio"
```
Android Studio zaten kurulu mu diye Applications klasörüne baktık. → Kuruluydu.

```
brew -v
```
Homebrew (Mac'te program kurma aracı) kurulu mu diye kontrol ettik. → Kuruluydu.

```
brew install watchman
```
Watchman'i Homebrew üzerinden kurduk.

```
watchman -v
```
Kurulumun gerçekten başarılı olduğunu doğrulamak için tekrar kontrol ettik. → Bu sefer bir sürüm numarası döndü (2026.07.27.00), yani kurulum başarılı.

```
cd ~/Desktop
npx create-expo-app TcmbKurMobil
```
Boş bir Expo/React Native proje iskeleti oluşturduk, gerekli paketler otomatik indirildi. Sonuç: proje hazır, `TcmbKurMobil` klasörü oluştu.

```
cd TcmbKurMobil
npm run android
```
Bu komut projeyi derleyip Pixel 7 emulatörüne yükledi. Sonuç: emulatörde "Welcome to Expo" ekranı başarıyla açıldı — uygulama çalışıyor.

## Görev Durumu

✅ **"React Native İle Mobil Geliştirme Temelleri" görevi tamamlandı.** Ortam kuruldu (Node/npm, Watchman, Android Studio + SDK + emulatör), ilk mobil proje (`TcmbKurMobil`) oluşturuldu ve Pixel 7 emulatöründe başarıyla çalıştırıldı.

## Sırada Ne Var

Plandaki bir sonraki görev: **Mobil Uygulama - Ekran 1** — o güne ait güncel döviz kurlarının tablo halinde listelendiği bir ekran geliştirilmesi.
