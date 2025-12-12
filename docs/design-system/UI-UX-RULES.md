# UI/UX Design System - Quy tắc TUYỆT ĐỐI phải tuân thủ

> **QUAN TRỌNG**: File này chứa các quy tắc UI/UX BẮT BUỘC cho toàn bộ hệ thống SSMS.
> Mọi code UI/UX phải tuân thủ 100% các quy tắc này để tránh trông "AI-generated".

---

## 📐 1. DESIGN TOKENS - Hệ thống biến CSS chuẩn

### 1.1. Color Palette (Bảng màu chuẩn)

```css
:root {
  /* Background & Surface */
  --bg-primary: #f8fafc;        /* Slate 50 - Background chính */
  --bg-secondary: #f1f5f9;      /* Slate 100 - Background phụ */
  --surface: #ffffff;           /* White - Card, Panel */

  /* Text Colors */
  --text-primary: #0f172a;      /* Slate 900 - Text chính */
  --text-secondary: #64748b;    /* Slate 500 - Text phụ, muted */
  --text-tertiary: #94a3b8;     /* Slate 400 - Placeholder, disabled */

  /* Border & Divider */
  --border-light: #e2e8f0;      /* Slate 200 - Border nhạt */
  --border-medium: #cbd5e1;     /* Slate 300 - Border đậm hơn */
  --border-dark: #94a3b8;       /* Slate 400 - Border focus */

  /* Brand Colors - TRÁNH dùng xanh-tím AI */
  --primary: #0369a1;           /* Sky 700 - Primary đậm hơn (thay #0ea5e9) */
  --primary-light: #0284c7;     /* Sky 600 */
  --primary-lighter: #bae6fd;   /* Sky 200 - Background hover */
  --primary-dark: #075985;      /* Sky 800 - Active state */

  /* Semantic Colors */
  --success: #059669;           /* Emerald 600 (thay #10b981) */
  --success-light: #d1fae5;     /* Emerald 100 */
  --warning: #d97706;           /* Amber 600 (thay #f59e0b) */
  --warning-light: #fef3c7;     /* Amber 100 */
  --danger: #dc2626;            /* Red 600 (thay #ef4444) */
  --danger-light: #fee2e2;      /* Red 100 */
  --info: #2563eb;              /* Blue 600 */
  --info-light: #dbeafe;        /* Blue 100 */
}
```

**Lý do thay đổi màu:**
- ❌ **TRÁNH**: `#0ea5e9`, `#60a5fa` (xanh-tím AI điển hình)
- ✅ **DÙNG**: `#0369a1`, `#0284c7` (Sky 700/600 - professional hơn)

---

### 1.2. Typography (Font chữ)

```css
:root {
  /* Font Family - KHÔNG DÙNG default system-ui */
  --font-primary: 'Inter', 'Be Vietnam Pro', -apple-system, BlinkMacSystemFont, sans-serif;
  --font-monospace: 'JetBrains Mono', 'Fira Code', Consolas, monospace;

  /* Font Sizes - Scale chuẩn */
  --text-xs: 11px;      /* Captions, labels nhỏ */
  --text-sm: 12px;      /* Muted text, table headers */
  --text-base: 13px;    /* Body text, table cells */
  --text-md: 14px;      /* Button text */
  --text-lg: 16px;      /* H4, Card titles */
  --text-xl: 18px;      /* H3, Page titles */
  --text-2xl: 20px;     /* H2, Section headers */
  --text-3xl: 24px;     /* H1, Dashboard KPI */

  /* Font Weights */
  --font-normal: 400;
  --font-medium: 500;
  --font-semibold: 600;
  --font-bold: 700;
  --font-extrabold: 800;

  /* Line Heights */
  --leading-tight: 1.25;
  --leading-normal: 1.5;
  --leading-relaxed: 1.75;

  /* Letter Spacing */
  --tracking-tight: -0.02em;
  --tracking-normal: 0;
  --tracking-wide: 0.05em;
  --tracking-wider: 0.08em;
}
```

**Font khuyến nghị:**
1. **Inter** - Modern, clean, excellent for UI (Google Fonts)
2. **Be Vietnam Pro** - Vietnamese-friendly, professional (Google Fonts)
3. **Montserrat** - Alternative, geometric sans-serif

**Cách import:**
```html
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&family=Be+Vietnam+Pro:wght@400;500;600;700&display=swap" rel="stylesheet">
```

---

### 1.3. Spacing System (Khoảng cách)

