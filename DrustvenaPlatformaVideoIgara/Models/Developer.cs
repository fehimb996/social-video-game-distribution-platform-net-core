using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class Developer
{
    public int DeveloperId { get; set; }

    public string DeveloperName { get; set; } = null!;

    public virtual ICollection<ProductDeveloper> ProductDevelopers { get; set; } = new List<ProductDeveloper>();
}
