# Staj Raporları Arşivi

Şevval Aytekin — TcmbKurDonusturucu Staj Projesi

---

## Genel Rapor — 1. Hafta

Bu hafta ASP.NET Core MVC kullanılarak geliştirilen döviz kuru uygulamasının frontend, backend ve veritabanı katmanlarının geliştirilmesine ve bu katmanların birbiriyle entegre edilmesine odaklanıldı. Çalışmalar kapsamında öncelikle kullanıcıların döviz dönüşümü gerçekleştirebileceği kullanıcı arayüzü geliştirildi. Bootstrap 5 kullanılarak responsive bir tasarım oluşturuldu, tarih seçimi, kaynak ve hedef para birimi seçimi ile miktar girişi için gerekli form elemanları hazırlandı. Kart yapısı ve Bootstrap grid sistemi kullanılarak farklı ekran boyutlarında kullanılabilir bir arayüz elde edildi.

Kullanıcı deneyimini geliştirmek amacıyla JavaScript tarafında çeşitli dinamik işlemler gerçekleştirildi. Tarih alanının varsayılan olarak güncel tarihi göstermesi ve ileri tarihlerin seçilememesi sağlandı. Form gönderme işlemi Fetch API kullanılarak asenkron hale getirildi, böylece kullanıcı işlem yaptığında sayfanın tamamen yenilenmesine gerek kalmadan backend'den sonuç alınması sağlandı. İşlem sırasında butonun devre dışı bırakılması ve yükleniyor göstergesinin kullanılmasıyla kullanıcıya işlemin devam ettiği konusunda geri bildirim verildi. Dönen sonuca göre başarı veya hata mesajlarının dinamik olarak ekranda gösterilmesi sağlandı.

Backend geliştirmeleri kapsamında ASP.NET Core MVC içerisinde Controller ve Service yapıları oluşturuldu. Kullanıcıdan gelen tarih, kaynak para birimi, hedef para birimi ve miktar bilgileri Controller tarafından alınarak gerekli hesaplama işlemleri gerçekleştirildi. Türkiye Cumhuriyet Merkez Bankası tarafından sunulan XML servisleri kullanılarak güncel ve geçmiş tarihli döviz kuru verilerinin alınması üzerine çalışıldı. Tarih bazlı işlemlerde TCMB'nin kur yayınlama zamanı dikkate alındı, hafta sonuna denk gelen tarihler için uygun kur tarihinin belirlenmesi sağlandı.

Alınan döviz kurları üzerinden farklı para birimleri arasında çapraz kur hesaplama işlemleri gerçekleştirildi. Türk Lirası referans alınarak kaynak ve hedef para birimleri arasındaki dönüşüm oranı hesaplandı ve kullanıcının girdiği miktar üzerinden toplam sonuç oluşturuldu. Backend tarafından hazırlanan sonuçlar JSON formatında frontend tarafına gönderilerek hesaplama sonucunun kullanıcıya dinamik şekilde gösterilmesi sağlandı.

Hafta içerisinde uygulamanın kalıcı veri saklama ihtiyacını karşılamak amacıyla PostgreSQL veritabanı entegrasyonu gerçekleştirildi. Entity Framework Core kullanılarak AppDbContext oluşturuldu ve DovizKuru modeli veritabanına bağlandı. PostgreSQL bağlantı ayarları uygulamaya eklendi ve Entity Framework Core migration mekanizması kullanılarak InitialCreate migration'ı oluşturuldu. Migration'ın uygulanması sonucunda döviz kuru bilgilerinin saklanacağı DovizKurlari tablosu PostgreSQL üzerinde oluşturuldu.

Veritabanı yapısında döviz kodu, döviz adı, tarih, birim, alış ve satış kuru gibi bilgilerin saklanması sağlandı. Aynı tarih ve döviz kuruna ait verilerin tekrar tekrar kaydedilmesini önlemek amacıyla gerekli veritabanı yapısının oluşturulması üzerine çalışıldı. Böylece uygulamanın her kur sorgusunda doğrudan TCMB servisine başvurmak yerine, daha önce veritabanına kaydedilmiş verilerden yararlanabilecek bir altyapı hazırlandı.

