# Staj Rehberi — 6. ile 10. Gün Arası Yapılanların Detaylı Anlatımı

Bu doküman, "ne yaptık" sorusundan çok **"neden böyle yaptık"** sorusuna cevap vermek için hazırlandı. Amaç, kod satırlarını ezberlemen değil; her kararın arkasındaki mantığı görmen. Kendi hızında oku, anlamadığın veya "peki ama neden illa bu şekilde?" dediğin her yeri işaretle — sonra birlikte konuşuruz.

Doküman gün gün ilerliyor ama günler arasında kavramlar birbirinin üzerine inşa ediliyor, o yüzden sırayla okumanı öneririm.

---

## 6. GÜN — Claude Code Kurulumu ve İlk Özellik: Kullanıcı Girişi

### 6.1. Claude Code nedir, neden bir "AI agent" kullanıyoruz?

Claude Code, senin yerine kod yazabilen ama **senin verdiğin talimatlarla, senin projenin kurallarına göre** çalışan bir araç. Burada kritik nokta şu: Claude Code kodu yazıyor ama **kararları hâlâ sen veriyorsun** — hangi özelliği yapacağını, nasıl davranacağını sen belirliyorsun. Stajının bu haftaki teması tam olarak bu: bir AI agent'ı doğru yönlendirerek gerçek bir yazılım projesine katkı sağlamak. Yani amaç "Claude Code'a her şeyi yaptırmak" değil, **onu bir ekip arkadaşı gibi yönetmeyi öğrenmek.**

### 6.2. Neden üç farklı mod var (Plan / Manual / Edit automatically)?

Bunu bir inşaat analojisiyle düşünebilirsin:

- **Plan modu** = mimarlık projesi çizmek. Hiçbir tuğla örülmüyor, sadece "şunu şöyle yapacağım" diye bir plan sunuluyor. Sen onu okuyup onaylıyorsun ya da değiştiriyorsun. **Neden önemli:** Login gibi güvenlik içeren, yanlış yapıldığında telafisi zor işlerde, kod yazılmadan önce yaklaşımı görmek işine gelir. Kötü bir plana "hayır böyle olmaz" demek, kötü yazılmış koda göre çok daha ucuza mal olur.
- **Manual (Default) modu** = ustaya "her adımdan önce bana sor" demek. Her dosya değişikliğinde onay istiyor. Emin olmadığın, ilk kez yaptığın bir işte güvenlik ağı gibi çalışır — yanlış giderse hemen durdurabilirsin.
- **Edit automatically** = plan zaten onaylandıysa, ustaya "artık biliyorsun ne yapman gerektiğini, devam et" demek. Her adımda durup sormasını istemezsin çünkü zaten neyi yapacağını biliyorsun, sadece hız kazanmak istiyorsun.

**Neden hepsini kullandık, tek bir mod seçmedik?** Çünkü riskin büyüklüğü işe göre değişiyor. Login = yüksek risk → Plan ile başladık. Küçük, geri alınması kolay bir değişiklik = düşük risk → Edit automatically ile hızlandık. Bu, gerçek yazılımcıların da yaptığı bir şey: riske göre kontrol seviyesini ayarlamak.

### 6.3. CLAUDE.md neden gerekli?

Claude Code her yeni konuşmaya "hafızasız" başlar — önceki oturumda ne konuştuğumuzu hatırlamaz. Eğer her seferinde "bu proje ASP.NET Core MVC, PostgreSQL kullanıyor, şu klasörde controller'lar var..." diye baştan anlatman gerekseydi, bu hem zaman kaybı hem de hataya açık olurdu (bir gün unutup yanlış bilgi verebilirdin).

CLAUDE.md, proje kök dizinine konan ve Claude Code'un **her oturumun başında otomatik okuduğu** bir dosya. İçine şunlar yazıldı:
- Teknoloji yığını (tech stack): .NET 10, ASP.NET Core MVC, EF Core, Npgsql/PostgreSQL, vanilla JS frontend
- Klasör yapısı ve mimari (Controller → Service → Model akışı)
- Proje çalıştırma komutları
- **Bilinen sorunlar:** örneğin veritabanı şifresinin git'e düz metin olarak commit'lenmiş olması, `.gitignore` dosyasının eksik olması

**Neden "bilinen sorunlar" da yazıldı?** Çünkü bir AI agent, projeye yeni başlayan bir insan gibi, mevcut kod tabanını "doğru" kabul etme eğilimindedir. Bilinen zayıf noktaları baştan belirtmek, Claude Code'un ileride yanlışlıkla aynı hatayı tekrarlamasını (örneğin yeni bir şifreyi de düz metin yazmasını) önler.

### 6.4. Kullanıcı adı/şifre ile giriş — neden şifreyi doğrudan saklamıyoruz?

Bir veritabanı bir şekilde ele geçirilirse (hack, yanlış yapılandırma, sızıntı), içindeki şifreler düz metin (plaintext) ise saldırgan doğrudan herkesin şifresini okur. Bu hem o sistem hem de kullanıcıların **başka sitelerde de aynı şifreyi kullanıyor olma ihtimali** yüzünden çok tehlikelidir.

