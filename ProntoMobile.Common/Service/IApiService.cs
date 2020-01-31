using ProntoMobile.Common.Models;
using System.Threading.Tasks;

namespace ProntoMobile.Common.Service
{
    public interface IApiService
    {
        Task<Response<UserResponse>> GetUserByEmailAsync(
            string urlBase,
            string servicePrefix,
            string controller,
            string tokenType,
            string accessToken,
            string dbname,
            string email);

        Task<Response<object>> GetByEmailAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string tokenType,
            string accessToken,
            string dbname,
            string email);

        Task<Response<object>> GetByEmail2Async<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string tokenType,
            string accessToken,
            string dbname,
            string email);

        Task<Response<TokenResponse>> GetTokenAsync(
            string urlBase,
            string servicePrefix,
            string controller,
            TokenRequest request);

        Task<bool> CheckConnection(string url);

        Task<Response<object>> RegisterUserAsync(
            string urlBase,
            string servicePrefix,
            string controller,
            UserRequest userRequest);

        Task<Response<object>> RecoverPasswordAsync(
            string urlBase,
            string servicePrefix,
            string controller,
            EmailRequest emailRequest);

        Task<Response<object>> PutAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            T model,
            string tokenType,
            string accessToken);

        Task<Response<object>> ChangePasswordAsync(
            string urlBase,
            string servicePrefix,
            string controller,
            ChangePasswordRequest changePasswordRequest,
            string tokenType,
            string accessToken);

        Task<Response<object>> GetListAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string tokenType,
            string accessToken,
            string dbname);

        Task<Response<object>> GetByIdAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string tokenType,
            string accessToken,
            int id);

        Task<Response<object>> GetByIdAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string tokenType,
            string accessToken,
            string dbname,
            int id);

        Task<Response<object>> PostAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string tokenType,
            string accessToken,
            string dbname,
            int id
            );

        Task<Response<object>> PostAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string tokenType,
            string accessToken,
            string dbname,
            T model
            );

        Task<Response<object>> PutAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            int id,
            T model,
            string tokenType,
            string accessToken);

        Task<Response<object>> DeleteAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            T model,
            string tokenType,
            string accessToken);
    }
}
