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
            entity.HasKey(e => e.CountryId).HasName("PK__Country__10D160BF8F69179C");

            entity.ToTable("Country");

            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.CountryName).HasMaxLength(30);
        });

        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.DeveloperId).HasName("PK__Develope__DE084CD12979A3DA");

            entity.ToTable("Developer");

            entity.Property(e => e.DeveloperId).HasColumnName("DeveloperID");
            entity.Property(e => e.DeveloperName).HasMaxLength(40);

            entity.HasMany(d => d.Products).WithMany(p => p.Developers)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductDeveloper",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductDeveloper_Product"),
                    l => l.HasOne<Developer>().WithMany()
                        .HasForeignKey("DeveloperId")
                        .HasConstraintName("FK_ProductDeveloper_Developer"),
                    j =>
                    {
                        j.HasKey("DeveloperId", "ProductId");
                        j.ToTable("ProductDeveloper");
                        j.IndexerProperty<int>("DeveloperId").HasColumnName("DeveloperID");
                        j.IndexerProperty<int>("ProductId").HasColumnName("ProductID");
                    });
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => e.FriendId).HasName("PK__Friend__A2CF65630ADDEA45");

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
            entity.HasKey(e => e.GenreId).HasName("PK__Genre__0385055E5B1DDF01");

            entity.ToTable("Genre");

            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.GenreName).HasMaxLength(40);

            entity.HasMany(d => d.Products).WithMany(p => p.Genres)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductGenre",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductGenre_Product"),
                    l => l.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("FK_ProductGenre_Genre"),
                    j =>
                    {
                        j.HasKey("GenreId", "ProductId");
                        j.ToTable("ProductGenre");
                        j.IndexerProperty<int>("GenreId").HasColumnName("GenreID");
                        j.IndexerProperty<int>("ProductId").HasColumnName("ProductID");
                    });
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__D796AAD5D46D6872");

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
            entity.HasKey(e => e.InvoiceDetailsId).HasName("PK__InvoiceD__9F18B3E5D56CAB67");

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
            entity.HasKey(e => e.MessageId).HasName("PK__Message__C87C037CDCFCD7E9");

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
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1F353CE4CCA");

            entity.ToTable("PaymentMethod");

            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.Method).HasMaxLength(40);
        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => e.PictureId).HasName("PK__Picture__8C2866F8F89DF0DB");

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
            entity.HasKey(e => e.PlatformId).HasName("PK__Platform__F559F6DA022D7239");

            entity.ToTable("Platform");

            entity.Property(e => e.PlatformId).HasColumnName("PlatformID");
            entity.Property(e => e.PlatformName).HasMaxLength(40);

            entity.HasMany(d => d.Products).WithMany(p => p.Platforms)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductPlatform",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductPlatform_Product"),
                    l => l.HasOne<Platform>().WithMany()
                        .HasForeignKey("PlatformId")
                        .HasConstraintName("FK_ProductPlatform_Platform"),
                    j =>
                    {
                        j.HasKey("PlatformId", "ProductId");
                        j.ToTable("ProductPlatform");
                        j.IndexerProperty<int>("PlatformId").HasColumnName("PlatformID");
                        j.IndexerProperty<int>("ProductId").HasColumnName("ProductID");
                    });
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6EDB7A8DE34");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(70);
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__Publishe__4C657E4BAE57CD33");

            entity.ToTable("Publisher");

            entity.Property(e => e.PublisherId).HasColumnName("PublisherID");
            entity.Property(e => e.PublisherName).HasMaxLength(40);

            entity.HasMany(d => d.Products).WithMany(p => p.Publishers)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductPublisher",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductPublisher_Product"),
                    l => l.HasOne<Publisher>().WithMany()
                        .HasForeignKey("PublisherId")
                        .HasConstraintName("FK_ProductPublisher_Publisher"),
                    j =>
                    {
                        j.HasKey("PublisherId", "ProductId");
                        j.ToTable("ProductPublisher");
                        j.IndexerProperty<int>("PublisherId").HasColumnName("PublisherID");
                        j.IndexerProperty<int>("ProductId").HasColumnName("ProductID");
                    });
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Review__74BC79AE7ED2E55C");

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
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACB5FFB022");

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
            entity.HasKey(e => e.WalletId).HasName("PK__Wallet__84D4F92EB0A0ABD1");

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
            entity.HasKey(e => e.TransactionId).HasName("PK__WalletTr__55433A4B4ECE1556");

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
            entity.HasKey(e => e.WishlistId).HasName("PK__Wishlist__233189CB1D69C963");

            entity.ToTable("Wishlist");

            entity.Property(e => e.WishlistId).HasColumnName("WishlistID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Wishlist_User");
        });

        modelBuilder.Entity<WishlistItem>(entity =>
        {
            entity.HasKey(e => e.WishlistItemId).HasName("PK__Wishlist__171E2181C9227DD5");

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
