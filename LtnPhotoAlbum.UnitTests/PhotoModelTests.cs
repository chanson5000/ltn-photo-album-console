using LtnPhotoAlbum.Model;
using NUnit.Framework;

namespace LtnPhotoAlbum.UnitTests
{
    [TestFixture]
    public class PhotoModelTests
    {
        [Test]
        public void SetId_ReturnsId()
        {
            var expected = 1;

            var photo = new Photo {Id = 1};

            var result = photo.Id;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void SetAlbumId_ReturnsAlbumId()
        {
            var expected = 1;

            var photo = new Photo {AlbumId = 1};

            var result = photo.AlbumId;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void SetTitle_ReturnsTitle()
        {
            var expected = "Photo Title";

            var photo = new Photo {Title = "Photo Title"};

            var result = photo.Title;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void SetUrl_ReturnsUrl()
        {
            var expected = "http://photourl.com/photo";

            var photo = new Photo {Url = "http://photourl.com/photo"};

            var result = photo.Url;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void SetThumbnailUrl_ReturnsThumbnailUrl()
        {
            var expected = "http://thumbnailurl.com/thumbnail";

            var photo = new Photo {ThumbnailUrl = "http://thumbnailurl.com/thumbnail"};

            var result = photo.ThumbnailUrl;

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
