# 3. Hafta, 2. Gün — Mobil Uygulama: Ekran 1 (Döviz Kurları Tablosu) Notları

Sen VS Code'da işlemleri yaparken ben de bu notu hazırlıyorum, okuyabilirsin.

## Bugünün Görevi Ne

Plandaki tanım: *"O güne ait güncel döviz kurlarının tablo halinde listelendiği bir ekran geliştirilmesi."*

Yani mobil uygulamada (`TcmbKurMobil`), açıldığında bugünün döviz kurlarını (USD, EUR, GBP gibi) bir tablo şeklinde gösteren bir ekran olacak.

**Neden bu görev tek başına anlamlı değil, önceki günlerle bağlantılı:** Bu ekran, veriyi sıfırdan bir yerden çekmeyecek — 10. günde ASP.NET tarafında yazdığımız `GET /api/kurlar/{tarih}` REST API'sini çağıracak. Yani bugün, geçen haftaki emeğin gerçekten kullanılmaya başladığı gün.

## Kavramlar — Basitçe

### Emulatör neden "localhost" ile backend'e ulaşamıyor, 10.0.2.2 nedir

`localhost` kelimesi normalde "bu cihazın kendisi" anlamına gelir. Ama Android emulatörü, senin Mac'inin içinde çalışan **ayrı, izole bir bilgisayar simülasyonu**. Emulatörün içinden `localhost` dediğinde, bu "emulatörün kendisi" anlamına geliyor — senin gerçek Mac'in değil. Bu yüzden emulatörün içinden çalışan mobil uygulama, `localhost:5183` dediğinde, aslında Mac'indeki `.NET` uygulamana değil, emulatörün kendi (boş) 5183 portuna gitmeye çalışıyor — ve tabii ki orada bir şey bulamıyor.

Google, bu sorunu çözmek için emulatöre özel, sabit bir adres tanımlamış: **`10.0.2.2`**. Emulatörün içinden bu adrese gidildiğinde, Android otomatik olarak bunu "beni çalıştıran gerçek bilgisayara git" şeklinde yönlendiriyor. Yani mobil uygulamamızın backend adresi `http://localhost:5183` değil, `http://10.0.2.2:5183` olacak.

**Not:** Bu sadece emulatöre özel bir kural. İleride gerçek bir telefonda test edersen (emulatör değil), bu sefer Mac'inin gerçek ağ adresini (IP) kullanman gerekir — `10.0.2.2` sadece emulatör için geçerli.

### API Key'i mobil tarafta nasıl saklayacağız — bu bir tartışma konusu olacak

Web tarafında (10. gün) API anahtarını `dotnet user-secrets` ile, yalnızca sunucuda duracak şekilde sakladık — kullanıcı bu anahtarı hiçbir zaman göremiyordu. Mobil uygulamalarda durum farklı: uygulamanın kendisi kullanıcının telefonuna/emulatörüne kuruluyor, yani "sadece sunucuda duran, gizli" bir yer yok. Koda gömülen her şey, teorik olarak uygulamayı inceleyen biri tarafından çıkarılabilir.

Bu, ileri seviye bir güvenlik konusu — bu staj kapsamında "mükemmel" bir çözüm şart değil, ama Claude Code'un bu konuda ne önereceğini görüp birlikte değerlendireceğiz. Muhtemelen plan aşamasında bize bunu soracak.

### Plan modunu neden yine kullanıyoruz

Bu, yeni bir ekran/mimari kararı (nereden veri çekilecek, nasıl gösterilecek, anahtar nasıl saklanacak) içeriyor — tıpkı 6. gündeki login özelliği gibi. Önce Claude Code'un yaklaşımını (planını) görüp onaylamak, doğrudan koda geçmekten daha güvenli.

## Buraya Kadar Ne Yaptık

