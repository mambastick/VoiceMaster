﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VoiceMaster.Database;

#nullable disable

namespace VoiceMaster.Database.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240411212436_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("VoiceMaster.Models.SetupChannel", b =>
                {
                    b.Property<ulong>("ChannelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("ChannelId");

                    b.ToTable("SetupChannels");
                });

            modelBuilder.Entity("VoiceMaster.Models.TempChannel", b =>
                {
                    b.Property<ulong>("ChannelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("SetupChannelId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("ChannelId");

                    b.HasIndex("SetupChannelId");

                    b.ToTable("TempChannels");
                });

            modelBuilder.Entity("VoiceMaster.Models.TempChannel", b =>
                {
                    b.HasOne("VoiceMaster.Models.SetupChannel", "SetupChannel")
                        .WithMany("TempChannels")
                        .HasForeignKey("SetupChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SetupChannel");
                });

            modelBuilder.Entity("VoiceMaster.Models.SetupChannel", b =>
                {
                    b.Navigation("TempChannels");
                });
#pragma warning restore 612, 618
        }
    }
}
