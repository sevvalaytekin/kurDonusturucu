# Staj Rehberi — İlk 5 Gün (1. Hafta: ASP.NET Temel Geliştirme)

Bu doküman, 6-10. günleri anlatan rehberin devamı niteliğinde ve aynı mantıkla hazırlandı: **"ne yapıldı" değil "neden böyle yapıldı."**

Buradaki bilgiler, projenin şu anki kodunu (`Controllers/HomeController.cs`, `Services/TcmbKurServisi.cs`, `Models/DovizKuru.cs`, `CLAUDE.md`) ve git commit geçmişini inceleyerek çıkarıldı — yani gerçekten projede var olan şeyleri anlatıyorum, tahmin değil.

Plandaki karşılığı: **1. HAFTA — ASP.net** (Asp.Net Temel Geliştirme: 3 gün, Veritabanı Desteğinin Eklenmesi: 2 gün).

---

## 1-3. GÜN — Asp.Net Temel Geliştirme

### 1.1. Uygulamanın amacı ne?

Plan şunu söylüyor: *"Merkez bankası web servisleri üzerinden günlük dolar kurlarını alıp, kullanıcıların tarih bazında kurlar arası çapraz dönüşüm yapmasını sağlayan bir web uygulaması."*

Yani üç temel yetenek gerekiyor:
1. TCMB'den (Türkiye Cumhuriyet Merkez Bankası) döviz kuru verisini çekebilmek.
2. Kullanıcının seçtiği iki para birimi arasında (örn. USD → EUR) çevrim yapabilmek — TCMB direkt USD→EUR kuru vermiyor, sadece her para biriminin TL karşılığını veriyor, o yüzden "çapraz kur" hesaplaması gerekiyor.
3. Bunu bir web arayüzünden kullanılabilir hale getirmek.

### 1.2. Neden ASP.NET Core MVC, neden bu mimari ayrımı (Controller / Service / Model / View)?

Kodun şu an nasıl bölündüğüne bak:
- **`HomeController.cs`** → sadece HTTP isteğini karşılıyor, parametreleri alıyor, sonucu JSON olarak döndürüyor. İçinde TCMB'ye nasıl bağlanılacağı ya da XML'in nasıl ayrıştırılacağı gibi detaylar **yok.**
- **`TcmbKurServisi.cs`** → asıl iş mantığı burada: veri nereden alınacak, nasıl işlenecek.
- **`DovizKuru.cs`** (Models) → verinin şeklini tanımlıyor (hangi alanlar var).
- **`Views/Home/Index.cshtml`** → kullanıcının gördüğü form.

**Neden bu ayrım önemli?** Diyelim ki yarın TCMB, veriyi XML yerine JSON olarak sunmaya başladı. Bu durumda sadece `TcmbKurServisi` değişir — `HomeController` ve `View` hiç dokunulmaz, çünkü onlar "veri nereden geliyor"yu bilmiyor, sadece "sonucu nasıl kullanacağını" biliyor. Bu prensibe **sorumlulukların ayrılması (separation of concerns)** denir — her parça tek bir işten sorumlu olursa, bir şeyi değiştirmek diğerini bozma riski taşımaz.

`HomeController`, `ITcmbKurServisi` interface'ini constructor'da alıyor (`private readonly ITcmbKurServisi _kurServisi`) — somut `TcmbKurServisi` sınıfını değil, onun *arayüzünü*. **Neden interface üzerinden?** Bu sayede controller, "veriyi nasıl aldığını" hiç bilmeden sadece "bana kurları getir" diyebiliyor. İleride (nitekim 8-9. günde yaptığımız gibi) bu servisin test edilmesi veya değiştirilmesi gerektiğinde, controller'a hiç dokunmadan bunu yapabiliyorsun.

### 1.3. TCMB'den veri nasıl çekiliyor, neden bu adres formatı?

