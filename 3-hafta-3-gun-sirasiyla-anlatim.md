# 3. Hafta, 3. Gün — Bugün Ne Yaptık, Neden Yaptık (Sırasıyla)

Bu dosya, dünkü "sırasıyla anlatım" dosyasının bugünkü karşılığı — gün ilerledikçe buraya adım adım ekleyeceğim, gün bitince tam bir baştan-sona anlatı haline gelecek.

## 1. Görev Neydi

Plandaki adı: **"Mobil Uygulama - Ekran 2"**. Amaç: kullanıcının kaynak/hedef para birimi seçip miktar girerek döviz çevirebileceği bir ekran, ve bunu dünkü Ekran 1 ile birlikte gezinebileceğimiz bir alt menü.

**Neden bugünün işi dünden bağımsız değil:** Dün tek ekranlı bir uygulamamız vardı. Bugün ikinci ekran eklenince, ilk kez "iki ekran arasında nasıl geçiş yapılır" sorusuyla karşılaşıyoruz — bu da tab menüsü ihtiyacını doğuruyor. Ayrıca çevirici mantığı, 1. haftada web tarafında zaten yazdığımız çapraz kur hesaplamasının mobildeki karşılığı olacak.

## 2. Neden Yine Plan Modu

Bu görev de mimari bir karar içeriyor: hesaplama nereden yapılacak (backend'de yeni bir endpoint mi, yoksa mevcut kurlarla mobil tarafta mı), ekranlar arası geçiş nasıl kurulacak. Dünkü gibi önce planı görüp değerlendirmek, doğrudan koda geçmekten daha güvenli.

## 3. Plan Modunda Bilerek Belirsiz Bıraktığımız Bir Nokta

Metne, hesaplamanın backend'de yeni bir endpoint mi gerektireceğini yoksa mobil tarafta mı yapılacağını **kesin söylemeden**, "önce kontrol et" diye eklettik. Bunu bilerek yaptık — çünkü backend'in mevcut çapraz kur mantığının dışarıya (REST API olarak) açık olup olmadığını biz de bilmiyoruz. Claude Code'un bunu incelemesini, tahmin etmemizden daha güvenilir bulduk.

## 4. Claude Code'un Sorduğu 3 Hızlı Soru

Claude Code, planını oluştururken bize 3 kısa soru sordu: tab menüsündeki etiket ne olsun (**Çevirici** dedik), hangi ikon kullanılsın (**swap**, iki oklu takas ikonu dedik) ve backend'de dönüştürme endpoint'i yoksa geçici bir **client-side fallback** (mobil tarafta hesaplama) oluşturulsun mu (ilk başta **evet** dedik).

## 5. Client-Side Fallback Kararını Geri Aldık — Neden

Onaylamadan hemen önce durup düşündük: 2. hafta 3. günde, çapraz kur hesaplama mantığını özellikle test edilebilir hale getirip 6 birim testle (kültür/decimal hatası dahil) doğrulamıştık. Aynı hesaplamayı şimdi JavaScript'te yeniden yazmak iki soruna yol açardı:

- İş mantığı iki ayrı yerde (backend + mobil) bulunurdu — biri değişince diğeri unutulabilirdi.
- Tam olarak daha önce bulup düzelttiğimiz decimal/kültür hatasına tekrar düşme riski vardı.

Bu yüzden fikrimizi değiştirip, client-side fallback yerine **backend'de zaten var olan, test edilmiş hesaplama mantığını kullanan küçük bir yeni endpoint** eklemeye karar verdik.

## 6. Claude Code, Backend'i Görmeden Kod Üretti — Yine Tahmin Hatası

Claude Code'un o an sadece mobil proje klasörüne erişimi vardı, backend'e değil. Bu yüzden istediğimiz `GET /api/donustur` endpoint'inin C# kodunu **tahmin ederek** üretti — tıpkı sabahki `forexAlis`/`forexSatis` alan isimlerini tahmin etmesi gibi.

Gerçek backend kodunu (`KurlarController.cs`, `CaprazKurHesaplayici.cs`) inceleyip karşılaştırdık ve yine tahminlerin gerçekle uyuşmadığını gördük:

1. Üretilen kodda güvenlik kontrolü (`[Authorize(AuthenticationSchemes = "ApiKey")]`) eksikti.
2. `CaprazKurHesaplayici`, Claude Code'un varsaydığı gibi enjekte edilen bir arayüz değil, statik bir metoddu — farklı bir imzaya sahipti.
3. Tarih ayrıştırma yine kültüre duyarlı, riskli bir yöntemle yazılmıştı (`DateTime.TryParse` yerine `TryParseExact` + `InvariantCulture` gerekiyordu).

**Bugünün tekrar eden dersi budur:** Yapay zeka aracı, erişimi olmayan bir kod tabanı hakkında ne kadar "mantıklı" görünürse görünsün tahmin ürettiğinde, gerçek kodla karşılaştırmadan güvenmemek gerekiyor — bunu sabah mobil tarafında, öğleden sonra da backend tarafında iki kez yaşadık.

## 7. Düzeltilmiş Kod Eklendi ve Test Edildi

Gerçek imzalara uygun `DonusturController.cs` ve testleri (`DonusturControllerTests.cs`) backend projesine eklendi. `dotnet build` ve `dotnet test` çalıştırıldı: derleme başarılı, **18 testin hepsi geçti** (önceki 12 + yeni 6).

## 8. Frontend Onaylandı, Uygulandı

Backend hazır olduğunu bildirdikten sonra Claude Code'a frontend değişikliklerine devam etmesi için onay verildi. Üç dosya değişti: `api.ts` (convert() fonksiyonu), `src/app/convert.tsx` (yeni Çevirici ekranı), `app-tabs.tsx`/`app-tabs.web.tsx` (alt menüye Çevirici sekmesi). TypeScript kontrolü hatasız geçti.

## 9. Test Etmeye Çalışırken Beş Farklı Sorunla Karşılaşıldı

Sırasıyla:

1. Emulatör kapanmıştı, açılmıyordu.
2. Tekrar açılınca sonsuz döngüde takıldı — muhtemelen önceki günlerde düzgün kapatılmayan emulatörün bozulan "hızlı açılış" kaydı yüzünden.
3. Cold Boot ve Wipe Data denendi, ikisi de çözmedi.
4. Web'de test etmeye çalışıldı (backend'i doğru adresle geçici olarak işaret ederek) — bu sefer tarayıcının CORS güvenlik kuralı devreye girdi ve istek engellendi.
5. Son çare olarak Mac yeniden başlatıldı — bu, sorunu kesin olarak çözdü.

**Buradaki ders:** Bazen sorun kodda değil, geliştirme ortamının kendisinde (bu sefer macOS'un sanallaştırma katmanında) oluyor. Böyle durumlarda kod tarafında daha fazla "düzeltme" aramak yerine, ortamı temizden başlatmak (yeniden başlatma) en hızlı çözüm olabiliyor.

## 10. Sonuç: İki Ekran da Çalışıyor

Mac yeniden başlatıldıktan sonra backend ve mobil dev server tekrar başlatıldı, emulatör sorunsuz açıldı. Hem **Ekran 1** (kur tablosu, gerçek verilerle dolu) hem **Ekran 2** (Çevirici — 10 USD girilince 14,101 AUD gibi doğru bir sonuç hesaplandı) başarıyla test edildi.

---

**Günün özeti:** Bugün hem mobil hem backend tarafında iki kez "tahmin etme, gerçek veriye/koda bak" dersini yaşadık (mobil alan isimleri, backend controller kodu), ayrıca kod dışı bir sorunu (emulatörün sanallaştırma tıkanıklığı) da deneyerek çözdük. "Mobil Uygulama - Ekran 2" görevi tamamlandı.
