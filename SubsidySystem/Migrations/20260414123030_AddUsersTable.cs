using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SubsidySystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "integer", nullable: false, comment: "Уникальный идентификатор категории")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Наименование категории граждан"),
                    discount_rate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true, comment: "Размер скидки на оплату ЖКУ в процентах"),
                    eligibility_conditions = table.Column<string>(type: "text", nullable: true, comment: "Описание условий предоставления льготы"),
                    regulatory_act = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, comment: "Наименование документа, устанавливающего льготу")
                },
                constraints: table =>
                {
                    table.PrimaryKey("categories_pkey", x => x.category_id);
                },
                comment: "Категории граждан-льготников");

            migrationBuilder.CreateTable(
                name: "income_types",
                columns: table => new
                {
                    income_type_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    income_type_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_considered = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true, comment: "Признак учета при расчете среднедушевого дохода")
                },
                constraints: table =>
                {
                    table.PrimaryKey("income_types_pkey", x => x.income_type_id);
                },
                comment: "Классификатор видов доходов");

            migrationBuilder.CreateTable(
                name: "payment_registries",
                columns: table => new
                {
                    registry_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    registry_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    registry_date = table.Column<DateTime>(type: "date", nullable: false),
                    period_year = table.Column<int>(type: "integer", nullable: false),
                    period_month = table.Column<int>(type: "integer", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    recipient_count = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, defaultValueSql: "'формируется'::character varying", comment: "Статус реестра (формируется, утвержден, отправлен)"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("payment_registries_pkey", x => x.registry_id);
                },
                comment: "Реестры на перечисление выплат");

            migrationBuilder.CreateTable(
                name: "standards",
                columns: table => new
                {
                    standard_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    standard_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Тип норматива (ПМ, стандарт ЖКУ, доля расходов и т.д.)"),
                    standard_value = table.Column<decimal>(type: "numeric", nullable: false),
                    territory_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true, comment: "Код территории применения норматива"),
                    valid_from = table.Column<DateTime>(type: "date", nullable: false),
                    valid_to = table.Column<DateTime>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("standards_pkey", x => x.standard_id);
                },
                comment: "Нормативные показатели для расчетов");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "utility_service_types",
                columns: table => new
                {
                    service_type_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    service_type_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    unit_of_measure = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true, comment: "Единица измерения потребления")
                },
                constraints: table =>
                {
                    table.PrimaryKey("utility_service_types_pkey", x => x.service_type_id);
                },
                comment: "Классификатор услуг ЖКУ");

            migrationBuilder.CreateTable(
                name: "citizens",
                columns: table => new
                {
                    citizen_id = table.Column<int>(type: "integer", nullable: false, comment: "Уникальный идентификатор гражданина")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    snils = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true, comment: "Страховой номер индивидуального лицевого счета"),
                    passport_series = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    passport_number = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    passport_issuer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    passport_issue_date = table.Column<DateOnly>(type: "date", nullable: true),
                    registration_address = table.Column<string>(type: "text", nullable: false),
                    actual_address = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: true, comment: "Идентификатор категории льготности (внешний ключ)"),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("citizens_pkey", x => x.citizen_id);
                    table.ForeignKey(
                        name: "citizens_category_id_fkey",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id");
                },
                comment: "Персональные данные заявителей");

            migrationBuilder.CreateTable(
                name: "compensation_calculations",
                columns: table => new
                {
                    compensation_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    citizen_id = table.Column<int>(type: "integer", nullable: false),
                    calculation_date = table.Column<DateTime>(type: "date", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    month = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    discount_rate = table.Column<decimal>(type: "numeric", nullable: false),
                    actual_charges = table.Column<decimal>(type: "numeric", nullable: false),
                    social_norm = table.Column<decimal>(type: "numeric", nullable: true),
                    consumption_rate = table.Column<decimal>(type: "numeric", nullable: true),
                    compensation_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("compensation_calculations_pkey", x => x.compensation_id);
                    table.ForeignKey(
                        name: "compensation_calculations_category_id_fkey",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id");
                    table.ForeignKey(
                        name: "compensation_calculations_citizen_id_fkey",
                        column: x => x.citizen_id,
                        principalTable: "citizens",
                        principalColumn: "citizen_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Результаты расчета компенсаций");

            migrationBuilder.CreateTable(
                name: "family_members",
                columns: table => new
                {
                    member_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    citizen_id = table.Column<int>(type: "integer", nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    relationship = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, comment: "Степень родства с заявителем"),
                    is_student = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false, comment: "Признак обучения (для учета иждивенцев)"),
                    snils = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("family_members_pkey", x => x.member_id);
                    table.ForeignKey(
                        name: "family_members_citizen_id_fkey",
                        column: x => x.citizen_id,
                        principalTable: "citizens",
                        principalColumn: "citizen_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Члены семьи заявителя");

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    registry_id = table.Column<int>(type: "integer", nullable: false),
                    citizen_id = table.Column<int>(type: "integer", nullable: false),
                    payment_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, comment: "Тип выплаты (субсидия, компенсация)"),
                    source_calculation_id = table.Column<int>(type: "integer", nullable: false, comment: "Идентификатор исходного расчета"),
                    payment_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    payment_method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    payment_details = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    payment_date = table.Column<DateTime>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, defaultValueSql: "'назначено'::character varying", comment: "Статус выплаты (назначено, выплачено, отменено)"),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payments_pkey", x => x.payment_id);
                    table.ForeignKey(
                        name: "payments_citizen_id_fkey",
                        column: x => x.citizen_id,
                        principalTable: "citizens",
                        principalColumn: "citizen_id");
                    table.ForeignKey(
                        name: "payments_registry_id_fkey",
                        column: x => x.registry_id,
                        principalTable: "payment_registries",
                        principalColumn: "registry_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Детальные записи о выплатах");

            migrationBuilder.CreateTable(
                name: "subsidy_calculations",
                columns: table => new
                {
                    calculation_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    citizen_id = table.Column<int>(type: "integer", nullable: false),
                    calculation_date = table.Column<DateTime>(type: "date", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    month = table.Column<int>(type: "integer", nullable: false),
                    total_family_income = table.Column<decimal>(type: "numeric", nullable: false),
                    average_per_capita_income = table.Column<decimal>(type: "numeric", nullable: false),
                    living_wage = table.Column<decimal>(type: "numeric", nullable: false),
                    correction_factor = table.Column<decimal>(type: "numeric", nullable: true),
                    housing_standard = table.Column<decimal>(type: "numeric", nullable: false),
                    max_allowed_share = table.Column<decimal>(type: "numeric", nullable: false),
                    subsidy_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Статус расчета (назначено, отказано, пересчитано)"),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("subsidy_calculations_pkey", x => x.calculation_id);
                    table.ForeignKey(
                        name: "subsidy_calculations_citizen_id_fkey",
                        column: x => x.citizen_id,
                        principalTable: "citizens",
                        principalColumn: "citizen_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Результаты расчета субсидий");

            migrationBuilder.CreateTable(
                name: "utility_charges",
                columns: table => new
                {
                    charge_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    citizen_id = table.Column<int>(type: "integer", nullable: false),
                    service_type_id = table.Column<int>(type: "integer", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false, comment: "Год начисления"),
                    month = table.Column<int>(type: "integer", nullable: false, comment: "Месяц начисления"),
                    charge_amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    consumption_volume = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("utility_charges_pkey", x => x.charge_id);
                    table.ForeignKey(
                        name: "utility_charges_citizen_id_fkey",
                        column: x => x.citizen_id,
                        principalTable: "citizens",
                        principalColumn: "citizen_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "utility_charges_service_type_id_fkey",
                        column: x => x.service_type_id,
                        principalTable: "utility_service_types",
                        principalColumn: "service_type_id");
                },
                comment: "Фактические начисления за ЖКУ");

            migrationBuilder.CreateTable(
                name: "incomes",
                columns: table => new
                {
                    income_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "integer", nullable: false),
                    income_type_id = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, comment: "Сумма дохода"),
                    period_start = table.Column<DateOnly>(type: "date", nullable: false, comment: "Начало периода, за который получен доход"),
                    period_end = table.Column<DateOnly>(type: "date", nullable: false, comment: "Окончание периода, за который получен доход"),
                    document_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("incomes_pkey", x => x.income_id);
                    table.ForeignKey(
                        name: "incomes_income_type_id_fkey",
                        column: x => x.income_type_id,
                        principalTable: "income_types",
                        principalColumn: "income_type_id");
                    table.ForeignKey(
                        name: "incomes_member_id_fkey",
                        column: x => x.member_id,
                        principalTable: "family_members",
                        principalColumn: "member_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Сведения о доходах членов семьи");

            migrationBuilder.CreateIndex(
                name: "idx_citizens_name",
                table: "citizens",
                columns: new[] { "last_name", "first_name" });

            migrationBuilder.CreateIndex(
                name: "idx_citizens_snils",
                table: "citizens",
                column: "snils");

            migrationBuilder.CreateIndex(
                name: "IX_citizens_category_id",
                table: "citizens",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_compensation_calculations_category_id",
                table: "compensation_calculations",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_compensation_calculations_citizen_id",
                table: "compensation_calculations",
                column: "citizen_id");

            migrationBuilder.CreateIndex(
                name: "idx_family_members_citizen",
                table: "family_members",
                column: "citizen_id");

            migrationBuilder.CreateIndex(
                name: "idx_incomes_member",
                table: "incomes",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "idx_incomes_period",
                table: "incomes",
                columns: new[] { "period_start", "period_end" });

            migrationBuilder.CreateIndex(
                name: "IX_incomes_income_type_id",
                table: "incomes",
                column: "income_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_citizen_id",
                table: "payments",
                column: "citizen_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_registry_id",
                table: "payments",
                column: "registry_id");

            migrationBuilder.CreateIndex(
                name: "IX_subsidy_calculations_citizen_id",
                table: "subsidy_calculations",
                column: "citizen_id");

            migrationBuilder.CreateIndex(
                name: "idx_utility_charges_citizen",
                table: "utility_charges",
                column: "citizen_id");

            migrationBuilder.CreateIndex(
                name: "idx_utility_charges_period",
                table: "utility_charges",
                columns: new[] { "year", "month" });

            migrationBuilder.CreateIndex(
                name: "idx_utility_charges_unique",
                table: "utility_charges",
                columns: new[] { "citizen_id", "service_type_id", "year", "month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_utility_charges_service_type_id",
                table: "utility_charges",
                column: "service_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "compensation_calculations");

            migrationBuilder.DropTable(
                name: "incomes");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "standards");

            migrationBuilder.DropTable(
                name: "subsidy_calculations");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "utility_charges");

            migrationBuilder.DropTable(
                name: "income_types");

            migrationBuilder.DropTable(
                name: "family_members");

            migrationBuilder.DropTable(
                name: "payment_registries");

            migrationBuilder.DropTable(
                name: "utility_service_types");

            migrationBuilder.DropTable(
                name: "citizens");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
