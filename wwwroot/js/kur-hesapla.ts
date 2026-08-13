interface KurHesaplaSonucu {
  success: boolean;
  message: string | null;
  kaynak: string;
  hedef: string;
  birimKur: number;
  girilenMiktar: number;
  sonuc: number;
}

document.addEventListener("DOMContentLoaded", function () {
  const tarihInput = document.getElementById("tarihInput") as HTMLInputElement | null;
  if (tarihInput) {
    const bugun = new Date().toISOString().split("T")[0];
    tarihInput.setAttribute("max", bugun);
    tarihInput.value = bugun;
  }

  const form = document.getElementById("kurForm") as HTMLFormElement | null;
  const btnHesapla = document.getElementById("btnHesapla") as HTMLButtonElement;
  const btnText = document.getElementById("btnText") as HTMLElement;
  const btnSpinner = document.getElementById("btnSpinner") as HTMLElement;
  const alertBox = document.getElementById("alertBox") as HTMLElement;

  if (form) {
    form.addEventListener("submit", function (e) {
      e.preventDefault();

      btnHesapla.disabled = true;
      btnText.textContent = "Hesaplanıyor...";
      btnSpinner.classList.remove("d-none");

      const formData = new FormData(form);

      fetch("/Home/KurHesapla", {
        method: "POST",
        body: formData,
      })
        .then((response) => response.json() as Promise<KurHesaplaSonucu>)
        .then((data) => {
          if (data.success) {
            alertBox.className = "alert alert-success mb-4";
            alertBox.innerHTML = `
                        <div>
                            <strong>Hesaplama Başarılı!</strong><br />
                            1 ${data.kaynak} = ${data.birimKur.toFixed(4)} ${data.hedef}<br />
                            <hr class="my-2">
                            <h5 class="mb-0">${data.girilenMiktar} ${data.kaynak} = <strong>${data.sonuc.toFixed(2)} ${data.hedef}</strong></h5>
                        </div>`;
          } else {
            alertBox.className = "alert alert-danger mb-4";
            alertBox.innerHTML = `<div>⚠️ ${data.message}</div>`;
          }
        })
        .catch((error) => {
          alertBox.className = "alert alert-danger mb-4";
          alertBox.innerHTML = `<div>Sunucu ile iletişim kurulurken bir hata oluştu.</div>`;
        })
        .finally(() => {
          // Yükleniyor durumunu kapat
          btnHesapla.disabled = false;
          btnText.textContent = "Hesapla";
          btnSpinner.classList.add("d-none");
        });
    });
  }
});
