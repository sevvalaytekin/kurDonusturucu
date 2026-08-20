# 2. Hafta, 5. Gün — REST API ve API Key Kimlik Doğrulama

## Bugün Ne Yapıldı

- `GET /api/kurlar/{tarih}` REST endpoint'i eklendi (`KurlarController`) — mevcut `ITcmbKurServisi` yeniden kullanılarak, belirtilen tarihe ait döviz kurları JSON olarak döndürülüyor.
- Ayrı bir **"ApiKey" authentication scheme**'i geliştirildi: `X-Api-Key` header'ını okuyup, user-secrets'taki değerle `CryptographicOperations.FixedTimeEquals` kullanarak karşılaştırıyor.
- Bu yeni şema, varsayılan Cookie/Google girişini etkilemeyecek şekilde eklendi — `/`, `/Account/Login`, `/Home/KurHesapla` akışları bozulmadan çalışmaya devam ediyor.
- Beş senaryo test edildi: header yok → 401, yanlış key → 401, geçerli key + geçersiz format → 400, geçerli key + gelecek tarih → 404, geçerli key + geçerli tarih → 200 + doğru JSON.

## Kavramlar — Basitçe

### REST API neden eklendi
Mevcut uygulama HTML sayfası döndürüyordu (insan için). REST API, aynı veriyi JSON olarak (program için) sunuyor — örneğin bir mobil uygulamanın kullanabilmesi için.

### Neden cookie değil, ayrı bir API Key şeması
Cookie, bir tarayıcı oturumu varsayıyor. Bir program/script'in "oturum açması" mantıklı değil — bunun yerine sabit bir anahtar (API Key) ile kimlik doğrulanıyor. `AddScheme` ile bu ayrı şema, varsayılan şemayı değiştirmeden eklendi.

### CryptographicOperations.FixedTimeEquals neden kullanıldı
Normal string karşılaştırması, ilk farklı karakterde durur — bu süre farkı, teoride bir saldırganın karakter karakter doğru anahtarı tahmin etmesine (zamanlama saldırısı) izin verebilir. `FixedTimeEquals`, karşılaştırmayı her zaman aynı sürede yaparak bu riski ortadan kaldırıyor.

### Neden bu kadar çok senaryo test edildi
Bir API'nin sağlam olduğunu göstermenin yolu, sadece doğru kullanıldığında değil, yanlış kullanıldığında da (eksik/yanlış anahtar, hatalı tarih) öngörülebilir davranmasından geçiyor.

## Görev Durumu

✅ "REST API ile Servis Ucu Geliştirilmesi" (1 gün) görevi tamamlandı — bu görev, 2. haftanın son günüydü.

**Not:** Bu API, 3. haftada (mobil geliştirme) `TcmbKurMobil` uygulamasının döviz kurlarını çekmek için kullanılıyor — yani bu günün emeği, 3. haftada gerçekten devreye girdi.