```css
:root {
  /* Spacing Scale - 4px base unit */
  --space-1: 4px;
  --space-2: 8px;
  --space-3: 12px;
  --space-4: 16px;
  --space-5: 20px;
  --space-6: 24px;
  --space-8: 32px;
  --space-10: 40px;
  --space-12: 48px;
  --space-16: 64px;

  /* Component-specific spacing */
  --padding-btn: 8px 14px;              /* Button padding */
  --padding-input: 9px 12px;            /* Input, Select padding */
  --padding-card: 14px;                 /* Card body */
  --padding-panel: 14px 16px;           /* Panel body */
  --gap-toolbar: 8px;                   /* Toolbar button gap */
  --gap-form: 12px;                     /* Form field gap */
}
```

**Quy tắc:**
- Luôn dùng bội số của 4px (4, 8, 12, 16, 20, 24...)
- Dùng CSS variables (`var(--space-*)`) thay vì hard-code

---

### 1.4. Border Radius (Bo góc)

```css
:root {
  /* QUAN TRỌNG: Rounded cha > Rounded con */
  --radius-sm: 6px;       /* Chip, badge, kbd */
  --radius-md: 8px;       /* Input, select, small buttons */
  --radius-lg: 10px;      /* Button, nav items */
  --radius-xl: 12px;      /* Card */
  --radius-2xl: 14px;     /* Panel, Modal, Sidebar */
  --radius-full: 9999px;  /* Pill buttons, avatar */

  /* Hierarchy Rule (QUY TẮC BẮT BUỘC) */
  /* Panel (14px) > Card (12px) > Button (10px) > Input (8px) > Chip (6px) */
}
```

**❌ SAI - Tất cả đều 12px:**
```css
.panel { border-radius: 12px; }
.card { border-radius: 12px; }
.button { border-radius: 12px; }
.input { border-radius: 12px; }
```

**✅ ĐÚNG - Hierarchy cha > con:**
```css
.panel { border-radius: var(--radius-2xl); }    /* 14px */
.card { border-radius: var(--radius-xl); }      /* 12px */
.button { border-radius: var(--radius-lg); }    /* 10px */
.input { border-radius: var(--radius-md); }     /* 8px */
.chip { border-radius: var(--radius-sm); }      /* 6px */
```

---

### 1.5. Shadows & Elevation - CẤM BOX-SHADOW AI

```css
:root {
  /* ⚠️ QUY TẮC: KHÔNG dùng box-shadow mặc định AI */
  /* Chỉ dùng border nhạt thay vì shadow */

  --shadow-none: none;
  --shadow-border: 0 0 0 1px var(--border-light);  /* Thay box-shadow */

  /* NGOẠI LỆ: Chỉ dùng shadow cho Modal/Dropdown (floating elements) */
  --shadow-modal: 0 20px 40px rgba(15, 23, 42, 0.12);
  --shadow-dropdown: 0 10px 20px rgba(15, 23, 42, 0.08);
}
```

**❌ SAI:**
```css
.card { box-shadow: 0 10px 25px rgba(2,6,23,.06); }
.panel { box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
.button { box-shadow: 0 2px 8px rgba(14, 165, 233, 0.3); }
```

**✅ ĐÚNG:**
```css
.card { border: 1px solid var(--border-light); }
.panel { border: 1px solid var(--border-light); }
.button { border: 1px solid var(--border-light); }

/* NGOẠI LỆ: Modal/Dropdown được dùng shadow */
dialog { box-shadow: var(--shadow-modal); }
.dropdown { box-shadow: var(--shadow-dropdown); }
```

---

## 🎨 2. COMPONENT DESIGN RULES

### 2.1. Buttons (Nút bấm)

```css
/* ✅ ĐÚNG - Button chuẩn */
.btn {
  appearance: none;
  border: 1px solid var(--border-light);
  background: var(--surface);
  border-radius: var(--radius-lg);        /* 10px */
  padding: var(--padding-btn);            /* 8px 14px */
  font-size: var(--text-md);              /* 14px */
  font-weight: var(--font-semibold);      /* 600 */
  color: var(--text-primary);
  cursor: pointer;
  transition: all 0.15s ease-in-out;      /* Smooth transition */
}

.btn:hover {
  background: var(--bg-secondary);
  border-color: var(--border-medium);
  transform: translateY(-1px);            /* Subtle lift effect */
}

.btn:active {
  transform: translateY(0);
}

/* Primary Button - KHÔNG dùng gradient */
.btn-primary {
  background: var(--primary);             /* Solid color, NO gradient */
  color: #ffffff;
  border-color: transparent;
}

.btn-primary:hover {
  background: var(--primary-light);       /* Darker on hover */
}

/* Secondary Button */
.btn-secondary {
  background: var(--bg-secondary);
  color: var(--text-primary);
  border-color: var(--border-light);
}

/* Success/Danger/Warning Buttons */
.btn-success {
  background: var(--success);
  color: #ffffff;
  border-color: transparent;
}

.btn-danger {
  background: var(--danger);
  color: #ffffff;
  border-color: transparent;
}

.btn-warning {
  background: var(--warning);
  color: #ffffff;
  border-color: transparent;
}

/* Ghost Button */
.btn-ghost {
  background: transparent;
  color: var(--text-primary);
  border-color: transparent;
}

.btn-ghost:hover {
  background: var(--bg-secondary);
}
```

