﻿using FatecLibrary.Web.Models.Entities;
using FatecLibrary.Web.Services.Interfaces;
using System.Text;
using System.Text.Json;


namespace FatecLibrary.Web.Services.Entities;
public class PublishingService : IPublisingService
{

    private readonly IHttpClientFactory _clientFactory;
    private readonly JsonSerializerOptions _options;
    private const string apiEndpoint = "/api/publishing/"; // ver isso aqui
    private PublishingViewModel _publishingViewModel;
    private IEnumerable<PublishingViewModel> publishers;

    public PublishingService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<IEnumerable<PublishingViewModel>> GetAllPublishers()
    {
        var client = _clientFactory.CreateClient("BookAPI");


        var response = await client.GetAsync(apiEndpoint);

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadAsStreamAsync();
            publishers = await JsonSerializer
                .DeserializeAsync<IEnumerable<PublishingViewModel>>(apiResponse, _options);

        }
        else
            return null;

        return publishers;
    }

    public async Task<PublishingViewModel> FindPublishingById(int id)
    {
        var client = _clientFactory.CreateClient("BookAPI");

        using (var response = await client.GetAsync(apiEndpoint + id))
        {
            if (response.IsSuccessStatusCode && response.Content is not null)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                _publishingViewModel = await JsonSerializer.DeserializeAsync<PublishingViewModel>(apiResponse, _options);
            }
            else
                return null;

        }
        return _publishingViewModel;
    }

    public async Task<PublishingViewModel> CreatePublishing(PublishingViewModel publishingVM)
    {
        var client = _clientFactory.CreateClient("BookAPI");

        StringContent content = new StringContent(JsonSerializer.Serialize(publishingVM),
                Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync(apiEndpoint, content))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                _publishingViewModel = await JsonSerializer.DeserializeAsync<PublishingViewModel>(apiResponse, _options);
            }
            else
                return null;
        }
        return _publishingViewModel;
    }

    public async Task<PublishingViewModel> UpdatePublishing(PublishingViewModel publishingVM)
    {
        var client = _clientFactory.CreateClient("BookAPI");

        PublishingViewModel publishingUpdate = new PublishingViewModel();

        using (var response = await client.PutAsJsonAsync(apiEndpoint, publishingVM))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                publishingUpdate = await JsonSerializer.DeserializeAsync<PublishingViewModel>(apiResponse, _options);
            }
            else
                return null;
        }

        return publishingUpdate;
    }

    public async Task<bool> DeletePublishingById(int id)
    {
        var client = _clientFactory.CreateClient("BookAPI");

        using (var response = await client.DeleteAsync(apiEndpoint + id))
        {
            if (response.IsSuccessStatusCode) return true;
        }
        return false;
    }
}