Bu yüzden `PasswordHasher<T>` kullanıldı. Bu, şifreyi **PBKDF2-HMAC-SHA256** denen bir algoritmayla "hash"liyor — yani şifreyi geri döndürülemez bir şekilde karıştırıyor. Veritabanında `SifreHash` diye bir alan var, gerçek şifre hiçbir zaman orada durmuyor.

Giriş yaparken şu olur: kullanıcı şifresini yazar → aynı hash fonksiyonu o şifreye uygulanır → çıkan sonuç veritabanındaki hash ile karşılaştırılır. Eşleşiyorsa şifre doğrudur, ama biz hâlâ gerçek şifreyi hiçbir zaman görmedik/saklamadık.

**Neden özel bir algoritma (PBKDF2), neden basit bir MD5/SHA1 değil?** PBKDF2 gibi algoritmalar *kasıtlı olarak yavaş* çalışacak şekilde tasarlanmıştır. Bu, bir saldırganın milyonlarca şifre kombinasyonunu saniyede deneyerek hash'i kırmaya çalışmasını (brute-force) çok daha maliyetli hale getirir. MD5/SHA1 gibi algoritmalar hızlı olduğu için bu tür saldırılara karşı zayıftır.

### 6.5. Cookie tabanlı authentication neden bu şekilde çalışıyor?

HTTP, doğası gereği "stateless" bir protokoldür — yani sunucu, art arda gelen iki isteğin aynı kullanıcıdan geldiğini normalde bilemez. Her istek birbirinden bağımsızdır.

Çözüm: kullanıcı giriş yaptığında sunucu, tarayıcıya küçük bir "kimlik kartı" (cookie) veriyor. Tarayıcı bu cookie'yi her sonraki istekte otomatik olarak sunucuya geri gönderiyor. Sunucu da "bu cookie geçerli, bu kullanıcı X" diye tanıyor.

**Neden cookie, neden mesela her sayfada tekrar şifre sormuyoruz?** Kullanıcı deneyimi açısından bu kabul edilemez olurdu. Cookie, "bir kere kimliğini kanıtla, ben seni bir süre hatırlayayım" mantığıyla çalışır — tıpkı bir etkinlikte girişte bilekliğe damga vurulması, sonra her kapıda yeniden bilet göstermek zorunda kalmaman gibi.

### 6.6. "Address already in use" hatası — neden oluyor, nasıl çözülür?

`dotnet run` komutu uygulamayı belirli bir port üzerinde (örneğin 5183) çalıştırır. Eğer o port zaten başka bir process tarafından kullanılıyorsa (örneğin önceki `dotnet run` düzgün kapanmadıysa), yeni başlatma denemesi "bu port zaten kullanımda" hatası verir.

Çözüm adımları ve **neden** her adımın gerekli olduğu:
1. `lsof -i :5183` → bu komut, o portu şu anda kim kullanıyor diye sorar, sana bir PID (process ID) verir. **Neden gerekli:** Kapatman gereken şeyin tam olarak hangi process olduğunu bilmeden rastgele bir şeyi kapatamazsın.
2. `kill -9 <gerçek PID numarası>` → bulunan process'i zorla sonlandırır. **Dikkat:** buradaki PID, `PID` diye yazılan bir kelime değil, `lsof` çıktısında gördüğün gerçek sayı (örneğin 42193).
3. `dotnet run` tekrar çalıştırılır → artık port boş olduğu için sorunsuz başlar.

Bu, gerçek geliştirme hayatında sürekli karşılaşacağın, "korkulacak" değil "tanınacak" bir hata.

---

## 7. GÜN — Google ile Giriş (OAuth 2.0)

### 7.1. OAuth 2.0 mantığı — neden "kendi" login sistemimiz varken buna ihtiyaç var?

Kullanıcı adı/şifre sistemi çalışıyor, ama kullanıcılara "bir şifre daha ezberle" demek yerine "zaten Google hesabın var, onunla gir" demek çoğu insanın tercih ettiği bir yöntem. Ayrıca güvenlik açısından da avantajlı: **kullanıcının Google şifresini biz hiçbir zaman görmüyoruz, saklamıyoruz.**

OAuth 2.0'ın temel mantığı şu: kimlik doğrulamayı (bu gerçekten o kişi mi?) biz değil, Google yapıyor. Google bize sadece "evet bu kullanıcı gerçek, işte bilgileri: benzersiz ID'si (`sub`), adı, e-postası" diyor. Biz bu bilgiye güveniyoruz çünkü Google'ın kendisi bunu doğruluyor.

