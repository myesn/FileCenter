using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FileCenter.Tools.SchemaMigrations.Migrations.FileCenterDb
{
    public partial class InitialFileCenterDbContextMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    Extensions = table.Column<string>(nullable: true),
                    PhysicalPath = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    Length = table.Column<long>(nullable: false),
                    DownloadTimes = table.Column<long>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UploadTime = table.Column<DateTime>(nullable: false),
                    DeleteTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
