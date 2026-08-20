# 3. Hafta, 4. Gün — Mobil Uygulama: Ekran 3 (Geri Bildirim) Notları

Bu dosya, bugün sorduğun sorulara göre hazırlandı — ne kadar anladığını görmen için.

## Bugünün Görevi Ne

Plandaki adı: **"Mobil Uygulama - Ekran 3"** — kullanıcının uygulama hakkında geri bildirim gönderebileceği bir ekran.

**Önceki ekranlardan farkı:** Ekran 1 ve 2 sadece veri **okuyordu** (`GET` istekleri — kur çekme, çevirme). Bu ekran ilk kez veri **yazıyor** (`POST` isteği) — kullanıcının yazdığı mesaj veritabanına kaydediliyor.

## REST API ve Endpoint

**API** = iki programın (backend ile mobil app) konuştuğu arayüz.

**Endpoint** = API'nin içindeki tek bir adres/kapı. Örnek: `GET /api/kurlar/{tarih}` bir endpoint, `POST /api/geribildirim` başka bir endpoint.

**REST API** = belirli kurallara göre çalışan API türü: her veri kendi adresinde durur, hangi işlemi yapacağını `GET` (oku), `POST` (yeni ekle) gibi yöntemlerle belirtirsin.

**`POST /api/geribildirim` ne anlama geliyor:** "Geri bildirim adresine yeni bir veri gönderiyorum, kaydet" demek.

## X-Api-Key — Ne, Neden, Nerede

**Ne:** İsteğe eklenen bir şifre gibi. Backend, bu şifre doğru gelmeyen istekleri reddediyor (401 hatası).

**Neden var:** Endpoint'i herkes çağırabilsin istemiyoruz — yoksa istenmeyen kişiler sahte veri gönderip veritabanını doldurabilir.

**Apartman benzetmesi:** Kapıda bir görevli (backend) var, sadece şifre bilen kişileri içeri alıyor. Şifreyi bilmeyen biri gelirse (kötü niyetli kişi/uygulama), görevli reddediyor.

**Nasıl otomatik ekleniyor:** Kullanıcı hiçbir yerde elle girmiyor. Mobil uygulamanın kodu (`api.ts`), her isteği atarken `.env`'den okuduğu anahtarı otomatik olarak `X-Api-Key` header'ına ekliyor — kullanıcı bunu görmüyor bile.

**Nerede saklanıyor:**
- Backend'de: **User Secrets** ile — proje klasörünün bile dışında, Mac'inde ayrı bir gizli klasörde duruyor. Git'in göreceği yerde değil.
- Mobil'de: **`.env`** dosyasında — proje klasöründe duruyor ama `.gitignore`'a eklendiği için git onu görmezden geliyor, hiç GitHub'a gitmiyor.

## .gitignore Ne İşe Yarıyor

`.gitignore` = git'e "bu dosyaları asla takip etme, asla commit'e katma" diyen bir liste dosyası. İçine `.env` yazınca, `git add -A` yapsan bile git o dosyayı otomatik atlıyor.

**Amaç:** Şifre/API anahtarı gibi gizli bilgilerin GitHub'a (herkese açık bir yere) hiç gitmemesini sağlamak.

## Bugün Bulduğumuz Güvenlik Sorunu (henüz düzeltilmedi)

Backend projesinde (`TcmbKurDonusturucu`) hiç `.gitignore` dosyası yok. `appsettings.json` dosyası git'e ekli ve GitHub'a zaten pushlanmış — içinde **veritabanı şifresi düz yazı halinde** duruyor.