**❌ SAI - Gradient AI style:**
```css
.btn-wrong {
  background: linear-gradient(135deg, #0ea5e9, #60a5fa);  /* KHÔNG dùng */
  box-shadow: 0 4px 12px rgba(14, 165, 233, 0.3);        /* KHÔNG dùng */
}
```

---

### 2.2. Cards (Thẻ)

```css
/* ✅ ĐÚNG - Card với border thay shadow */
.card {
  background: var(--surface);
  border: 1px solid var(--border-light);  /* Border thay box-shadow */
  border-radius: var(--radius-xl);        /* 12px - Nhỏ hơn Panel */
  padding: var(--padding-card);           /* 14px */
  transition: border-color 0.2s ease;
}

.card:hover {
  border-color: var(--border-medium);     /* Border đậm hơn khi hover */
}

/* Card Header */
.card-header {
  padding-bottom: var(--space-3);
  margin-bottom: var(--space-3);
  border-bottom: 1px solid var(--border-light);
}

.card-title {
  font-size: var(--text-lg);
  font-weight: var(--font-semibold);
  color: var(--text-primary);
  margin: 0;
}

/* Card Footer */
.card-footer {
  padding-top: var(--space-3);
  margin-top: var(--space-3);
  border-top: 1px solid var(--border-light);
}
```

**❌ SAI - Card AI style:**
```css
.card-wrong {
  box-shadow: 0 10px 25px rgba(2, 6, 23, 0.06);  /* KHÔNG dùng */
  background: rgba(255, 255, 255, 0.8);          /* KHÔNG blur/glass */
  backdrop-filter: blur(10px);                   /* KHÔNG glassmorphism */
}
```

---

### 2.3. Panels (Panel chứa nội dung)

```css
/* ✅ ĐÚNG - Panel */
.panel {
  background: var(--surface);
  border: 1px solid var(--border-light);
  border-radius: var(--radius-2xl);       /* 14px - Lớn nhất */
}

.panel-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-3) var(--space-4); /* 12px 16px */
  border-bottom: 1px solid var(--border-light);
}

.panel-title {
  font-size: var(--text-xl);
  font-weight: var(--font-semibold);
  color: var(--text-primary);
  margin: 0;
}

.panel-body {
  padding: var(--padding-panel);          /* 14px 16px */
}

.panel-footer {
  padding: var(--space-3) var(--space-4);
  border-top: 1px solid var(--border-light);
}
```

---

### 2.4. Form Controls (Input, Select, Textarea)

```css
/* ✅ ĐÚNG - Input fields */
input, select, textarea {
  appearance: none;
  width: 100%;
  padding: var(--padding-input);          /* 9px 12px */
  border: 1px solid var(--border-light);
  border-radius: var(--radius-md);        /* 8px - Nhỏ hơn Button */
  background: var(--surface);
  color: var(--text-primary);
  font-size: var(--text-base);            /* 13px */
  font-family: var(--font-primary);
  transition: border-color 0.15s ease, box-shadow 0.15s ease;
}

input:hover, select:hover, textarea:hover {
  border-color: var(--border-medium);
}

input:focus, select:focus, textarea:focus {
  outline: none;
  border-color: var(--primary);
  box-shadow: 0 0 0 3px var(--primary-lighter);  /* Focus ring */
}

input::placeholder {
  color: var(--text-tertiary);
}

input:disabled, select:disabled, textarea:disabled {
  background: var(--bg-secondary);
  color: var(--text-tertiary);
  cursor: not-allowed;
}

/* Label styling */
label {
  display: block;
  font-size: var(--text-sm);              /* 12px */
  font-weight: var(--font-medium);        /* 500 */
  color: var(--text-secondary);
  margin-bottom: var(--space-1);          /* 4px */
  letter-spacing: var(--tracking-wide);   /* 0.05em */
}

/* Field wrapper */
.field {
  display: flex;
  flex-direction: column;
  gap: var(--space-1);
}

/* Form grid */
.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: var(--gap-form);                   /* 12px */
}

/* Validation states */
input.error, select.error, textarea.error {
  border-color: var(--danger);
}

input.error:focus, select.error:focus, textarea.error:focus {
  box-shadow: 0 0 0 3px var(--danger-light);
}

.error-message {
  font-size: var(--text-xs);
  color: var(--danger);
  margin-top: var(--space-1);
}
```

---

### 2.5. Tables (Bảng dữ liệu)

