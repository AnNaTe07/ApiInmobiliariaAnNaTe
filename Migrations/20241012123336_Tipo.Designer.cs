﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ApiInmobiliariaAnNaTe.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20241012123336_Tipo")]
    partial class Tipo
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("ApiInmobiliariaAnNaTe.Models.Contrato", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Desde")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Estado")
                        .HasColumnType("int");

                    b.Property<DateTime>("Hasta")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("InmuebleId")
                        .HasColumnType("int");

                    b.Property<int>("InquilinoId")
                        .HasColumnType("int");

                    b.Property<decimal>("Monto")
                        .HasColumnType("decimal(65,30)");

                    b.Property<int?>("Pagos")
                        .HasColumnType("int");

                    b.Property<int>("PropId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("InmuebleId");

                    b.HasIndex("InquilinoId");

                    b.HasIndex("PropId");

                    b.ToTable("Contratos");
                });

            modelBuilder.Entity("ApiInmobiliariaAnNaTe.Models.Inmueble", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Ambientes")
                        .HasColumnType("int");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Estado")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Foto")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("IdPropietario")
                        .HasColumnType("int");

                    b.Property<decimal>("Latitud")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("Longitud")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("Precio")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("Superficie")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("TipoDescripcion")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("TipoId")
                        .HasColumnType("int");

                    b.Property<int>("Uso")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdPropietario");

                    b.ToTable("Inmuebles");
                });

            modelBuilder.Entity("ApiInmobiliariaAnNaTe.Models.Inquilino", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Documento")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Inquilinos");
                });

            modelBuilder.Entity("ApiInmobiliariaAnNaTe.Models.Pago", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<int>("ContratoId")
                        .HasColumnType("int");

                    b.Property<string>("Detalle")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool?>("Estado")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("FechaAnulacion")
                        .HasColumnType("datetime(6)");

                    b.Property<decimal>("Monto")
                        .HasColumnType("decimal(65,30)");

                    b.Property<int>("Nro")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ContratoId");

                    b.ToTable("Pagos");
                });

            modelBuilder.Entity("ApiInmobiliariaAnNaTe.Models.Propietario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Apellido")
                        .HasColumnType("longtext");

                    b.Property<string>("Avatar")
                        .HasColumnType("longtext");

                    b.Property<string>("Dni")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<bool>("Estado")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Nombre")
                        .HasColumnType("longtext");

                    b.Property<string>("Pass")
                        .HasColumnType("longtext");

                    b.Property<string>("Salt")
                        .HasColumnType("longtext");

                    b.Property<string>("Telefono")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Propietarios");
                });

            modelBuilder.Entity("ApiInmobiliariaAnNaTe.Models.Contrato", b =>
                {
                    b.HasOne("ApiInmobiliariaAnNaTe.Models.Inmueble", "Inmu")
                        .WithMany()
                        .HasForeignKey("InmuebleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApiInmobiliariaAnNaTe.Models.Inquilino", "Inqui")
                        .WithMany()
                        .HasForeignKey("InquilinoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApiInmobiliariaAnNaTe.Models.Propietario", "Prop")
                        .WithMany()
                        .HasForeignKey("PropId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inmu");

                    b.Navigation("Inqui");

                    b.Navigation("Prop");
                });

            modelBuilder.Entity("ApiInmobiliariaAnNaTe.Models.Inmueble", b =>
                {
                    b.HasOne("ApiInmobiliariaAnNaTe.Models.Propietario", "PropietarioInmueble")
                        .WithMany()
                        .HasForeignKey("IdPropietario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PropietarioInmueble");
                });

            modelBuilder.Entity("ApiInmobiliariaAnNaTe.Models.Pago", b =>
                {
                    b.HasOne("ApiInmobiliariaAnNaTe.Models.Contrato", "Contrato")
                        .WithMany()
                        .HasForeignKey("ContratoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contrato");
                });
#pragma warning restore 612, 618
        }
    }
}