X-Api-Key güvende (User Secrets'ta), ama bu veritabanı şifresi güvende değil.

**Yapılması gerekenler (ileride):**
1. Veritabanı şifresini değiştir.
2. Connection string'i User Secrets'a taşı.
3. Backend'e `.gitignore` ekle.
4. Commit + push.

*(Bu iş görev listesine eklendi, hatırlatılacak.)*

## Backend'de Bugün Eklenen Kod — Dosya Dosya

### `Models/Geribildirim.cs` — iki sınıf, neden aynı dosyada

```csharp
public class Geribildirim
{
    public int Id { get; set; }
    public string? Ad { get; set; }
    public string Mesaj { get; set; } = string.Empty;
    public DateTime GonderimTarihi { get; set; }
}

public class GeribildirimGonderRequest
{
    [MaxLength(200)]
    public string? Ad { get; set; }

    [Required(ErrorMessage = "Mesaj alanı zorunludur.")]
    [MaxLength(2000)]
    public string Mesaj { get; set; } = string.Empty;
}
```

- **`Geribildirim`** = veritabanına kaydedilen gerçek hali (Id ve tarih dahil — bunları kullanıcı göndermiyor, backend kendisi ekliyor).
- **`GeribildirimGonderRequest`** = kullanıcının gönderdiği hali (sadece Ad + Mesaj, doğrulama kurallarıyla: `[Required]` = boş olamaz, `[MaxLength]` = karakter sınırı).
- **Neden ayrı sınıf:** Kullanıcıdan gelen veri ile veritabanına kaydedilen veri birebir aynı değil.
- **Neden aynı dosyada:** Küçük, ilişkili iki sınıf olduğu için — `DovizKuru.cs`'teki proje stiline uygun.

### `Migrations/..._AddGeribildirim.cs` — tabloyu oluşturan kod

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "Geribildirimler",
        columns: table => new { Id = ..., Ad = ..., Mesaj = ..., GonderimTarihi = ... },
        constraints: table => { table.PrimaryKey("PK_Geribildirimler", x => x.Id); });
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropTable(name: "Geribildirimler");
}
```

- **Migration nedir:** C# modeline bakıp EF Core'un otomatik ürettiği, veritabanı değişikliğini tanımlayan dosya. Elle SQL yazmaya gerek kalmıyor.
- **`Up()`** = uygulanınca ne olur: yeni tablo oluşur, `Id` otomatik artan + birincil anahtar olur.
- **`Down()`** = geri alınırsa ne olur: tablo silinir.
- Bu migration zaten çalıştırılıp gerçek veritabanına uygulandı.

### `Controllers/GeribildirimController.cs` — endpoint'in kendisi

```csharp
[ApiController]
[Route("api/geribildirim")]
[Authorize(AuthenticationSchemes = "ApiKey")]
public class GeribildirimController : ControllerBase
{
    private readonly IGeribildirimServisi _geribildirimServisi;

    [HttpPost]
    public async Task<IActionResult> Gonder([FromBody] GeribildirimGonderRequest request)
    {
        var geribildirim = await _geribildirimServisi.KaydetAsync(request.Ad, request.Mesaj);
        return StatusCode(StatusCodes.Status201Created, geribildirim);
    }
}
```

- **`[Route("api/geribildirim")]`** (class'ın üstünde) → adresi belirliyor.
- **`[Authorize(AuthenticationSchemes = "ApiKey")]`** → X-Api-Key koruması.
- **`[HttpPost]`** (parantez boş) → adres zaten yukarıda tanımlı olduğu için tekrar yazmaya gerek yok, sadece "POST isteğine cevap ver" diyor. Adres + yöntem birlikte: `POST /api/geribildirim`.
- **Neden `_geribildirimServisi.KaydetAsync(...)` çağırıyor, kendisi kaydetmiyor:** Controller'ın işi sadece isteği karşılamak. Asıl kaydetme mantığı ayrı bir Service dosyasında tutuluyor (`KurlarController`'ın `TcmbKurServisi` kullanması gibi) — mantık ile istek karşılama işi ayrı tutuluyor, MVVM'deki View/ViewModel ayrımının aynı fikri.

## Veritabanına Bağlanma Zinciri

**Controller → Service → DbContext → connection string → gerçek veritabanı.**

1. `appsettings.json` (ya da User Secrets) içinde bir **connection string** var — veritabanının adresi.
2. **`DbContext`** (`AppDbContext.cs`), C# koduyla veritabanı arasındaki köprü. `Geribildirimler` tablosunu `DbSet<Geribildirim> Geribildirimler` satırıyla temsil ediyor.
3. Service, `dbContext.Geribildirimler.Add(yeniKayit)` + `SaveChangesAsync()` ile gerçekten veritabanına yazıyor.

## İstek Atıldığında Tam Olarak Ne Oluyor

1. Kullanıcı mobil uygulamada "Gönder" butonuna basar.
2. Uygulama, mesajı alıp `POST /api/geribildirim` adresine istek atar (içine otomatik olarak X-Api-Key'i de ekleyerek).
3. Backend, X-Api-Key'i kontrol eder.
4. Doğruysa, veriyi veritabanına yeni bir satır olarak kaydeder.
5. Backend, mobil uygulamaya "201 Created" (başarılı, kaydedildi) cevabı döner.
6. Uygulama, kullanıcıya "Geri bildiriminiz alındı" gibi bir mesaj gösterir.

## ✅ Şu Ana Kadarki Durum

Backend tarafı (model, migration, servis, controller, testler — 23/23 yeşil) tamamlandı ve doğrulandı. Sırada: mobil tarafta geri bildirim ekranının yapılması.
