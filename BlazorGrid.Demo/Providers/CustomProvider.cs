using BlazorGrid.Providers;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using BlazorGrid.Abstractions;
using BlazorGrid.Abstractions.Extensions;

namespace BlazorGrid.Demo.Providers
{
    public class CustomProvider : DefaultHttpProvider
    {
        private readonly HttpClient http;

        public CustomProvider(HttpClient http) : base(http)
        {
            this.http = http;
        }

        public override Task<T> ReloadAsync<T>(string BaseUrl, T Row)
        {
            return Task.FromResult<T>(default);
        }

        public override async Task<BlazorGridResult<T>> GetAsync<T>(string BaseUrl, int Offset, int Length, string OrderBy, bool OrderByDescending, string SearchQuery)
        {
            var url = GetRequestUrl(BaseUrl, Offset, Length, OrderBy, OrderByDescending, SearchQuery);
            var response = await http.GetAsync(url);
            var result = await DeserializeJsonAsync<BlazorGridResult<T>>(response);

            var data = result.Data.AsQueryable();

            if (OrderBy != null)
            {
                if (OrderByDescending)
                {
                    data = data.OrderByDescending(OrderBy);
                }
                else
                {
                    data = data.OrderBy(OrderBy);
                }
            }

            var finalResult = new BlazorGridResult<T>
            {
                TotalCount = result.TotalCount,
                Data = data.Skip(Offset).Take(Length).ToList()
            };

            return finalResult;
        }
    }
}