```css
/* ✅ ĐÚNG - Table styling */
.table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

.table th {
  padding: var(--space-2) var(--space-3); /* 8px 12px */
  border-bottom: 1px solid var(--border-light);
  text-align: left;
  font-size: var(--text-sm);              /* 12px */
  font-weight: var(--font-semibold);      /* 600 */
  color: var(--text-secondary);
  letter-spacing: var(--tracking-wider);  /* 0.08em */
  text-transform: uppercase;
  background: var(--bg-primary);
}

.table td {
  padding: var(--space-3);                /* 12px */
  border-bottom: 1px solid var(--border-light);
  font-size: var(--text-base);            /* 13px */
  color: var(--text-primary);
  vertical-align: middle;
}

.table tr:last-child td {
  border-bottom: none;
}

.table tbody tr:hover {
  background: var(--bg-primary);          /* Subtle hover */
}

/* Table actions column */
.table td:last-child {
  white-space: nowrap;
  text-align: right;
}

/* Striped table */
.table-striped tbody tr:nth-child(even) {
  background: var(--bg-primary);
}

/* Bordered table */
.table-bordered {
  border: 1px solid var(--border-light);
  border-radius: var(--radius-lg);
}

.table-bordered th,
.table-bordered td {
  border-right: 1px solid var(--border-light);
}

.table-bordered th:last-child,
.table-bordered td:last-child {
  border-right: none;
}
```

---

### 2.6. Navigation (Sidebar Menu)

```css
/* ✅ ĐÚNG - Navigation items */
.nav-item {
  display: flex;
  align-items: center;
  gap: var(--space-3);                    /* 12px */
  padding: var(--space-2) var(--space-3); /* 8px 12px */
  border-radius: var(--radius-lg);        /* 10px */
  color: var(--text-primary);
  text-decoration: none;
  font-size: var(--text-base);            /* 13px */
  font-weight: var(--font-medium);        /* 500 */
  transition: all 0.15s ease;
}

.nav-item:hover {
  background: var(--bg-secondary);        /* NO gradient */
}

.nav-item.active {
  background: var(--primary-lighter);     /* Sky 200 */
  color: var(--primary-dark);             /* Sky 800 */
  font-weight: var(--font-semibold);      /* 600 */
}

/* Navigation section header */
.nav-header {
  padding: var(--space-2);
  font-size: var(--text-xs);
  font-weight: var(--font-semibold);
  color: var(--text-secondary);
  letter-spacing: var(--tracking-wider);
  text-transform: uppercase;
  margin-top: var(--space-3);
}

.nav-header:first-child {
  margin-top: 0;
}

/* Navigation divider */
.nav-divider {
  height: 1px;
  background: var(--border-light);
  margin: var(--space-2) 0;
}
```

**❌ SAI - Active state AI style:**
```css
.nav-item-wrong.active {
  background: linear-gradient(135deg, #e0f2fe, #bae6fd);  /* KHÔNG gradient */
  box-shadow: inset 0 2px 4px rgba(0,0,0,0.1);          /* KHÔNG shadow */
}
```

---

### 2.7. Tabs (Tab navigation)

```css
/* ✅ ĐÚNG - Tab buttons */
.tab-head {
  display: flex;
  gap: var(--space-1);                    /* 4px */
  border-bottom: 1px solid var(--border-light);
  margin-bottom: var(--space-3);          /* 12px */
}

.tab-btn {
  appearance: none;
  border: none;
  background: transparent;
  padding: var(--space-3);                /* 12px */
  border-radius: var(--radius-lg) var(--radius-lg) 0 0;  /* Rounded top only */
  font-size: var(--text-base);            /* 13px */
  font-weight: var(--font-semibold);      /* 600 */
  color: var(--text-secondary);
  cursor: pointer;
  border-bottom: 2px solid transparent;   /* Indicator space */
  transition: all 0.2s ease;
}

.tab-btn:hover {
  background: var(--bg-primary);
  color: var(--text-primary);
}

.tab-btn.active {
  color: var(--primary-dark);
  background: var(--primary-lighter);
  border-bottom-color: var(--primary);    /* Active indicator */
}

/* Tab content */
.tab-content {
  display: none;
}

.tab-content.active {
  display: block;
}
```

---

### 2.8. Modals (Dialog)

