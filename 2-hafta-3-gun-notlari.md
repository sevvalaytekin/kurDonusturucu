# 2. Hafta, 3. Gün — Arkaplan Servisi ve Unit Testler

## Bugün Ne Yapıldı

- `DovizKuruTamamlamaServisi` adlı bir `BackgroundService` yazıldı: uygulama başladığında son 30 günü tarıyor, hafta sonu/tatilleri atlayıp eksik kur kayıtlarını otomatik tamamlıyor.
- Bu servis test edilirken iki kritik hata bulundu ve düzeltildi:
  - **DateTime.Kind hatası:** PostgreSQL'in `timestamptz` kolonu UTC-kind istiyor; tarih `DateTime.SpecifyKind(..., DateTimeKind.Utc)` ile normalize edildi.
  - **Decimal kültür hatası (en kritik):** `decimal.TryParse`, kültür belirtmeden kullanıldığında, sunucunun Türkçe kültüründe nokta (`.`) binlik ayracı sayıldığı için TCMB'nin ondalık değerleri ~10.000 kat yanlış okunuyordu. `CultureInfo.InvariantCulture` + `NumberStyles.Number` ile düzeltildi, bozuk kayıtlar silinip yeniden dolduruldu.
- xUnit tabanlı bir test projesi (`TcmbKurDonusturucu.Tests`) oluşturuldu. Test edilebilirlik için `XmlAyristir` ve `CaprazKurHesaplayici` ayrı, saf (pure) metodlara çıkarıldı.
- Toplam 12 test yazıldı (6 XML ayrıştırma, 6 çapraz kur hesaplama) — kültür/decimal regresyon testi dahil.
- Regresyon testinin gerçekten işe yaradığı, düzeltme geçici olarak geri alınıp testin kırıldığı görülerek kanıtlandı.

## Kavramlar — Basitçe

### BackgroundService nedir
Uygulama başladığında otomatik çalışan, kullanıcı isteklerinden bağımsız bir arkaplan görevi.

### DateTime.Kind / UTC neden önemli
Npgsql (PostgreSQL sürücüsü), `timestamptz` kolonu için bir `DateTime`'ın **kesin olarak UTC** olduğunu bilmek istiyor; aksi halde reddediyor veya yanlış yorumluyor.

### Kültür (culture) hatası neden bu kadar ciddiydi
.NET'te sayı ayrıştırma, kültür ayarına göre değişiyor — Türkçe kültürde nokta binlik ayracı, virgül ondalık ayracı. TCMB'nin nokta-ondalık verisi, kültür belirtilmeden ayrıştırılınca yanlış (çok büyük) sayılara dönüşüyordu. Bu, uygulamanın **ana amacını** (doğru kur göstermek) geçersiz kılan, sessiz ama çok ciddi bir hataydı.

### Unit test ve test edilebilirlik için refactoring
Saf mantığı (veritabanı/ağ bağımlılığı olmayan kısmı) ayrı, `internal static` metodlara çıkarmak, testin veritabanı/ağ olmadan çalışmasını sağlıyor. `InternalsVisibleTo` ile test projesine özel erişim izni veriliyor.

### Regresyon testinin geçerliliğini kanıtlama tekniği
Bir testin gerçekten işe yaradığını kanıtlamanın yolu: düzeltmeyi geçici olarak geri al → test gerçekten başarısız oluyor mu gör → düzeltmeyi geri koy → test şimdi geçiyor mu gör.

## Görev Durumu

✅ "Arkaplan Veri Tamamlama Servisi Geliştirilmesi" (0.5 gün) ve "Unit Testlerin Geliştirilmesi" (0.5 gün) görevleri tamamlandı.
