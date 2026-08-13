import { test, expect } from '@playwright/test';

test('kur hesaplama formu ucdan uca calisir', async ({ page }) => {
  await page.goto('/');

  await page.selectOption('select[name="kaynakDoviz"]', 'USD');
  await page.selectOption('select[name="hedefDoviz"]', 'EUR');
  await page.fill('input[name="miktar"]', '100');

  await page.click('#btnHesapla');

  const alertBox = page.locator('#alertBox');
  await expect(alertBox).toHaveClass(/alert-success/);
  await expect(alertBox).toContainText('Hesaplama Başarılı');
  await expect(alertBox).toContainText('USD');
  await expect(alertBox).toContainText('EUR');
});
