using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

[DbContext(typeof(ChatDbContext))]
partial class ChatDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "10.0.9");

        modelBuilder.Entity("ChatEntry", entity =>
        {
            entity.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            entity.Property<string>("Content")
                .IsRequired()
                .HasColumnType("TEXT");

            entity.Property<string>("Role")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("TEXT");

            entity.Property<int>("SessionId")
                .HasColumnType("INTEGER");

            entity.Property<DateTime>("Timestamp")
                .HasColumnType("TEXT");

            entity.HasKey("Id");

            entity.HasIndex("SessionId", "Timestamp");

            entity.ToTable("Entries");
        });

        modelBuilder.Entity("ConversationSession", entity =>
        {
            entity.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            entity.Property<DateTime>("CreatedAt")
                .HasColumnType("TEXT");

            entity.Property<string>("ModelName")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("TEXT");

            entity.Property<string>("Title")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("TEXT");

            entity.HasKey("Id");

            entity.ToTable("Sessions");
        });

        modelBuilder.Entity("ChatEntry", entity =>
        {
            entity.HasOne("ConversationSession", "Session")
                .WithMany("Entries")
                .HasForeignKey("SessionId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            entity.Navigation("Session");
        });

        modelBuilder.Entity("ConversationSession", entity =>
        {
            entity.Navigation("Entries");
        });
#pragma warning restore 612, 618
    }
}
