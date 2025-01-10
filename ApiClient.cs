using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;

namespace URLShortener_client
{
    public class ApiClient : IApiClient
    {
        protected readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<IApiClient> _logger;

        public ApiClient(IHttpClientFactory clientFactory, ILogger<IApiClient> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }


        public async Task<ResponseModel> GetResponse(string arg)
        {
            
            var endpointUrl = new Uri($"/shorten/" + arg, UriKind.Relative);
            

            _logger.LogInformation("Requesting data from {Endpoint}", endpointUrl);

            try
            {
                using (var client = _clientFactory.CreateClient("ApiClient"))
                {
                    
                    var response = await client.GetAsync(endpointUrl);
                    response.EnsureSuccessStatusCode();

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        return await JsonSerializer.DeserializeAsync<ResponseModel>(contentStream);
                    }

                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error getting API response: {ex}");
                return new ResponseModel { StatusCode = (int)HttpStatusCode.InternalServerError, Message = ex.Message };
                // throw;
            }
        }
    }

}