```css
/* ✅ ĐÚNG - Modal styling */
dialog {
  border: none;
  border-radius: var(--radius-2xl);       /* 14px */
  padding: 0;
  width: min(900px, 96vw);
  box-shadow: var(--shadow-modal);        /* Shadow OK cho floating */
}

dialog::backdrop {
  background: rgba(15, 23, 42, 0.4);      /* Slate 900 with opacity */
}

.modal-head {
  padding: var(--space-3) var(--space-4); /* 12px 16px */
  border-bottom: 1px solid var(--border-light);
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.modal-title {
  font-size: var(--text-xl);
  font-weight: var(--font-semibold);
  color: var(--text-primary);
  margin: 0;
}

.modal-body {
  padding: var(--space-4);                /* 16px */
}

.modal-foot {
  padding: var(--space-3) var(--space-4); /* 12px 16px */
  border-top: 1px solid var(--border-light);
  display: flex;
  gap: var(--space-2);                    /* 8px */
  justify-content: flex-end;
}
```

---

## 🚫 3. QUY TẮC CẤM - TUYỆT ĐỐI KHÔNG ĐƯỢC VI PHẠM

### 3.1. CẤM EMOJI ICONS

**❌ SAI - Emoji icons:**
```html
<a href="#">📊 Dashboard</a>
<a href="#">📚 Quy trình</a>
<a href="#">🧾 Biểu mẫu</a>
<a href="#">✅ Phê duyệt</a>
<a href="#">🕒 Nhật ký</a>
<a href="#">⚙️ Cài đặt</a>
```

**✅ ĐÚNG - SVG icons hoặc icon fonts:**
```html
<!-- Lucide Icons -->
<a href="#"><i data-lucide="layout-dashboard"></i> Dashboard</a>
<a href="#"><i data-lucide="book-open"></i> Quy trình</a>
<a href="#"><i data-lucide="file-text"></i> Biểu mẫu</a>

<!-- Font Awesome -->
<a href="#"><i class="fa-solid fa-chart-line"></i> Dashboard</a>
<a href="#"><i class="fa-solid fa-book"></i> Quy trình</a>
<a href="#"><i class="fa-solid fa-file-lines"></i> Biểu mẫu</a>

<!-- SVG inline -->
<a href="#">
  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
    <path d="M3 3v18h18"/>
    <path d="M18 17V9"/>
    <path d="M13 17V5"/>
    <path d="M8 17v-3"/>
  </svg>
  Dashboard
</a>
```

**Icon Libraries khuyến nghị:**

| Library | URL | Ưu điểm |
|---------|-----|---------|
| **Lucide Icons** | https://lucide.dev | Modern, lightweight, 1000+ icons |
| **Heroicons** | https://heroicons.com | Tailwind team design, clean |
| **Font Awesome 6** | https://fontawesome.com | Popular, 30,000+ icons |
| **Tabler Icons** | https://tabler-icons.io | Consistent, 4,600+ icons |
| **Feather Icons** | https://feathericons.com | Simple, minimal, beautiful |

**Setup Lucide Icons (khuyến nghị):**
```html
<!-- CDN -->
<script src="https://unpkg.com/lucide@latest"></script>
<script>
  lucide.createIcons();
</script>

<!-- Usage -->
<i data-lucide="layout-dashboard"></i>
<i data-lucide="book-open"></i>
<i data-lucide="file-text"></i>
```

---

### 3.2. CẤM BOX-SHADOW THƯỜNG (trừ Modal/Dropdown)

**❌ SAI:**
```css
.card { box-shadow: 0 10px 25px rgba(2,6,23,.06); }
.panel { box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
.button { box-shadow: 0 2px 8px rgba(14, 165, 233, 0.3); }
.nav-item { box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
```

**✅ ĐÚNG:**
```css
.card { border: 1px solid var(--border-light); }
.panel { border: 1px solid var(--border-light); }
.button { border: 1px solid var(--border-light); }
.nav-item { background: var(--bg-secondary); }

/* ✅ NGOẠI LỆ: Modal/Dropdown được dùng shadow */
dialog { box-shadow: 0 20px 40px rgba(15, 23, 42, 0.12); }
.dropdown-menu { box-shadow: 0 10px 20px rgba(15, 23, 42, 0.08); }
.popover { box-shadow: 0 10px 20px rgba(15, 23, 42, 0.08); }
.tooltip { box-shadow: 0 4px 12px rgba(15, 23, 42, 0.1); }
```

**Lý do:**
- Box-shadow làm UI trông "AI-generated"
- Border nhạt cho cảm giác clean, modern hơn
- Chỉ floating elements (modal, dropdown) mới cần shadow để tách biệt khỏi background

---

### 3.3. CẤM GRADIENT (trừ logo/illustration)

**❌ SAI:**
```css
.button-primary {
  background: linear-gradient(135deg, #0ea5e9, #60a5fa);
}
.header {
  background: linear-gradient(180deg, #ffffff, #f8fafc);
}
.card {
  background: linear-gradient(45deg, #f8fafc, #ffffff);
}
.panel {
  background: linear-gradient(to right, #e0f2fe, #bae6fd);
}
```

