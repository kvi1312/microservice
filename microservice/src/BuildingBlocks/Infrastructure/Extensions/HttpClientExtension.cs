using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.Extensions;

public static class HttpClientExtension
{
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        T data)
    {
        var dataAsString = JsonSerializer.Serialize(data);
        var content = new StringContent(dataAsString);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        return client.PostAsync(requestUri, content);
    }

    public static async Task<T> ReadContentAs<T>(
        this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException($"Something went wrong calling the api : {response.ReasonPhrase}");
        }
        var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<T>(dataAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
        });
    }
}
