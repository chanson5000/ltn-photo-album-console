using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LtnPhotoAlbum.Interface;
using LtnPhotoAlbum.Model;

namespace LtnPhotoAlbum.Repository
{
    public class PhotoAlbumRepository : IPhotoAlbumRepository
    {
        private readonly HttpClient _client;

        public PhotoAlbumRepository()
        {
            _client = new HttpClient { BaseAddress = new Uri("https://jsonplaceholder.typicode.com/photos") };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.Timeout = TimeSpan.FromMilliseconds(4000);
        }

        public async Task<List<Photo>> GetAllPhotos() => await GetPhotos();

        public async Task<List<Photo>> GetPhotosByAlbumId(ushort albumId) => await GetPhotos(albumId);

        private async Task<List<Photo>> GetPhotos(ushort? albumId = null)
        {
            List<Photo> photos = null;

            try
            {
                var response = albumId == null
                    ? await _client.GetAsync(_client.BaseAddress)
                    : await _client.GetAsync(_client.BaseAddress + "?albumId=" + albumId);

                if (response.IsSuccessStatusCode)
                {
                    photos = await response.Content.ReadAsAsync<List<Photo>>();
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                var innerException = e.InnerException;
                if (innerException == null) throw;
                throw innerException;
            }

            return photos;
        }
    }
}