1. Görev tanımı ve teknik gereksinimler (10.0.2.2 adresi, API'nin yeniden kullanılması) belirlendi.
2. Claude Code'a VS Code'da (`TcmbKurMobil` klasöründe), **Plan modunda** görev verildi — şu an Claude Code'un planı hazırlamasını bekliyoruz.

## Plan Moduna Yazdığımız Metin

```
React Native (Expo) projesinde, .NET backend'deki GET /api/kurlar/{tarih}
REST API endpoint'ini (X-Api-Key header'ı ile korumalı) çağırıp bugünün
tarihine ait döviz kurlarını bir tabloda (döviz kodu, isim, alış, satış)
listeleyen bir ekran oluşturmak istiyorum. Backend'e emulatörden erişim
için 10.0.2.2 adresi kullanılmalı, localhost değil.
```

**Neden bu şekilde yazıldı, kısaca:**
- "GET /api/kurlar/{tarih} ... X-Api-Key header'ı ile korumalı" → Claude Code'a hangi endpoint'i, hangi kimlik doğrulamayla çağıracağını net söylüyoruz — tahmin etmesine gerek kalmıyor.
- "döviz kodu, isim, alış, satış" → tablonun hangi sütunları göstereceğini baştan belirtiyoruz.
- "10.0.2.2 adresi kullanılmalı, localhost değil" → bir önceki bölümde anlattığımız emulatör/localhost sorununu Claude Code'un yaşayıp bizi bir hata ile şaşırtmasını önceden engelliyoruz.

## Claude Code'un Sunduğu Plan (Özet)

Claude Code, projeyi (mevcut Expo dosya yapısı, tema bileşenleri, tab düzeni) inceledikten sonra şu planı sundu:

- **Ana ekran:** `index.tsx` — mevcut Expo Router tab yapısı kullanılacak, gerekirse `app-tabs.tsx` güncellenecek.
- **API çağrısı:** `GET http://10.0.2.2:PORT/api/kurlar/YYYY-MM-DD`, header'larda `Accept`, `Content-Type` ve `X-Api-Key`.
- **Veri modeli:** Backend'in tam olarak hangi JSON şeklini döndüreceği kesin bilinmediği için, gelen veri (`data`/`rates`/`kurlar`/doğrudan dizi gibi farklı olası formatlar) tek bir standart modele (`code`, `name`, `buying`, `selling`) çevrilecek — "normalizasyon" katmanı.
- **Ekran davranışı:** Açılışta bugünün verisi çekilecek; yüklenirken "Kur bilgileri yükleniyor..." gösterilecek; hata/boş veri durumları için ayrı mesajlar olacak; bir "Yenile" butonu eklenecek.
- **Tablo:** Kod, İsim, Alış, Satış sütunları; sayılar Türkçe biçimde (virgüllü, örn. `32,75`).
- **Doğrulama planı:** Emulatörde çalıştırıp network/console üzerinden isteğin gerçekten `10.0.2.2` adresine gittiğini, `X-Api-Key` header'ının eklendiğini ve hata/boş durumların da manuel test edildiğini kontrol etmek.

## Planda Bulduğumuz İki Sorun ve Yaptığımız Düzeltme

Planı onaylamadan önce, dikkatlice okuyunca iki eksik/hatalı nokta fark ettik:

1. **Port numarası yanlıştı:** Plan `10.0.2.2:5000` yazmış, ama gerçek backend portumuz **5183**. Claude Code, mobil proje klasöründeyken `.NET` projesini görmediği için portu tahmin etmiş. **Neden önemli:** Bu düzeltilmeden çalıştırılsaydı, uygulama "bağlantı reddedildi" gibi bir hatayla karşılaşacaktı ve nedenini anlamak zaman alabilirdi — baştan yakaladığımız için bu sorunu hiç yaşamayacağız.
2. **API anahtarının nereden geleceği belirsizdi:** Plan `X-Api-Key: <apiKey>` diyordu ama gerçek değerin koda mı gömüleceği, ayrı bir dosyadan mı okunacağı net değildi. Claude Code'a, anahtarı doğrudan koda yazıp git'e commit etmek yerine, `.gitignore`'lanmış ayrı bir `.env` dosyasında (`EXPO_PUBLIC_API_KEY` gibi) tutmasını istedik — web tarafındaki `user-secrets` mantığının mobildeki en yakın karşılığı bu.

Bu iki düzeltmeyi içeren mesaj Claude Code'a gönderildi.

## Bir Ara Verilen Hata: "token expired or invalid"

Claude Code, düzeltme mesajını işlerken bir kere `401 token expired or invalid` hatası verdi. **Bu bir kod hatası değildi** — Claude Code'un kendi oturum bağlantısında geçici bir kimlik doğrulama sorunuydu. Aynı mesaj tekrar gönderilerek/gerekirse oturum yenilenerek çözüldü, koddan kaynaklanmıyordu.

## Uygulama Başladı — 3 Adımlık TODO Listesi

Claude Code, düzeltmeleri uygulamak için kendi içinde 3 adımlık bir TODO listesi oluşturdu ve **1. adımı tamamladı**:

1. ✅ **Backend base URL'i `10.0.2.2:5183` olarak güncelleme** — tamamlandı.
2. ⏳ Mevcut fetch/axios çağrılarının yeni `api.ts` üzerinden kullanılması.
3. ⏳ Ekranın (veri çekme + tablo) tamamlanması.

**1. adımda somut olarak ne oluşturuldu:**
- **`.env`** dosyası — `EXPO_PUBLIC_API_BASE_URL` ve `EXPO_PUBLIC_API_KEY` değerlerini tutacak.
- **`.gitignore`** güncellendi — `.env` dosyasının git'e commit edilmemesi için.
- **`src/constants/api.ts`** — `.env`'deki değerleri okuyup `API_BASE_URL` ve `API_KEY` olarak koddan kullanılabilir hale getiren yardımcı dosya.

**Önemli:** Claude Code, projede daha önceden hiç fetch/API çağrısı bulamadı ("No matches found") — bu normal, çünkü ekran henüz yazılmamıştı, sadece iskelet/config hazırlandı.

**Benim yapmam gereken bir adım kaldı:** `.env` dosyasındaki `EXPO_PUBLIC_API_KEY` şu an boş/örnek — buraya 10. günde `dotnet user-secrets` ile ASP.NET tarafında kaydedilen **gerçek API anahtarını** yazmam lazım (`dotnet user-secrets list` ile bulunabilir). Bu yapılmazsa backend her isteği 401 ile reddeder.

## Sonradan Hatırlanması Gereken Bir Komut

Claude Code'un cevabının sonunda şu komut geçti:

```
expo start -c
```

**Ne işe yarıyor:** `.env` dosyasına yeni bir değişken eklendiğinde, Metro bundler bunu bazen eski (cache'lenmiş) haliyle hatırlamaya devam edebiliyor. `-c` (clear cache) parametresi, önbelleği temizleyip Expo'yu yeniden başlatıyor — böylece `.env`'deki güncel değerler gerçekten okunuyor. Bunu, tüm değişiklikler bittikten sonra, emulatörde test etmeden hemen önce çalıştıracağız.

## TODO Listesi Tamamlandı (2/3 ve 3/3)

Claude Code, kalan iki adımı da tamamladı:

2. ✅ **`api.ts` güncellendi** — `getRates()` fonksiyonu artık gerçek endpoint'imizi (`GET /api/kurlar/{tarih}`) çağırıyor, varsayılan tarih bugün, ISO formatta (`YYYY-MM-DD`).
3. ✅ **`index.tsx` (ana ekran) tamamlandı** — bugünün kurlarını çeken fetch çağrısı ve Kod/İsim/Alış/Satış sütunlu tablo eklendi.

**Nasıl çalışıyor:**
- `getRates()`, bugünün ISO tarihiyle `/api/kurlar/{tarih}` isteği atıyor, `EXPO_PUBLIC_API_KEY` varsa isteğe `x-api-key` header'ı olarak ekliyor.
- Ekran, backend'in cevabını esnek şekilde okuyor — farklı olası alan isimlerini (`kod`/`code`, `isim`/`name`, `alis`/`buy`, `satis`/`sell`) tanıyıp tabloya yerleştiriyor (bu, daha önce planda bahsedilen "normalizasyon" katmanı).
- `npx tsc --noEmit` (type-check) çalıştırıldı, hatasız geçti — kod TypeScript kurallarına uygun.
- Ayrıca `rates.tsx` adlı, ilk denemede oluşturulmuş ama kullanılmayan bir dosyadaki stil hatası da düzeltildi.

**Claude Code'un sorduğu 3 soruya verdiğimiz cevaplar:**
1. Backend tarih formatı (`DD.MM.YYYY` vb.) sorusu → Şimdilik değiştirmedik, önce emulatörde gerçekten test edip bir format hatası (400) alıp almadığımızı göreceğiz — tahmin yerine doğrulama.
2. Eski `rates.tsx` dosyasını kaldırma → Evet, kaldırılması istendi (tek ekranlı düzen, kullanılmayan kod bırakmamak için).
3. Ayrı `/rates` route'u ekleme → Şimdilik hayır — sekme/menü yapısı planda **Ekran 2** görevine ait ("alt kısmına ekran geçişleri için menü eklenmesi"), bugün erken eklemek karışıklık yaratabilirdi.

## Emulatör Sorunu ve Çözümü

Metro başlatıldıktan sonra `a` tuşuna basınca `CommandError: No Android connected device found` hatası alındı — Pixel 7 emulatörü kapanmıştı. Android Studio'nun Welcome ekranındaki **⋮ (More Actions) → Virtual Device Manager** yolu izlenerek Pixel 7 tekrar başlatıldı.

**Bir ara kafa karıştıran nokta:** Emulatör penceresinde önce "BeautyAnalysis" adlı, bambaşka/eski bir projeye ait bir uygulama görüldü — bu, `TcmbKurMobil` ile ilgisi olmayan, daha önce açılmış farklı bir emulatör penceresiydi, karıştırılmaması gerekiyordu.

**Ayrıca fark edilen bir güvenlik notu:** Bu sırada paylaşılan bir ekran görüntüsünde, WhatsApp'ta kendine atılan bir mesajda gerçek Google Client Secret ve API Key değerleri açık şekilde görünüyordu. Bu değerlerin ekran görüntüsü/mesaj olarak paylaşılması, gizli kalması gereken bilgilerin dışarı sızması riski taşıyor — ileride bu tür gerçek anahtarları ekran görüntüsüne dahil etmemek, gerekirse mevcut anahtarları iptal edip yenilemek önemli bir ders.

## İlk Test Sonucu: Tablo Açıldı Ama Alış/Satış Sütunları Boş

Doğru emulatör (Pixel 7) açılıp `TcmbKurMobil` uygulaması çalıştırıldığında:

- **İyi haber:** Uygulama gerçekten backend'e bağlanabildi — "Bugün için döviz kuru bulunamadı" boş durumu **değil**, gerçek bir tablo göründü. USD, AUD, DKK, EUR, GBP, CHF, SEK gibi kodlar ve isimleri (US DOLLAR, EURO, vb.) doğru şekilde listelendi.
- **Bulunan sorun:** Tablodaki **Alış ve Satış sütunları tüm satırlarda boş** — hiçbir sayı görünmüyor.

**Bunun muhtemel nedeni:** `index.tsx`'teki normalizasyon katmanı, backend'in JSON cevabındaki alış/satış alanlarını `alis`/`buy` veya `satis`/`sell` gibi isimlerle aramaya çalışıyordu (bkz. yukarıdaki "TODO Listesi Tamamlandı" bölümü). Ama gerçek backend cevabındaki alan isimleri muhtemelen bunlardan farklı (örneğin `AlisFiyati`/`SatisFiyati`, ya da başka bir isimlendirme) — yani "tahmin ettiğimiz" 4 olası isim gerçek veriyle eşleşmiyor, sadece `kod`/`isim` şans eseri eşleşmiş olabilir.

**Bu neden önemli bir bulgu:** Bu, planın "Doğrulama" adımının tam olarak neden gerekli olduğunu gösteriyor — emulatörde gerçekten test etmeden, sadece kodun "mantıklı görünmesine" güvenseydik bu sorunu fark etmeyecektik.

## Bulunan Gerçek Alan İsimleri ve Düzeltme

Claude Code, backend'in gerçek JSON cevabını inceleyip normalizasyon kodunu güncelledi. Gerçek alan isimleri, tahmin ettiğimiz `alis`/`buy`, `satis`/`sell` değil, TCMB'nin kendi isimlendirmesiymiş:

- `ForexBuying`, `ForexSelling` (döviz alış/satış)
- `BanknoteBuying`, `BanknoteSelling` (efektif alış/satış)
- `CurrencyName` (isim alanı)

**Neden bu isimler farklıydı:** Backend'in `TcmbKurServisi`si, TCMB'nin resmi XML'inden veriyi çekerken muhtemelen TCMB'nin kendi alan isimlerini olduğu gibi kullanmış (1. haftadan beri süregelen bir isimlendirme) — mobil tarafta tahmin ettiğimiz genel isimler bunlarla eşleşmiyordu. Bu, "normalizasyon katmanı" fikrinin tam olarak neden var olduğunu gösteriyor: iki taraf farklı isimlendirme kullanabiliyor, aradaki köprüyü bu katman kuruyor.

**Ek olarak sorulan soru:** Sayısal formatlamayı da (`toLocaleString('tr-TR')` ile virgüllü, örn. `32,7500`) uygulamak isteyip istemediğimiz soruldu → Evet denildi — web tarafında da (10. günden beri) Türkçe kültür/format kullanılıyor, mobilde de tutarlı olması için aynı yaklaşım uygun.

## İkinci Tahmin de Tutmadı — Konsola Yazdırıp Gerçek Veriyi Görme

`ForexBuying`/`ForexSelling`/`CurrencyName` düzeltmesi de işe yaramadı — sütunlar yine boş kaldı. Bu noktada tahmin etmeyi bırakıp, `getRates()` fonksiyonunun döndürdüğü ham JSON'u doğrudan konsola (`console.debug`) yazdırdık ve Metro'nun çalıştığı terminalde gerçek veriyi gördük.

**Gerçek veri şekli, iki açıdan da tahminlerden farklı çıktı:**

1. **Cevap bir dizi değil, bir obje (dictionary):** `{"USD": {...}, "EUR": {...}, "GBP": {...}, ...}` — anahtar, döviz kodunun kendisi. Kod/İsim'in baştan beri doğru gelmesinin nedeni, bu şeklin zaten doğru işlenmesiydi.
2. **Gerçek alan isimleri Türkçe ve camelCase:** `kod`, `isim`, `forexAlis`, `forexSatis`, `birim`, `id`, `tarih`. Ne ilk tahmin (`alis`/`buy`, `satis`/`sell`) ne ikinci tahmin (`ForexBuying`/`ForexSelling`, İngilizce PascalCase) bunlarla eşleşmiş — doğru isimler `forexAlis` ve `forexSatis` imiş.

**Buradaki önemli ders:** İki kere art arda "mantıklı görünen" isim tahmin edip denedik, ikisi de yanlış çıktı. Üçüncü denemede tahmin etmek yerine **gerçek veriyi konsola yazdırıp görmek**, sorunu kesin olarak çözdü — tahmin yerine doğrulama, burada da işe yaradı (tıpkı 2. haftadaki decimal/kültür hatasında olduğu gibi: regresyonu kanıtlamak için testi bilerek bozup görmüştük).

## Sonuç: Alış/Satış Sütunları Doldu

Claude Code, normalizasyon kodunu `forexAlis`/`forexSatis` alanlarını önceliklendirecek şekilde güncelledi. Emulatörde tekrar test edildi: **tablo artık tamamen doğru** — Kod, İsim, Alış, Satış sütunlarının hepsi gerçek verilerle doluyor.

## Sayısal Format Teyidi ve Network Doğrulaması

Sayılar emulatörde `47,8293` gibi virgüllü Türkçe formatta görünüyor — `toLocaleString('tr-TR')` doğru çalışıyor.

**Network/header doğrulaması ayrı bir adım gerektirmedi, dolaylı olarak zaten kanıtlanmış oldu:**
- Adres (`10.0.2.2:5183`) yanlış olsaydı → bağlantı hatası alınırdı (tıpkı planın ilk halindeki 5000 port hatasında beklediğimiz gibi).
- `X-Api-Key` header'ı eksik/yanlış olsaydı → backend `401` döndürürdü (10. günde tam olarak bu senaryo test edilmişti).

İkisi de olmadan, gerçek veri sorunsuz geldiğine göre, hem doğru adrese hem doğru kimlik doğrulamayla ulaşıldığı kanıtlanmış oluyor.

## ✅ Görev Durumu

**"Mobil Uygulama - Ekran 1" (Döviz Kurları Tablosu) tamamlandı.** Uygulama, emulatörde `10.0.2.2:5183` üzerinden 10. günde yazılan REST API'yi çağırıyor, `.env`'de saklanan API Key ile kimlik doğruluyor, gelen veriyi (obje şeklinde, Türkçe camelCase alan isimleriyle: `kod`, `isim`, `forexAlis`, `forexSatis`) normalize edip Kod/İsim/Alış/Satış sütunlu bir tabloda, Türkçe sayı formatıyla gösteriyor.

**Sırada:** Bu günün raporu yazılıp `raporlar.md`'ye eklenecek, ardından "Ekran 2" (döviz çevirici + alt menü) görevine geçilecek.
