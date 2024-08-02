using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class ProductGenre
{
    public int ProductGenreId { get; set; }

    public int? GenreId { get; set; }

    public int? ProductId { get; set; }

    public virtual Genre? Genre { get; set; }

    public virtual Product? Product { get; set; }
}
