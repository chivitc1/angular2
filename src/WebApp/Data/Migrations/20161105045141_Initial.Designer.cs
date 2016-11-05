using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApp.Data;

namespace WebApp.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20161105045141_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebApp.Data.Comments.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("Flags");

                    b.Property<int>("ItemId");

                    b.Property<DateTime>("LastModifiedDate");

                    b.Property<int?>("ParentId");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("ParentId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("WebApp.Data.Items.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description");

                    b.Property<int>("Flags");

                    b.Property<DateTime>("LastModifiedDate");

                    b.Property<string>("Notes");

                    b.Property<string>("Text");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.Property<int>("ViewCount");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("WebApp.Data.Users.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<int>("Flags");

                    b.Property<DateTime>("LastModifiedDate");

                    b.Property<string>("Notes");

                    b.Property<int>("Type");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 128);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebApp.Data.Comments.Comment", b =>
                {
                    b.HasOne("WebApp.Data.Items.Item", "Item")
                        .WithMany("Comments")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApp.Data.Comments.Comment", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.HasOne("WebApp.Data.Users.ApplicationUser", "Author")
                        .WithMany("Comments")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("WebApp.Data.Items.Item", b =>
                {
                    b.HasOne("WebApp.Data.Users.ApplicationUser", "Author")
                        .WithMany("Items")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
