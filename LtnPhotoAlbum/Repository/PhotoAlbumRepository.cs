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

        public async Task<List<Photo>> GetAllPhotos()
        {
            List<Photo> photos = null;

            try
            {
                var response = await _client.GetAsync(_client.BaseAddress);
                if (response.IsSuccessStatusCode)
                {
                    photos = await response.Content.ReadAsAsync<List<Photo>>();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                var innerException = e.InnerException;
                if (innerException != null)
                {
                    Debug.Print(innerException.Message);
                }
            }

            return photos;
        }

        public async Task<List<Photo>> GetPhotosByAlbumId(ushort albumId)
        {
            List<Photo> photos = null;

            try
            {
                var response = await _client.GetAsync(_client.BaseAddress + "?albumId=" + albumId);
                if (response.IsSuccessStatusCode)
                {
                    photos = await response.Content.ReadAsAsync<List<Photo>>();
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                var innerException = e.InnerException;
                if (innerException != null)
                {
                    Debug.Print(innerException.Message);
                }
            }

            return photos;
        }
    }
}