using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Maba.Migrations
{
    public partial class Start : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IDNumber = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(maxLength: 30, nullable: false),
                    Hours = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 150, nullable: true),
                    Grade = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    LastLogin = table.Column<DateTime>(nullable: false),
                    Role = table.Column<string>(nullable: true),
                    School = table.Column<string>(nullable: true),
                    Start = table.Column<DateTime>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });
        }

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Users");

			migrationBuilder.CreateTable(
				name: "Student",
				columns: table => new
				{
					ID = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					Description = table.Column<string>(nullable: true),
					FullName = table.Column<string>(nullable: true),
					Hours = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Student", x => x.ID);
				});
		}
	}
}
