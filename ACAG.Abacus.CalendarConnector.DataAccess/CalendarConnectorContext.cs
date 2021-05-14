using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;

namespace ACAG.Abacus.CalendarConnector.DataAccess
{
  public class CalendarConnectorContext : DbContext
  {
    public CalendarConnectorContext(DbContextOptions options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      #region Config Builder

      modelBuilder.Entity<AbacusSetting>().ToTable("AbacusSetting");
      modelBuilder.Entity<AbacusSetting>(entity =>
      {
        entity.HasKey(x => x.Id);
        entity.Property(e => e.Id)
                  .IsRequired();

      });

      modelBuilder.Entity<AppRole>().ToTable("AppRole");
      modelBuilder.Entity<AppRole>(entity =>
      {
        entity.HasKey(x => x.Id);
        entity.Property(e => e.Id)
                  .IsRequired();

      });

      modelBuilder.Entity<AppRoleRel>().ToTable("AppRoleRel");
      modelBuilder.Entity<AppRoleRel>(entity =>
      {
        entity.HasKey(x => x.Id);
        entity.Property(e => e.Id)
                  .IsRequired();

        entity.HasOne(x => x.User).WithMany(x => x.AppRoleRels).HasForeignKey(x => x.UserId).HasConstraintName("FK_AppRoleRel_User");
        entity.HasOne(x => x.AppRole).WithMany(x => x.AppRoleRels).HasForeignKey(x => x.AppRoleId).HasConstraintName("FK_AppRoleRel_AppRole");

      });

      modelBuilder.Entity<Culture>().ToTable("Culture");
      modelBuilder.Entity<Culture>(entity =>
      {
        entity.HasKey(x => x.Id);
        entity.Property(e => e.Id)
                  .IsRequired();

      });

      modelBuilder.Entity<ExchangeSetting>().ToTable("ExchangeSetting");
      modelBuilder.Entity<ExchangeSetting>(entity =>
      {
        entity.HasKey(x => x.Id);
        entity.Property(e => e.Id)
                  .IsRequired();

        entity.HasOne(x => x.Tenant).WithMany(x => x.ExchangeSettings).HasForeignKey(x => x.TenantId).HasConstraintName("FK_ExchangeSetting_Tenant");

      });

      modelBuilder.Entity<PayType>().ToTable("PayType");
      modelBuilder.Entity<PayType>(entity =>
      {
        entity.HasKey(x => x.Id);
        entity.Property(e => e.Id)
                  .IsRequired();

        entity.HasOne(x => x.Tenant).WithMany(x => x.PayTypes).HasForeignKey(x => x.TenantId).HasConstraintName("FK_PayType_Tenant");

      });

      modelBuilder.Entity<Tenant>().ToTable("Tenant");
      modelBuilder.Entity<Tenant>(entity =>
      {
        entity.HasKey(x => x.Id);
        entity.Property(e => e.Id)
                  .IsRequired();

        entity.HasOne(x => x.AbacusSetting).WithMany(x => x.Tenants).HasForeignKey(x => x.AbacusSettingId).HasConstraintName("FK_Tenant_AbacusSetting");

      });

      modelBuilder.Entity<TenantUserRel>().ToTable("TenantUserRel");
      modelBuilder.Entity<TenantUserRel>(entity =>
      {
        entity.HasKey(x => x.Id);
        entity.Property(e => e.Id)
                  .IsRequired();

        entity.HasOne(x => x.Tenant).WithMany(x => x.TenantUserRels).HasForeignKey(x => x.TenantId).HasConstraintName("FK_TenantUserRel_Tenant");
        entity.HasOne(x => x.User).WithMany(x => x.TenantUserRels).HasForeignKey(x => x.UserId).HasConstraintName("FK_TenantUserRel_User");

      });

      modelBuilder.Entity<User>().ToTable("User");
      modelBuilder.Entity<User>(entity =>
      {
        entity.HasKey(x => x.Id);
        entity.Property(e => e.Id)
                  .IsRequired();

        entity.HasOne(x => x.Culture).WithMany(x => x.Users).HasForeignKey(x => x.CultureId).HasConstraintName("FK_User_Culture");
      });

      modelBuilder.Entity<AppSetting>().ToTable("AppSetting");
      modelBuilder.Entity<AppSetting>(entity =>
      {
        entity.HasKey(a => a.Id);
        entity.Property(a => a.Id)
        .IsRequired();
      });
          
      modelBuilder.Entity<AbacusData>().ToTable("AbacusData");
      modelBuilder.Entity<AbacusData>(entity =>
      {
        entity.HasKey(x => x.ID);
        entity.Property(e => e.ID)
            .IsRequired();

      });

      #endregion BaseData Builder
    }

    public virtual DbSet<AbacusSetting> AbacusSettings { get; set; }

    public virtual DbSet<AppRole> AppRoles { get; set; }

    public virtual DbSet<AppRoleRel> AppRoleRels { get; set; }

    public virtual DbSet<Culture> Cultures { get; set; }

    public virtual DbSet<ExchangeSetting> ExchangeSettings { get; set; }

    public virtual DbSet<PayType> PayTypes { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<TenantUserRel> TenantUserRels { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<AppSetting> AppSettings { get; set; }    

    public virtual DbSet<AbacusData> AbacusDatas { get; set; }
    
    public virtual DbSet<LogDiary> LogDiary { get; set; }
  }
}
