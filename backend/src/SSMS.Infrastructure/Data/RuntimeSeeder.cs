using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SSMS.Core.Entities;

namespace SSMS.Infrastructure.Data;

public static class RuntimeSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("RuntimeSeeder");

        var users = await context.AppUsers.ToListAsync();
        if (!users.Any())
        {
            logger.LogWarning("Runtime seeding aborted: no users found.");
            return;
        }

        var existingUnits = await context.Units
            .IgnoreQueryFilters()
            .ToListAsync();
        var visibleUnits = await context.Units.ToListAsync();

        if (!visibleUnits.Any())
        {
            var existingUnitCodes = new HashSet<string>(
                existingUnits.Select(u => u.Code),
                StringComparer.OrdinalIgnoreCase);

            var units = new List<Unit>
            {
                new() { Code = "HQ", Name = "Trụ sở chính", Type = "Headquarters", Description = "Văn phòng trụ sở chính", IsActive = true },
                new() { Code = "SHIP001", Name = "Tàu Hải Phòng 01", Type = "Ship", Description = "Tàu khai thác số 1", IsActive = true },
                new() { Code = "SHIP002", Name = "Tàu Hải Phòng 02", Type = "Ship", Description = "Tàu khai thác số 2", IsActive = true },
                new() { Code = "DEPT-QHSE", Name = "Phòng QHSE", Type = "Department", Description = "Phòng QHSE", IsActive = true },
                new() { Code = "DEPT-OPS", Name = "Phòng Khai thác", Type = "Department", Description = "Phòng Khai thác", IsActive = true }
            };

            var newUnits = units.Where(u => !existingUnitCodes.Contains(u.Code)).ToList();
            if (newUnits.Any())
            {
                context.Units.AddRange(newUnits);
            }

            var reviveUnits = existingUnits.Where(u => existingUnitCodes.Contains(u.Code)).ToList();
            foreach (var unit in reviveUnits)
            {
                unit.IsDeleted = false;
                unit.IsActive = true;
            }
            await context.SaveChangesAsync();
        }

        var existingProcedures = await context.OpsProcedures
            .IgnoreQueryFilters()
            .ToListAsync();
        var targetProcedureCount = 20;
        var missingProcedures = targetProcedureCount - existingProcedures.Count;
        if (missingProcedures <= 0)
        {
            logger.LogInformation("Runtime seeding skipped: procedure count already {Count}.", existingProcedures.Count);
            return;
        }

        var now = DateTime.UtcNow;
        var seedUsers = users.OrderBy(u => u.Id).ToList();
        var ownerId = seedUsers.First().Id;
        var approverId = ownerId;

        var procedures = new List<OpsProcedure>();
        var existingCodes = new HashSet<string>(
            existingProcedures.Select(p => p.Code),
            StringComparer.OrdinalIgnoreCase);

        var maxCodeNumber = existingProcedures
            .Select(p => p.Code)
            .Where(c => c.StartsWith("OPS-", StringComparison.OrdinalIgnoreCase) && c.Length > 4)
            .Select(c => int.TryParse(c.Substring(4), out var num) ? num : 0)
            .DefaultIfEmpty(0)
            .Max();

        var created = 0;
        var offset = 0;
        while (created < missingProcedures)
        {
            offset++;
            var codeNumber = maxCodeNumber + offset;
            var code = $"OPS-{codeNumber:D2}";
            if (existingCodes.Contains(code))
            {
                continue;
            }

            procedures.Add(new OpsProcedure
            {
                Code = code,
                Name = $"Quy trình an toàn {code}",
                Version = "1.0",
                State = codeNumber % 3 == 0 ? "Submitted" : "Approved",
                Description = $"Mô tả quy trình {code}.",
                OwnerUserId = ownerId,
                AuthorUserId = ownerId,
                ApproverUserId = approverId,
                CreatedDate = now.AddDays(-(30 + codeNumber)),
                ReleasedDate = codeNumber % 3 == 0 ? null : now.AddDays(-(10 + codeNumber))
            });

            existingCodes.Add(code);
            created++;
        }

        context.OpsProcedures.AddRange(procedures);
        await context.SaveChangesAsync();

        var templates = new List<OpsTemplate>();
        var templateIndex = await context.OpsTemplates.CountAsync();
        foreach (var procedure in procedures)
        {
            templates.Add(new OpsTemplate
            {
                ProcedureId = procedure.Id,
                TemplateKey = $"T{++templateIndex:D3}",
                TemplateNo = $"FM-{procedure.Code}-01",
                Name = $"Biểu mẫu {procedure.Code} - Kiểm tra",
                TemplateType = "Form",
                State = "Approved",
                IsActive = true
            });

            templates.Add(new OpsTemplate
            {
                ProcedureId = procedure.Id,
                TemplateKey = $"T{++templateIndex:D3}",
                TemplateNo = $"CL-{procedure.Code}-01",
                Name = $"Checklist {procedure.Code} - An toàn",
                TemplateType = "Checklist",
                State = "Draft",
                IsActive = true
            });

            templates.Add(new OpsTemplate
            {
                ProcedureId = procedure.Id,
                TemplateKey = $"T{++templateIndex:D3}",
                TemplateNo = $"FM-{procedure.Code}-02",
                Name = $"Biểu mẫu {procedure.Code} - Báo cáo",
                TemplateType = "Report",
                State = "Draft",
                IsActive = true
            });
        }

        context.OpsTemplates.AddRange(templates);
        await context.SaveChangesAsync();

        var submissions = new List<OpsSubmission>();
        var existingSubmissionsCount = await context.OpsSubmissions.CountAsync();
        var targetSubmissionCount = 40;
        var totalToAdd = Math.Max(0, targetSubmissionCount - existingSubmissionsCount);
        var submissionIndex = 1;
        foreach (var template in templates.Take(totalToAdd))
        {
            var submitterId = seedUsers[(submissionIndex - 1) % seedUsers.Count].Id;
            submissions.Add(new OpsSubmission
            {
                SubmissionCode = $"SUB-{now:yyyyMMdd}-{submissionIndex:D3}",
                ProcedureId = template.ProcedureId,
                TemplateId = template.Id,
                Title = $"Nộp biểu mẫu {template.TemplateNo}",
                Content = "Nội dung mẫu để kiểm thử luồng phê duyệt.",
                SubmittedByUserId = submitterId,
                SubmittedAt = now.AddDays(-submissionIndex),
                Status = submissionIndex % 3 == 0 ? "Approved" : "Submitted"
            });
            submissionIndex++;
        }

        context.OpsSubmissions.AddRange(submissions);
        await context.SaveChangesAsync();

        var approvals = new List<OpsApproval>();
        foreach (var submission in submissions.Where(s => s.Status == "Approved"))
        {
            approvals.Add(new OpsApproval
            {
                SubmissionId = submission.Id,
                ApproverUserId = approverId,
                Action = "Approved",
                Note = "Phê duyệt theo quy trình",
                ActionDate = submission.SubmittedAt.AddHours(4)
            });
        }

        context.OpsApprovals.AddRange(approvals);

        var auditLogs = new List<OpsAuditLog>();
        foreach (var procedure in procedures)
        {
            auditLogs.Add(new OpsAuditLog
            {
                UserId = ownerId,
                UserName = seedUsers.First().Username,
                Action = "Create",
                TargetType = "Procedure",
                TargetId = procedure.Id,
                TargetName = procedure.Code,
                Detail = "Seed data",
                ActionTime = procedure.CreatedAt
            });
        }

        context.OpsAuditLogs.AddRange(auditLogs);
        await context.SaveChangesAsync();

        logger.LogInformation("Runtime seeding completed: +{Procedures} procedures, +{Templates} templates, +{Submissions} submissions.",
            procedures.Count, templates.Count, submissions.Count);
    }
}
