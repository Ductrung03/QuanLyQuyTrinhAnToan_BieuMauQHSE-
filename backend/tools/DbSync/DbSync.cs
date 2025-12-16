using Microsoft.Data.SqlClient;

var connectionString = "Server=localhost,1433;Database=SSMS_KhaiThacTau;User Id=sa;Password=MyStrong@Password123;TrustServerCertificate=True;MultipleActiveResultSets=true";

Console.WriteLine("=== Database Schema Inspector ===");
Console.WriteLine();

try
{
    using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();
    Console.WriteLine("✓ Connected to database\n");

    var tables = new[] { "OpsProcedure", "OpsProcedureDocument", "OpsSubmission", "OpsTemplate", "OpsSubmissionFile", "OpsSubmissionRecipient", "OpsApproval" };

    foreach (var table in tables)
    {
        var query = $@"
            SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = '{table}'
            ORDER BY ORDINAL_POSITION";
        
        using var cmd = new SqlCommand(query, connection);
        using var reader = await cmd.ExecuteReaderAsync();
        
        Console.WriteLine($"=== {table} ===");
        while (await reader.ReadAsync())
        {
            var name = reader["COLUMN_NAME"].ToString();
            var type = reader["DATA_TYPE"].ToString();
            var nullable = reader["IS_NULLABLE"].ToString();
            var maxLen = reader["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value ? "" : $"({reader["CHARACTER_MAXIMUM_LENGTH"]})";
            Console.WriteLine($"  {name,-25} {type}{maxLen,-15} {(nullable == "YES" ? "NULL" : "NOT NULL")}");
        }
        await reader.CloseAsync();
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Error: {ex.Message}");
    Environment.Exit(1);
}
