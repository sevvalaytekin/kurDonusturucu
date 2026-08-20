# 1. Hafta, 1. Gün — Proje İskeleti ve Temel Form

**Not (dürüstlük payı):** 1. haftanın (1-5. günler) çalışması, ben bu projeye dahil olmadan önce tamamlanmıştı. Bu notu, mevcut koddan ve git commit geçmişinden çıkardım. Hangi işin tam olarak hangi günde yapıldığı benim için kesin değil — plandaki gün sayılarına (Asp.Net Temel Geliştirme: 3 gün) ve commit sırasına göre en makul bölünmeyi yaptım. Bu yüzden gün numaraları yaklaşık, ama içerik (ne yapıldığı) gerçek koddan doğrulanmış.

## Bugün Ne Yapıldı (tahmini)

- Proje iskeleti oluşturuldu: ASP.NET Core MVC yapısı (`Controllers/`, `Models/`, `Views/`, `Program.cs`).
- Kullanıcının tarih, kaynak/hedef para birimi ve miktar girebileceği bir form hazırlandı (`Views/Home/Index.cshtml`), Bootstrap ile stillendirildi.
- Form gönderme işlemi JavaScript'te **Fetch API** kullanılarak asenkron hale getirildi — yani form gönderildiğinde sayfa tamamen yenilenmiyor, arka planda backend'e istek atılıyor.

## Kavramlar — Basitçe

### MVC nedir (kısa hatırlatma)
**Model-View-Controller.** Controller isteği karşılar, Model veriyi temsil eder, View kullanıcıya gösterilen HTML'i üretir. Bu ayrım, "hangi kod nerede" sorusuna netlik katar — controller HTTP ile, view görünümle, model veriyle ilgilenir.

### Fetch API nedir
Tarayıcının, sayfayı yenilemeden sunucuya istek atmasını sağlayan bir JavaScript aracı. **Neden önemli:** Fetch olmadan, her form gönderiminde tüm sayfa yeniden yüklenirdi — kullanıcı deneyimi kötü olurdu (ekran "yanıp sönerdi"). Fetch ile sadece gereken veri gidip geliyor, sayfa yerinde kalıyor.

### Bootstrap nedir
Hazır, test edilmiş CSS/JS bileşenleri sunan bir kütüphane (buton, form, kart gibi görsel öğeler). **Neden kullanıldı:** Sıfırdan CSS yazmak yerine, hazır ve düzgün görünen bileşenleri kullanarak zamandan tasarruf sağlıyor.

## Görev Durumu

Bu gün, "Asp.Net Temel Geliştirme" (3 günlük) görevinin ilk parçası olarak değerlendirilebilir — temel form ve proje yapısı kuruldu, henüz TCMB entegrasyonu yok.