**✅ ĐÚNG:**
```css
.button-primary {
  background: var(--primary);  /* Solid color only */
}
.header {
  background: var(--surface);  /* Solid color only */
}
.card {
  background: var(--surface);
}
.panel {
  background: var(--surface);
}

/* ✅ NGOẠI LỆ: Logo, brand icon, illustrations */
.logo-icon {
  background: linear-gradient(135deg, #0369a1, #0284c7);  /* OK cho logo */
}
.hero-illustration::before {
  background: linear-gradient(180deg, #bae6fd, transparent);  /* OK cho decoration */
}
```

**Lý do:**
- Gradient là dấu hiệu AI điển hình
- Solid colors cho cảm giác professional, trustworthy
- Gradient chỉ phù hợp cho branding/illustration

---

### 3.4. CẤM GLASSMORPHISM

**❌ SAI:**
```css
.card {
  background: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);
}

.modal {
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(20px);
}

.sidebar {
  background: rgba(248, 250, 252, 0.9);
  backdrop-filter: saturate(180%) blur(10px);
}
```

**✅ ĐÚNG:**
```css
.card {
  background: var(--surface);  /* Solid white */
}

.modal {
  background: var(--surface);  /* Solid white */
}

.sidebar {
  background: var(--surface);  /* Solid white */
}
```

**Lý do:**
- Glassmorphism đã lỗi thời (peak 2021-2022)
- Làm text khó đọc, accessibility kém
- Performance impact (blur filter tốn tài nguyên)

---

### 3.5. CẤM BORDER-RADIUS ĐỒNG NHẤT

**❌ SAI - Tất cả đều 12px:**
```css
.panel { border-radius: 12px; }
.card { border-radius: 12px; }
.button { border-radius: 12px; }
.input { border-radius: 12px; }
.chip { border-radius: 12px; }
```

**✅ ĐÚNG - Hierarchy: Cha > Con:**
```css
.panel { border-radius: 14px; }    /* Lớn nhất - Container */
.card { border-radius: 12px; }     /* Vừa - Child of Panel */
.button { border-radius: 10px; }   /* Nhỏ - Interactive element */
.input { border-radius: 8px; }     /* Nhỏ hơn - Form control */
.chip { border-radius: 6px; }      /* Nhỏ nhất - Tiny element */
```

**Quy tắc hierarchy:**
1. **Panel/Modal** (14px) - Containers lớn nhất
2. **Card** (12px) - Sub-containers
3. **Button/Nav items** (10px) - Interactive elements
4. **Input/Select** (8px) - Form controls
5. **Chip/Badge** (6px) - Smallest elements

**Visual harmony:**
```
┌─────────────────────────────┐ Panel (14px)
│  ┌───────────────────────┐  │
│  │ Card (12px)           │  │
│  │  ┌──────┐ ┌────────┐ │  │
│  │  │Button│ │Input   │ │  │
│  │  │(10px)│ │(8px)   │ │  │
│  │  └──────┘ └────────┘ │  │
│  │  [Chip] (6px)        │  │
│  └───────────────────────┘  │
└─────────────────────────────┘
```

---

## ⚡ 4. ANIMATION & TRANSITIONS

### 4.1. Transition Standards

```css
/* Timing chuẩn */
:root {
  --transition-fast: 0.15s ease-in-out;     /* Button, input - Quick feedback */
  --transition-base: 0.2s ease-in-out;      /* Card, navigation - Standard */
  --transition-slow: 0.3s ease-in-out;      /* Modal, drawer - Noticeable */

  --ease-smooth: cubic-bezier(0.4, 0, 0.2, 1);          /* Default smooth */
  --ease-bounce: cubic-bezier(0.68, -0.55, 0.265, 1.55); /* Playful bounce */
  --ease-sharp: cubic-bezier(0.4, 0, 0.6, 1);           /* Sharp exit */
}

/* ✅ ĐÚNG - Smooth transitions */
.btn {
  transition: all var(--transition-fast);
}

.card {
  transition: border-color var(--transition-base);
}

.modal {
  transition: opacity var(--transition-slow);
}

/* Specific properties transition */
.input {
  transition:
    border-color var(--transition-fast),
    box-shadow var(--transition-fast);
}
```

**Lý do dùng easing functions:**
- `ease-in-out`: Smooth, natural (default)
- `ease-smooth` (cubic-bezier): Material Design standard
- `ease-bounce`: Playful, attention-grabbing (dùng tiết kiệm)

---

### 4.2. Micro-interactions

