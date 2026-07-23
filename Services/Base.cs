namespace Summary.Shopfa.Services
{
    using Microsoft.Extensions.Options;
    using Core.Workflows;
    using Summary.Shopfa.Settings;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public abstract class BaseService
    {
        private readonly ShopfaSettings _options;
        private readonly HttpRequestClient _client;

        protected BaseService(IOptions<ShopfaSettings> options, 
            HttpRequestClient client)
        {
            _options = options.Value;
            _client = client;
        }

        protected async Task<string> GenerateNewTokenAsync(string apiAddress,
            string username,
            string password)
        {
            apiAddress ??= _options.ApiAddress;
            username ??=_options.Username;
            password ??=_options.Password;

            ThrowExceptionIfBaseRequestInfoIsNotValid(apiAddress,
                username,
                password
            );

            var data = new MultipartFormDataContent
            {
                { new StringContent(username), "user_name" },
                { new StringContent(password), "user_password" }
            };

            var response = await _client.SendPostRequestAsync<ResponseTokenInfo>(
                $"{apiAddress}/api/user/signin", 
                data
            );

            if (response.Error_Code.HasValue)  throw new NotImplementedException(response.Error);

            else return response.Private_Key;
        }

        private void ThrowExceptionIfBaseRequestInfoIsNotValid(string apiAddress,
            string username,
            string password)
        {
            if (String.IsNullOrWhiteSpace(apiAddress))
                throw new ArgumentNullException(nameof(apiAddress));

            if (String.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            if (String.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));
        }
    }
}