import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  reporter: [['list'], ['html', { open: 'never' }]],
  use: {
    baseURL: 'http://localhost:5183',
    trace: 'retain-on-failure',
  },
  webServer: {
    command: 'dotnet run --urls http://localhost:5183',
    url: 'http://localhost:5183',
    reuseExistingServer: !process.env.CI,
    timeout: 120_000,
  },
});
