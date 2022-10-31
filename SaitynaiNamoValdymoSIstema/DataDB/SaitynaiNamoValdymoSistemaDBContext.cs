using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SaitynaiNamoValdymoSIstema.DataDB
{
    public partial class SaitynaiNamoValdymoSistemaDBContext : DbContext
    {
        public SaitynaiNamoValdymoSistemaDBContext()
        {
        }

        public SaitynaiNamoValdymoSistemaDBContext(DbContextOptions<SaitynaiNamoValdymoSistemaDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Flat> Flats { get; set; } = null!;
        public virtual DbSet<Floor> Floors { get; set; } = null!;
        public virtual DbSet<Messagee> Messagees { get; set; } = null!;
        public virtual DbSet<Person> People { get; set; } = null!;
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public virtual DbSet<Pswrd> Pswrds { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=SaitynaiNamoValdymoSistemaDB;Trusted_Connection=True;");
                optionsBuilder.UseSqlServer(@"Server=tcp:saitynaidb.database.windows.net,1433;Initial Catalog=SaitynaiNamoValdymoSistemaDB;Persist Security Info=False;User ID=marjas10;Password=MariusMeik12345;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flat>(entity =>
            {
                entity.ToTable("Flat");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FloorId).HasColumnName("FloorID");
            });

            modelBuilder.Entity<Floor>(entity =>
            {
                entity.ToTable("Floor");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Messagee>(entity =>
            {
                entity.ToTable("Messagee");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.TextMessage).HasColumnType("text");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("Person");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FlatId).HasColumnName("FlatID");

                entity.Property(e => e.LastName).HasColumnType("text");

                entity.Property(e => e.Name).HasColumnType("text");

                entity.Property(e => e.Password).HasColumnType("varbinary");

                entity.Property(e => e.PhoneNumber).HasColumnType("text");

                entity.Property(e => e.Role).HasColumnType("text");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshToken");

                entity.Property(e => e.ExpiresAt).HasColumnType("datetime");

                entity.Property(e => e.IssuedAt).HasColumnType("datetime");

                entity.Property(e => e.Token).HasColumnType("text");

                entity.Property(e => e.Id).HasColumnName("ID");
            });
            modelBuilder.Entity<Pswrd>(entity =>
            {
                entity.ToTable("pswrd");

                entity.Property(e => e.Pswrdd).HasColumnType("text");

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
