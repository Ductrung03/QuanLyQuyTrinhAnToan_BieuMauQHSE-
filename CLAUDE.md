# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Project Name**: SSMS – Quản lý Khai thác tàu (Ship Safety Management System - Vessel Operations Management)
**Version**: v3.4.2
**Type**: Document/Form Management System for Maritime Operations
**Tech Stack**: C# .NET (planned), MS SQL Server, HTML/CSS/JavaScript (prototype)

This is a maritime operations management system for managing operational procedures, forms/checklists, submissions, and approvals with complete audit trails for Petrolimex vessel operations.

## Current Project Status

- **Database schema**: Complete (SSMS_Ops_DBV2.sql)
- **HTML/JS prototype**: Complete with embedded demo data (v3.4.2)
- **C# .NET backend**: Not yet implemented (in planning phase)
- **Documentation**: Available in Document/ folder

## Database Architecture

**Database Name**: `SSMS_KhaiThacTau`
**Database Type**: MS SQL Server

### Core Database Entities

**1. Common Master Data**
- `AppUser`: System users (UserId, UserName, Email, Phone, LoginName, IsActive)
- `Unit`: Organization units including Ships (Tàu Petrolimex 15, 16, 18, 21) and Departments (Phòng Khai thác, Phòng Kỹ thuật, QHSE, etc.)

**2. Procedure & Document Management**
- `OpsProcedure`: Operational procedures (OPS-01 through OPS-07) with Code, Version, State (Draft/Submitted/Approved)
  - OPS-01: Bill of Lading (Vận Đơn)
  - OPS-02: Charter Party (Hợp Đồng Vận Tải)
  - OPS-03: Cargo Operation Procedure
  - OPS-04: VEC Systems
  - OPS-05: Tank Cleaning
  - OPS-06: Inert Gas System
  - OPS-07: Ballast Water Management
- `OpsProcedureDocument`: Attachments (DOCX/PDF) for procedures
- `OpsProcedureChange`: Change log with action tracking (Create, Edit, Approve)
- `OpsTemplate`: Forms and Checklists linked to procedures

**3. Form Submission & Approval Workflow**
- `OpsSubmission`: Submission records with SUB-XXXXX codes and states
- `OpsSubmissionRecipient`: Multi-recipient routing (many-to-many)
- `OpsSubmissionFile`: Filled form attachments
- `OpsApproval`: Approval queue with decision tracking

**4. Audit & Logging**
- `OpsAuditLog`: System audit trail with timestamps, user actions, targets

### Database Relationships

```
AppUser (1) → (n) OpsProcedure (owner/author/approver)
        ├→ (n) OpsSubmission (sender)
        └→ (n) OpsApproval (approver)

OpsProcedure (1) → (n) OpsProcedureDocument
             ├→ (n) OpsProcedureChange
             ├→ (n) OpsTemplate
             └→ (n) OpsSubmission

Unit (1) → (n) OpsSubmission (sending/receiving unit)

OpsTemplate (1) → (n) OpsSubmission

OpsSubmission (1) → (n) OpsApproval
```

## Directory Structure

```
QuanLyQuyTrinhAnToan_BieuMauQHSE/
├── Database/              # SQL Server database schema
│   └── SSMS_Ops_DBV2.sql # Complete database definition with demo data
├── Document/              # Project documentation
│   └── MÔ TẢ GIẢI PHÁP PHẦN MỀM.docx
├── Template/              # UI prototype
│   └── SSMS_QuanLyKhaiThacTau_v3_4_2_unit_filtered.html
├── .vscode/              # VS Code settings
├── .specify/             # Specification framework (gitignored)
├── .claude/              # Claude AI commands (gitignored)
└── .gemini/              # Gemini AI commands (gitignored)
```

## Development Commands

### Database Setup
```bash
# Connect to SQL Server and run the schema
sqlcmd -S localhost -U sa -P <password> -i Database/SSMS_Ops_DBV2.sql

# Or using Management Studio
# Open SSMS → Open File → Database/SSMS_Ops_DBV2.sql → Execute
```