Son aşamada uygulamanın frontend, backend ve veritabanı katmanlarının birlikte çalışması test edildi. Geliştirme sırasında karşılaşılan dotnet-ef, model tanımlaması, migration, veritabanı bağlantısı ve uygulamanın çalıştırılmasıyla ilgili çeşitli hatalar incelenerek giderildi. Özellikle Entity Framework Core'un DovizKuru modeli için primary key gerektirmesi üzerine modele Id alanı eklendi ve migration işlemi başarıyla tamamlandı. PostgreSQL veritabanı ile uygulama arasındaki bağlantı doğrulandı ve dotnet ef database update komutu ile veritabanı şeması başarıyla oluşturuldu.

Hafta sonunda, döviz dönüşüm uygulamasının kullanıcı arayüzü, TCMB veri servisi, kur hesaplama mekanizması ve PostgreSQL veritabanı temel seviyede entegre edildi. Uygulamanın daha sonraki geliştirmelerde kullanılabilecek çalışan bir altyapıya ulaşması sağlandı.

---

## Genel Rapor — 2. Hafta

Bu hafta ASP.NET Core MVC tabanlı döviz kuru uygulamasının bir yapay zeka ajanı (Claude Code) ile geliştirilmesi süreci üzerinde çalışıldı. Öncelikle Claude Code'un VS Code eklentisi kurularak proje ortamına entegre edildi ve aracın farklı çalışma modları (plan sunma, her adımda onay isteme, doğrudan uygulama) tanındı. Proje köküne bir CLAUDE.md dosyası oluşturularak uygulamanın teknoloji yığını, klasör yapısı, mimarisi ve bilinen zayıf noktaları belgelenmiş, böylece yapay zeka ajanının her oturumda proje bağlamını otomatik olarak edinmesi sağlandı.

Kullanıcı kimlik doğrulama altyapısı kapsamında kullanıcı adı ve şifre ile giriş yapılabilen bir oturum açma özelliği geliştirildi. Şifreler PasswordHasher sınıfı üzerinden PBKDF2 tabanlı bir algoritma ile hash'lenerek veritabanında saklandı, düz metin şifre saklama uygulamasından kaçınıldı. Giriş sonrasında çerez tabanlı (cookie) bir oturum yönetimi kuruldu ve arayüzde kullanıcının oturum durumuna göre dinamik içerik gösterimi sağlandı.

Kimlik doğrulama tarafı, Google OAuth 2.0 entegrasyonu ile genişletildi. Google Cloud Console üzerinden bir OAuth istemcisi oluşturularak Client ID ve Client Secret bilgileri elde edildi, bu bilgiler güvenlik amacıyla appsettings.json yerine dotnet user-secrets aracılığıyla saklandı. Kullanıcının Google hesabıyla giriş yapabilmesi için gerekli yönlendirme (redirect) akışı kuruldu; giriş yapan kullanıcı, Google'ın sağladığı benzersiz kimlik (GoogleId) üzerinden veritabanında aranarak mevcutsa doğrudan oturum açtırıldı, mevcut değilse otomatik olarak yeni bir kayıt oluşturuldu.

Uygulamanın veri bütünlüğünü korumak amacıyla, uygulama başlangıcında çalışan bir arkaplan servisi (BackgroundService) geliştirildi. Bu servis, son 30 güne ait eksik döviz kuru kayıtlarını tespit ederek hafta sonu ve tatil günlerini dışarıda bırakacak şekilde otomatik olarak tamamlanmasını sağladı. Bu çalışma sırasında iki önemli hata tespit edilip giderildi: PostgreSQL'in zaman damgası alanları için UTC formatı gerektirmesinden kaynaklanan bir tarih uyuşmazlığı düzeltildi; ayrıca TCMB'den gelen ondalıklı sayı verilerinin sunucu kültür ayarına bağlı olarak yanlış ayrıştırılmasına yol açan kritik bir hata, kültürden bağımsız (invariant culture) ayrıştırma yöntemine geçilerek çözüldü ve etkilenen veriler yeniden oluşturuldu.

Backend tarafının doğruluğunu otomatik olarak doğrulamak amacıyla xUnit tabanlı bir test projesi oluşturuldu. TCMB XML ayrıştırma mantığı ve çapraz kur hesaplama mantığı test edilebilir, bağımsız metodlara ayrıştırılarak toplam on iki birim testi yazıldı; bu testler arasında kültür/ondalık ayrıştırma regresyonunu doğrulayan bir test de yer aldı. Ayrıca frontend ve backend unit testlerinin kavramsal farkları (DOM simülasyonu ve ağ isteklerinin taklit edilmesi ile saf fonksiyon testleri arasındaki ayrım) incelendi.

