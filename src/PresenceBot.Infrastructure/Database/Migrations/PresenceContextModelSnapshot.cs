﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PresenceBot.Infrastructure.Database;

#nullable disable

namespace PresenceBot.Infrastructure.Database.Migrations
{
    [DbContext(typeof(PresenceContext))]
    partial class PresenceContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.7");

            modelBuilder.Entity("PresenceBot.Infrastructure.Database.Models.ClientPresenceModel", b =>
                {
                    b.Property<string>("Identity")
                        .HasColumnType("TEXT");

                    b.Property<string>("IdentityComponents")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("LastPresentedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Identity");

                    b.ToTable("Clients");
                });
#pragma warning restore 612, 618
        }
    }
}