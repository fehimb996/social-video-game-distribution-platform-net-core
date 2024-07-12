using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int UserId1 { get; set; }

    public int UserId2 { get; set; }

    public string MessageContent { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public virtual User UserId1Navigation { get; set; } = null!;

    public virtual User UserId2Navigation { get; set; } = null!;
}
