using System.Collections.Generic;
using System.Threading.Tasks;
using LtnPhotoAlbum.Model;
using LtnPhotoAlbum.Repository;
using NUnit.Framework;

namespace LtnPhotoAlbum.UnitTests
{
    [TestFixture]
    public class PhotoAlbumRepositoryTests
    {
        private PhotoAlbumRepository _photoAlbumRepository;

        [SetUp]
        public void SetUp()
        {
            _photoAlbumRepository = new PhotoAlbumRepository();
        }

        [Test]
        public async Task GetAllPhotose_ReturnsAllPhotos()
        {
            var expected = typeof(List<Photo>);

            var result = await _photoAlbumRepository.GetAllPhotos();

            Assert.That(result, Is.InstanceOf(expected));
        }

        [Test]
        public async Task GetPhotosByAlbumId_ReturnsPhotosByAlbumId()
        {
            var expected = typeof(List<Photo>);

            var result = await _photoAlbumRepository.GetPhotosByAlbumId(1);

            Assert.That(result, Is.InstanceOf(expected));
        }
    }
}