```css
/* Hover lift effect cho cards */
.card:hover {
  transform: translateY(-2px);
  transition: transform 0.2s ease;
}

/* Button press effect */
.btn:active {
  transform: scale(0.98);
}

/* Focus ring - Accessibility */
input:focus {
  box-shadow: 0 0 0 3px var(--primary-lighter);
  transition: box-shadow 0.15s ease;
}

button:focus-visible {
  outline: 2px solid var(--primary);
  outline-offset: 2px;
}

/* Loading spinner */
.spinner {
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

/* Fade in animation */
.fade-in {
  animation: fadeIn 0.3s ease-in-out;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

/* Slide in from bottom */
.slide-up {
  animation: slideUp 0.3s ease-out;
}

@keyframes slideUp {
  from {
    transform: translateY(20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}
```

**Quy tắc animation:**
- Luôn có `transition` cho interactive elements
- Dùng `transform` thay vì `top/left` (performance tốt hơn)
- Animation duration: 150ms (fast), 200ms (base), 300ms (slow)
- Tránh animation quá lố, chỉ dùng khi cần thiết

---

### 4.3. Loading States

```css
/* Skeleton loader */
.skeleton {
  background: linear-gradient(
    90deg,
    var(--bg-secondary) 0%,
    var(--bg-primary) 50%,
    var(--bg-secondary) 100%
  );
  background-size: 200% 100%;
  animation: skeleton 1.5s ease-in-out infinite;
}

@keyframes skeleton {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}

/* Pulse animation */
.pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

/* Progress bar */
.progress-bar {
  transition: width 0.3s ease;
}
```

---

## 📱 5. RESPONSIVE DESIGN

### 5.1. Breakpoints

```css
:root {
  --screen-sm: 640px;    /* Mobile landscape */
  --screen-md: 768px;    /* Tablet portrait */
  --screen-lg: 1024px;   /* Tablet landscape / Small desktop */
  --screen-xl: 1280px;   /* Desktop */
  --screen-2xl: 1536px;  /* Large desktop */
}

/* Mobile First approach */
@media (max-width: 640px) {
  /* Mobile styles */
  .container { padding: var(--space-3); }
  .grid { grid-template-columns: 1fr; }
  .btn { width: 100%; }
}

@media (max-width: 768px) {
  /* Tablet styles */
  .layout {
    grid-template-columns: 1fr;  /* Single column */
  }

  aside {
    display: none;  /* Hide sidebar */
  }

  .panel-head {
    flex-direction: column;
    gap: var(--space-2);
  }

  .form-grid {
    grid-template-columns: 1fr;  /* Stack form fields */
  }

  .table {
    font-size: var(--text-sm);  /* Smaller text */
  }
}

@media (min-width: 1024px) {
  /* Desktop enhancements */
  .container {
    max-width: 1280px;
  }
}
```

---

### 5.2. Mobile-Specific Rules

```css
/* Touch-friendly tap targets (minimum 44x44px) */
@media (max-width: 768px) {
  .btn {
    min-height: 44px;
    padding: 12px 16px;
  }

  .nav-item {
    min-height: 44px;
  }

  /* Larger touch areas */
  input, select, textarea {
    font-size: 16px;  /* Prevent iOS zoom on focus */
    padding: 12px;
  }

  /* Stack toolbars */
  .toolbar {
    flex-direction: column;
    width: 100%;
  }

  .toolbar .btn {
    width: 100%;
  }

  /* Full-width modals on mobile */
  dialog {
    width: 100vw;
    height: 100vh;
    max-width: 100vw;
    border-radius: 0;
  }
}
```

---

## 🎯 6. STATE STYLING

### 6.1. Status Badges

```css
/* Badge/Chip với màu semantic */
.badge {
  display: inline-flex;
  align-items: center;
  gap: var(--space-1);
  padding: 2px 10px;
  border-radius: var(--radius-full);  /* Pill shape */
  font-size: var(--text-xs);          /* 11px */
  font-weight: var(--font-semibold);  /* 600 */
  letter-spacing: var(--tracking-wide);
  line-height: 1.5;
}

/* State: Draft */
.badge-draft {
  background: var(--bg-secondary);
  color: var(--text-secondary);
  border: 1px solid var(--border-light);
}

/* State: In Progress */
.badge-progress {
  background: var(--info-light);
  color: var(--info);
  border: 1px solid var(--info);
}

/* State: Submitted */
.badge-submitted {
  background: var(--info-light);
  color: var(--info);
  border: 1px solid var(--info);
}

/* State: Approved */
.badge-approved {
  background: var(--success-light);
  color: var(--success);
  border: 1px solid var(--success);
}

/* State: Rejected */
.badge-rejected {
  background: var(--danger-light);
  color: var(--danger);
  border: 1px solid var(--danger);
}

/* State: Archived */
.badge-archived {
  background: var(--bg-secondary);
  color: var(--text-tertiary);
  border: 1px solid var(--border-light);
}

/* Badge with dot indicator */
.badge-dot::before {
  content: '';
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: currentColor;
}
```

---

### 6.2. Alert/Notification Components

