using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class ProductPublisher
{
    public int ProductPublisherId { get; set; }

    public int PublisherId { get; set; }

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Publisher Publisher { get; set; } = null!;
}
