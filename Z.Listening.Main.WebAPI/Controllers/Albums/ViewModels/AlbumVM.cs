namespace Z.Listening.Main.WebAPI.Controllers.Albums.ViewModels
{
    public record AlbumVM(Guid Id, MultilingualString Name, Guid CategoryId)
    {
        public static AlbumVM? Create(Album? album)
        {
            if (album == null)
            {
                return null;
            }
            return new AlbumVM(album.Id, album.Name, album.CategoryId);
        }

        public static AlbumVM[] Create(Album[] albums)
        {
            return albums.Select(e => Create(e)).ToArray();
        }
    }
}