```css
.alert {
  padding: var(--space-3) var(--space-4);
  border-radius: var(--radius-lg);
  border-left: 3px solid;
  display: flex;
  align-items: flex-start;
  gap: var(--space-3);
}

.alert-success {
  background: var(--success-light);
  border-color: var(--success);
  color: var(--success);
}

.alert-warning {
  background: var(--warning-light);
  border-color: var(--warning);
  color: var(--warning);
}

.alert-danger {
  background: var(--danger-light);
  border-color: var(--danger);
  color: var(--danger);
}

.alert-info {
  background: var(--info-light);
  border-color: var(--info);
  color: var(--info);
}

.alert-title {
  font-weight: var(--font-semibold);
  margin-bottom: var(--space-1);
}
```

---

## 📋 7. CHECKLIST CUỐI CÙNG

Trước khi commit code UI, kiểm tra:

### ✅ Design Tokens
- [ ] **Colors**: Dùng CSS variables (`var(--primary)`) thay vì hard-code
- [ ] **Spacing**: Dùng `var(--space-*)` thay vì px values
- [ ] **Font sizes**: Dùng `var(--text-*)` scale
- [ ] **Border radius**: Tuân thủ hierarchy (cha > con)

### ✅ Typography
- [ ] **Font**: Có import Be Vietnam Pro / Inter / Montserrat?
- [ ] **Font weights**: Dùng `var(--font-semibold)` thay vì `font-weight: 600`
- [ ] **Line height**: Appropriate cho readability (1.5 cho body text)
- [ ] **Letter spacing**: Dùng cho uppercase text (table headers, labels)

### ✅ Components
- [ ] **Icons**: Đã thay emoji bằng SVG/icon font (Lucide/Heroicons)?
- [ ] **Shadow**: Chỉ dùng border (trừ modal/dropdown)?
- [ ] **Gradient**: Không dùng gradient (trừ logo)?
- [ ] **Glassmorphism**: Không dùng blur/backdrop-filter?
- [ ] **Buttons**: Solid colors, no gradient, proper hover states?
- [ ] **Forms**: Focus rings, validation states, proper padding?

### ✅ Interactions
- [ ] **Transitions**: Smooth và consistent (0.15s-0.3s)?
- [ ] **Hover states**: Tất cả interactive elements có hover?
- [ ] **Active states**: Button press effects?
- [ ] **Focus states**: Keyboard navigation support (focus-visible)?

### ✅ Responsive
- [ ] **Mobile-friendly**: Touch targets >= 44px?
- [ ] **Breakpoints**: Layout responsive ở tất cả screen sizes?
- [ ] **Font sizes**: iOS không zoom in khi focus input (min 16px)?
- [ ] **Overflow**: Không bị horizontal scroll trên mobile?

### ✅ Accessibility
- [ ] **Color contrast**: Text có contrast ratio >= 4.5:1?
- [ ] **Focus indicators**: Visible cho keyboard navigation?
- [ ] **Alt text**: Images có alt descriptions?
- [ ] **ARIA labels**: Interactive elements có proper labels?

### ✅ Performance
- [ ] **Animations**: Dùng `transform` thay vì `top/left`?
- [ ] **Images**: Có optimize và lazy loading?
- [ ] **CSS**: Minimize, no unused styles?

---

## 🎓 TÀI LIỆU THAM KHẢO

### Design Systems
- **Tailwind CSS**: https://tailwindcss.com/docs (Color palette inspiration)
- **Shadcn/ui**: https://ui.shadcn.com (Component patterns)
- **Radix UI**: https://www.radix-ui.com (Accessible primitives)
- **Material Design 3**: https://m3.material.io (Design principles)

### Icon Libraries
- **Lucide Icons**: https://lucide.dev
- **Heroicons**: https://heroicons.com
- **Font Awesome**: https://fontawesome.com
- **Tabler Icons**: https://tabler-icons.io

### Fonts
- **Google Fonts**: https://fonts.google.com
  - Inter: https://fonts.google.com/specimen/Inter
  - Be Vietnam Pro: https://fonts.google.com/specimen/Be+Vietnam+Pro
  - Montserrat: https://fonts.google.com/specimen/Montserrat

### Accessibility
- **WCAG Guidelines**: https://www.w3.org/WAI/WCAG21/quickref/
- **WebAIM Contrast Checker**: https://webaim.org/resources/contrastchecker/

---

**📌 LƯU Ý QUAN TRỌNG:**

1. **100% tuân thủ**: Mọi UI component phải follow rules này
2. **Code review**: Kiểm tra checklist trước mỗi commit
3. **Consistency**: Nhất quán trong toàn bộ hệ thống
4. **No exceptions**: Không được tự ý thay đổi design tokens

**Khi có thắc mắc**: Refer back to this document!
