using Microsoft.EntityFrameworkCore;
using NWSDB5013.Models;

namespace NWSDB5013.Data;

public class AppDbContext (DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // mapping each model class to a table in database
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<WaterConnection> WaterConnections => Set<WaterConnection>();
    public DbSet<MeterReading> MeterReadings => Set<MeterReading>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Receipt> Receipts => Set<Receipt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // customer
        modelBuilder.Entity<Customer>()
        .ToTable("Customers");

        modelBuilder.Entity<Customer>()
        .HasKey(c => c.CustomerId);

        modelBuilder.Entity<Customer>()
        .Property(c => c.FullName)
        .IsRequired()
        .HasMaxLength(150);

        modelBuilder.Entity<Customer>()
        .Property(c => c.Email)
        .IsRequired()
        .HasMaxLength(255);

        modelBuilder.Entity<Customer>()
        .Property(c => c.PasswordHash)
        .IsRequired();

        modelBuilder.Entity<Customer>()
        .Property(c => c.NIC)
        .IsRequired()
        .HasMaxLength(20);

        modelBuilder.Entity<Customer>()
        .Property(c => c.PhoneNo)
        .IsRequired()
        .HasMaxLength(15);

        modelBuilder.Entity<Customer>()
        .HasIndex(c => c.Email)
        .IsUnique();

        modelBuilder.Entity<Customer>()
        .HasIndex(c => c.NIC)
        .IsUnique();

        // partner
        modelBuilder.Entity<Partner>()
        .ToTable("Partners");

        modelBuilder.Entity<Partner>()
        .HasKey(p => p.PartnerId);

        modelBuilder.Entity<Partner>()
        .Property(p => p.PartnerName)
        .IsRequired()
        .HasMaxLength(150);

        modelBuilder.Entity<Partner>()
        .Property(p => p.Email)
        .IsRequired()
        .HasMaxLength(255);

        modelBuilder.Entity<Partner>()
        .Property(p => p.PasswordHash)
        .IsRequired();

        modelBuilder.Entity<Partner>()
        .Property(p => p.Status)
        .IsRequired()
        .HasMaxLength(20);

        modelBuilder.Entity<Partner>()
        .Property(p => p.CreatedAt)
        .IsRequired();

        modelBuilder.Entity<Partner>()
        .HasIndex(p => p.Email)
        .IsUnique();

        modelBuilder.Entity<Partner>()
        .ToTable(t => t.HasCheckConstraint("CK_Partner_Status", "Status IN ('Activated', 'Deactivated')"));

        // staff
        modelBuilder.Entity<Staff>()
        .ToTable("Staff");

        modelBuilder.Entity<Staff>()
        .HasKey(s => s.StaffId);

        modelBuilder.Entity<Staff>()
        .Property(s => s.FullName)
        .IsRequired()
        .HasMaxLength(150);

        modelBuilder.Entity<Staff>()
        .Property(s => s.Email)
        .IsRequired()
        .HasMaxLength(255);

        modelBuilder.Entity<Staff>()
        .Property(s => s.PasswordHash)
        .IsRequired();

        modelBuilder.Entity<Staff>()
        .HasIndex(s => s.Email)
        .IsUnique();

        // water connection
         modelBuilder.Entity<WaterConnection>()
            .ToTable("WaterConnections");

        modelBuilder.Entity<WaterConnection>()
            .HasKey(w => w.ConnId);

        modelBuilder.Entity<WaterConnection>()
            .Property(w => w.AccNo)
            .IsRequired()
            .HasMaxLength(30);

        modelBuilder.Entity<WaterConnection>()
            .Property(w => w.Address)
            .IsRequired()
            .HasMaxLength(300);

        modelBuilder.Entity<WaterConnection>()
            .Property(w => w.Status)
            .IsRequired()
            .HasMaxLength(20);

        modelBuilder.Entity<WaterConnection>()
            .HasIndex(w => w.AccNo)
            .IsUnique();

        modelBuilder.Entity<WaterConnection>()
        .ToTable(t => t.HasCheckConstraint("CK_WaterConnection_Status", "Status IN ('Active', 'Suspended', 'Closed')"));

        modelBuilder.Entity<WaterConnection>()
        .HasOne(w => w.Customer)
        .WithMany(c => c.WaterConnections)
        .HasForeignKey(w => w.CustomerId)
        .OnDelete(DeleteBehavior.Restrict);

        // meter reading
        modelBuilder.Entity<MeterReading>()
            .ToTable("MeterReadings");

        modelBuilder.Entity<MeterReading>()
            .HasKey(m => m.ReadingId);

        modelBuilder.Entity<MeterReading>()
            .Property(m => m.ReadingDate)
            .IsRequired();

        modelBuilder.Entity<MeterReading>()
            .Property(m => m.CurrentValue)
            .IsRequired();

        modelBuilder.Entity<MeterReading>().ToTable(t => t.HasCheckConstraint("CK_MeterReading_Value_NonNegative", "CurrentValue >= 0"));

        modelBuilder.Entity<MeterReading>()
        .HasOne(m => m.WaterConnection)
        .WithMany(w => w.MeterReadings)
        .HasForeignKey(m => m.ConnId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MeterReading>()
        .HasIndex(m => new { m.ConnId, m.ReadingDate });

        // bill
        modelBuilder.Entity<Bill>()
        .ToTable("Bills");

        modelBuilder.Entity<Bill>()
        .HasKey(b => b.BillId);

        modelBuilder.Entity<Bill>()
        .Property(b => b.IssuedDate)
        .IsRequired();

        modelBuilder.Entity<Bill>()
        .Property(b => b.DueDate)
        .IsRequired();

        modelBuilder.Entity<Bill>()
        .Property(b => b.TotalPayable)
        .IsRequired()
        .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Bill>()
        .Property(b => b.UnitsUsed)
        .IsRequired();

        modelBuilder.Entity<Bill>()
        .Property(b => b.Status)
        .IsRequired()
        .HasMaxLength(20);

        modelBuilder.Entity<Bill>().ToTable(t=>t.HasCheckConstraint("CK_Bill_TotalPayable_NotNegative", "TotalPayable >= 0"));

        modelBuilder.Entity<Bill>().ToTable(t=>t.HasCheckConstraint("CK_Bill_UnitsUsed_NotNegative", "UnitsUsed >= 0"));

        modelBuilder.Entity<Bill>().ToTable(t=>t.HasCheckConstraint("CK_Bill_DueDate_AfterIssued", "DueDate >= IssuedDate"));

        modelBuilder.Entity<Bill>().ToTable(t=>t.HasCheckConstraint("CK_Bill_Status", "Status IN ('Unpaid', 'Paid', 'Overdue')"));

        modelBuilder.Entity<Bill>()
        .HasOne(b => b.WaterConnection)
        .WithMany(w => w.Bills)
        .HasForeignKey(b => b.ConnId)
        .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Bill>()
        .HasOne(b => b.Staff)
        .WithMany(s => s.BillsGenerated)
        .HasForeignKey(b => b.StaffId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Bill>()
        .HasIndex(b => new { b.ConnId, b.IssuedDate });

        // payment
        modelBuilder.Entity<Payment>()
        .ToTable("Payments");

        modelBuilder.Entity<Payment>()
        .HasKey(p => p.PaymentId);

        modelBuilder.Entity<Payment>()
        .Property(p => p.PaymentDate)
        .IsRequired();

        modelBuilder.Entity<Payment>()
        .Property(p => p.TotalPaid)
        .IsRequired()
        .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Payment>()
        .Property(p => p.Status)
        .IsRequired()
        .HasMaxLength(20);

        modelBuilder.Entity<Payment>()
        .Property(p => p.PGRef)
        .HasMaxLength(100);

        modelBuilder.Entity<Payment>()
        .Property(p => p.IdempotencyKey)
        .IsRequired()
        .HasMaxLength(100);

        modelBuilder.Entity<Payment>()
        .HasIndex(p => p.IdempotencyKey)
        .IsUnique();

        modelBuilder.Entity<Payment>().ToTable(t=>t.HasCheckConstraint("CK_Payment_Amount_Positive", "TotalPaid > 0"));

        modelBuilder.Entity<Payment>().ToTable(t=>t.HasCheckConstraint("CK_Payment_Status", "Status IN ('Pending', 'Success', 'Failed')"));

        // either customer id or partner id
        modelBuilder.Entity<Payment>().ToTable(t=>t.HasCheckConstraint("CK_Payment_ExactlyOneCaller",
        "(CASE WHEN CustomerId IS NOT NULL THEN 1 ELSE 0 END + " + "CASE WHEN PartnerId IS NOT NULL THEN 1 ELSE 0 END) = 1"));

        modelBuilder.Entity<Payment>()
        .HasOne(p => p.Customer)
        .WithMany(c => c.Payments)
        .HasForeignKey(p => p.CustomerId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
        .HasOne(p => p.Partner)
        .WithMany(pt => pt.Payments)
        .HasForeignKey(p => p.PartnerId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
        .HasOne(p => p.Bill)
        .WithMany(b => b.Payments)
        .HasForeignKey(p => p.BillId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
        .HasIndex(p => p.BillId);

        // receipt
        modelBuilder.Entity<Receipt>()
        .ToTable("Receipts");

        modelBuilder.Entity<Receipt>()
        .HasKey(r => r.ReceiptId);

        modelBuilder.Entity<Receipt>()
        .HasOne(r => r.Payment)
        .WithOne(p => p.Receipt)
        .HasForeignKey<Receipt>(r => r.PaymentId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Receipt>()
        .HasIndex(r => r.PaymentId)
        .IsUnique();
    }
}