### Viewing the HTML Prototype
```bash
# Using VS Code Live Server (port 5501 configured)
# Right-click Template/SSMS_QuanLyKhaiThacTau_v3_4_2_unit_filtered.html → Open with Live Server
```

### Git Commands
```bash
# View status
git status

# Commit changes
git add .
git commit -m "Your message"

# Push changes
git push origin main
```

## Naming Conventions

- **Procedures**: `OPS-XX` format (e.g., OPS-01, OPS-02)
- **Submissions**: `SUB-XXXXX` alphanumeric codes
- **Approvals**: `APR-SUB-XXXXX` format
- **Templates**: Alphanumeric IDs (T1, T2, T6ihr, Ts3ks, etc.)
- **Form Numbers**: `FM-OPS-XX` format (e.g., FM-OPS-01)
- **Checklist Numbers**: `CL-OPS-XX` format (e.g., CL-OPS-01)
- **SOF Numbers**: `SOF XX-XX-XX` format (Standard Operating Form)

## State Management

- **States**: `Draft` → `Submitted` → `Approved` / `Rejected`
- **Versions**: String format (e.g., "1.0", "0.8")
- **Temporal tracking**: CreatedDate, ReleasedDate, Timestamps

## Multi-language Support

- **Primary language**: Vietnamese (Tiếng Việt)
- **Secondary language**: English (for international compliance)
- All major fields (procedure names, descriptions) should be bilingual

## UI/UX Design System

**⚠️ QUAN TRỌNG**: Tất cả quy tắc UI/UX chi tiết đã được tách ra file riêng.

👉 **ĐỌC BẮT BUỘC**: [docs/design-system/UI-UX-RULES.md](docs/design-system/UI-UX-RULES.md)

### Tóm tắt nhanh - QUY TẮC CẤM

1. ❌ **CẤM EMOJI ICONS** → Dùng Lucide Icons / Heroicons / Font Awesome
2. ❌ **CẤM BOX-SHADOW** (trừ modal/dropdown) → Dùng border nhạt
3. ❌ **CẤM GRADIENT** (trừ logo/illustration) → Dùng solid colors
4. ❌ **CẤM GLASSMORPHISM** → Không dùng blur/backdrop-filter
5. ❌ **CẤM BORDER-RADIUS ĐỒNG NHẤT** → Hierarchy: Panel (14px) > Card (12px) > Button (10px) > Input (8px) > Chip (6px)

### Design Tokens Quick Reference

```css
/* Colors */
--primary: #0369a1;        /* Sky 700 - KHÔNG dùng #0ea5e9 */
--success: #059669;        /* Emerald 600 */
--danger: #dc2626;         /* Red 600 */
--border-light: #e2e8f0;   /* Slate 200 */

/* Typography */
--font-primary: 'Inter', 'Be Vietnam Pro', sans-serif;
--text-base: 13px;
--text-md: 14px;

/* Spacing (4px base) */
--space-2: 8px;
--space-3: 12px;
--space-4: 16px;

/* Border Radius Hierarchy */
--radius-md: 8px;      /* Input */
--radius-lg: 10px;     /* Button */
--radius-xl: 12px;     /* Card */
--radius-2xl: 14px;    /* Panel */
```

### Checklist trước khi commit UI

- [ ] Font: Dùng Inter / Be Vietnam Pro?
- [ ] Icons: Đã thay emoji bằng icon library?
- [ ] Shadow: Chỉ dùng border (trừ modal/dropdown)?
- [ ] Gradient: Không dùng gradient?
- [ ] Border-radius: Hierarchy đúng (cha > con)?
- [ ] Transitions: Smooth (0.15s-0.3s)?
- [ ] Responsive: Mobile-friendly?

**Chi tiết đầy đủ**: [docs/design-system/UI-UX-RULES.md](docs/design-system/UI-UX-RULES.md)

---

## Documentation Structure

