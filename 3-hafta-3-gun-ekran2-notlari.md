# 3. Hafta, 3. Gün — Mobil Uygulama: Ekran 2 (Döviz Çevirici) Notları

Bu dosya, dünkü gibi gün boyunca dolduruluyor — sen VS Code'da işlemleri yaparken ben de burayı güncelliyorum.

## Bugünün Görevi Ne

Plandaki tanım: **"Mobil Uygulama - Ekran 2"** — kullanıcının kaynak/hedef para birimi seçip miktar girerek döviz çevirisi yapabileceği bir ekran, ayrıca ekranlar arası geçiş için alt kısımda bir tab menüsü.

**Neden bu görev tek başına anlamlı değil, önceki günlerle bağlantılı:**
- 1. haftada web tarafında yazdığımız "kur hesaplama" (çapraz kur hesaplama) mantığının mobildeki karşılığı bu olacak — sıfırdan bir hesaplama mantığı yazmıyoruz, var olan bir işi mobile taşıyoruz.
- Dün (Ekran 1) uygulamada tek ekran vardı. Bugün ikinci bir ekran eklenince, aralarında gezinmek için ilk kez bir **navigasyon (tab menü)** yapısı kurmamız gerekiyor.

## Kavramlar — Basitçe

### Neden "önce backend'i kontrol et" dedik

Backend'de şu an sadece `GET /api/kurlar/{tarih}` var — tüm kurları döndüren bir endpoint. Çapraz kur hesaplama mantığı (`HomeController` içinde) web formu için yazılmıştı ama bunun ayrı bir REST endpoint'i olarak dışarı açılıp açılmadığı belirsiz. Bu yüzden Claude Code'a, hesaplama için yeni bir endpoint mi gerekiyor yoksa mobil tarafta (Ekran 1'deki kurları kullanarak) hesaplamayı biz mi yapacağız, önce bunu netleştirmesini istedik — tahmin etmek yerine kontrol etmek.

### Tab menü (alt menü) nedir

Uygulamanın altında, ekranlar arasında geçiş yapmayı sağlayan sabit bir menü çubuğu (çoğu mobil uygulamada görülen "Home / Explore" gibi ikonlu yapı). Expo Router'ın kendi "tabs" yapısı bu iş için kullanılacak.

## Buraya Kadar Ne Yaptık

1. Görev tanımı ve önceki günlerle bağlantısı belirlendi.
2. Claude Code'a verilecek Plan modu metni hazırlandı.

## Plan Moduna Yazdığımız Metin

```
React Native (Expo) projesinde, kullanıcının kaynak ve hedef para birimi
seçip miktar girerek döviz çevirisi yapabileceği bir ekran (Ekran 2)
oluşturmak istiyorum. Hesaplama, .NET backend'deki mevcut çapraz kur
hesaplama mantığını kullanan bir endpoint üzerinden yapılmalı (henüz
böyle bir endpoint yoksa, önce bunun var olup olmadığını kontrol et).
Ayrıca Ekran 1 (kur tablosu) ile Ekran 2 arasında geçiş yapabilmek için
alt kısımda bir tab menüsü eklenmeli.
```

## Claude Code'un Planı ve Bulduğumuz Mimari Sorun

Claude Code, planını hazırlarken 3 kısa soru sordu: tab etiketi (**Çevirici** seçildi), tab ikonu (**swap** seçildi), backend'de dönüştürme endpoint'i yoksa **client-side fallback** oluşturulsun mu (evet denildi — geçici çözüm olarak).

**Ama client-side fallback'i onaylamadan önce bir sorun fark ettik:** 2. hafta 3. günde, çapraz kur hesaplama mantığını (`CaprazKurHesaplayici`) özellikle test edilebilir, saf bir metoda çıkarıp 6 birim testle (kültür/decimal regresyon testi dahil) doğrulamıştık. Aynı hesaplamayı JavaScript'te sıfırdan yazmak, hem iş mantığını iki yerde tutmak (biri değişince diğeri unutulabilir) hem de tam olarak daha önce düzelttiğimiz decimal/kültür hatasına tekrar düşme riski taşıyordu.

**Karar:** Client-side fallback yerine, backend'de zaten var olan `CaprazKurHesaplayici`'yi kullanan küçük bir yeni REST endpoint (`GET /api/donustur`) eklenmesine karar verildi.

## Claude Code'un Ürettiği Backend Kodu — Gerçek Kodla Karşılaştırma

Claude Code, mobil proje klasöründeyken backend'e erişimi olmadığı için, `ConvertController` kodunu **tahmin ederek** üretti. Gerçek backend dosyalarını (`KurlarController.cs`, `CaprazKurHesaplayici.cs`, `ITcmbKurServisi.cs`) inceleyip karşılaştırdığımızda **3 önemli fark** bulduk:

1. **Güvenlik eksikti:** Gerçek `KurlarController`'da `[Authorize(AuthenticationSchemes = "ApiKey")]` var, üretilen kodda bu attribute hiç yoktu — eklenseydi yeni endpoint anahtar gerektirmeden herkese açık olurdu.
2. **`CaprazKurHesaplayici`nin gerçek yapısı farklıydı:** Claude Code, bunun DI ile enjekte edilen bir arayüz (`ICaprazKurHesaplayici`) olduğunu varsaymıştı. Gerçekte bu, `internal static class` içinde `Hesapla(Dictionary<string, DovizKuru> kurlar, string kaynakDoviz, string hedefDoviz, decimal miktar)` imzalı statik bir metod.
3. **Tarih ayrıştırma yine kültür riski taşıyordu:** Üretilen kodda düz `DateTime.TryParse` kullanılmıştı; gerçek `KurlarController` ise `DateTime.TryParseExact(..., CultureInfo.InvariantCulture, ...)` kullanıyor — 2. haftadaki decimal/kültür dersinin aynısı, bu sefer tarih için.

**Bu, bugünkü mobil taraftaki "iki kez tahmin edip yanılma" olayının backend'deki karşılığı oldu** — gerçek kodu kontrol etmeden bu 3 fark fark edilemezdi.

## Düzeltilmiş Kod Eklendi ve Doğrulandı

Gerçek imzalara uygun, düzeltilmiş `DonusturController.cs` ve `DonusturControllerTests.cs` (Moq kullanmadan, projenin kendi stiline uygun elle yazılmış bir sahte servisle) `TcmbKurDonusturucu` projesine eklendi.

```
dotnet build   → Build succeeded (1 uyarı, bizim kodumuzla ilgisiz bir NuGet paket versiyon çakışması)
dotnet test    → 18/18 test başarılı (önceki 12 + yeni eklenen 6)
```

## Frontend Uygulaması

Claude Code, backend hazır olduktan sonra 3 dosyada değişiklik yaptı:

- **`api.ts`** — `convert()` fonksiyonu eklendi, `GET /api/donustur` endpoint'ini çağırıyor.
- **`src/app/convert.tsx`** — yeni Çevirici ekranı: kaynak/hedef para birimi seçimi, miktar girişi, "Çevir" butonu, sonuç gösterimi.
- **`app-tabs.tsx`** ve **`app-tabs.web.tsx`** — alt menüye "Çevirici" sekmesi (swap ikonu) eklendi.

`npx tsc --noEmit` hatasız geçti.

## Test Süreci — Emulatör Sorunları ve Çözümü

Test aşamasında sırasıyla şu sorunlarla karşılaşıldı:

1. **Emulatör açılmıyordu** ("No Android connected device found") — Pixel 7 kapanmıştı.
2. **Yeniden açılınca sonsuz döngüde takıldı** (beyaz ekranda dönen spinner, dakikalarca bitmedi) — muhtemel neden: önceki gün(ler)de emulatörün düzgün kapatılmadan pencereden kapatılması, "hızlı açılış" (quick-boot) snapshot'ını bozmuş olması.
3. **Cold Boot denendi, çözmedi. Wipe Data denendi, o da çözmedi.**
4. **Geçici çözüm olarak web'de test edildi** (`EXPO_PUBLIC_API_BASE_URL` .env'e dokunmadan, tek seferlik komut satırı override'ıyla `localhost:5183`'e yönlendirildi) — ama bu sefer **CORS hatası** ("Failed to fetch") çıktı, çünkü tarayıcılar farklı porttaki bir API'ye isteği güvenlik gereği engelliyor (emulatör/mobilde bu kural yok).
5. **Kalıcı çözüm: Mac'in yeniden başlatılması.** Bu, emulatörün sanallaştırma katmanındaki tıkanıklığı giderdi — yeniden başlatma sonrası emulatör sorunsuz açıldı.

## ✅ Sonuç: Her İki Ekran da Çalışıyor

Mac yeniden başlatıldıktan sonra: backend (`dotnet run`) ve mobil dev server (`npx expo start -c`) tekrar başlatıldı, emulatör sorunsuz açıldı.

- **Ekran 1 (Döviz Kurları):** Tablo gerçek verilerle doluyor.
- **Ekran 2 (Çevirici):** Kaynak/hedef seçilip miktar girilince "Çevir" butonuyla doğru sonuç hesaplanıyor (test: 10 USD → 14,101 AUD, günün kurlarına göre doğru).

**"Mobil Uygulama - Ekran 2" görevi tamamlandı.**
