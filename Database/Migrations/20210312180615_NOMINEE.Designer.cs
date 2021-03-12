﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20210312180615_NOMINEE")]
    partial class NOMINEE
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Database.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<int?>("EmpId")
                        .HasColumnType("int");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TcNid")
                        .HasColumnType("int");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EmpId");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("TcNid");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Database.tblBankMaster", b =>
                {
                    b.Property<int>("BankId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BankName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.HasKey("BankId");

                    b.ToTable("tblBankMaster");
                });

            modelBuilder.Entity("Database.tblCaptcha", b =>
                {
                    b.Property<string>("SaltId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CaptchaCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDt")
                        .HasColumnType("datetime2");

                    b.HasKey("SaltId");

                    b.ToTable("tblCaptcha");
                });

            modelBuilder.Entity("Database.tblCountryMaster", b =>
                {
                    b.Property<int>("CountryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Capital")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CountryCode")
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("CountryCodeIso2")
                        .HasMaxLength(2)
                        .HasColumnType("nvarchar(2)");

                    b.Property<string>("CountryName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CurrencySymbol")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Domain")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Native")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Region")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubRegion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("currency")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CountryId");

                    b.HasIndex("CountryName")
                        .IsUnique()
                        .HasFilter("[CountryName] IS NOT NULL");

                    b.ToTable("tblCountryMaster");
                });

            modelBuilder.Entity("Database.tblEmpMaster", b =>
                {
                    b.Property<int>("EmpId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EmpCode")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("IsTerminate")
                        .HasColumnType("bit");

                    b.Property<DateTime>("JoiningDt")
                        .HasColumnType("datetime2");

                    b.HasKey("EmpId");

                    b.ToTable("tblEmpMaster");
                });

            modelBuilder.Entity("Database.tblKycMaster", b =>
                {
                    b.Property<int>("DetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ApprovalRemarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ApprovedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ApprovedDt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("IdDocumentName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IdDocumentNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdProofType")
                        .HasColumnType("int");

                    b.Property<byte>("IsApproved")
                        .HasColumnType("tinyint");

                    b.Property<bool>("Isdeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TcNid")
                        .HasColumnType("int");

                    b.HasKey("DetailId");

                    b.HasIndex("TcNid");

                    b.ToTable("tblKycMaster");
                });

            modelBuilder.Entity("Database.tblRegistration", b =>
                {
                    b.Property<int>("Nid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Dob")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("Husband_father_name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("IsKycUpdated")
                        .HasColumnType("int");

                    b.Property<bool>("IsTerminate")
                        .HasColumnType("bit");

                    b.Property<bool>("Isblock")
                        .HasColumnType("bit");

                    b.Property<DateTime>("JoiningDt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("JoiningState")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("SpId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SpLegNumber")
                        .HasColumnType("int");

                    b.Property<int?>("SpNid")
                        .HasColumnType("int");

                    b.Property<int>("TCRanks")
                        .HasColumnType("int");

                    b.HasKey("Nid");

                    b.HasIndex("Id");

                    b.HasIndex("JoiningState");

                    b.HasIndex("SpNid");

                    b.ToTable("tblRegistration");
                });

            modelBuilder.Entity("Database.tblStateMaster", b =>
                {
                    b.Property<int>("StateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUT")
                        .HasColumnType("bit");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateCode")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("StateName")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<decimal>("latitude")
                        .HasColumnType("decimal(18,14)");

                    b.Property<decimal>("longitude")
                        .HasColumnType("decimal(18,14)");

                    b.HasKey("StateId");

                    b.HasIndex("CountryId", "StateName")
                        .IsUnique()
                        .HasFilter("[CountryId] IS NOT NULL AND [StateName] IS NOT NULL");

                    b.ToTable("tblStateMaster");
                });

            modelBuilder.Entity("Database.tblTcAddressDetail", b =>
                {
                    b.Property<int>("DetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AddressType")
                        .HasColumnType("int");

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Pincode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StateId")
                        .HasColumnType("int");

                    b.Property<int?>("TcNid")
                        .HasColumnType("int");

                    b.Property<string>("address_line1")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("address_line2")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("DetailId");

                    b.HasIndex("CountryId");

                    b.HasIndex("StateId");

                    b.HasIndex("TcNid");

                    b.ToTable("tblTcAddressDetail");
                });

            modelBuilder.Entity("Database.tblTcBankDetails", b =>
                {
                    b.Property<int>("DetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountNo")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ApprovalRemarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ApprovedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ApprovedDt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("BankId")
                        .HasColumnType("int");

                    b.Property<string>("BranchAddress")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("IFSC")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<byte>("IsApproved")
                        .HasColumnType("tinyint");

                    b.Property<bool>("Isdeleted")
                        .HasColumnType("bit");

                    b.Property<string>("NameasonBank")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TcNid")
                        .HasColumnType("int");

                    b.Property<string>("UploadImages")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("DetailId");

                    b.HasIndex("BankId");

                    b.HasIndex("TcNid");

                    b.ToTable("tblTcBankDetails");
                });

            modelBuilder.Entity("Database.tblTcNominee", b =>
                {
                    b.Property<int>("DetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Isdeleted")
                        .HasColumnType("bit");

                    b.Property<string>("NomineeName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("NomineeRelation")
                        .HasColumnType("int");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TcNid")
                        .HasColumnType("int");

                    b.HasKey("DetailId");

                    b.HasIndex("TcNid");

                    b.ToTable("TblTcNominee");
                });

            modelBuilder.Entity("Database.tblTcPanDetails", b =>
                {
                    b.Property<int>("DetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ApprovalRemarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ApprovedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ApprovedDt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDt")
                        .HasColumnType("datetime2");

                    b.Property<byte>("IsApproved")
                        .HasColumnType("tinyint");

                    b.Property<bool>("Isdeleted")
                        .HasColumnType("bit");

                    b.Property<string>("PANName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PANNo")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TcNid")
                        .HasColumnType("int");

                    b.Property<string>("UploadImages")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("DetailId");

                    b.HasIndex("TcNid");

                    b.ToTable("TblTcPanDetails");
                });

            modelBuilder.Entity("Database.tblTcRanksDetails", b =>
                {
                    b.Property<int>("DetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Isdeleted")
                        .HasColumnType("bit");

                    b.Property<double>("PPDone")
                        .HasColumnType("float");

                    b.Property<double>("PPRequired")
                        .HasColumnType("float");

                    b.Property<DateTime>("QualifyDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TCRanks")
                        .HasColumnType("int");

                    b.Property<int?>("TcNid")
                        .HasColumnType("int");

                    b.HasKey("DetailId");

                    b.HasIndex("TcNid");

                    b.ToTable("tblTcRanksDetails");
                });

            modelBuilder.Entity("Database.tblTcSequcence", b =>
                {
                    b.Property<int>("TcSequcence")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CurrentSeq")
                        .HasColumnType("int");

                    b.Property<int>("Monthyear")
                        .HasColumnType("int");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<int>("StateId")
                        .HasColumnType("int");

                    b.HasKey("TcSequcence");

                    b.HasIndex("Monthyear", "StateId")
                        .IsUnique();

                    b.ToTable("tblTcSequcence");
                });

            modelBuilder.Entity("Database.tblTree", b =>
                {
                    b.Property<int>("TreeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("TcNid")
                        .HasColumnType("int");

                    b.Property<int>("TcSpNid")
                        .HasColumnType("int");

                    b.HasKey("TreeId");

                    b.HasIndex("TcNid");

                    b.HasIndex("TcSpNid");

                    b.ToTable("tblTree");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Database.ApplicationUser", b =>
                {
                    b.HasOne("Database.tblEmpMaster", "tblEmpMaster")
                        .WithMany()
                        .HasForeignKey("EmpId");

                    b.HasOne("Database.tblRegistration", "tblRegistration")
                        .WithMany()
                        .HasForeignKey("TcNid");

                    b.Navigation("tblEmpMaster");

                    b.Navigation("tblRegistration");
                });

            modelBuilder.Entity("Database.tblKycMaster", b =>
                {
                    b.HasOne("Database.tblRegistration", "tblRegistration")
                        .WithMany()
                        .HasForeignKey("TcNid");

                    b.Navigation("tblRegistration");
                });

            modelBuilder.Entity("Database.tblRegistration", b =>
                {
                    b.HasOne("Database.tblStateMaster", "tblStateMaster")
                        .WithMany()
                        .HasForeignKey("JoiningState");

                    b.HasOne("Database.tblRegistration", "tblRegistrationSponsor")
                        .WithMany()
                        .HasForeignKey("SpNid");

                    b.Navigation("tblRegistrationSponsor");

                    b.Navigation("tblStateMaster");
                });

            modelBuilder.Entity("Database.tblStateMaster", b =>
                {
                    b.HasOne("Database.tblCountryMaster", "tblCountryMaster")
                        .WithMany()
                        .HasForeignKey("CountryId");

                    b.Navigation("tblCountryMaster");
                });

            modelBuilder.Entity("Database.tblTcAddressDetail", b =>
                {
                    b.HasOne("Database.tblCountryMaster", "tblCountryMaster")
                        .WithMany()
                        .HasForeignKey("CountryId");

                    b.HasOne("Database.tblStateMaster", "tblStateMaster")
                        .WithMany()
                        .HasForeignKey("StateId");

                    b.HasOne("Database.tblRegistration", "tblRegistration")
                        .WithMany()
                        .HasForeignKey("TcNid");

                    b.Navigation("tblCountryMaster");

                    b.Navigation("tblRegistration");

                    b.Navigation("tblStateMaster");
                });

            modelBuilder.Entity("Database.tblTcBankDetails", b =>
                {
                    b.HasOne("Database.tblBankMaster", "tblBankMaster")
                        .WithMany()
                        .HasForeignKey("BankId");

                    b.HasOne("Database.tblRegistration", "tblRegistration")
                        .WithMany()
                        .HasForeignKey("TcNid");

                    b.Navigation("tblBankMaster");

                    b.Navigation("tblRegistration");
                });

            modelBuilder.Entity("Database.tblTcNominee", b =>
                {
                    b.HasOne("Database.tblRegistration", "tblRegistration")
                        .WithMany()
                        .HasForeignKey("TcNid");

                    b.Navigation("tblRegistration");
                });

            modelBuilder.Entity("Database.tblTcPanDetails", b =>
                {
                    b.HasOne("Database.tblRegistration", "tblRegistration")
                        .WithMany()
                        .HasForeignKey("TcNid");

                    b.Navigation("tblRegistration");
                });

            modelBuilder.Entity("Database.tblTcRanksDetails", b =>
                {
                    b.HasOne("Database.tblRegistration", "tblRegistration")
                        .WithMany()
                        .HasForeignKey("TcNid");

                    b.Navigation("tblRegistration");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Database.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Database.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Database.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Database.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