Toàn bộ documentation được tổ chức trong folder `docs/`:

```
docs/
├── README.md                      # Documentation index
├── design-system/
│   ├── UI-UX-RULES.md            # ⭐ QUY TẮC UI/UX TUYỆT ĐỐI
│   └── COMPONENTS.md             # Component library (coming soon)
├── development/
│   ├── GETTING-STARTED.md        # Setup guide (coming soon)
│   ├── CODING-STANDARDS.md       # C# standards (coming soon)
│   └── API-DOCUMENTATION.md      # API docs (coming soon)
└── database/
    ├── SCHEMA.md                 # Schema details (coming soon)
    └── MIGRATIONS.md             # Migration guide (coming soon)
```


## Code Quality Requirements

### Clean Code Principles
- Code phải clean, dễ đọc, dễ bảo trì
- Tối ưu performance
- Tuân thủ SOLID principles
- Sử dụng meaningful names cho variables, methods, classes

### Project Structure
- Cấu trúc project phải clean, dễ mở rộng
- Separation of Concerns (SoC)
- Layered Architecture: Presentation → Business Logic → Data Access
- Chuẩn thực chiến (production-ready)

### C# .NET Best Practices (khi implement)
- Sử dụng Dependency Injection
- Async/Await cho I/O operations
- Entity Framework Core hoặc Dapper cho data access
- Repository Pattern & Unit of Work
- DTO (Data Transfer Objects) cho API responses
- Exception handling đầy đủ
- Logging (Serilog hoặc NLog)

## Future Backend Implementation Plan

Khi phát triển C# .NET backend:

### Technology Stack (Khuyến nghị)
- **Framework**: ASP.NET Core 8.0+ (Web API)
- **ORM**: Entity Framework Core hoặc Dapper
- **Authentication**: JWT Bearer Authentication
- **Documentation**: Swagger/OpenAPI
- **Logging**: Serilog
- **Testing**: xUnit hoặc NUnit

### Folder Structure (Khuyến nghị)
```
SSMS.API/
├── Controllers/         # API endpoints
├── Models/
│   ├── Entities/       # Database entities
│   ├── DTOs/           # Data transfer objects
│   └── ViewModels/     # View models
├── Services/           # Business logic
│   ├── Interfaces/
│   └── Implementations/
├── Repositories/       # Data access
│   ├── Interfaces/
│   └── Implementations/
├── Middleware/         # Custom middleware
├── Helpers/            # Utility classes
└── appsettings.json    # Configuration
```

### API Endpoints (Planned)
```
/api/procedures          # CRUD for procedures
/api/templates           # CRUD for templates
/api/submissions         # CRUD for submissions
/api/approvals           # CRUD for approvals
/api/users               # User management
/api/units               # Unit/department management
/api/audit               # Audit log queries
```

## Important Notes

- **Language**: Code comments có thể bằng tiếng Việt hoặc tiếng Anh
- **Database**: Luôn test SQL queries trước khi integrate vào code
- **Security**: Implement proper authentication & authorization
- **Validation**: Validate input ở cả client-side và server-side
- **Error Handling**: Xử lý lỗi đầy đủ với meaningful error messages
- **Audit Trail**: Mọi thay đổi quan trọng phải được log vào OpsAuditLog

## Demo Data

Database script bao gồm demo data:
- 5 Users: Nguyễn Văn A, Trần Thị B, Lê Văn C, Phạm D, Vũ E
- 9 Units: 4 ships + 5 departments
- 7 Operational Procedures (OPS-01 to OPS-07)
- 8 Form/Checklist Templates
- 2 Sample Submissions
- Complete Audit Log

## References

- Database schema: [Database/SSMS_Ops_DBV2.sql](Database/SSMS_Ops_DBV2.sql)
- HTML prototype: [Template/SSMS_QuanLyKhaiThacTau_v3_4_2_unit_filtered.html](Template/SSMS_QuanLyKhaiThacTau_v3_4_2_unit_filtered.html)
- Project documentation: [Document/](Document/)
