using System;
using System.Collections.Generic;
using Blog__Net.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog__Net.Data;

public partial class DbBlogContext : DbContext
{

    public DbBlogContext(DbContextOptions<DbBlogContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comments> Comments { get; set; }

    public virtual DbSet<InfoUser> InfoUsers { get; set; }

    public virtual DbSet<Posts> Posts { get; set; }

    public virtual DbSet<PostLike> PostLike { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comments>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCA00CE2FF2");

            entity.ToTable(tb => tb.HasTrigger("tr_DeleteCommentson"));

            entity.Property(e => e.Content).IsUnicode(false);
            entity.Property(e => e.Creationdate).HasColumnType("datetime");

            entity.HasOne(d => d.Commentparent).WithMany(p => p.InverseCommentparent)
                .HasForeignKey(d => d.CommentparentId)
                .HasConstraintName("fk_Posts_CommentparentId");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_Comments_IdUser");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_Comments_PostId");
        });

        modelBuilder.Entity<InfoUser>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__InfoUser__B7C926389D8E24CE");

            entity.ToTable("InfoUser");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Passcode).IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Rol).WithMany(p => p.InfoUsers)
                .HasForeignKey(d => d.RolId)
                .HasConstraintName("fk_InfoUser_Roles");
        });

        modelBuilder.Entity<Posts>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA126018E263DA79");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Content).IsUnicode(false);
            entity.Property(e => e.Publicationdate).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__Roles__F92302F11E40ABEB");

            entity.Property(e => e.RolName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
