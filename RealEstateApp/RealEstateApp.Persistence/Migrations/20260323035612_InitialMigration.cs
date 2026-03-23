using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mejoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mejoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposPropiedades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPropiedades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposVentas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposVentas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Propiedades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TamanoEnMetros = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CantidadHabitaciones = table.Column<int>(type: "int", nullable: false),
                    CantidadBanos = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoPropiedadId = table.Column<int>(type: "int", nullable: false),
                    TipoVentaId = table.Column<int>(type: "int", nullable: false),
                    AgenteId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propiedades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Propiedades_TiposPropiedades_TipoPropiedadId",
                        column: x => x.TipoPropiedadId,
                        principalTable: "TiposPropiedades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Propiedades_TiposVentas_TipoVentaId",
                        column: x => x.TipoVentaId,
                        principalTable: "TiposVentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClienteId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    AgenteId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PropiedadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chats_Propiedades_PropiedadId",
                        column: x => x.PropiedadId,
                        principalTable: "Propiedades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImagenesPropiedades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrlImagen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EsPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    PropiedadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagenesPropiedades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagenesPropiedades_Propiedades_PropiedadId",
                        column: x => x.PropiedadId,
                        principalTable: "Propiedades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ofertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClienteId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PropiedadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ofertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ofertas_Propiedades_PropiedadId",
                        column: x => x.PropiedadId,
                        principalTable: "Propiedades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropiedadesFavoritas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PropiedadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropiedadesFavoritas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropiedadesFavoritas_Propiedades_PropiedadId",
                        column: x => x.PropiedadId,
                        principalTable: "Propiedades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropiedadesMejoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropiedadId = table.Column<int>(type: "int", nullable: false),
                    MejoraId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropiedadesMejoras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropiedadesMejoras_Mejoras_MejoraId",
                        column: x => x.MejoraId,
                        principalTable: "Mejoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropiedadesMejoras_Propiedades_PropiedadId",
                        column: x => x.PropiedadId,
                        principalTable: "Propiedades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mensajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contenido = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EsLeido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    EmisorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ReceptorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mensajes_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_AgenteId",
                table: "Chats",
                column: "AgenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ClienteId",
                table: "Chats",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ClienteId_AgenteId_PropiedadId",
                table: "Chats",
                columns: new[] { "ClienteId", "AgenteId", "PropiedadId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_PropiedadId",
                table: "Chats",
                column: "PropiedadId");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesPropiedades_PropiedadId",
                table: "ImagenesPropiedades",
                column: "PropiedadId");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesPropiedades_PropiedadId_Principal",
                table: "ImagenesPropiedades",
                columns: new[] { "PropiedadId", "EsPrincipal" },
                unique: true,
                filter: "[EsPrincipal] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Mejoras_Nombre",
                table: "Mejoras",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_ChatId",
                table: "Mensajes",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_ChatId_FechaEnvio",
                table: "Mensajes",
                columns: new[] { "ChatId", "FechaEnvio" });

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_EmisorId",
                table: "Mensajes",
                column: "EmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_EsLeido",
                table: "Mensajes",
                column: "EsLeido");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_FechaEnvio",
                table: "Mensajes",
                column: "FechaEnvio");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_ReceptorId",
                table: "Mensajes",
                column: "ReceptorId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_ReceptorId_EsLeido",
                table: "Mensajes",
                columns: new[] { "ReceptorId", "EsLeido" });

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_ClienteId",
                table: "Ofertas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_Estado",
                table: "Ofertas",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_FechaCreacion",
                table: "Ofertas",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_PropiedadId",
                table: "Ofertas",
                column: "PropiedadId");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_PropiedadId_Estado",
                table: "Ofertas",
                columns: new[] { "PropiedadId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_Propiedades_AgenteId",
                table: "Propiedades",
                column: "AgenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Propiedades_Codigo",
                table: "Propiedades",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Propiedades_Estado",
                table: "Propiedades",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Propiedades_Estado_TipoPropiedadId",
                table: "Propiedades",
                columns: new[] { "Estado", "TipoPropiedadId" });

            migrationBuilder.CreateIndex(
                name: "IX_Propiedades_Estado_TipoVentaId",
                table: "Propiedades",
                columns: new[] { "Estado", "TipoVentaId" });

            migrationBuilder.CreateIndex(
                name: "IX_Propiedades_TipoPropiedadId",
                table: "Propiedades",
                column: "TipoPropiedadId");

            migrationBuilder.CreateIndex(
                name: "IX_Propiedades_TipoVentaId",
                table: "Propiedades",
                column: "TipoVentaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropiedadesFavoritas_ClienteId",
                table: "PropiedadesFavoritas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PropiedadesFavoritas_ClienteId_PropiedadId",
                table: "PropiedadesFavoritas",
                columns: new[] { "ClienteId", "PropiedadId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropiedadesFavoritas_PropiedadId",
                table: "PropiedadesFavoritas",
                column: "PropiedadId");

            migrationBuilder.CreateIndex(
                name: "IX_PropiedadesMejoras_MejoraId",
                table: "PropiedadesMejoras",
                column: "MejoraId");

            migrationBuilder.CreateIndex(
                name: "IX_PropiedadesMejoras_PropiedadId",
                table: "PropiedadesMejoras",
                column: "PropiedadId");

            migrationBuilder.CreateIndex(
                name: "IX_PropiedadesMejoras_PropiedadId_MejoraId",
                table: "PropiedadesMejoras",
                columns: new[] { "PropiedadId", "MejoraId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposPropiedades_Nombre",
                table: "TiposPropiedades",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposVentas_Nombre",
                table: "TiposVentas",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagenesPropiedades");

            migrationBuilder.DropTable(
                name: "Mensajes");

            migrationBuilder.DropTable(
                name: "Ofertas");

            migrationBuilder.DropTable(
                name: "PropiedadesFavoritas");

            migrationBuilder.DropTable(
                name: "PropiedadesMejoras");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Mejoras");

            migrationBuilder.DropTable(
                name: "Propiedades");

            migrationBuilder.DropTable(
                name: "TiposPropiedades");

            migrationBuilder.DropTable(
                name: "TiposVentas");
        }
    }
}