Frontend geliştirmeleri kapsamında JavaScript ile yazılmış kur hesaplama betiği TypeScript'e taşınarak tip güvenliği sağlandı, backend'den dönen veri yapısı arayüzler (interface) ile tanımlandı. Ayrıca Playwright kullanılarak gerçek tarayıcı ortamında çalışan bir uçtan uca (E2E) test ortamı kuruldu; test senaryosunda kullanıcının para birimi seçip miktar girerek hesaplama yaptığı akış otomatikleştirildi ve başarıyla doğrulandı.

Hafta sonunda, döviz kurlarının dış sistemlere programatik olarak sunulabilmesi amacıyla bir REST API endpoint'i geliştirildi. Bu endpoint, X-Api-Key başlığı ile korunacak şekilde özel bir kimlik doğrulama şeması üzerinden güvenli hale getirildi ve anahtar karşılaştırmasında zamanlama saldırılarına karşı dayanıklı bir yöntem kullanıldı. Geliştirilen uç nokta, geçersiz anahtar, eksik başlık, gelecek tarih ve hatalı format gibi farklı senaryolar test edilerek doğrulandı.

Hafta genelinde, uygulamanın kimlik doğrulama, veri güvenilirliği, otomatik test kapsamı ve dış sistemlere entegrasyon kabiliyeti açısından önemli ölçüde olgunlaştırıldığı görüldü. Bu hafta yapılan çalışmalar, uygulamanın hem güvenlik hem de sürdürülebilirlik açısından bir sonraki geliştirme aşamalarına (mobil geliştirme) daha sağlam bir temelle geçmesini sağladı.

---

## 10. Gün: REST API ile Servis Ucu Geliştirilmesi

* **API Key Kimlik Doğrulama Mekanizması:** `X-Api-Key` header'ını okuyan, user-secrets'taki değerle zamanlama saldırılarına karşı güvenli (`CryptographicOperations.FixedTimeEquals`) şekilde karşılaştıran ayrı bir "ApiKey" authentication scheme'i geliştirildi.
* **REST Endpoint'in Eklenmesi:** `GET /api/kurlar/{tarih}` endpoint'i eklendi; mevcut `ITcmbKurServisi` yeniden kullanılarak belirtilen tarihe ait tüm döviz kurları JSON formatında dışarıya sunuldu.
* **Mevcut Girişlerle Uyumluluk:** Yeni kimlik doğrulama şeması, siteye varsayılan Cookie/Google girişini etkilemeyecek şekilde eklendi; `/`, `/Account/Login` ve `/Home/KurHesapla` akışlarının bozulmadığı doğrulandı.
* **Güvenlik ve Hata Senaryolarının Test Edilmesi:** Header'sız istek (401), yanlış API key (401), geçerli key + geçersiz tarih formatı (400), geçerli key + gelecek tarih (404) ve geçerli key + geçerli tarih (200 + doğru JSON) senaryolarının tümü beklenen sonuçla doğrulandı.

---

## 11. Gün (3. Hafta, 1. Gün): React Native İle Mobil Geliştirme Temelleri

* **Geliştirme Ortamının Hazırlanması:** Mobil geliştirme için gerekli JavaScript çalışma ortamı (Node.js v22.22.2, npm 10.9.7) doğrulandı; React Native'in dosya değişikliklerini izlemesi için kullanılan Watchman aracı, Homebrew paket yöneticisi üzerinden kurularak sisteme entegre edildi.
* **Android Geliştirme Altyapısının Yapılandırılması:** Android Studio üzerinden Device Manager aracılığıyla bir Android sanal cihazı (Pixel 7, Android 16.0, API 36) yapılandırılıp çalıştırıldı; emulatör ortamının sorunsuz çalıştığı doğrulandı.
* **İlk Mobil Projenin Oluşturulması:** Expo'nun `create-expo-app` aracı kullanılarak React Native tabanlı ilk mobil uygulama projesi (`TcmbKurMobil`) oluşturuldu, gerekli bağımlılıklar `npm install` ile kuruldu.
* **Uygulamanın Derlenip Emulatörde Çalıştırılması:** Proje `npm run android` komutuyla derlenerek Pixel 7 emulatörüne yüklendi; uygulamanın emulatörde başarıyla açılıp çalıştığı (Expo varsayılan karşılama ekranı) doğrulandı.

