using DrustvenaPlatformaVideoIgara.Models;

namespace DrustvenaPlatformaVideoIgara.ViewModels
{
    public class GenreAndPlatformViewModel
    {
        public List<Genre> Genres { get; set; }
        public List<Platform> Platforms { get; set; }
        public List<Developer> Developers { get; set; }
        public List<Publisher> Publishers { get; set; }
    }
}
