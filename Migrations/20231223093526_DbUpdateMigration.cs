using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace asp_net_web_api.Migrations
{
    /// <inheritdoc />
    public partial class DbUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Sku = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    PermissionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, null, null, "Read" },
                    { 2, null, null, "Write" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IsAvailable", "ModifiedAt", "Name", "Price", "Sku" },
                values: new object[,]
                {
                    { 1, 1, null, "", true, null, "Grunge Skater Jeans", 68m, "AWMGSJ" },
                    { 2, 1, null, "", true, null, "Polo Shirt", 35m, "AWMPS" },
                    { 3, 1, null, "", true, null, "Skater Graphic T-Shirt", 33m, "AWMSGT" },
                    { 4, 1, null, "", true, null, "Slicker Jacket", 125m, "AWMSJ" },
                    { 5, 1, null, "", true, null, "Thermal Fleece Jacket", 60m, "AWMTFJ" },
                    { 6, 1, null, "", true, null, "Unisex Thermal Vest", 95m, "AWMUTV" },
                    { 7, 1, null, "", true, null, "V-Neck Pullover", 65m, "AWMVNP" },
                    { 8, 1, null, "", true, null, "V-Neck Sweater", 65m, "AWMVNS" },
                    { 9, 1, null, "", true, null, "V-Neck T-Shirt", 17m, "AWMVNT" },
                    { 10, 2, null, "", true, null, "Bamboo Thermal Ski Coat", 99m, "AWWBTSC" },
                    { 11, 2, null, "", false, null, "Cross-Back Training Tank", 0m, "AWWCTT" },
                    { 12, 2, null, "", true, null, "Grunge Skater Jeans", 68m, "AWWGSJ" },
                    { 13, 2, null, "", true, null, "Slicker Jacket", 125m, "AWWSJ" },
                    { 14, 2, null, "", true, null, "Stretchy Dance Pants", 55m, "AWWSDP" },
                    { 15, 2, null, "", true, null, "Ultra-Soft Tank Top", 22m, "AWWUTT" },
                    { 16, 2, null, "", true, null, "Unisex Thermal Vest", 95m, "AWWUTV" },
                    { 17, 2, null, "", true, null, "V-Next T-Shirt", 17m, "AWWVNT" },
                    { 18, 3, null, "", true, null, "Blueberry Mineral Water", 2.8m, "MWB" },
                    { 19, 3, null, "", true, null, "Lemon-Lime Mineral Water", 2.8m, "MWLL" },
                    { 20, 3, null, "", true, null, "Orange Mineral Water", 2.8m, "MWO" },
                    { 21, 3, null, "", true, null, "Peach Mineral Water", 2.8m, "MWP" },
                    { 22, 3, null, "", true, null, "Raspberry Mineral Water", 2.8m, "MWR" },
                    { 23, 3, null, "", true, null, "Strawberry Mineral Water", 2.8m, "MWS" },
                    { 24, 4, null, "", true, null, "In the Kitchen with H+ Sport", 24.99m, "PITK" },
                    { 25, 5, null, "", true, null, "Calcium 400 IU (150 tablets)", 9.99m, "SC400" },
                    { 26, 5, null, "", true, null, "Flaxseed Oil 100 mg (90 capsules)", 12.49m, "SFO100" },
                    { 27, 5, null, "", true, null, "Iron 65 mg (150 caplets)", 13.99m, "SI65" },
                    { 28, 5, null, "", true, null, "Magnesium 250 mg (100 tablets)", 12.49m, "SM250" },
                    { 29, 5, null, "", true, null, "Multi-Vitamin (90 capsules)", 9.99m, "SMV" },
                    { 30, 5, null, "", true, null, "Vitamin A 10,000 IU (125 caplets)", 11.99m, "SVA" },
                    { 31, 5, null, "", true, null, "Vitamin B-Complex (100 caplets)", 12.99m, "SVB" },
                    { 32, 5, null, "", true, null, "Vitamin C 1000 mg (100 tablets)", 9.99m, "SVC" },
                    { 33, 5, null, "", true, null, "Vitamin D3 1000 IU (100 tablets)", 12.49m, "SVD3" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, null, null, "Admin" },
                    { 2, null, null, "User" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId", "CreatedAt", "Id", "ModifiedAt" },
                values: new object[,]
                {
                    { 1, 1, null, 0, null },
                    { 2, 1, null, 0, null },
                    { 2, 2, null, 0, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Sku = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "InventoryItems",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IsAvailable", "ModifiedAt", "Name", "Price", "Sku" },
                values: new object[,]
                {
                    { 1, 1, null, "", true, null, "Grunge Skater Jeans", 68m, "AWMGSJ" },
                    { 2, 1, null, "", true, null, "Polo Shirt", 35m, "AWMPS" },
                    { 3, 1, null, "", true, null, "Skater Graphic T-Shirt", 33m, "AWMSGT" },
                    { 4, 1, null, "", true, null, "Slicker Jacket", 125m, "AWMSJ" },
                    { 5, 1, null, "", true, null, "Thermal Fleece Jacket", 60m, "AWMTFJ" },
                    { 6, 1, null, "", true, null, "Unisex Thermal Vest", 95m, "AWMUTV" },
                    { 7, 1, null, "", true, null, "V-Neck Pullover", 65m, "AWMVNP" },
                    { 8, 1, null, "", true, null, "V-Neck Sweater", 65m, "AWMVNS" },
                    { 9, 1, null, "", true, null, "V-Neck T-Shirt", 17m, "AWMVNT" },
                    { 10, 2, null, "", true, null, "Bamboo Thermal Ski Coat", 99m, "AWWBTSC" },
                    { 11, 2, null, "", false, null, "Cross-Back Training Tank", 0m, "AWWCTT" },
                    { 12, 2, null, "", true, null, "Grunge Skater Jeans", 68m, "AWWGSJ" },
                    { 13, 2, null, "", true, null, "Slicker Jacket", 125m, "AWWSJ" },
                    { 14, 2, null, "", true, null, "Stretchy Dance Pants", 55m, "AWWSDP" },
                    { 15, 2, null, "", true, null, "Ultra-Soft Tank Top", 22m, "AWWUTT" },
                    { 16, 2, null, "", true, null, "Unisex Thermal Vest", 95m, "AWWUTV" },
                    { 17, 2, null, "", true, null, "V-Next T-Shirt", 17m, "AWWVNT" },
                    { 18, 3, null, "", true, null, "Blueberry Mineral Water", 2.8m, "MWB" },
                    { 19, 3, null, "", true, null, "Lemon-Lime Mineral Water", 2.8m, "MWLL" },
                    { 20, 3, null, "", true, null, "Orange Mineral Water", 2.8m, "MWO" },
                    { 21, 3, null, "", true, null, "Peach Mineral Water", 2.8m, "MWP" },
                    { 22, 3, null, "", true, null, "Raspberry Mineral Water", 2.8m, "MWR" },
                    { 23, 3, null, "", true, null, "Strawberry Mineral Water", 2.8m, "MWS" },
                    { 24, 4, null, "", true, null, "In the Kitchen with H+ Sport", 24.99m, "PITK" },
                    { 25, 5, null, "", true, null, "Calcium 400 IU (150 tablets)", 9.99m, "SC400" },
                    { 26, 5, null, "", true, null, "Flaxseed Oil 100 mg (90 capsules)", 12.49m, "SFO100" },
                    { 27, 5, null, "", true, null, "Iron 65 mg (150 caplets)", 13.99m, "SI65" },
                    { 28, 5, null, "", true, null, "Magnesium 250 mg (100 tablets)", 12.49m, "SM250" },
                    { 29, 5, null, "", true, null, "Multi-Vitamin (90 capsules)", 9.99m, "SMV" },
                    { 30, 5, null, "", true, null, "Vitamin A 10,000 IU (125 caplets)", 11.99m, "SVA" },
                    { 31, 5, null, "", true, null, "Vitamin B-Complex (100 caplets)", 12.99m, "SVB" },
                    { 32, 5, null, "", true, null, "Vitamin C 1000 mg (100 tablets)", 9.99m, "SVC" },
                    { 33, 5, null, "", true, null, "Vitamin D3 1000 IU (100 tablets)", 12.49m, "SVD3" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_CategoryId",
                table: "InventoryItems",
                column: "CategoryId");
        }
    }
}
