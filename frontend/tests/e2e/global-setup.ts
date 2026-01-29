import { chromium, request } from '@playwright/test';

const credentials = {
  username: process.env.E2E_USERNAME ?? 'admin',
  password: process.env.E2E_PASSWORD ?? 'Admin@123',
};

async function globalSetup() {
  const browser = await chromium.launch();
  const page = await browser.newPage();
  const baseUrl = process.env.E2E_BASE_URL ?? 'http://localhost:5173';
  const apiBaseUrl = process.env.E2E_API_BASE_URL ?? 'http://localhost:5265';

  const api = await request.newContext({ baseURL: apiBaseUrl });
  const response = await api.post('/api/auth/login', {
    data: {
      loginName: credentials.username,
      password: credentials.password,
    },
  });

  if (!response.ok()) {
    throw new Error(`Login API failed: ${response.status()} ${response.statusText()}`);
  }

  const payload = await response.json();
  const data = payload?.data ?? payload?.Data;
  const token = data?.token ?? data?.Token;
  const user = data?.user ?? data?.User;

  if (!token || !user) {
    throw new Error('Login API missing token or user data');
  }

  const authState = {
    state: {
      token,
      user,
      currentUnitId: user.UnitId ?? user.unitId,
      isAuthenticated: true,
    },
    version: 0,
  };

  await page.addInitScript((auth) => {
    window.localStorage.setItem('ssms-auth', JSON.stringify(auth));
  }, authState);

  await page.goto(baseUrl, { waitUntil: 'domcontentloaded', timeout: 60000 });
  await page.getByRole('heading', { name: 'Tổng quan' }).waitFor({ timeout: 20000 });

  await page.context().storageState({ path: 'tests/.auth/admin.json' });
  await api.dispose();
  await browser.close();
}

export default globalSetup;
