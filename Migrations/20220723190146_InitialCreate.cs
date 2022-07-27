using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectX.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SightCategory = table.Column<string>(type: "TEXT", nullable: true),
                    EmploymentStatus = table.Column<string>(type: "TEXT", nullable: true),
                    JobTitle = table.Column<string>(type: "TEXT", nullable: true),
                    Company = table.Column<string>(type: "TEXT", nullable: true),
                    Industry = table.Column<string>(type: "TEXT", nullable: true),
                    WorkingSince = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BreakTime = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkRoles = table.Column<string>(type: "TEXT", nullable: true),
                    Strengths = table.Column<string>(type: "TEXT", nullable: true),
                    IsParent = table.Column<string>(type: "TEXT", nullable: true),
                    Biog = table.Column<string>(type: "TEXT", nullable: true),
                    Joined = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ImageSrc = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discussion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Image = table.Column<string>(type: "TEXT", nullable: true),
                    CreatorId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discussion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportTicket",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Creator = table.Column<string>(type: "TEXT", nullable: true),
                    IsResolved = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Details = table.Column<string>(type: "TEXT", nullable: true),
                    DiscussionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ReplyId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TicketId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    LoggedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscussionLike",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: true),
                    DiscussionId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionLike", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionLike_Discussion_DiscussionId",
                        column: x => x.DiscussionId,
                        principalTable: "Discussion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reply",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Respondent = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DiscussionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SupportTicketId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reply_Discussion_DiscussionId",
                        column: x => x.DiscussionId,
                        principalTable: "Discussion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reply_SupportTicket_SupportTicketId",
                        column: x => x.SupportTicketId,
                        principalTable: "SupportTicket",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "DateCreated", "Description", "ImageSrc", "Name" },
                values: new object[] { new Guid("2e5996ee-4cff-4271-8b80-47cb0488bd89"), new DateTime(2022, 7, 23, 19, 1, 46, 841, DateTimeKind.Utc).AddTicks(8173), "For parents to discuss maternal/paternal stuff.", "parents-child.svg", "Parents' Zone" });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "DateCreated", "Description", "ImageSrc", "Name" },
                values: new object[] { new Guid("2ffd0c88-f356-4f48-8850-a436aeb10d36"), new DateTime(2022, 7, 23, 19, 1, 46, 841, DateTimeKind.Utc).AddTicks(8171), "For job application advice.", "hunting.svg", "Job-hunting" });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "DateCreated", "Description", "ImageSrc", "Name" },
                values: new object[] { new Guid("4eb33dc4-79e9-4308-94e6-31e9a150e283"), new DateTime(2022, 7, 23, 19, 1, 46, 841, DateTimeKind.Utc).AddTicks(8175), "For students to discuss matters pertaining to academic life.", "students.svg", "College Library" });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "DateCreated", "Description", "ImageSrc", "Name" },
                values: new object[] { new Guid("7d96fc32-9ae5-4432-9a04-f4e57aaf96ed"), new DateTime(2022, 7, 23, 19, 1, 46, 841, DateTimeKind.Utc).AddTicks(8172), "A forum for discussing the UK Government scheme of the same name.", "universal-access-circle.svg", "Access to Work" });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "DateCreated", "Description", "ImageSrc", "Name" },
                values: new object[] { new Guid("f522dea1-ad1b-42cc-bd3b-fc90fd6d0141"), new DateTime(2022, 7, 23, 19, 1, 46, 841, DateTimeKind.Utc).AddTicks(8164), "Newcomers should divulge their innermost secrets in this forum.", "introductions.svg", "Introductions" });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "DateCreated", "Description", "ImageSrc", "Name" },
                values: new object[] { new Guid("fbcb1a26-ce25-483d-84ad-59d1f003c3e2"), new DateTime(2022, 7, 23, 19, 1, 46, 841, DateTimeKind.Utc).AddTicks(8169), "For professionals to discuss professional stuff.", "office.svg", "The Office" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionLike_DiscussionId",
                table: "DiscussionLike",
                column: "DiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_Reply_DiscussionId",
                table: "Reply",
                column: "DiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_Reply_SupportTicketId",
                table: "Reply",
                column: "SupportTicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "DiscussionLike");

            migrationBuilder.DropTable(
                name: "Reply");

            migrationBuilder.DropTable(
                name: "UserAction");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Discussion");

            migrationBuilder.DropTable(
                name: "SupportTicket");
        }
    }
}
