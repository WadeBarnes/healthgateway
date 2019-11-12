﻿//-------------------------------------------------------------------------
// Copyright © 2019 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------
#pragma warning disable SA1118, SA1200, SA1205, SA1413, SA1600, SA1601, CA1062, CS1591, CA1812, CA1814
using System;
using HealthGateway.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace HealthGateway.Database.Migrations
{
    [DbContext(typeof(DrugDbContext))]
    [Migration("20191107192622_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("Relational:Sequence:.trace_seq", "'trace_seq', '', '1', '1', '1', '999999', 'Int64', 'True'");

            modelBuilder.Entity("HealthGateway.Database.Models.ActiveIngredient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ActiveIngredientId");

                    b.Property<int>("ActiveIngredientCode");

                    b.Property<string>("Base")
                        .HasMaxLength(1);

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<string>("DosageUnit")
                        .HasMaxLength(40);

                    b.Property<string>("DosageUnitFrench")
                        .HasMaxLength(80);

                    b.Property<string>("DosageValue")
                        .HasMaxLength(20);

                    b.Property<Guid>("DrugProductId");

                    b.Property<string>("Ingredient")
                        .HasMaxLength(240);

                    b.Property<string>("IngredientFrench")
                        .HasMaxLength(400);

                    b.Property<string>("IngredientSuppliedInd")
                        .HasMaxLength(1);

                    b.Property<string>("Notes")
                        .HasMaxLength(2000);

                    b.Property<string>("Strength")
                        .HasMaxLength(20);

                    b.Property<string>("StrengthType")
                        .HasMaxLength(40);

                    b.Property<string>("StrengthTypeFrench")
                        .HasMaxLength(80);

                    b.Property<string>("StrengthUnit")
                        .HasMaxLength(40);

                    b.Property<string>("StrengthUnitFrench")
                        .HasMaxLength(80);

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("DrugProductId")
                        .IsUnique();

                    b.ToTable("ActiveIngredient");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CompanyId");

                    b.Property<string>("AddressBillingFlag")
                        .HasMaxLength(1);

                    b.Property<string>("AddressMailingFlag")
                        .HasMaxLength(1);

                    b.Property<string>("AddressNotificationFlag")
                        .HasMaxLength(1);

                    b.Property<string>("AddressOther")
                        .HasMaxLength(1);

                    b.Property<string>("CityName")
                        .HasMaxLength(60);

                    b.Property<int>("CompanyCode");

                    b.Property<string>("CompanyName")
                        .HasMaxLength(80);

                    b.Property<string>("CompanyType")
                        .HasMaxLength(40);

                    b.Property<string>("Country")
                        .HasMaxLength(40);

                    b.Property<string>("CountryFrench")
                        .HasMaxLength(100);

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<Guid>("DrugProductId");

                    b.Property<string>("ManufacturerCode")
                        .HasMaxLength(5);

                    b.Property<string>("PostOfficeBox")
                        .HasMaxLength(15);

                    b.Property<string>("PostalCode")
                        .HasMaxLength(20);

                    b.Property<string>("Province")
                        .HasMaxLength(40);

                    b.Property<string>("ProvinceFrench")
                        .HasMaxLength(100);

                    b.Property<string>("StreetName")
                        .HasMaxLength(80);

                    b.Property<string>("SuiteNumber")
                        .HasMaxLength(20);

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("DrugProductId")
                        .IsUnique();

                    b.ToTable("Company");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.DrugProduct", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("DrugProductId");

                    b.Property<string>("AccessionNumber")
                        .HasMaxLength(5);

                    b.Property<string>("AiGroupNumber")
                        .HasMaxLength(10);

                    b.Property<string>("BrandName")
                        .HasMaxLength(200);

                    b.Property<string>("BrandNameFrench")
                        .HasMaxLength(300);

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<string>("Descriptor")
                        .HasMaxLength(150);

                    b.Property<string>("DescriptorFrench")
                        .HasMaxLength(200);

                    b.Property<string>("DrugClass")
                        .HasMaxLength(40);

                    b.Property<string>("DrugClassFrench")
                        .HasMaxLength(80);

                    b.Property<string>("DrugCode")
                        .IsRequired()
                        .HasMaxLength(8);

                    b.Property<string>("DrugIdentificationNumber")
                        .HasMaxLength(29);

                    b.Property<Guid>("FileDownloadId");

                    b.Property<DateTime>("LastUpdate");

                    b.Property<string>("NumberOfAis")
                        .HasMaxLength(10);

                    b.Property<string>("PediatricFlag")
                        .HasMaxLength(1);

                    b.Property<string>("ProductCategorization")
                        .HasMaxLength(80);

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("FileDownloadId");

                    b.ToTable("DrugProduct");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.FileDownload", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("FileDownloadId");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasMaxLength(44);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(35);

                    b.Property<int>("ProgramTypeCodeId");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("Hash")
                        .IsUnique();

                    b.HasIndex("ProgramTypeCodeId");

                    b.ToTable("FileDownload");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.Form", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("FormId");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<Guid>("DrugProductId");

                    b.Property<string>("PharmaceuticalForm")
                        .HasMaxLength(40);

                    b.Property<int>("PharmaceuticalFormCode");

                    b.Property<string>("PharmaceuticalFormFrench")
                        .HasMaxLength(80);

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("DrugProductId")
                        .IsUnique();

                    b.ToTable("Form");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.Packaging", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("PackagingId");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<Guid>("DrugProductId");

                    b.Property<string>("PackageSize")
                        .HasMaxLength(5);

                    b.Property<string>("PackageSizeUnit")
                        .HasMaxLength(40);

                    b.Property<string>("PackageSizeUnitFrench")
                        .HasMaxLength(80);

                    b.Property<string>("PackageType")
                        .HasMaxLength(40);

                    b.Property<string>("PackageTypeFrench")
                        .HasMaxLength(80);

                    b.Property<string>("ProductInformation")
                        .HasMaxLength(80);

                    b.Property<string>("UPC")
                        .HasMaxLength(12);

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("DrugProductId")
                        .IsUnique();

                    b.ToTable("Packaging");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.PharmaCareDrug", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("PharmaCareDrugId");

                    b.Property<string>("BenefitGroupList")
                        .HasMaxLength(60);

                    b.Property<string>("BrandName")
                        .HasMaxLength(80);

                    b.Property<string>("CFRCode")
                        .HasMaxLength(1);

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<string>("DINPIN")
                        .IsRequired()
                        .HasMaxLength(8);

                    b.Property<string>("DosageForm")
                        .HasMaxLength(20);

                    b.Property<DateTime>("EffectiveDate")
                        .HasColumnType("Date");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("Date");

                    b.Property<Guid>("FileDownloadId");

                    b.Property<DateTime>("FormularyListDate")
                        .HasColumnType("Date");

                    b.Property<string>("GenericName")
                        .HasMaxLength(60);

                    b.Property<string>("LCAIndicator")
                        .HasMaxLength(2);

                    b.Property<decimal?>("LCAPrice")
                        .HasColumnType("decimal(8,4)");

                    b.Property<string>("LimitedUseFlag")
                        .HasMaxLength(1);

                    b.Property<string>("Manufacturer")
                        .HasMaxLength(6);

                    b.Property<int?>("MaximumDaysSupply");

                    b.Property<decimal?>("MaximumPrice")
                        .HasColumnType("decimal(8,4)");

                    b.Property<string>("PayGenericIndicator")
                        .HasMaxLength(1);

                    b.Property<string>("PharmaCarePlanDescription")
                        .HasMaxLength(80);

                    b.Property<string>("Plan")
                        .HasMaxLength(2);

                    b.Property<int?>("QuantityLimit");

                    b.Property<string>("RDPCategory")
                        .HasMaxLength(4);

                    b.Property<string>("RDPExcludedPlans")
                        .HasMaxLength(20);

                    b.Property<decimal?>("RDPPrice")
                        .HasColumnType("decimal(8,4)");

                    b.Property<string>("RDPSubCategory")
                        .HasMaxLength(4);

                    b.Property<string>("TrialFlag")
                        .HasMaxLength(1);

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("FileDownloadId");

                    b.ToTable("PharmaCareDrug");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.PharmaceuticalStd", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("PharmaceuticalStdId");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<Guid>("DrugProductId");

                    b.Property<string>("PharmaceuticalStdDesc");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("DrugProductId")
                        .IsUnique();

                    b.ToTable("PharmaceuticalStd");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.ProgramTypeCode", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnName("ProgramTypeCodeId");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.ToTable("ProgramTypeCode");

                    b.HasData(
                        new
                        {
                            Id = 105,
                            CreatedBy = "System",
                            CreatedDateTime = new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "FederalApproved",
                            UpdatedBy = "System",
                            UpdatedDateTime = new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Version = 0u
                        },
                        new
                        {
                            Id = 110,
                            CreatedBy = "System",
                            CreatedDateTime = new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "FederalMarketed",
                            UpdatedBy = "System",
                            UpdatedDateTime = new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Version = 0u
                        },
                        new
                        {
                            Id = 115,
                            CreatedBy = "System",
                            CreatedDateTime = new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "FederalCancelled",
                            UpdatedBy = "System",
                            UpdatedDateTime = new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Version = 0u
                        },
                        new
                        {
                            Id = 120,
                            CreatedBy = "System",
                            CreatedDateTime = new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "FederalDormant",
                            UpdatedBy = "System",
                            UpdatedDateTime = new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Version = 0u
                        },
                        new
                        {
                            Id = 200,
                            CreatedBy = "System",
                            CreatedDateTime = new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Provincial",
                            UpdatedBy = "System",
                            UpdatedDateTime = new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Version = 0u
                        });
                });

            modelBuilder.Entity("HealthGateway.Database.Models.Route", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RouteId");

                    b.Property<string>("Administration")
                        .HasMaxLength(40);

                    b.Property<int>("AdministrationCode");

                    b.Property<string>("AdministrationFrench")
                        .HasMaxLength(80);

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<Guid>("DrugProductId");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("DrugProductId")
                        .IsUnique();

                    b.ToTable("Route");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.Schedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ScheduleId");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<Guid>("DrugProductId");

                    b.Property<string>("ScheduleDesc")
                        .HasMaxLength(40);

                    b.Property<string>("ScheduleDescFrench")
                        .HasMaxLength(80);

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.ToTable("Schedule");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.Status", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("StatusId");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<string>("CurrentStatusFlag")
                        .HasMaxLength(1);

                    b.Property<Guid>("DrugProductId");

                    b.Property<DateTime?>("ExpirationDate");

                    b.Property<DateTime?>("HistoryDate");

                    b.Property<string>("LotNumber")
                        .HasMaxLength(80);

                    b.Property<string>("StatusDesc")
                        .HasMaxLength(40);

                    b.Property<string>("StatusDescFrench")
                        .HasMaxLength(80);

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("DrugProductId");

                    b.ToTable("Status");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.TherapeuticClass", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("TherapeuticClassId");

                    b.Property<string>("Ahfs")
                        .HasMaxLength(80);

                    b.Property<string>("AhfsFrench")
                        .HasMaxLength(160);

                    b.Property<string>("AhfsNumber")
                        .HasMaxLength(20);

                    b.Property<string>("Atc")
                        .HasMaxLength(120);

                    b.Property<string>("AtcFrench")
                        .HasMaxLength(240);

                    b.Property<string>("AtcNumber")
                        .HasMaxLength(8);

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<Guid>("DrugProductId");

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("DrugProductId")
                        .IsUnique();

                    b.ToTable("TherapeuticClass");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.VeterinarySpecies", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("VeterinarySpeciesId");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<Guid>("DrugProductId");

                    b.Property<string>("Species")
                        .HasMaxLength(80);

                    b.Property<string>("SpeciesFrench")
                        .HasMaxLength(160);

                    b.Property<string>("SubSpecies")
                        .HasMaxLength(80);

                    b.Property<string>("UpdatedBy")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.HasKey("Id");

                    b.HasIndex("DrugProductId")
                        .IsUnique();

                    b.ToTable("VeterinarySpecies");
                });

            modelBuilder.Entity("HealthGateway.Database.Models.ActiveIngredient", b =>
                {
                    b.HasOne("HealthGateway.Database.Models.DrugProduct")
                        .WithOne("ActiveIngredient")
                        .HasForeignKey("HealthGateway.Database.Models.ActiveIngredient", "DrugProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HealthGateway.Database.Models.Company", b =>
                {
                    b.HasOne("HealthGateway.Database.Models.DrugProduct")
                        .WithOne("Company")
                        .HasForeignKey("HealthGateway.Database.Models.Company", "DrugProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HealthGateway.Database.Models.DrugProduct", b =>
                {
                    b.HasOne("HealthGateway.Database.Models.FileDownload", "FileDownload")
                        .WithMany()
                        .HasForeignKey("FileDownloadId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HealthGateway.Database.Models.FileDownload", b =>
                {
                    b.HasOne("HealthGateway.Database.Models.ProgramTypeCode", "ProgramTypeCode")
                        .WithMany()
                        .HasForeignKey("ProgramTypeCodeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HealthGateway.Database.Models.Form", b =>
                {
                    b.HasOne("HealthGateway.Database.Models.DrugProduct")
                        .WithOne("Form")
                        .HasForeignKey("HealthGateway.Database.Models.Form", "DrugProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HealthGateway.Database.Models.Packaging", b =>
                {
                    b.HasOne("HealthGateway.Database.Models.DrugProduct")
                        .WithOne("Packaging")
                        .HasForeignKey("HealthGateway.Database.Models.Packaging", "DrugProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HealthGateway.Database.Models.PharmaCareDrug", b =>
                {
                    b.HasOne("HealthGateway.Database.Models.FileDownload", "FileDownload")
                        .WithMany()
                        .HasForeignKey("FileDownloadId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HealthGateway.Database.Models.PharmaceuticalStd", b =>
                {
                    b.HasOne("HealthGateway.Database.Models.DrugProduct", "DrugProduct")
                        .WithOne("PharmaceuticalStd")
                        .HasForeignKey("HealthGateway.Database.Models.PharmaceuticalStd", "DrugProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HealthGateway.Database.Models.Route", b =>
                {
                    b.HasOne("HealthGateway.Database.Models.DrugProduct")
                        .WithOne("Route")
                        .HasForeignKey("HealthGateway.Database.Models.Route", "DrugProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HealthGateway.Database.Models.Status", b =>
                {
                    b.HasOne("HealthGateway.Database.Models.DrugProduct")
                        .WithMany("Statuses")
                        .HasForeignKey("DrugProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HealthGateway.Database.Models.TherapeuticClass", b =>
                {
                    b.HasOne("HealthGateway.Database.Models.DrugProduct")
                        .WithOne("TherapeuticClass")
                        .HasForeignKey("HealthGateway.Database.Models.TherapeuticClass", "DrugProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HealthGateway.Database.Models.VeterinarySpecies", b =>
                {
                    b.HasOne("HealthGateway.Database.Models.DrugProduct")
                        .WithOne("VeterinarySpecies")
                        .HasForeignKey("HealthGateway.Database.Models.VeterinarySpecies", "DrugProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}