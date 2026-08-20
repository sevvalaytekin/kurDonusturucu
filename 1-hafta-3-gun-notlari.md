# 1. Hafta, 3. Gün — "Resmi Tatil Kontrolü" Denemesi

**Not:** Aynı dürüstlük payı — gün numarası tahmini, içerik gerçek koddan ve git commit geçmişinden doğrulanmış.

## Bugün Ne Yapıldı (tahmini)

Git geçmişinde bu güne karşılık gelebilecek commit: `"Resmi tatil kontrolü eklendi"`.

**Dürüst not (önemli):** CLAUDE.md hazırlanırken kodu incelediğimde şunu fark ettim: bu commit **gerçek bir resmi tatil takvimi eklememiş**. Önceden var olan bir "hafta sonu kaydırma" mantığını kaldırıp, yerine genel bir `try/catch` bloğu koymuş. Yani şu an kodda, TCMB bir gün için veri döndürmezse (tatil, hafta sonu, vs.), kullanıcıya sadece genel bir hata mesajı gösteriliyor — özel bir tatil mantığı yok.

## Kavramlar — Basitçe

### Commit mesajı ile kodun gerçekte yaptığı şey arasındaki fark
Bir commit mesajı "ne yapıldığını iddia eder", ama kodun kendisi "gerçekte ne yaptığını gösterir." Bu ikisi bazen örtüşmeyebilir — kötü niyetten değil, çoğunlukla zaman baskısından ya da "sonra tamamlarım" düşüncesinden. **Ders:** Bir işi "bitti" diye işaretlemeden önce, commit mesajına değil kodun gerçekte yaptığına bakmak gerekir.

### try/catch nedir (kısaca)
Bir kod bloğunda hata oluşursa, uygulamanın çökmesi yerine o hatayı "yakalayıp" kontrollü bir şekilde ele almanı sağlayan yapı. `try` içinde riskli kod çalışır, hata olursa `catch` bloğu devreye girer.

## Görev Durumu

Bu iş kalemi kısmen tamamlanmış sayılır — hata yönetimi var, ama gerçek bir "resmi tatil kontrolü" hâlâ eksik. İleride ele alınması gereken bir konu olarak not edildi.