---

## 12. Gün (3. Hafta, 2. Gün): Mobil Uygulama - Ekran 1 (Döviz Kurları Tablosu)

* **REST API'nin Mobil Tarafta Yeniden Kullanılması:** 10. günde geliştirilen `GET /api/kurlar/{tarih}` endpoint'i, `TcmbKurMobil` uygulamasından `X-Api-Key` header'ı ile çağrılarak bugünün döviz kurlarının çekilmesi sağlandı; böylece backend ve mobil tarafı arasında ilk gerçek entegrasyon kuruldu.
* **Emulatör Ağ Yapılandırması:** Android emulatörünün host makineye (`localhost` üzerinden değil) `10.0.2.2` adresi üzerinden erişebildiği tespit edilip backend base URL'i buna göre yapılandırıldı; ayrıca gerçek çalışan port (5183) ile uyumsuz bir varsayımın (5000) plan aşamasında fark edilip düzeltilmesi sağlandı.
* **API Anahtarının Mobilde Saklanması:** Sunucu tarafındaki `user-secrets` yaklaşımının mobilde doğrudan bir karşılığı olmadığından, API anahtarı `.gitignore`'a eklenmiş bir `.env` dosyasında (`EXPO_PUBLIC_API_KEY`) tutuldu; anahtarın koda gömülüp versiyon kontrolüne dahil edilmesi engellendi.
* **Veri Normalizasyon Katmanının Geliştirilmesi:** Backend'in gerçek JSON cevabı incelenerek, dönen verinin döviz koduna göre anahtarlanmış bir obje olduğu ve alan isimlerinin (`kod`, `isim`, `forexAlis`, `forexSatis`) baştan tahmin edilenlerden farklı olduğu tespit edildi; normalizasyon kodu bu gerçek alan isimlerine göre güncellendi.
* **Ekranın Tamamlanması ve Doğrulanması:** Kod, İsim, Alış, Satış sütunlarından oluşan bir tablo geliştirildi; sayısal değerler `toLocaleString('tr-TR')` ile Türkçe formatta (virgüllü) gösterildi. Emulatörde yapılan testte tüm sütunların doğru verilerle dolduğu, isteğin doğru adrese ve kimlik doğrulamayla ulaştığı doğrulandı.

---

## 13. Gün (3. Hafta, 3. Gün): Mobil Uygulama - Ekran 2 (Döviz Çevirici)

* **Mimari Karar — Backend'de Yeniden Kullanım:** Döviz çevirme hesaplaması için mobil tarafta yeni bir mantık yazmak yerine, 2. haftada test edilmiş (`CaprazKurHesaplayici`, regresyon testi dahil) mevcut backend mantığını kullanan yeni bir REST endpoint (`GET /api/donustur`) eklenmesine karar verildi; böylece iş mantığı tek bir yerde (single source of truth) tutuldu.
* **Backend Endpoint Geliştirme ve Doğrulama:** `DonusturController` eklendi — mevcut `ApiKey` kimlik doğrulama şemasıyla korunuyor, tarih ayrıştırması kültürden bağımsız (`InvariantCulture`) yapılıyor. Endpoint için 6 yeni birim testi yazıldı; `dotnet build` ve `dotnet test` ile toplam 18 testin tamamının başarılı olduğu doğrulandı.
* **Frontend Geliştirme:** `api.ts`'e `convert()` fonksiyonu eklendi; kaynak/hedef para birimi seçimi, miktar girişi ve sonuç gösterimi içeren yeni bir "Çevirici" ekranı (`convert.tsx`) oluşturuldu; hem native hem web sekme bileşenlerine yeni bir tab eklendi. TypeScript kontrolü hatasız tamamlandı.
* **Geliştirme Ortamı Sorunlarının Giderilmesi:** Test aşamasında Android emulatörünün sanallaştırma katmanında oluşan bir tıkanıklık (cold boot ve veri sıfırlama ile çözülemeyen sonsuz açılış döngüsü) tespit edildi; sorun, geliştirme makinesinin yeniden başlatılmasıyla giderildi.
* **Uçtan Uca Doğrulama:** Emulatörde her iki ekran da başarıyla test edildi — döviz kurları tablosu güncel verilerle görüntülendi, çevirici ekranında girilen miktar günün kuruna göre doğru şekilde hesaplanıp gösterildi.
