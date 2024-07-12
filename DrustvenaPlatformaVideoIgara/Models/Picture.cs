using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class Picture
{
    public int PictureId { get; set; }

    public int ProductId { get; set; }

    public string PictureName { get; set; } = null!;

    public string ImagePath { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
