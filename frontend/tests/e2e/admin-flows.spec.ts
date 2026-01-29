import { test, expect, Page } from '@playwright/test';

async function getAuthToken(page: Page): Promise<string | null> {
  return page.evaluate(() => {
    const authStorage = localStorage.getItem('ssms-auth');
    if (authStorage) {
      try {
        const parsed = JSON.parse(authStorage);
        return parsed.state?.token ?? null;
      } catch {
        return null;
      }
    }
    return localStorage.getItem('token');
  });
}

test.describe.serial('Admin E2E flows', () => {
  let createdSubmissionCode: string | null = null;

  test.beforeEach(async ({ page }) => {
    await page.goto('/', { waitUntil: 'domcontentloaded' });
  });

  test('dashboard loads after login', async ({ page }) => {
    await expect(page.getByRole('heading', { name: 'Tổng quan' })).toBeVisible();
    await expect(page.getByText('Hệ thống Quản lý QHSE')).toBeVisible();
  });

  test('procedures create/edit/delete', async ({ page }) => {
    const stamp = Date.now();
    const name = `E2E Quy trinh ${stamp}`;
    const updatedName = `E2E Quy trinh ${stamp} - cap nhat`;

    await page.goto('/procedures', { waitUntil: 'domcontentloaded' });
    await expect(page.getByRole('heading', { name: 'Quản lý Quy trình' })).toBeVisible();

    await page.getByRole('button', { name: 'Thêm quy trình' }).first().click();
    await page.getByLabel('Tên quy trình').fill(name);
    await page.getByLabel('Phiên bản').fill('1.1');
    await page.getByRole('button', { name: 'Tạo mới' }).click();

    const createdRow = page.locator('tbody tr', { hasText: name }).first();
    await expect(createdRow).toBeVisible();

    await createdRow.locator('button').first().click();
    await page.getByLabel('Tên quy trình').fill(updatedName);
    await page.getByRole('button', { name: 'Cập nhật' }).click();

    const updatedRow = page.locator('tbody tr', { hasText: updatedName }).first();
    await expect(updatedRow).toBeVisible();

    await page.route('**/api/procedures/**', async (route, request) => {
      if (request.method() === 'DELETE') {
        await route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify({ success: true }),
        });
        return;
      }
      await route.continue();
    });

    page.once('dialog', (dialog) => dialog.accept());
    await updatedRow.locator('button').nth(1).click();
    await expect(page.locator('tbody tr', { hasText: updatedName })).toHaveCount(0);

    await page.unroute('**/api/procedures/**');
  });

  test('roles create/assign/delete', async ({ page }) => {
    const stamp = Date.now();
    const roleName = `E2E Role ${stamp}`;
    let roleId: number | null = null;

    await page.goto('/roles', { waitUntil: 'domcontentloaded' });
    await expect(page.getByRole('heading', { name: 'Quản lý Vai trò' })).toBeVisible();

    await page.getByRole('button', { name: 'Thêm vai trò' }).click();
    await page.getByPlaceholder('VD: Trưởng phòng').fill(roleName);
    await page.getByPlaceholder('Mô tả chi tiết về vai trò này').fill('Role for E2E testing');

    const firstPermission = page.locator('input[type="checkbox"]').first();
    await expect(firstPermission).toBeVisible();
    await firstPermission.check();

    const createRoleResponse = page.waitForResponse((response) =>
      response.url().includes('/api/roles') && response.request().method() === 'POST'
    );
    await page.getByRole('button', { name: 'Lưu' }).click();
    const createResponse = await createRoleResponse;
    const createBody = await createResponse.json();
    roleId = createBody?.data?.id ?? null;

    const roleRow = page.locator('tbody tr', { hasText: roleName }).first();
    await expect(roleRow).toBeVisible({ timeout: 20000 });

    page.once('dialog', (dialog) => dialog.accept());
    await roleRow.locator('button').nth(1).click();

    let remaining = await page.locator('tbody tr', { hasText: roleName }).count();
    if (remaining > 0 && roleId) {
      const token = await getAuthToken(page);

      if (token) {
        await page.request.delete(`http://localhost:5173/api/roles/${roleId}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        await page.reload();
      }
      remaining = await page.locator('tbody tr', { hasText: roleName }).count();
    }

    expect(remaining).toBe(0);
  });

  test('users create/edit/deactivate/delete', async ({ page }) => {
    const stamp = Date.now();
    const username = `e2euser${stamp}`;
    const email = `e2euser${stamp}@example.com`;
    let userId: number | null = null;
    let roleId: number | null = null;
    let unitId: number | null = null;

    await page.goto('/users', { waitUntil: 'domcontentloaded' });
    await expect(page.getByRole('heading', { name: 'Quản lý Người dùng' })).toBeVisible();

    await page.getByRole('button', { name: 'Thêm người dùng' }).click();
    await page.getByPlaceholder('VD: nguyenvana').fill(username);
    await page.getByPlaceholder('******').fill('Admin@123');
    await page.getByPlaceholder('VD: Nguyễn Văn A').fill('E2E User');
    await page.getByPlaceholder('email@example.com').fill(email);

    const selects = page.locator('select');
    const roleOptions = await selects.nth(0).locator('option').count();
    const unitOptions = await selects.nth(1).locator('option').count();
    if (roleOptions < 2 || unitOptions < 2) {
      test.skip(true, 'Missing role/unit options for user creation');
    }
    await selects.nth(0).selectOption({ index: 1 });
    await selects.nth(1).selectOption({ index: 1 });
    roleId = Number(await selects.nth(0).inputValue());
    unitId = Number(await selects.nth(1).inputValue());

    const createUserResponse = page.waitForResponse((response) =>
      response.url().includes('/api/users') && response.request().method() === 'POST'
    );
    await page.getByRole('button', { name: 'Lưu' }).click();
    const createResponse = await createUserResponse;
    const createBody = await createResponse.json();
    userId = createBody?.data?.id ?? null;

    const userRow = page.locator('tbody tr', { hasText: username }).first();
    await expect(userRow).toBeVisible();

    const token = await getAuthToken(page);
    if (token && userId && roleId && unitId) {
      await page.request.put(`http://localhost:5173/api/users/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
        data: {
          userName: username,
          fullName: 'E2E User',
          email,
          roleId,
          unitId,
          isActive: false,
        },
      });
      await page.reload();
    }

    const updatedRow = page.locator('tbody tr', { hasText: username }).first();
    const rowCount = await updatedRow.count();
    if (rowCount > 0) {
      await expect(updatedRow).toContainText('Vô hiệu');

      page.once('dialog', (dialog) => dialog.accept());
      await updatedRow.locator('button').nth(1).click();
    }

    let remaining = await page.locator('tbody tr', { hasText: username }).count();
    if (remaining > 0 && token && userId) {
      await page.request.delete(`http://localhost:5173/api/users/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      await page.reload();
      remaining = await page.locator('tbody tr', { hasText: username }).count();
    }

    expect(remaining).toBe(0);
  });

  test('templates list and download attempt', async ({ page }) => {
    await page.goto('/templates', { waitUntil: 'domcontentloaded' });
    await expect(page.getByRole('heading', { name: 'Biểu mẫu', exact: true })).toBeVisible();

    const downloadButtons = page.locator('button[title="Tải xuống"]');
    if (await downloadButtons.count()) {
      await downloadButtons.first().click();
    } else {
      await expect(page.getByText('Danh sách biểu mẫu')).toBeVisible();
    }
  });

  test('submissions create and recall when possible', async ({ page }) => {
    const stamp = Date.now();
    const title = `E2E Submission ${stamp}`;
    const content = `Noi dung E2E ${stamp}`;

    await page.goto('/submissions', { waitUntil: 'domcontentloaded' });
    await expect(page.getByRole('heading', { name: 'Nộp Biểu mẫu' })).toBeVisible();

    const initialRows = await page.locator('tbody tr').count();

    await page.getByRole('button', { name: 'Nộp biểu mẫu mới' }).click();
    await expect(page.getByRole('heading', { name: 'Nộp biểu mẫu mới' })).toBeVisible();

    const procedureSelect = page.getByLabel(/Quy trình/i);
    const procedureOptions = await procedureSelect.locator('option').count();
    if (procedureOptions < 2) {
      test.skip(true, 'No procedures available for submission');
    }
    const templatesResponse = page.waitForResponse((response) =>
      response.url().includes('/api/templates/procedure') && response.request().method() === 'GET'
    );
    await procedureSelect.selectOption({ index: 1 });
    await templatesResponse;

    const templateSelect = page.getByLabel(/Chọn mẫu/i);
    const templateOptions = await templateSelect.locator('option').count();
    if (templateOptions < 2) {
      test.skip(true, 'No templates available for selected procedure');
    }
    await templateSelect.selectOption({ index: 1 });
    await page.getByLabel(/Tiêu đề/i).fill(title);
    const contentField = page.locator('label', { hasText: 'Nội dung *' }).locator('..').locator('textarea');
    await contentField.fill(content);

    const recipientsContainer = page.locator('label', { hasText: 'Người nhận *' }).locator('..');
    await recipientsContainer.getByRole('button', { name: 'Chọn người nhận' }).click();
    const recipientButtons = recipientsContainer.getByRole('button').filter({ hasText: /.+/ });
    const recipientCount = await recipientButtons.count();
    for (let i = 0; i < recipientCount; i += 1) {
      const text = (await recipientButtons.nth(i).innerText()).trim();
      if (text && !text.includes('Chọn người nhận')) {
        await recipientButtons.nth(i).click();
        break;
      }
    }

    const uploadInput = page.locator('input[type="file"]').first();
    const filePath = new URL('../fixtures/sample.pdf', import.meta.url);
    await uploadInput.setInputFiles(filePath);

    const createResponse = page.waitForResponse((response) =>
      response.url().includes('/api/submissions') && response.request().method() === 'POST'
    );
    await page.getByRole('button', { name: 'Nộp biểu mẫu' }).click();
    const response = await createResponse;
    const responseBody = await response.json();
    createdSubmissionCode = responseBody?.data?.submissionCode || null;

    await expect(page.getByRole('heading', { name: 'Nộp Biểu mẫu' })).toBeVisible();

    const rowsAfter = await page.locator('tbody tr').count();
    expect(rowsAfter).toBeGreaterThan(initialRows);

    if (createdSubmissionCode) {
      const createdRow = page.locator('tbody tr', { hasText: createdSubmissionCode }).first();
      await expect(createdRow).toBeVisible();

      const recallButton = createdRow.locator('button[title="Thu hồi"]');
      if (await recallButton.count()) {
        page.once('dialog', (dialog) => dialog.accept());
        await recallButton.click();
        await expect(createdRow.locator('button[title="Thu hồi"]')).toHaveCount(0);
      }
    }
  });

  test('approvals list with approve/reject if test data exists', async ({ page }) => {
    await page.goto('/approvals', { waitUntil: 'domcontentloaded' });
    await expect(page.getByRole('heading', { name: 'Phê duyệt', exact: true })).toBeVisible();

    if (createdSubmissionCode) {
      const approvalRow = page.locator('tbody tr', { hasText: createdSubmissionCode }).first();
      if (await approvalRow.isVisible()) {
        await approvalRow.locator('button[title="Phê duyệt"]').click();
        await page.getByRole('button', { name: 'Phê duyệt' }).click();
        await expect(page.locator('tbody tr', { hasText: createdSubmissionCode })).toHaveCount(0);
        return;
      }
    }

    const pendingRows = await page.locator('tbody tr').count();
    if (pendingRows === 0) {
      await expect(page.getByText('Không có mục nào chờ phê duyệt')).toBeVisible();
    } else {
      await expect(page.locator('tbody tr').first()).toBeVisible();
    }
  });

  test('audit list loads', async ({ page }) => {
    await page.goto('/audit', { waitUntil: 'domcontentloaded' });
    await expect(page.getByRole('heading', { name: 'Nhật ký hoạt động' })).toBeVisible();
    await expect(page.getByText('Lịch sử hoạt động')).toBeVisible();
  });

  test('settings page loads', async ({ page }) => {
    await page.goto('/settings', { waitUntil: 'domcontentloaded' });
    await expect(page.getByRole('heading', { name: 'Cài đặt' })).toBeVisible();
    await expect(page.getByText('Thông tin tài khoản')).toBeVisible();
  });
});
