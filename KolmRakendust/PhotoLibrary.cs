using System.Collections.Generic;

namespace KolmRakendust
{
    public class PhotoAlbum
    {
        public string Name { get; set; }
        public List<string> Photos { get; set; }

        public PhotoAlbum()
        {
            Photos = new List<string>();
        }
    }

    public class PhotoLibrary
    {
        public List<PhotoAlbum> Albums { get; set; }
        public List<string> Favorites { get; set; }

        public PhotoLibrary()
        {
            Albums = new List<PhotoAlbum>();
            Favorites = new List<string>();
        }
    }
}
