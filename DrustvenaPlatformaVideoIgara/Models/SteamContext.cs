using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class SteamContext : DbContext
{
    public SteamContext()
    {
    }

    public SteamContext(DbContextOptions<SteamContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Picture> Pictures { get; set; }

    public virtual DbSet<Platform> Platforms { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductDeveloper> ProductDevelopers { get; set; }

    public virtual DbSet<ProductGenre> ProductGenres { get; set; }

    public virtual DbSet<ProductPlatform> ProductPlatforms { get; set; }

    public virtual DbSet<ProductPublisher> ProductPublishers { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    public virtual DbSet<WishlistItem> WishlistItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__Country__10D160BFD678C330");

            entity.ToTable("Country");

            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.CountryName).HasMaxLength(30);
        });

        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.DeveloperId).HasName("PK__Develope__DE084CD1B88AEA86");

            entity.ToTable("Developer");

            entity.Property(e => e.DeveloperId).HasColumnName("DeveloperID");
            entity.Property(e => e.DeveloperName).HasMaxLength(40);
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => e.FriendId).HasName("PK__Friend__A2CF6563182AF1D4");

            entity.ToTable("Friend");

            entity.Property(e => e.FriendId).HasColumnName("FriendID");
            entity.Property(e => e.UserId1).HasColumnName("UserID1");
            entity.Property(e => e.UserId2).HasColumnName("UserID2");

            entity.HasOne(d => d.UserId1Navigation).WithMany(p => p.FriendUserId1Navigations)
                .HasForeignKey(d => d.UserId1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friend_UserID1");

            entity.HasOne(d => d.UserId2Navigation).WithMany(p => p.FriendUserId2Navigations)
                .HasForeignKey(d => d.UserId2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friend_UserID2");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genre__0385055EB75D8AB6");

            entity.ToTable("Genre");

            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.GenreName).HasMaxLength(40);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__D796AAD5011FC926");

            entity.ToTable("Invoice");

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.DateIssued).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK_Invoice_PaymentMethod");

            entity.HasOne(d => d.User).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_User");
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.InvoiceDetailsId).HasName("PK__InvoiceD__9F18B3E5A316237F");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdatePrice"));

            entity.Property(e => e.InvoiceDetailsId).HasColumnName("InvoiceDetailsID");
            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK_InvoiceDetails_Invoice");

            entity.HasOne(d => d.Product).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_InvoiceDetails_Product");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Message__C87C037C5F6BC994");

            entity.ToTable("Message");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.MessageContent).HasMaxLength(500);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
            entity.Property(e => e.UserId1).HasColumnName("UserID1");
            entity.Property(e => e.UserId2).HasColumnName("UserID2");

            entity.HasOne(d => d.UserId1Navigation).WithMany(p => p.MessageUserId1Navigations)
                .HasForeignKey(d => d.UserId1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_UserID1");

            entity.HasOne(d => d.UserId2Navigation).WithMany(p => p.MessageUserId2Navigations)
                .HasForeignKey(d => d.UserId2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEssage_UserID2");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1F33D93FCA2");

            entity.ToTable("PaymentMethod");

            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.Method).HasMaxLength(40);
        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => e.PictureId).HasName("PK__Picture__8C2866F83C9ECF6C");

            entity.ToTable("Picture");

            entity.Property(e => e.PictureId).HasColumnName("PictureID");
            entity.Property(e => e.PictureName).HasMaxLength(100);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.Pictures)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Picture_Product");
        });

        modelBuilder.Entity<Platform>(entity =>
        {
            entity.HasKey(e => e.PlatformId).HasName("PK__Platform__F559F6DAD4D4D084");

            entity.ToTable("Platform");

            entity.Property(e => e.PlatformId).HasColumnName("PlatformID");
            entity.Property(e => e.PlatformName).HasMaxLength(40);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6ED154E565D");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(70);
        });

        modelBuilder.Entity<ProductDeveloper>(entity =>
        {
            entity.HasKey(e => e.ProductDeveloperId).HasName("PK__ProductD__B0619AA2C50AEBF3");

            entity.ToTable("ProductDeveloper");

            entity.Property(e => e.ProductDeveloperId).HasColumnName("ProductDeveloperID");
            entity.Property(e => e.DeveloperId).HasColumnName("DeveloperID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Developer).WithMany(p => p.ProductDevelopers)
                .HasForeignKey(d => d.DeveloperId)
                .HasConstraintName("FK_ProductDeveloper_Developer");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductDevelopers)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductDeveloper_Product");
        });

        modelBuilder.Entity<ProductGenre>(entity =>
        {
            entity.HasKey(e => e.ProductGenreId).HasName("PK__ProductG__02B490791124D500");

            entity.ToTable("ProductGenre");

            entity.Property(e => e.ProductGenreId).HasColumnName("ProductGenreID");
            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Genre).WithMany(p => p.ProductGenres)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK_ProductGenre_Genre");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductGenres)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductGenre_Product");
        });

        modelBuilder.Entity<ProductPlatform>(entity =>
        {
            entity.HasKey(e => e.ProductPlatformId).HasName("PK__ProductP__0CD1A56CB0529CE9");

            entity.ToTable("ProductPlatform");

            entity.Property(e => e.ProductPlatformId).HasColumnName("ProductPlatformID");
            entity.Property(e => e.PlatformId).HasColumnName("PlatformID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Platform).WithMany(p => p.ProductPlatforms)
                .HasForeignKey(d => d.PlatformId)
                .HasConstraintName("FK_ProductPlatform_Platform");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPlatforms)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductPlatform_Product");
        });

        modelBuilder.Entity<ProductPublisher>(entity =>
        {
            entity.HasKey(e => e.ProductPublisherId).HasName("PK__ProductP__32D21D4CD26D82B6");

            entity.ToTable("ProductPublisher");

            entity.Property(e => e.ProductPublisherId).HasColumnName("ProductPublisherID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.PublisherId).HasColumnName("PublisherID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPublishers)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductPublisher_Product");

            entity.HasOne(d => d.Publisher).WithMany(p => p.ProductPublishers)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK_ProductPublisher_Publisher");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__Publishe__4C657E4B272BA05E");

            entity.ToTable("Publisher");

            entity.Property(e => e.PublisherId).HasColumnName("PublisherID");
            entity.Property(e => e.PublisherName).HasMaxLength(40);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Review__74BC79AEC888C84B");

            entity.ToTable("Review");

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Review_Product");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC5C4EFC72");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(40);
            entity.Property(e => e.LastName).HasMaxLength(40);
            entity.Property(e => e.NickName).HasMaxLength(50);
            entity.Property(e => e.ProfileDescription).HasMaxLength(300);

            entity.HasOne(d => d.Country).WithMany(p => p.Users)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_User_Country");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.WalletId).HasName("PK__Wallet__84D4F92E7E05EA4C");

            entity.ToTable("Wallet");

            entity.Property(e => e.WalletId).HasColumnName("WalletID");
            entity.Property(e => e.Balance).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Wallet_User");
        });

        modelBuilder.Entity<WalletTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__WalletTr__55433A4B374ECDEA");

            entity.ToTable("WalletTransaction");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TransactionType).HasMaxLength(20);
            entity.Property(e => e.WalletId).HasColumnName("WalletID");

            entity.HasOne(d => d.Wallet).WithMany(p => p.WalletTransactions)
                .HasForeignKey(d => d.WalletId)
                .HasConstraintName("FK_WalletTransaction_Wallet");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("PK__Wishlist__233189CB5801D674");

            entity.ToTable("Wishlist");

            entity.Property(e => e.WishlistId).HasColumnName("WishlistID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Wishlist_User");
        });

        modelBuilder.Entity<WishlistItem>(entity =>
        {
            entity.HasKey(e => e.WishlistItemId).HasName("PK__Wishlist__171E2181D35D1B8D");

            entity.ToTable("WishlistItem");

            entity.Property(e => e.WishlistItemId).HasColumnName("WishlistItemID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.WishlistId).HasColumnName("WishlistID");

            entity.HasOne(d => d.Product).WithMany(p => p.WishlistItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_WishlistItem_Product");

            entity.HasOne(d => d.Wishlist).WithMany(p => p.WishlistItems)
                .HasForeignKey(d => d.WishlistId)
                .HasConstraintName("FK_WishlistItem_Wishlist");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
