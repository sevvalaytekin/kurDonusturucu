# 1. Hafta, 5. Gün — Veritabanı Bağlantısının Tamamlanması

**Not:** Aynı dürüstlük payı geçerli — gün numarası tahmini.

## Bugün Ne Yapıldı (tahmini)

- `dotnet ef database update` komutu ile migration veritabanına uygulandı, `DovizKurlari` tablosu PostgreSQL'de oluştu.
- **Cache-aside mantığı** tamamlandı: `KurlariGetirAsync` önce veritabanına bakıyor, kayıt yoksa TCMB'ye gidip veriyi çekip veritabanına yazıyor.
- Uçtan uca test edildi: form → controller → servis → veritabanı → (gerekirse) TCMB akışının sorunsuz çalıştığı doğrulandı.

## Kavramlar — Basitçe

### Cache-aside deseni
"Önce hızlı/ucuz kaynağa (veritabanı) bak, orada yoksa yavaş/pahalı kaynağa (TCMB) git, sonucu bir dahaki sefere bulmak için sakla" mantığı.

### Neden aynı tarih için tekrar TCMB'ye gidilmiyor
Geçmiş bir günün resmi döviz kuru, yayınlandıktan sonra **değişmez** (immutable veri). Bu yüzden bir kere veritabanına kaydedildikten sonra, aynı tarih için tekrar TCMB'ye gitmenin hiçbir faydası yok — sadece gereksiz bir dış istek olurdu.

## Görev Durumu

✅ "Veritabanı Desteğinin Eklenmesi" görevi tamamlandı. Bu günün sonunda uygulama; kullanıcı arayüzü, TCMB veri servisi, çapraz kur hesaplama ve PostgreSQL cache mantığı ile temel seviyede uçtan uca çalışır durumdaydı — 2. haftanın (AI Agent ile çalışma) üzerine inşa edileceği temel budur.
