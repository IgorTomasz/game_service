﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using game_service.context;

#nullable disable

namespace game_service.Migrations
{
    [DbContext(typeof(GameDatabaseContext))]
    [Migration("20241216211153_additional_fields_in_gameSession")]
    partial class additional_fields_in_gameSession
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("game_service.models.GameAction", b =>
                {
                    b.Property<Guid>("GameActionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ActionData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("GameSessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("GameActionId");

                    b.HasIndex("GameSessionId");

                    b.ToTable("GameActions");
                });

            modelBuilder.Entity("game_service.models.GameHistory", b =>
                {
                    b.Property<Guid>("GameHistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("BetAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("GameSessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("GameType")
                        .HasColumnType("int");

                    b.Property<decimal>("MaxMultiplier")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Result")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("GameHistoryId");

                    b.HasIndex("GameSessionId");

                    b.ToTable("GameHistory");
                });

            modelBuilder.Entity("game_service.models.GameSession", b =>
                {
                    b.Property<Guid>("GameSessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("BetAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("CashWon")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("CashedOutEarly")
                        .HasColumnType("bit");

                    b.Property<decimal>("CurrentMultiplier")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("GameHistoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("GameType")
                        .HasColumnType("int");

                    b.Property<int?>("Result")
                        .HasColumnType("int");

                    b.Property<string>("SerializedGame")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserSessionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("GameSessionId");

                    b.HasIndex("GameHistoryId");

                    b.ToTable("GameSessions");
                });

            modelBuilder.Entity("game_service.models.GameAction", b =>
                {
                    b.HasOne("game_service.models.GameSession", "GameSession")
                        .WithMany("GameActions")
                        .HasForeignKey("GameSessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameSession");
                });

            modelBuilder.Entity("game_service.models.GameHistory", b =>
                {
                    b.HasOne("game_service.models.GameSession", "GameSession")
                        .WithMany()
                        .HasForeignKey("GameSessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameSession");
                });

            modelBuilder.Entity("game_service.models.GameSession", b =>
                {
                    b.HasOne("game_service.models.GameHistory", "History")
                        .WithMany()
                        .HasForeignKey("GameHistoryId");

                    b.Navigation("History");
                });

            modelBuilder.Entity("game_service.models.GameSession", b =>
                {
                    b.Navigation("GameActions");
                });
#pragma warning restore 612, 618
        }
    }
}
