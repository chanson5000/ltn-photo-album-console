using System;
using System.Collections.Generic;
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
        }

        public async Task<List<Photo>> GetAllPhotos()
        {
            List<Photo> photos = null;
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                photos = await response.Content.ReadAsAsync<List<Photo>>();
            }

            return photos;
        }

        public async Task<List<Photo>> GetPhotosByAlbumId(int albumId)
        {
            List<Photo> photos = null;
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "?albumId=" + albumId);
            if (response.IsSuccessStatusCode)
            {
                photos = await response.Content.ReadAsAsync<List<Photo>>();
            }

            return photos;
        }
    }
}