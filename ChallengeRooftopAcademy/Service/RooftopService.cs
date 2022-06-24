using ChallengeRooftopAcademy.Models;
using ChallengeRooftopAcademy.Service.Interfaces;
using ChallengeRooftopAcademy.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ChallengeRooftopAcademy.Service
{
    public class RooftopService : IHttpService
    {
        public readonly IServiceCache _serviceCache;
        public IHttpClientFactory _httpFactory { get; set; }
        public RooftopService(IHttpClientFactory httpFactory, IServiceCache serviceCache)
        {
            _httpFactory = httpFactory;
            _serviceCache = serviceCache;
        }

        private async Task<T> executeRequest<T>(string path, bool isGet, object data = null)
        {
            try
            {
                var method = isGet ? HttpMethod.Get : HttpMethod.Post;

                var request = new HttpRequestMessage(method, Constant.BaseUrl + path);

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (data != null)
                {
                    var content = JsonSerializer.Serialize(data);
                    request.Content = new StringContent(content, Encoding.UTF8);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }

                using var client = _httpFactory.CreateClient();

                var cancellationTokenSource = new CancellationToken();
                var response = await client.SendAsync(request,HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                var castResult = result.ToCast<T>();
                return castResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task getToken(string email)
        {
            try
            {
                var result = await executeRequest<ResponseToken>("token?email=" + email, true);
                result.email = email;
                _serviceCache.set("token", result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Blocks> getBlocks()
        {
            try
            {
                var responseTokenCache = _serviceCache.get<ResponseToken>("token");
                if (string.IsNullOrEmpty(responseTokenCache.token))
                {
                    Console.WriteLine("Debe relizar el login");
                    return null;
                }

                var result = await executeRequest<Blocks>("blocks?token=" + responseTokenCache.token, true);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseCheck> checkAllChain(List<string> blocks, string key)
        {
            try {
                string combinedString = string.Join("", blocks);

                var result = _serviceCache.get<ResponseCheck>(combinedString);

                if (result == null)
                {
                    result = await getCheck(new ResponseCheck
                    {
                        encoded = combinedString
                    }) ?? new ResponseCheck();

                    result.blocks = blocks;

                    if (result.message )
                    {
                        _serviceCache.set(key, result);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseCheck> getCheck(ResponseCheck blocks)
        {
            try
            {
                var responseCheckCache = _serviceCache.get<ResponseToken>("token");
                if (string.IsNullOrEmpty(responseCheckCache.token))
                {
                    Console.WriteLine("Debe relizar el login");
                    return null;
                }

                var key = JsonSerializer.Serialize(blocks);
                //Consultamos Cache
                var result = _serviceCache.get<ResponseCheck>(key);

                //si no hay nada buscamos en el servicio
                if (result == null)
                {
                    result = await executeRequest<ResponseCheck>("check?token=" + responseCheckCache.token, false, blocks);
                    _serviceCache.set(key, result);
                }
                else {
                    result.consultedToApi = false;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
