using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class Publisher
{
    public int PublisherId { get; set; }

    public string PublisherName { get; set; } = null!;

    public virtual ICollection<ProductPublisher> ProductPublishers { get; set; } = new List<ProductPublisher>();
}
