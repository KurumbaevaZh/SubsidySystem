using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SubsidySystem.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Citizen> Citizens { get; set; }

    public virtual DbSet<CompensationCalculation> CompensationCalculations { get; set; }

    public virtual DbSet<FamilyMember> FamilyMembers { get; set; }

    public virtual DbSet<Income> Incomes { get; set; }

    public virtual DbSet<IncomeType> IncomeTypes { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentRegistry> PaymentRegistries { get; set; }

    public virtual DbSet<Standard> Standards { get; set; }

    public virtual DbSet<SubsidyCalculation> SubsidyCalculations { get; set; }

    public virtual DbSet<UtilityCharge> UtilityCharges { get; set; }

    public virtual DbSet<UtilityServiceType> UtilityServiceTypes { get; set; }

    public virtual DbSet<VCalculationSummary> VCalculationSummaries { get; set; }

    public virtual DbSet<VCitizenDetail> VCitizenDetails { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=subsidy_system;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("categories_pkey");

            entity.ToTable("categories", tb => tb.HasComment("Категории граждан-льготников"));

            entity.Property(e => e.CategoryId).HasComment("Уникальный идентификатор категории");
            entity.Property(e => e.CategoryName).HasComment("Наименование категории граждан");
            entity.Property(e => e.DiscountRate).HasComment("Размер скидки на оплату ЖКУ в процентах");
            entity.Property(e => e.EligibilityConditions).HasComment("Описание условий предоставления льготы");
            entity.Property(e => e.RegulatoryAct).HasComment("Наименование документа, устанавливающего льготу");
        });

        modelBuilder.Entity<Citizen>(entity =>
        {
            entity.HasKey(e => e.CitizenId).HasName("citizens_pkey");

            entity.ToTable("citizens", tb => tb.HasComment("Персональные данные заявителей"));

            entity.Property(e => e.CitizenId).HasComment("Уникальный идентификатор гражданина");
            entity.Property(e => e.CategoryId).HasComment("Идентификатор категории льготности (внешний ключ)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Snils).HasComment("Страховой номер индивидуального лицевого счета");

            entity.HasOne(d => d.Category).WithMany(p => p.Citizens).HasConstraintName("citizens_category_id_fkey");
        });

        modelBuilder.Entity<CompensationCalculation>(entity =>
        {
            entity.HasKey(e => e.CompensationId).HasName("compensation_calculations_pkey");

            entity.ToTable("compensation_calculations", tb => tb.HasComment("Результаты расчета компенсаций"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Category).WithMany(p => p.CompensationCalculations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("compensation_calculations_category_id_fkey");

            entity.HasOne(d => d.Citizen).WithMany(p => p.CompensationCalculations).HasConstraintName("compensation_calculations_citizen_id_fkey");
        });

        modelBuilder.Entity<FamilyMember>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("family_members_pkey");

            entity.ToTable("family_members", tb => tb.HasComment("Члены семьи заявителя"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsStudent)
                .HasDefaultValue(false)
                .HasComment("Признак обучения (для учета иждивенцев)");
            entity.Property(e => e.Relationship).HasComment("Степень родства с заявителем");

            entity.HasOne(d => d.Citizen).WithMany(p => p.FamilyMembers).HasConstraintName("family_members_citizen_id_fkey");
        });

        modelBuilder.Entity<Income>(entity =>
        {
            entity.HasKey(e => e.IncomeId).HasName("incomes_pkey");

            entity.ToTable("incomes", tb => tb.HasComment("Сведения о доходах членов семьи"));

            entity.Property(e => e.Amount).HasComment("Сумма дохода");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.PeriodEnd).HasComment("Окончание периода, за который получен доход");
            entity.Property(e => e.PeriodStart).HasComment("Начало периода, за который получен доход");

            entity.HasOne(d => d.IncomeType).WithMany(p => p.Incomes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("incomes_income_type_id_fkey");

            entity.HasOne(d => d.Member).WithMany(p => p.Incomes).HasConstraintName("incomes_member_id_fkey");
        });

        modelBuilder.Entity<IncomeType>(entity =>
        {
            entity.HasKey(e => e.IncomeTypeId).HasName("income_types_pkey");

            entity.ToTable("income_types", tb => tb.HasComment("Классификатор видов доходов"));

            entity.Property(e => e.IsConsidered)
                .HasDefaultValue(true)
                .HasComment("Признак учета при расчете среднедушевого дохода");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payments_pkey");

            entity.ToTable("payments", tb => tb.HasComment("Детальные записи о выплатах"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.PaymentType).HasComment("Тип выплаты (субсидия, компенсация)");
            entity.Property(e => e.SourceCalculationId).HasComment("Идентификатор исходного расчета");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'назначено'::character varying")
                .HasComment("Статус выплаты (назначено, выплачено, отменено)");

            entity.HasOne(d => d.Citizen).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_citizen_id_fkey");

            entity.HasOne(d => d.Registry).WithMany(p => p.Payments).HasConstraintName("payments_registry_id_fkey");
        });

        modelBuilder.Entity<PaymentRegistry>(entity =>
        {
            entity.HasKey(e => e.RegistryId).HasName("payment_registries_pkey");

            entity.ToTable("payment_registries", tb => tb.HasComment("Реестры на перечисление выплат"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'формируется'::character varying")
                .HasComment("Статус реестра (формируется, утвержден, отправлен)");
        });

        modelBuilder.Entity<Standard>(entity =>
        {
            entity.HasKey(e => e.StandardId).HasName("standards_pkey");

            entity.ToTable("standards", tb => tb.HasComment("Нормативные показатели для расчетов"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.StandardType).HasComment("Тип норматива (ПМ, стандарт ЖКУ, доля расходов и т.д.)");
            entity.Property(e => e.TerritoryCode).HasComment("Код территории применения норматива");
        });

        modelBuilder.Entity<SubsidyCalculation>(entity =>
        {
            entity.HasKey(e => e.CalculationId).HasName("subsidy_calculations_pkey");

            entity.ToTable("subsidy_calculations", tb => tb.HasComment("Результаты расчета субсидий"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasComment("Статус расчета (назначено, отказано, пересчитано)");

            entity.HasOne(d => d.Citizen).WithMany(p => p.SubsidyCalculations).HasConstraintName("subsidy_calculations_citizen_id_fkey");
        });

        modelBuilder.Entity<UtilityCharge>(entity =>
        {
            entity.HasKey(e => e.ChargeId).HasName("utility_charges_pkey");

            entity.ToTable("utility_charges", tb => tb.HasComment("Фактические начисления за ЖКУ"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Month).HasComment("Месяц начисления");
            entity.Property(e => e.Year).HasComment("Год начисления");

            entity.HasOne(d => d.Citizen).WithMany(p => p.UtilityCharges).HasConstraintName("utility_charges_citizen_id_fkey");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.UtilityCharges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("utility_charges_service_type_id_fkey");
        });

        modelBuilder.Entity<UtilityServiceType>(entity =>
        {
            entity.HasKey(e => e.ServiceTypeId).HasName("utility_service_types_pkey");

            entity.ToTable("utility_service_types", tb => tb.HasComment("Классификатор услуг ЖКУ"));

            entity.Property(e => e.UnitOfMeasure).HasComment("Единица измерения потребления");
        });

        modelBuilder.Entity<VCalculationSummary>(entity =>
        {
            entity.ToView("v_calculation_summary");
        });

        modelBuilder.Entity<VCitizenDetail>(entity =>
        {
            entity.ToView("v_citizen_details");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
