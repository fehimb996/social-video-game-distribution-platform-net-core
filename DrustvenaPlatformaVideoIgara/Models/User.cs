using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class User
{
    public int UserId { get; set; }

    public string NickName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? ProfilePicture { get; set; }

    public string? ProfileDescription { get; set; }

    public int? CountryId { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<Friend> FriendUserId1Navigations { get; set; } = new List<Friend>();

    public virtual ICollection<Friend> FriendUserId2Navigations { get; set; } = new List<Friend>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Message> MessageUserId1Navigations { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageUserId2Navigations { get; set; } = new List<Message>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
