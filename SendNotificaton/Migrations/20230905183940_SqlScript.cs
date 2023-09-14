using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SendNotificaton.Migrations
{
    public partial class SqlScript : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExecutionLog",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcedureName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    LastExecutionTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionLog", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecordId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    UserEmail = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    DataValue = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NotificationFlag = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.Id);
                });
            migrationBuilder.Sql(@"CREATE PROCEDURE dbo.spGetRecentUsersRecords
                    AS
                    BEGIN
                        -- SET NOCOUNT ON added to prevent extra result sets from
                        -- interfering with SELECT statements.
                        SET NOCOUNT ON;
                        DECLARE @lastExecutionTime DATETIME;

                        -- Get last execution time from ExecutionLog table
                        SELECT @lastExecutionTime = MAX(LastExecutionTime) FROM [dbo].[ExecutionLog] WHERE ProcedureName = 'spGetRecentUsersRecords';

                        IF @lastExecutionTime IS NULL 
                            -- If LastExecutionTime is null then fetch all records
                            SELECT * FROM [dbo].[Records]
                        ELSE
                        -- Retrieve recent data based on last execution time
                        SELECT * FROM [dbo].[Records] WHERE CreatedTime > @LastExecutionTime;

                        -- -- Insert the last execution time in ExecutionLog table
                        INSERT INTO [dbo].[ExecutionLog] (ProcedureName, LastExecutionTime) VALUES ('spGetRecentUsersRecords', GETDATE());
                    END
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExecutionLog");

            migrationBuilder.DropTable(
                name: "Records");

            migrationBuilder.Sql("DROP PROCEDURE dbo.spGetRecentUsersRecords");
        }
    }
}