`TcmbKurServisi` içinde şu satırı görüyoruz:
```
https://www.tcmb.gov.tr/kurlar/{yyyyMM}/{ddMMyyyy}.xml
```
TCMB, her gün için ayrı bir XML dosyası yayınlıyor, adres kalıbı yıl-ay klasörü + gün-ay-yıl dosya adı şeklinde. Yani "2026 Ağustos ayının 12'si" için `202608/12082026.xml` gibi bir adrese gidiliyor.

`HttpClient`, .NET'in dış bir web adresine istek atmak için kullandığı standart sınıf — burada dependency injection ile (`TcmbKurServisi`'nin constructor'ına) veriliyor. **Neden constructor'a "enjekte ediliyor", neden `new HttpClient()` diye doğrudan oluşturulmuyor?** `HttpClient`'i doğrudan her seferinde `new` ile oluşturmak, .NET'te bilinen bir performans/kaynak sorunu (soket tükenmesi) yaratabilir. ASP.NET Core'un yerleşik `HttpClient` yönetimi (`IHttpClientFactory` altyapısı üzerinden), bu sınıfın ömrünü ve bağlantı havuzunu doğru yönetiyor. Bu da "framework'ün sana sunduğu altyapıyı, kendi çözümünü yazmak yerine kullan" prensibinin bir örneği.

### 1.4. XML nasıl ayrıştırılıyor (`XmlAyristir`)?

TCMB'nin XML'i, her para birimi için bir `<Currency>` elemanı içeriyor, bunun içinde `CurrencyCode` (örn. "USD"), `Unit` (bazı kurlar 1 birim değil 100 birim üzerinden verilir), `ForexBuying`/`ForexSelling` (döviz alış/satış) gibi alanlar var.

Kod, `XDocument.Parse` ile bu XML'i bir nesne ağacına çeviriyor, sonra her `Currency` elemanını gezip bir `DovizKuru` nesnesine dönüştürüyor. Şu satır dikkat çekici:
```csharp
Birim = birim == 0 ? 1 : birim,
```
**Neden bu kontrol var?** Eğer TCMB'nin verdiği `Unit` alanı herhangi bir sebeple ayrıştırılamazsa (`decimal.TryParse` başarısız olursa), `birim` değişkeni varsayılan olarak `0` kalır. Ama bir kur hesaplamasında **sıfıra bölme** felaket bir hataya (exception, ya da sonsuz/tanımsız sonuç) yol açar. `birim == 0 ? 1 : birim` ifadesi, "eğer ayrıştırma başarısız olduysa, en azından güvenli bir varsayılan (1) kullan, uygulamayı çökertme" diyen savunmacı bir programlama tekniği.

