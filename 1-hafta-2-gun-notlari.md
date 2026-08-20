# 1. Hafta, 2. Gün — TCMB Entegrasyonu ve Çapraz Kur Hesaplama

**Not:** Bu notta da aynı dürüstlük payı geçerli — gün numarası, git commit sırası ve plan süresine göre tahmini.

## Bugün Ne Yapıldı (tahmini)

- TCMB'nin (Türkiye Cumhuriyet Merkez Bankası) günlük yayınladığı XML servisinden veri çekme özelliği eklendi (`TcmbKurServisi`, `HttpClient` ile).
- XML'i ayrıştırıp (`CurrencyCode`, `Unit`, `ForexBuying`, `ForexSelling` alanlarını okuyup) bir `DovizKuru` nesnesine dönüştüren mantık yazıldı.
- İki para birimi arasında **çapraz kur hesaplama** mantığı eklendi.

## Kavramlar — Basitçe

### XML nedir
Verinin etiketler içinde (`<Currency>...</Currency>` gibi) düzenli bir şekilde saklandığı bir format — JSON'a benzer bir amacı var, farklı bir yazım şekli. TCMB, döviz kurlarını bu formatta yayınlıyor.

### HttpClient nedir
.NET'in, dış bir web adresine (bu durumda TCMB'nin sunucusuna) istek atmak için kullandığı sınıf.

### Çapraz kur hesaplama mantığı
TCMB, her para biriminin sadece TL karşılığını veriyor (örn. "1 USD = X TL"). İki farklı para birimini (örn. USD → EUR) karşılaştırmak için, ikisinin de TL karşılığı bulunup birbirine oranlanıyor: `1 USD = (X / Y) EUR`. **Neden TL üzerinden:** Çünkü TCMB'nin verdiği tüm veriler zaten TL bazında, bu doğal bir ortak referans noktası.

## Görev Durumu

"Asp.Net Temel Geliştirme" görevinin çekirdeği bu günde tamamlandı: TCMB'den veri çekme ve çapraz kur hesaplama artık çalışıyor.
