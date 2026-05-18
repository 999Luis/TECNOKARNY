using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TECNOKARNY.Models;

public partial class BdtecnokarnyContext : DbContext
{
    public BdtecnokarnyContext()
    {
    }

    public BdtecnokarnyContext(DbContextOptions<BdtecnokarnyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Clientes> Clientes { get; set; }

    public virtual DbSet<Cotizaciones> Cotizaciones { get; set; }

    public virtual DbSet<DetalleCotizacion> DetalleCotizacion { get; set; }

    public virtual DbSet<DetalleVenta> DetalleVenta { get; set; }

    public virtual DbSet<Pagos> Pagos { get; set; }

    public virtual DbSet<Productos> Productos { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    public virtual DbSet<Ventas> Ventas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=ConnectionStrings:conexionSQL");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Clientes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clientes__3214EC078D904FC0");

            entity.HasIndex(e => e.Correo, "UQ__Clientes__60695A1923C7FAAF").IsUnique();

            entity.Property(e => e.ApeMat)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ApePat)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Cotizaciones>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cotizaci__3214EC07887647A5");

            entity.Property(e => e.Anticipo).HasDefaultValue(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EstadoCot)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Estado_Cot");
            entity.Property(e => e.FechaEmision).HasColumnName("Fecha_Emision");
            entity.Property(e => e.FechaEvento).HasColumnName("Fecha_Evento");
            entity.Property(e => e.IdCliente).HasColumnName("Id_Cliente");
            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");
            entity.Property(e => e.SaldoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Saldo_Total");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Cotizaciones)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_C_Cot");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Cotizaciones)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_U_Cot");
        });

        modelBuilder.Entity<DetalleCotizacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Detalle___3214EC07EBA329C6");

            entity.ToTable("Detalle_Cotizacion");

            entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IdCotizacion).HasColumnName("Id_Cotizacion");
            entity.Property(e => e.IdProducto).HasColumnName("Id_Producto");
            entity.Property(e => e.PrecioCotizado)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Precio_Cotizado");
            entity.Property(e => e.PrecioKilo)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Precio_Kilo");

            entity.HasOne(d => d.IdCotizacionNavigation).WithMany(p => p.DetalleCotizacion)
                .HasForeignKey(d => d.IdCotizacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Cot_DetCot");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleCotizacion)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Prod_DetCot");
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Detalle___3214EC0727AE4A46");

            entity.ToTable("Detalle_Venta");

            entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IdProducto).HasColumnName("Id_Producto");
            entity.Property(e => e.IdVenta).HasColumnName("Id_Venta");
            entity.Property(e => e.PrecioKilo)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Precio_Kilo");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Prod_DetVent");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_V_DetVent");
        });

        modelBuilder.Entity<Pagos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pagos__3214EC077F7D7620");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FechaPago).HasColumnName("Fecha_Pago");
            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_U_Pagos");
        });

        modelBuilder.Entity<Productos>(entity =>
{
    entity.HasKey(e => e.Id).HasName("PK__Producto__3214EC07CB84F146");

    entity.Property(e => e.Nombre)
        .HasMaxLength(50)
        .IsUnicode(false);
    entity.Property(e => e.PrecioKilo)
        .HasColumnType("decimal(10, 2)")
        .HasColumnName("Precio_Kilo");

    entity.Property(e => e.Estado)
        .HasMaxLength(10)
        .IsUnicode(false);
});

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07FAF7EB9F");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Rol)
                .HasMaxLength(13)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC07CEE87D42");

            entity.HasIndex(e => e.Correo, "UQ__Usuarios__60695A1926507AA2").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("Activo");
            entity.Property(e => e.IdRol).HasColumnName("Id_Rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Pwd)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Usr_Rol");
        });

        modelBuilder.Entity<Ventas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ventas__3214EC07943DC9F3");

            entity.Property(e => e.Estado)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasDefaultValue("Activa");
            entity.Property(e => e.FechaVencimiento).HasColumnName("Fecha_Vencimiento");
            entity.Property(e => e.IdCliente).HasColumnName("Id_Cliente");
            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Monto_Total");
            entity.Property(e => e.MotivoCancelacion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Motivo_Cancelacion");
            entity.Property(e => e.Saldo).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Tipo)
                .HasMaxLength(7)
                .IsUnicode(false);

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Ventas)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_C_V");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Ventas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_U_V");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