Bu metodun `internal static` olarak yazılmış olması (yani DB veya ağ bağlantısına ihtiyaç duymadan, saf bir XML string'i alıp sonuç döndürmesi) tam olarak 8-9. gün rehberinde bahsettiğimiz "test edilebilirlik" tasarımının **temelini** burada, ilk günlerde atmış — o yüzden hafta 2'de unit test yazmak nispeten kolay oldu.

### 1.5. Çapraz kur hesaplama — neden böyle bir formül gerekiyor?

TCMB, örneğin USD için "1 USD = X TL", EUR için "1 EUR = Y TL" bilgisini veriyor. Ama kullanıcı "100 USD kaç EUR eder?" diye sorduğunda, TCMB'de doğrudan "USD→EUR" kuru yok.

Çözüm: her iki para birimini **ortak bir referans** (TL) üzerinden karşılaştırmak.
- `1 USD = X TL`
- `1 EUR = Y TL`
- O halde `1 USD = (X / Y) EUR`

`CaprazKurHesaplayici` (bu sınıf sonradan, 8-9. günde `HomeController`'ın içinden çıkarılıp ayrı, test edilebilir bir dosyaya taşındı — ama mantığı ilk kez bu günlerde yazıldı) tam olarak bunu yapıyor: her iki para biriminin TL karşılığını buluyor, birbirine oranlıyor.

**Neden TL "ortak referans" olarak seçildi, başka bir para birimi değil?** Çünkü TCMB'nin yayınladığı tüm kurlar zaten TL bazında (`ForexAlis`/`ForexSatis` hep "X birim döviz kaç TL eder" şeklinde). Yani TL, bu veri kaynağının doğal ortak paydası — başka bir para birimi seçmek gereksiz bir ek dönüşüm adımı gerektirirdi.

### 1.6. "Resmi tatil kontrolü" hakkında dürüst bir not

Git geçmişinde `3025c51` commit'i "Resmi tatil kontrolü eklendi" diyor. Ama CLAUDE.md'yi hazırlarken kodu incelediğimde şunu fark ettim (ve bunu olduğu gibi, saklamadan söylüyorum çünkü senin için "gerçekten ne var" bilmek önemli): **bu commit gerçek bir resmi tatil takvimi eklememiş.** Önceden var olan bir "hafta sonu kaydırma" mantığını kaldırıp, yerine genel bir `try/catch` bloğu koymuş. Yani şu anki kodda, TCMB bir gün için veri döndürmezse (tatil, hafta sonu ya da başka bir sebepten), kullanıcıya sadece genel bir hata mesajı gösteriliyor — "bugün resmi tatil olduğu için veri yok" gibi özel/akıllı bir mantık yok.

**Bunu neden burada anlatıyorum, neden görmezden gelmiyorum?** Çünkü bir commit mesajının "ne yapıldığını iddia ettiği" ile kodun "gerçekte ne yaptığı" bazen örtüşmeyebilir — bu, deneyimli yazılımcıların bile düştüğü, kötü niyetten değil çoğunlukla zaman baskısı veya "sonra düzeltirim" düşüncesinden kaynaklanan bir durum. Senin için önemli olan ders şu: **bir işi "bitti" diye işaretlemeden önce, commit mesajına değil kodun gerçekte yaptığına bak.** Bu tam olarak, senin de baştan beri benden istediğin dürüstlük ilkesiyle örtüşüyor — ve bu, ileride gerçek bir resmi tatil kontrolü eklemek istersen, bunun hâlâ **yapılmamış bir iş** olduğunu bilmen gerektiği anlamına geliyor.

---

## 4-5. GÜN — Veritabanı Desteğinin Eklenmesi

### 2.1. Neden bir veritabanına ihtiyaç var — TCMB'ye her seferinde gitmek neden yeterli değil?

İlk 3 günün sonunda uygulama zaten çalışıyordu: kullanıcı tarih seçiyor, TCMB'den veri çekiliyor, çapraz kur hesaplanıyordu. Ama bunun iki sorunu var:

1. **Performans:** Her hesaplama isteğinde TCMB'nin sunucusuna gidip XML indirmek, veritabanından okumaktan çok daha yavaş — ağ gecikmesi (network latency) var.
2. **Dış sisteme gereksiz yük:** TCMB, kendi sunucusuna gelen isteklerin sana özel olmadığını, herkesin kullandığı paylaşımlı bir kaynak olduğunu unutma. Aynı tarih için binlerce kez aynı veriyi tekrar tekrar istemek, hem gereksiz hem de TCMB'nin bakış açısından "kötü vatandaşlık" (iyi bir API tüketicisi olmamak) sayılır. Ayrıca geçmiş bir tarihin kuru **zaten değişmeyecek** — yani onu tekrar tekrar TCMB'den istemenin hiçbir mantıklı gerekçesi yok.

Bu yüzden PostgreSQL eklendi ve bir **cache (önbellek)** mantığı kuruldu.

### 2.2. Cache mantığı tam olarak nasıl çalışıyor — neden bu sıra?

`TcmbKurServisi.KurlariGetirAsync` metodunun akışı şu:
1. Önce veritabanında (`DovizKurlari` tablosu) o tarihe ait kayıt var mı diye bakılıyor.
2. **Varsa** → TCMB'ye hiç gidilmeden, doğrudan veritabanındaki veri döndürülüyor.
3. **Yoksa** → TCMB'den XML çekiliyor, ayrıştırılıyor, **veritabanına kaydediliyor**, sonra sonuç döndürülüyor.

Bu deseni yazılımda **"cache-aside" (önbellek-yanında)** deseni olarak bilirsin — yani "önce hızlı/ucuz kaynağa bak, orada yoksa yavaş/pahalı kaynağa git, sonucu bir dahaki sefere hızlı kaynakta bulmak için sakla."

**Neden bu sıra (DB önce, TCMB sonra) ve tam tersi değil?** Çünkü amaç TCMB'ye gitmeyi *minimuma indirmek*. Eğer tam tersini yapsaydık (her zaman TCMB'ye gidip, sonra DB'yi güncelleseydik), cache'in hiçbir performans faydası kalmazdı — asıl amaç zaten gereksiz dış istekleri önlemekti.

**Neden aynı tarih için bir daha TCMB'ye gidilmiyor, "belki veri değişmiştir" diye kontrol edilmiyor?** Çünkü geçmiş bir günün resmi döviz kuru, o gün TCMB tarafından yayınlandıktan sonra **değişmez** — bu, "zamanla değişmeyen" (immutable) bir veri türü. Bu tip veriler cache'lemek için ideal, çünkü "cache bayatladı mı" diye endişelenmene gerek yok (örneğin hava durumu gibi sürekli değişen bir veriyi böyle cache'lemek yanlış olurdu).

### 2.3. Entity Framework Core ve migration kavramı neden kullanıldı?

`AppDbContext.cs`, EF Core'un `DbContext` sınıfından türeyen ve `DbSet<DovizKuru> DovizKurlari` tanımlayan bir sınıf. Bu, "veritabanındaki `DovizKurlari` tablosunu, C#'taki `DovizKuru` nesneleriyle eşleştir" demenin yolu — buna **ORM (Object-Relational Mapping)** denir.

**Neden ham SQL yazmak yerine EF Core kullanıldı?** Ham SQL de çalışırdı, ama EF Core ile `_dbContext.DovizKurlari.Where(x => x.Tarih.Date == tarih.Date)` gibi C# kodu yazman yeterli — EF Core bunu arka planda doğru SQL sorgusuna çeviriyor. Bu hem daha az hataya açık (SQL injection gibi güvenlik risklerine karşı da EF Core doğal olarak koruma sağlıyor, çünkü parametreler otomatik olarak güvenli şekilde işleniyor) hem de C# tip sisteminin (derleme zamanı kontrolü) avantajından faydalanmanı sağlıyor.

**Migration nedir, neden gerekli?** Veritabanı şemasını (hangi tablo, hangi kolonlar var) elle SQL yazarak değil, C# model sınıflarından (`DovizKuru.cs`) otomatik olarak **türetmek** için kullanılıyor. `InitialCreate` migration'ı, `DovizKuru` sınıfına bakıp "buna karşılık gelen `DovizKurlari` tablosunu şu kolonlarla oluştur" diyen bir talimat seti üretiyor. **Neden bu önemli:** Migration'lar git'e commit edilebiliyor — yani veritabanı şemasının değişim geçmişi, kod gibi versiyonlanabiliyor. Başka biri (ya da sen, başka bir bilgisayarda) projeyi indirip `dotnet ef database update` çalıştırdığında, veritabanı otomatik olarak doğru şemaya kavuşuyor; elle "şu tabloyu oluştur, şu kolonu ekle" yapmasına gerek kalmıyor.

### 2.4. "backend tekrar yazıldı" commit'i ne anlama geliyor?

Git geçmişinde bu ifadeyi görüyoruz — muhtemelen veritabanı entegrasyonu ilk denemede istenen şekilde oturmadığı için servis katmanının bir kısmı yeniden yazıldı. Bunu **kötü bir şey olarak görmemelisin.** Yazılım geliştirmede ilk yaklaşımın "yeterince iyi olmadığını" görüp yeniden yazmak (**refactoring** veya bazen daha kapsamlı hâliyle yeniden tasarım) son derece normal, hatta sağlıklı bir süreç. Önemli olan, mevcut davranışı bozmadan/kaybetmeden yeniden yazabilmek — ki bunu ileride (8-9. günde) otomatik testlerle güvence altına almayı öğrendin zaten.

### 2.5. appsettings.json'daki bağlantı dizesi — ileride neden sorun oldu?

Bu günlerde `appsettings.json` içine PostgreSQL bağlantı bilgisi (`Host`, `Database`, `Username`, `Password`) eklendi ve bu dosya git'e commit edildi — **şifre dahil, düz metin olarak.**

O anda uygulamanın çalışması için bu yeterliydi, ama CLAUDE.md hazırlanırken bu durum bilinçli olarak **bir risk** olarak işaretlendi: eğer bu proje bir gün GitHub'a push edilirse veya başkalarıyla paylaşılırsa, veritabanı şifresi herkese açık hale gelir.

**Neden bu, 6-10. gün rehberinde bahsettiğimiz `dotnet user-secrets` kararını doğrudan etkiledi?** Çünkü tam olarak bu geçmiş hatadan ders çıkarıldı — 7. günde Google Client ID/Secret'ı eklerken, "geçen sefer appsettings.json'a yazıp commit ettik, bu sefer aynı hatayı tekrarlamayalım" diye bilinçli olarak `dotnet user-secrets` tercih edildi. Yani 1. haftadaki bu küçük gözden kaçırma, 2. haftadaki bir tasarım kararını doğrudan şekillendirdi — bu, gerçek projelerde "geçmiş hatalardan öğrenmenin" nasıl işlediğine dair iyi bir örnek.

(appsettings.json'daki gerçek şifre değeri hâlâ commit geçmişinde duruyor — bunu düzeltmek, yani şifreyi değiştirip user-secrets'a taşımak ve `.gitignore` eklemek, CLAUDE.md'de "gelecekte yapılması gereken" olarak not edilmiş bir iş, henüz yapılmadı.)

---

## Hafta 1'i Hafta 2'ye Bağlayan Bakış

İlk hafta, uygulamanın **temelini** kurdu: veri nereden geliyor (TCMB), nasıl saklanıyor (PostgreSQL cache), nasıl işleniyor (çapraz kur hesaplama). Ama bu temelde henüz şunlar yoktu:
- Kimin bu uygulamayı kullandığına dair hiçbir kontrol (herkes, kimlik doğrulamadan erişebiliyordu).
- Hiçbir otomatik test (bir değişikliğin bir şeyi bozup bozmadığını anlamanın tek yolu elle denemekti).
- appsettings.json'daki şifre sorunu gibi, henüz fark edilmemiş zayıf noktalar.

2. hafta (6-10. günler), tam olarak bu boşlukları kapatmaya odaklandı: kim erişebilir (login + Google OAuth), veri her zaman doğru ve eksiksiz mi (BackgroundService + iki kritik bug düzeltmesi), kodun doğruluğu nasıl kanıtlanır (unit test + E2E test), ve sistem başka programlara nasıl güvenli açılır (REST API + API Key). Yani 1. hafta "çalışan bir şey inşa etmek", 2. hafta ise "bu şeyi güvenilir, doğrulanabilir ve güvenli hale getirmek" temalıydı.

## Şimdi Ne Yapmalısın?

1. Bu dokümanı, 6-10. gün rehberiyle birlikte bir bütün olarak düşün — ikisi projenin baştan sona hikayesini anlatıyor.
2. Özellikle bölüm 1.6'daki "Resmi tatil kontrolü" durumunu ve bölüm 2.5'teki appsettings.json şifre meselesini iyi anla — bunlar "iddia edilen" ile "gerçekte var olan" arasındaki farkı gösteren, gerçek dünyadan iki somut örnek.
3. Anlamadığın veya tartışmak istediğin bir yer varsa konuşalım, sonra 3. haftaya (React Native ile mobil geliştirme) geçebiliriz.
