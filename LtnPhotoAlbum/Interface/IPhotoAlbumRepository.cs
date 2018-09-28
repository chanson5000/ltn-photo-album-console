using System.Collections.Generic;
using System.Threading.Tasks;
using LtnPhotoAlbum.Model;

namespace LtnPhotoAlbum.Interface
{
    public interface IPhotoAlbumRepository
    {
        Task<List<Photo>> GetAllPhotos();
        Task<List<Photo>> GetPhotosByAlbumId(int albumId);
    }
}