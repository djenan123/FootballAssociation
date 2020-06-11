﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Transfermarkt.WebAPI.Database;

namespace Transfermarkt.WebAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20200407134957_StadiumClass")]
    partial class StadiumClass
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Transfermarkt.WebAPI.Database.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int>("PostalCode");

                    b.HasKey("Id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("Transfermarkt.WebAPI.Database.Club", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Abbreviation");

                    b.Property<int>("CityId");

                    b.Property<DateTime>("Founded");

                    b.Property<int>("LeagueId");

                    b.Property<byte[]>("Logo");

                    b.Property<int>("MarketValue");

                    b.Property<string>("Name");

                    b.Property<string>("Nickname");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("LeagueId");

                    b.ToTable("Clubs");
                });

            modelBuilder.Entity("Transfermarkt.WebAPI.Database.League", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Established");

                    b.Property<string>("Name");

                    b.Property<string>("Organizer");

                    b.HasKey("Id");

                    b.ToTable("Leagues");
                });

            modelBuilder.Entity("Transfermarkt.WebAPI.Database.Club", b =>
                {
                    b.HasOne("Transfermarkt.WebAPI.Database.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Transfermarkt.WebAPI.Database.League", "League")
                        .WithMany()
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
