using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CarSharing.Models;

namespace CarSharing.Data
{
    public partial class car_sharingContext : DbContext
    {
        public car_sharingContext()
        {
        }

        public car_sharingContext(DbContextOptions<car_sharingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdditionalService> AdditionalServices { get; set; }
        public virtual DbSet<CarModel> CarModels { get; set; }
        public DbSet<CarMark> CarMarks { get; set; }
        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Rent> Rents { get; set; }
        public virtual DbSet<Service> Services { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=carSharing;Trusted_Connection=True;");
            }
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<AdditionalService>(entity =>
        //    {
        //        entity.HasOne(d => d.Rent)
        //            .WithMany(p => p.AdditionalServices)
        //            .HasForeignKey(d => d.RentId)
        //            .HasConstraintName("FK__Additiona__RentI__34C8D9D1");

        //        entity.HasOne(d => d.Service)
        //            .WithMany(p => p.AdditionalServices)
        //            .HasForeignKey(d => d.ServiceId)
        //            .HasConstraintName("FK__Additiona__Servi__35BCFE0A");
        //    });

        //    modelBuilder.Entity<CarModel>(entity =>
        //    {
        //        entity.HasKey(e => e.CarModelId)
        //            .HasName("PK__CarModel__C585C08FF46F5230");

        //        entity.Property(e => e.Description)
        //            .HasMaxLength(1000)
        //            .IsUnicode(false);

        //        entity.Property(e => e.Name)
        //            .HasMaxLength(20)
        //            .IsUnicode(false);
        //    });

        //    modelBuilder.Entity<Car>(entity =>
        //    {
        //        entity.HasKey(e => e.CarId)
        //            .HasName("PK__Cars__68A0342E060286E5");

        //        entity.Property(e => e.IssueDate).HasColumnType("datetime");

        //        entity.Property(e => e.Price).HasColumnType("money");

        //        entity.Property(e => e.RentalPrice).HasColumnType("money");

        //        entity.Property(e => e.Specs)
        //            .HasMaxLength(200)
        //            .IsUnicode(false);

        //        entity.Property(e => e.TechnicalMaintenanceDate).HasColumnType("datetime");

        //        entity.Property(e => e.VINcode)
        //            .HasColumnName("VINCode")
        //            .HasMaxLength(20)
        //            .IsUnicode(false);

        //        entity.HasOne(d => d.CarModel)
        //            .WithMany(p => p.Cars)
        //            .HasForeignKey(d => d.CarModelId)
        //            .HasConstraintName("FK__Cars__CarModelId__286302EC");

        //        entity.HasOne(d => d.Employee)
        //            .WithMany(p => p.Cars)
        //            .HasForeignKey(d => d.EmployeeId)
        //            .HasConstraintName("FK__Cars__EmployeeId__29572725");
        //    });

        //    modelBuilder.Entity<Customer>(entity =>
        //    {
        //        entity.HasKey(e => e.CustomerId)
        //            .HasName("PK__Customer__A4AE64D8E404F4FF");

        //        entity.Property(e => e.Address)
        //            .HasMaxLength(60)
        //            .IsUnicode(false);

        //        entity.Property(e => e.BirthDate).HasColumnType("datetime");

        //        entity.Property(e => e.Name)
        //            .HasMaxLength(20)
        //            .IsUnicode(false);

        //        entity.Property(e => e.PassportInfo)
        //            .HasMaxLength(20)
        //            .IsUnicode(false);

        //        entity.Property(e => e.Patronymic)
        //            .HasMaxLength(20)
        //            .IsUnicode(false);

        //        entity.Property(e => e.PhoneNum)
        //            .HasMaxLength(13)
        //            .IsUnicode(false);

        //        entity.Property(e => e.Surname)
        //            .HasMaxLength(20)
        //            .IsUnicode(false);
        //    });

        //    modelBuilder.Entity<Employee>(entity =>
        //    {
        //        entity.HasKey(e => e.EmployeeId)
        //            .HasName("PK__Employee__7AD04F119618475A");

        //        entity.Property(e => e.EmploymentDate).HasColumnType("datetime");

        //        entity.Property(e => e.Name)
        //            .HasMaxLength(20)
        //            .IsUnicode(false);

        //        entity.Property(e => e.Patronymic)
        //            .HasMaxLength(20)
        //            .IsUnicode(false);

        //        entity.Property(e => e.Post)
        //            .HasMaxLength(20)
        //            .IsUnicode(false);

        //        entity.Property(e => e.Surname)
        //            .HasMaxLength(20)
        //            .IsUnicode(false);
        //    });

        //    modelBuilder.Entity<Rent>(entity =>
        //    {
        //        entity.HasKey(e => e.RentId)
        //            .HasName("PK__Rents__783D47F5BFAAA285");

        //        entity.Property(e => e.DeliveryDate).HasColumnType("datetime");

        //        entity.Property(e => e.Price).HasColumnType("money");

        //        entity.Property(e => e.ReturnDate).HasColumnType("datetime");

        //        entity.HasOne(d => d.Car)
        //            .WithMany(p => p.Re)
        //            .HasForeignKey(d => d.CarId)
        //            .HasConstraintName("FK__Rents__CarId__2E1BDC42");

        //        entity.HasOne(d => d.Customer)
        //            .WithMany(p => p.Rents)
        //            .HasForeignKey(d => d.CustomerId)
        //            .HasConstraintName("FK__Rents__CustomerI__2F10007B");

        //        entity.HasOne(d => d.Employee)
        //            .WithMany(p => p.Rents)
        //            .HasForeignKey(d => d.EmployeeId)
        //            .HasConstraintName("FK__Rents__EmployeeI__300424B4");
        //    });

        //    modelBuilder.Entity<Service>(entity =>
        //    {
        //        entity.HasKey(e => e.ServiceId)
        //            .HasName("PK__Services__C51BB00A9ED4511E");

        //        entity.Property(e => e.Description)
        //            .HasMaxLength(100)
        //            .IsUnicode(false);

        //        entity.Property(e => e.Name)
        //            .HasMaxLength(20)
        //            .IsUnicode(false);

        //        entity.Property(e => e.Price).HasColumnType("money");
        //    });


        //    OnModelCreatingPartial(modelBuilder);
        //}

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
