using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public bool? Rating { get; set; }

    public string? Comment { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