Bunu bir analoji ile düşünebilirsin: bir binaya girerken kimliğini güvenliğe göstermek yerine, "devlet onaylı" bir kart (nüfus cüzdanı gibi) gösteriyorsun. Binanın güvenliği senin gerçek kimliğini bilmiyor, sadece devletin (Google'ın) "bu kişi gerçek ve doğrulanmış" demesine güveniyor.

### 7.2. Client ID / Client Secret nedir, neden panelde oluşturuluyor, kodla değil?

Google'a "istek gönderen ben gerçekten TcmbKurDonusturucu uygulamasıyım" demek için bir kimlik gerekiyor. Bu, Google Cloud Console'da bir proje açıp "OAuth Client" oluşturarak elde ediliyor:
- **Client ID:** herkese açık, uygulamanın "kimliği" — gizli değil.
- **Client Secret:** gizli, sadece sunucu tarafında bilinmesi gereken bir "şifre" gibi. Bu sızarsa, biri senin uygulaman gibi davranarak Google'a istek atabilir.

**Neden kodla değil panelden?** Çünkü bu, Google'ın kendi güvenlik altyapısının bir parçası — her uygulamanın kaydını tutmaları, kötüye kullanımı izlemeleri gerekiyor. Bu bir kod satırıyla değil, Google'ın kendi sisteminde bir "kayıt" işlemiyle oluyor.

### 7.3. Redirect URI (yönlendirme adresi) — bu güvenlik önlemi neyi engelliyor?

Google, kullanıcı girişi onayladıktan sonra kullanıcıyı **sadece önceden Google Console'da tanımladığımız adreslere** geri gönderir (bizim örneğimizde `/signin-google` gibi bir adres).

**Neden bu bir güvenlik önlemi?** Diyelim ki bu kısıtlama olmasaydı. Kötü niyetli biri, senin Client ID'ni kullanarak (Client ID zaten gizli değil, herkese açık) kullanıcıyı Google girişine yönlendirebilir, ama girişten sonra kullanıcıyı **kendi sahte sitesine** yönlendirebilirdi — kullanıcı "bu TcmbKurDonusturucu'ya giriş yapıyorum" sanırken, kimlik bilgileri aslında başka bir yere gidebilirdi. Redirect URI kısıtlaması, "Google sadece önceden kayıtlı, güvenilir adreslere geri gönderir" diyerek bunu engelliyor. Adres kayıtlı değilse `redirect_uri_mismatch` hatası alınır — bu hata aslında bir güvenlik özelliğinin çalıştığının göstergesi, bir "arıza" değil.

Burada iki farklı alanın karıştırılmaması önemli:
- **Authorized JavaScript origins:** sadece domain, yol (path) YOK. Örnek: `http://localhost:5183`
- **Authorized redirect URIs:** tam yol dahil. Örnek: `http://localhost:5183/signin-google`

### 7.4. dotnet user-secrets — neden appsettings.json değil?

Client ID ve Client Secret gibi hassas bilgileri `appsettings.json`'a yazarsak, bu dosya git'e commit edilir ve GitHub gibi bir yere push edilirse **herkes bu gizli bilgileri görebilir.** CLAUDE.md'de zaten bu projenin geçmişinde böyle bir hata olduğu (veritabanı şifresinin düz metin commit edilmesi) not edilmişti — aynı hatayı tekrarlamamak için bilinçli bir tercih yapıldı.

`dotnet user-secrets`, bu bilgileri **proje klasörünün dışında**, sadece senin bilgisayarındaki gizli bir dosyada saklar. `dotnet user-secrets set "Key" "Value"` ile eklenir. Kod içinde normal bir ayar gibi okunur (`builder.Configuration["Authentication:Google:ClientId"]` gibi), ama bu değer **hiçbir zaman git repository'sine girmez.**

**Neden bu önemli senin için:** Ekran görüntüsü paylaşırken veya kodu GitHub'a push ederken, gizli bilgilerin yanlışlıkla sızma riski böylece ortadan kalkıyor.

### 7.5. "Testing" modundaki Google uygulaması ne demek?

Google OAuth uygulamaları önce "Testing" (test) aşamasında başlar. Bu aşamada **sadece Google Console'da "Test users" listesine eklediğin hesaplar** giriş yapabilir — rastgele bir Google hesabı deneyemez.

**Neden bu kısıtlama var?** Google, henüz incelenmemiş/onaylanmamış bir uygulamanın gerçek kullanıcıların verilerine erişmesini istemiyor. Uygulama "Published/Verified" olmadan herkese açık kullanılamaz. Senin projen bir staj projesi olduğu için bu bir sorun değil — zaten sadece kendi hesabınla test ediyorsun.

### 7.6. Neden `SifreHash` alanı nullable yapıldı, `GoogleId` eklendi?

Google ile giriş yapan bir kullanıcının bizim sistemimizde ayrı bir şifresi yok — kimlik doğrulamayı Google yapıyor, biz hiç şifre almıyoruz. Ama `Kullanici` tablosundaki `SifreHash` alanı başta zorunlu (not null) olarak tasarlanmıştı (çünkü ilk özellik kullanıcı adı/şifre idi). Google kullanıcıları için bu alanı doldurmamız mümkün değil, o yüzden **nullable** (boş olabilir) yapıldı.

Kullanıcıyı ayırt etmek için ise `GoogleId` eklendi — bu, Google'ın her hesaba verdiği benzersiz `sub` (subject) değeri. Kullanıcı adı yerine bunu kullanıyoruz çünkü isim/e-posta değişebilir ama `sub` değişmez, yani güvenilir bir "birincil anahtar" adayı.

### 7.7. "Find-or-create" mantığı neden bu şekilde kuruldu?

Google ile giriş yapan bir kullanıcı sisteme geldiğinde iki ihtimal var: ya daha önce bu hesapla giriş yapmış (kaydı var) ya da ilk kez giriyor (kaydı yok). Kod, `GoogleId`'ye göre veritabanında arama yapıyor:
- Bulursa → doğrudan o kullanıcıyla giriş yaptırıyor.
- Bulamazsa → otomatik olarak (şifresiz) yeni bir kayıt oluşturuyor, sonra giriş yaptırıyor.

**Neden bu otomatik olmalı, neden ayrı bir "kayıt ol" adımı yok?** Çünkü Google zaten kullanıcının kimliğini doğruladı — ayrıca bir "kayıt formu" doldurtmak gereksiz bir sürtünme (friction) yaratırdı. Bu, çoğu modern uygulamanın ("Google ile devam et" butonları) kullandığı standart bir desen.

---

## 8. GÜN — Arkaplan Servisi (BackgroundService) ve İki Kritik Hata

### 8.1. BackgroundService nedir, neden gerekli?

TCMB'nin döviz kuru verileri, günlük olarak yayınlanıyor ve bizim uygulamamız bunları PostgreSQL'e "cache"liyor (yani her seferinde TCMB'ye gitmek yerine, bir kere çekip veritabanında saklıyor — hem hız hem de TCMB sunucusuna gereksiz yük bindirmemek için).

Ama ya uygulama birkaç gün kapalı kalırsa? Ya da bir gün için veri çekme işlemi başarısız olursa? O zaman veritabanında **boşluklar** oluşur.

`BackgroundService` (`IHostedService`), uygulama başladığında otomatik olarak çalışan, ana kullanıcı isteklerinden bağımsız bir arkaplan görevi tanımlamamızı sağlıyor. Bizim yazdığımız `DovizKuruTamamlamaServisi`, uygulama her açıldığında son 30 günü tarıyor, hafta sonlarını/tatilleri atlıyor (çünkü TCMB o günler veri yayınlamıyor), ve eksik olan günleri otomatik olarak dolduruyor.

**Neden mevcut `ITcmbKurServisi.KurlariGetirAsync` metodunu tekrar yazmadık, onu yeniden kullandık?** Kod tekrarı (duplication), bir hatayı iki farklı yerde ayrı ayrı düzeltmek zorunda kalman anlamına gelir — biri unutulursa tutarsızlık oluşur. Var olan, test edilmiş bir metodu çağırmak hem daha güvenli hem daha az bakım gerektiren bir yaklaşım.

### 8.2. Birinci kritik hata: DateTime.Kind uyuşmazlığı

PostgreSQL'de `timestamptz` (time zone'lu zaman damgası) tipi kullanılıyor. Npgsql (PostgreSQL için .NET sürücüsü), bir `DateTime` değerinin **UTC olduğunu kesin olarak bilmek istiyor** — yani `DateTime.Kind` özelliğinin `Utc` olması gerekiyor. Eğer `Kind` belirsizse (`Unspecified`) veya yerel saat (`Local`) ise, Npgsql bunu reddediyor veya (daha kötüsü) yanlış yorumluyor.

Bu hata, BackgroundService test edilirken ortaya çıktı — çünkü ilk kez veri, kullanıcı arayüzünden değil otomatik bir arkaplan sürecinden ekleniyordu, ve o path'te `DateTime.Kind` doğru ayarlanmamıştı. Çözüm: `KurlariGetirAsync` metodunun başında değeri UTC'ye normalize etmek.

**Neden bu önemliydi:** Bu hata çözülmeseydi, arkaplan servisi hiç çalışmayacaktı — yani "eksik günleri otomatik doldur" özelliği baştan işlevsiz kalacaktı.

### 8.3. İkinci kritik hata: Ondalık sayı ayrıştırmada kültür (culture) hatası — en ciddi bug

Bu, staj boyunca bulunan en ciddi hataydı, o yüzden üzerinde biraz daha durmakta fayda var.

**Sorunun kökeni:** TCMB'nin XML verisinde sayılar `47.7537` gibi, **nokta ondalık ayracı** kullanılarak yazılıyor (İngilizce/uluslararası format). Kod, bu string'i `decimal.TryParse(deger, out sonuc)` ile sayıya çeviriyordu — ama **kültür (culture) belirtmeden.**

.NET'te sayı ayrıştırma, çalıştığın sunucunun/işletim sisteminin **kültür ayarına** göre değişir. Türkçe kültürde:
- Nokta (`.`) = **binlik ayracı** (örn. `1.000` = bin)
- Virgül (`,`) = ondalık ayracı (örn. `47,75` = kırk yedi virgül yetmiş beş)

Yani sunucu Türkçe kültürde çalışıyorsa, `"47.7537"` string'i `decimal.TryParse` tarafından **"47.7537 değil, 477537" gibi bir binlik sayı olarak** yorumlanabiliyordu — yaklaşık **10.000 kat büyük** bir hata! Bu, döviz kurlarının veritabanında tamamen yanlış (devasa) değerlerle saklanması anlamına geliyordu.

**Neden bu kadar tehlikeliydi:** Bu bug, uygulamanın **ana amacını** (doğru döviz kuru göstermek) doğrudan geçersiz kılıyordu. Üstelik sessizce oluyordu — uygulama çökmüyordu, sadece yanlış sonuç üretiyordu. Bu tip "sessiz ama yanlış" hatalar, çökme hatalarından daha tehlikelidir çünkü fark edilmesi zaman alır.

**Çözüm:** `CultureInfo.InvariantCulture` ve `NumberStyles.Number` parametreleri eklendi:
```csharp
decimal.TryParse(deger, NumberStyles.Number, CultureInfo.InvariantCulture, out sonuc)
```
`InvariantCulture`, hiçbir yerel ayara bağlı olmayan, sabit bir format kullanır (nokta = her zaman ondalık ayracı). Bu, TCMB verisinin formatıyla birebir uyuşuyor.

**Neden bu üç yerde (Unit, ForexBuying, ForexSelling) ayrı ayrı düzeltildi?** Çünkü hepsi ayrı `TryParse` çağrılarıydı, aynı hataya ayrı ayrı düşme riski taşıyorlardı.

Düzeltmeden sonra, veritabanındaki eski (bozuk) kayıtlar silindi ve arkaplan servisi yeniden çalıştırılarak doğru değerlerle dolduruldu.

**Bu senin için önemli bir ders:** Sayı/tarih ayrıştırma yaparken kültür/locale'i asla varsayılan bırakma — özellikle uygulamanın farklı sunucularda, farklı işletim sistemi dil ayarlarıyla çalışacağı durumlarda. `InvariantCulture`, dış kaynaklardan (API, dosya, XML) gelen, sabit formatlı verileri ayrıştırırken güvenli varsayılan bir tercih.

---

## 8-9. GÜN — xUnit ile Unit Test

### 9.1. Unit test nedir, neden yazıyoruz?

Unit test, kodun **küçük, izole bir parçasının** (genelde tek bir fonksiyon/metod) beklenen şekilde çalıştığını otomatik olarak doğrulayan kısa bir kod parçası. "Otomatik" kelimesi kritik: sen elle her seferinde "acaba doğru çalışıyor mu" diye kontrol etmek yerine, testi çalıştırıyorsun ve o sana anında "evet/hayır" cevabı veriyor.

**Neden önemli — az önceki decimal-culture hatasını hatırla:** Eğer `XmlAyristir` metodunun bir unit testi olsaydı, bu hata muhtemelen kod yazılırken hemen yakalanırdı, üretim ortamında haftalarca yanlış veri birikmeden. Unit testler, hataları **en ucuz olduğu noktada** (geliştirme sırasında) yakalamayı amaçlıyor — bir hata üretime çıktıktan sonra düzeltmek çok daha maliyetli.

### 9.2. Neden kod "test edilebilir" hale getirmek için yeniden düzenlendi (refactor)?

`XmlAyristir` mantığı başta `TcmbKurServisi` içinde, veritabanı bağlantısına ve ağ isteğine bağımlı bir metodun içine gömülüydü. Böyle bir metodu test etmek zor çünkü test sırasında gerçek bir veritabanına veya gerçek bir ağ isteğine ihtiyaç duyardın — bu hem yavaş hem kırılgan (internet kesilirse test de başarısız olur) hem de "unit" (izole birim) test tanımına aykırı.

Çözüm: saf mantığı (sadece XML string alıp, sonuç döndüren kısmı) ayrı, **static** bir metoda çıkarmak (`internal static XmlAyristir(...)`). Böylece test, hiçbir veritabanı/ağ bağlantısı olmadan, doğrudan bir XML string verip çıktısını kontrol edebiliyor.

Benzer şekilde, `HomeController` içindeki çapraz kur hesaplama mantığı da `CaprazKurHesaplayici` adında ayrı bir static sınıfa çıkarıldı.

**Neden `internal` (public değil) ama yine de test edilebilir?** `internal` erişim belirleyicisi, bu metodun normalde sadece kendi projesi içinden erişilebilir olduğu anlamına gelir — dışarıya (public API gibi) açık olmasını istemiyoruz çünkü bu bir iç uygulama detayı. Ama test projesinin buna erişebilmesi için `InternalsVisibleTo` özelliği kullanıldı — bu, "şu belirli test projesine özel izin ver" diyen bir mekanizma. Yani kapsülleme (encapsulation) bozulmadan test edilebilirlik sağlandı.

### 9.3. Yazılan testler neyi kontrol ediyor?

`TcmbKurServisiXmlAyristirTests.cs` içinde 6 test var, her biri farklı bir senaryoyu kontrol ediyor:
- Kültür/decimal regresyon testi (az önce bahsedilen bug'ın bir daha geri gelmemesini garanti eden test)
- Birden fazla para birimi olan XML
- `Unit=0` durumunda fallback davranışı
- Boş `CurrencyCode` alanının atlanması
- Büyük/küçük harf duyarsız (case-insensitive) dictionary kullanımı
- Eksik alan durumu

`CaprazKurHesaplayiciTests.cs` içinde 6 test: TRY→TRY, USD→TRY, USD→EUR çapraz formülü, bilinmeyen kod, `Birim≠1` durumunda bölme işlemi, `TlKarsiligiBul`.

**Neden bu kadar çok farklı senaryo (sadece "normal" durum değil)?** Çünkü gerçek hatalar genelde "normal" durumda değil, **kenar durumlarda (edge cases)** ortaya çıkar — boş veri, sıfır değer, beklenmeyen format gibi. İyi bir test seti, sadece "doğru çalıştığını" değil "beklenmeyen durumlarda da makul davrandığını" da kontrol eder.

### 9.4. "Regresyon testinin geçerliliğini kanıtlama" tekniği neydi, neden yaptık?

Decimal-culture bug'ı için yazılan test, önce şöyle doğrulandı: **düzeltmeyi geçici olarak geri al → testin gerçekten BAŞARISIZ olduğunu gör → düzeltmeyi tekrar uygula → testin şimdi BAŞARILI olduğunu gör.**

**Neden bu adım gerekliydi?** Bir test yazıp "geçti" demek, aslında bir şey **doğru** kanıtlamaz — belki test hiçbir şeyi gerçekten kontrol etmiyordur (yanlış yazılmış, her zaman geçen bir test olabilir). Testin "kırılması gereken durumda gerçekten kırıldığını" görmek, o testin **gerçekten işe yaradığının** kanıtı. Bu, deneysel bilimdeki kontrol grubu mantığına benziyor: bir ilacın işe yaradığını göstermek için, ilacı almayan grupta hastalığın devam ettiğini de görmen gerekir.

### 9.5. Frontend unit test, backend'den neden farklı?

Backend'de test ettiğimiz şeyler saf fonksiyonlardı — girdi ver, çıktı kontrol et, ortada bir "ekran" yok. Frontend'de ise kod genelde DOM (HTML sayfası) ile etkileşiyor — bir butona tıklanınca ne oluyor, bir input'a değer yazılınca sayfa nasıl güncelleniyor gibi.

Bu yüzden frontend unit testleri (Jest/Vitest gibi araçlarla), gerçek bir tarayıcı olmadan **DOM'u simüle eden** bir ortamda çalışır, ve backend'e giden ağ isteklerini de gerçekten göndermek yerine **mock'lar** (sahte, kontrol edilen cevaplar döndüren taklit fonksiyonlar). Bu, testin hızlı ve backend'in gerçekten çalışır olup olmamasından bağımsız olmasını sağlar.

**Neden bu ayrım önemli senin için:** Frontend ve backend testleri farklı problemi çözüyor — biri "mantık doğru mu" (backend), diğeri "arayüz kullanıcı etkileşimine doğru tepki veriyor mu" (frontend). İkisi birbirinin yerine geçmez.

---

## 9. GÜN — TypeScript'e Geçiş

### 10.1. TypeScript nedir, JavaScript'ten farkı ne?

JavaScript, dinamik tipli bir dil — yani bir değişkenin ne tür bir veri tuttuğunu (sayı mı, string mi, obje mi) çalışma zamanına kadar garanti edemezsin. Bu, küçük projelerde sorun değil ama proje büyüdükçe "bu fonksiyona yanlış tipte bir şey gönderdim" gibi hatalar, ancak **çalışma zamanında** (kullanıcı sayfayı açtığında) ortaya çıkar.

TypeScript, JavaScript'in üzerine **tip sistemi** ekleyen bir dil. `kur-hesapla.ts` dosyasında, backend'den dönen veri şeklini tanımlayan bir `KurHesaplaSonucu` interface'i yazıldı, DOM elemanları da (`HTMLInputElement` gibi) tipli şekilde ele alındı.

**Neden bu değerli:** Eğer backend'in döndürdüğü JSON'un şekli değişirse (bir alan adı değişir, kaldırılır), TypeScript bunu **kod yazarken, derleme (compile) anında** sana söyler — sayfa üretime çıkıp kullanıcı hata görene kadar beklemene gerek kalmaz.

### 10.2. Neden tarayıcı `.ts` dosyasını doğrudan çalıştıramıyor, bir derleme adımı gerekiyor?

Tarayıcılar sadece JavaScript anlar, TypeScript anlamaz. TypeScript kodu, `tsc` (TypeScript compiler) ile normal `.js` dosyasına **dönüştürülüyor (transpile)** — tip bilgileri bu süreçte kontrol edilip sonra kod üretiminden çıkarılıyor (çünkü tarayıcı zaten tipleri anlamıyor, sadece bizim geliştirme sırasında bu kontrolden faydalanmamız gerekiyordu).

`tsconfig.json` dosyası bu derlemenin kurallarını tanımlıyor (strict mode = en sıkı tip kontrolü, ES2020+DOM hedef ortamı). `package.json`'daki `npm run build` komutu, `tsc`'yi çalıştırıp `.ts` dosyalarını `.js`'e çeviriyor.

**Neden "bağımsız script" yaklaşımı seçildi, MSBuild'e (yani `dotnet build`'e) entegre edilmedi?** Bu senin tercihindi — iki yaklaşım da geçerli, ama ayrı tutmak, .NET build sürecini JavaScript araç zincirine (toolchain) bağımlı kılmıyor. Yani backend geliştiren biri npm/node kurulu olmasa bile projeyi build edebiliyor; frontend'de değişiklik yapan biri ayrıca `npm run build` çalıştırıyor.

---

## 9-10. GÜN — Playwright ile E2E (Uçtan Uca) Test

### 11.1. E2E test, unit testten nasıl farklı, neden ikisine de ihtiyaç var?

Unit test, izole bir fonksiyonun doğru çalıştığını kontrol eder — ama fonksiyonlar doğru olsa bile, **hepsi bir araya geldiğinde** (sayfa yüklensin, kullanıcı bir seçim yapsın, butona tıklasın, sonuç ekranda görünsün) sistem gerçekten çalışıyor mu? Bunu unit testler garanti edemez.

E2E (end-to-end / uçtan uca) test, **gerçek bir tarayıcıyı** (Playwright burada Chromium kullanıyor) otomatik olarak açıp, gerçek bir kullanıcı gibi davranıyor: sayfayı açıyor, dropdown'dan USD/EUR seçiyor, `100` giriyor, "Hesapla" butonuna tıklıyor, ve ekranda beklenen sonucun (her iki para birimi kodunu içeren bir başarı mesajının) göründüğünü kontrol ediyor.

**Neden ikisi birlikte gerekli:** Unit testler hızlı ve odaklı ama "parçalar birlikte çalışıyor mu"yu göremez. E2E testler bunu görür ama daha yavaştır ve bir tarayıcı başlatmayı gerektirir. İyi bir test stratejisi genelde çok sayıda hızlı unit test + az sayıda kapsamlı E2E test şeklindedir (buna bazen "test piramidi" denir).

### 11.2. `webServer` konfigürasyonu ne işe yarıyor?

`playwright.config.ts` içinde, test çalıştırılmadan önce `dotnet run`'ı **otomatik başlatıp**, testler bitince **otomatik kapatan** bir ayar var (`webServer`).

**Neden bu önemli:** Sen her seferinde elle "önce uygulamayı başlat, sonra testi çalıştır, sonra uygulamayı kapat" yapmak zorunda kalmıyorsun — bu hem unutulabilecek bir adım hem de CI/CD (otomatik test/deploy) süreçlerinde kritik: bir sunucu, hiçbir insan müdahalesi olmadan testleri çalıştırabilmeli.

Test sonucu: `1 passed (5.9s)` — yani gerçek bir tarayıcıda, gerçek bir HTTP sunucusuna karşı, uçtan uca senaryo başarıyla doğrulandı.

---

## 10. GÜN — REST API ve API Key Authentication

### 12.1. Neden ayrı bir REST API endpoint'i eklendi, mevcut web sayfaları yetmiyor muydu?

Mevcut uygulama, tarayıcıda HTML sayfaları gösteren bir MVC uygulaması. Ama bazen "başka bir program/sistem, bizim döviz kuru verimizi programatik olarak (HTML değil, JSON gibi yapılandırılmış veri olarak) kullanmak istesin" senaryosu olur — örneğin bir mobil uygulama, bir başka servis, ya da bir otomasyon scripti.

`GET /api/kurlar/{tarih}` endpoint'i tam olarak bunu sağlıyor: belirli bir tarihin kur verilerini JSON formatında döndürüyor, insan gözüyle okunacak bir sayfa değil, program tarafından işlenecek veri.

### 12.2. Neden cookie tabanlı auth değil, ayrı bir "API Key" authentication şeması?

Cookie tabanlı authentication, bir **tarayıcı oturumu** varsayımına dayanıyor — kullanıcı giriş yapıyor, tarayıcı cookie'yi tutuyor. Ama bir API'yi çağıran şey bir program/script ise, "tarayıcı oturumu" diye bir şey yok; ayrıca her programatik istekte bir insanın giriş yapmasını beklemek de mantıksız.

Bunun yerine, **API Key** (uzun, rastgele bir gizli anahtar) tabanlı bir kimlik doğrulama şeması eklendi. İstek gönderen taraf, `X-Api-Key` header'ına bu anahtarı koyuyor; sunucu bunu kontrol ediyor.

Bu, `AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", ...)` ile ayrı bir authentication şeması olarak eklendi — **varsayılan (default) şema değiştirilmeden**, yani normal web sayfaları hâlâ cookie ile, API endpoint'i ise API Key ile korunuyor. İki farklı "giriş kapısı", iki farklı anahtar türü.

### 12.3. `CryptographicOperations.FixedTimeEquals` neden kullanıldı, neden basitçe `==` ile karşılaştırmadık?

Bu, ince ama gerçek bir güvenlik detayı. Normal bir string karşılaştırması (`==` veya `.Equals()`), genelde **ilk farklı karakterde durur** — yani "doğru" anahtarın ilk 3 karakteri tutuyorsa, karşılaştırma az da olsa daha uzun sürer; hiç tutmuyorsa daha kısa sürer.

Bu süre farkı, teoride bir saldırganın **zamanlama saldırısı (timing attack)** ile karakter karakter doğru anahtarı tahmin etmesine izin verebilir — her denemede hangi karakterin doğru olduğunu, o denemenin ne kadar sürdüğüne bakarak anlayabilir.

`FixedTimeEquals`, karşılaştırmayı **her zaman aynı sürede** yapacak şekilde tasarlanmış — string ne kadar eşleşirse eşleşsin, süre değişmiyor. Bu, zamanlama bilgisinden hiçbir şey sızdırmıyor.

**Neden bu detay önemliydi burada:** API Key, tek başına bir kimlik doğrulama yöntemi olduğu için (cookie/session gibi ek bir katman yok), onun karşılaştırma mantığının sağlam olması daha kritik.

### 12.4. Test edilen senaryolar neden bu şekilde seçildi?

- Header yok → 401 (Unauthorized) — kimlik bilgisi hiç verilmemiş.
- Yanlış key → 401 — kimlik bilgisi var ama geçersiz.
- Doğru key + bugünün tarihi → 200 — normal başarı senaryosu.
- Doğru key + gelecekteki bir tarih → 404 (Not Found) — mantıklı, çünkü henüz olmamış bir tarihin kuru olamaz.
- Doğru key + geçersiz format → 400 (Bad Request) — istek biçimsel olarak hatalı.

**Neden bu kadar çeşitli:** Yine, "mutlu yol" (happy path) dışındaki durumları test etmek — bir API'nin gerçekten sağlam olduğunu göstermenin yolu, sadece doğru kullanıldığında değil, **yanlış kullanıldığında da öngörülebilir ve doğru** davrandığını göstermekten geçiyor.

---

## Genel Bir Bakış: Bütün Bu Günler Birbirine Nasıl Bağlanıyor?

Eğer geriye dönüp bakarsan, bu beş günün ortak bir teması var: **"doğru çalışıyor gibi görünen" ile "gerçekten doğru çalışan" arasındaki farkı kapatmak.**

- 6. gün: kimlik doğrulama — kullanıcı gerçekten kim olduğunu kanıtlamadan sisteme giremiyor.
- 7. gün: kimlik doğrulamayı güvenilir bir üçüncü tarafa (Google) devretmenin güvenli yolu.
- 8. gün: veri her zaman eksiksiz ve **doğru** olsun diye otomatik tamamlama + iki sessiz ama ciddi hatanın bulunup düzeltilmesi.
- 8-9. gün: kodun doğru çalıştığını **iddia etmek** yerine, otomatik testlerle **kanıtlamak.**
- 9. gün: kodun tip hatalarını çalışma zamanına kadar beklemeden, yazarken yakalamak.
- 9-10. gün: parçaların ayrı ayrı doğru olmasının yetmediğini, hepsinin birlikte de doğru çalıştığını kanıtlamak.
- 10. gün: sistemi sadece insanlara değil, diğer programlara da güvenli şekilde açmak.

Bu yüzden bu iş sadece "özellik ekleme" değildi — her adımda, "bu doğru mu, güvenli mi, ileride biri (bir başka geliştirici ya da gelecekteki sen) bunu anlayabilecek mi" sorusuna cevap arandı.

---

## Şimdi Ne Yapmalısın?

1. Bu dokümanı kendi hızında oku, anlamadığın veya "peki neden tam olarak bu şekilde" dediğin yerleri not al.
2. Özellikle 8. gündeki decimal-culture hatasını (bölüm 8.3) iyi anla — bu, gerçek bir yazılım hatasının nasıl "sessizce" büyük hasar verebileceğinin çok iyi bir örneği ve mülakatlarda bile anlatabileceğin türden bir hikaye.
3. Sorularını yazılı ya da sohbet ederek bana getir, birlikte netleştirelim — hazır olduğunda 3. haftaya (Mobil Geliştirme / React Native) geçeriz.
