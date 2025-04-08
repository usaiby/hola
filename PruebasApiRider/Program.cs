using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Variables de configuración
        var clientId = "ce2f2dc8d61845df89bdf8ff148d04a2";
        var clientSecret = "wuiMJq6RL6ERsq9ZJ0YOB4a5QC5PtLcqplyIaWZXpcjbR4icddF6ecmomKAC3nX1";
        var url = "https://santander-es.api.eu.nexthink.cloud/api/v1/token";

        // Codificar credenciales en base64
        var credentials = $"{clientId}:{clientSecret}";
        var credentialsBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

        using (var client = new HttpClient())
        {
            // Encabezados de autenticación
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentialsBase64);

            // Contenido del cuerpo (x-www-form-urlencoded)
            var content = new StringContent("grant_type=client_credentials&scope=service:integration", Encoding.UTF8, "application/x-www-form-urlencoded");

            // Petición POST
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            // Mostrar JSON completo
            //Console.WriteLine("Respuesta completa:");
            //Console.WriteLine(responseString);

            // Extraer el token
            using (JsonDocument doc = JsonDocument.Parse(responseString))
            {
                if (doc.RootElement.TryGetProperty("access_token", out JsonElement tokenElement))
                {
                    string token = tokenElement.GetString();
                    Console.WriteLine($"\n✅ Token obtenido: {token}");
                    await QueryDevicesAsync(token);
                    
                    Console.WriteLine("\t\n\t\n\t\n\t\n");
                    await QueryDevicesAsyncConVariable(token);
                }
                else
                {
                    Console.WriteLine("\n❌ No se encontró el token en la respuesta.");
                }
            }
        }
    }
    
    private static readonly HttpClient client2 = new HttpClient();

    public static async Task QueryDevicesAsync ( string token )
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://santander-es.api.eu.nexthink.cloud/api/v1/nql/execute"),
            Headers =
            {
                { "Authorization", $"Bearer {token}" },
                { "Accept", "application/json" },
            },
            Content = new StringContent("{\n \"queryId\": \"#chew_test_query\"\n}")
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue ( "application/json" )
                }
            }
        };
        using (var response = await client.SendAsync(request))
        {
            //response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine(body);
        }
    }


   
    public static async Task QueryDevicesAsyncConVariable(string token)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://santander-es.api.eu.nexthink.cloud/api/v1/nql/execute"),
            Headers =
            {
                { "Authorization", $"Bearer {token}" },
                { "Accept", "application/json" },
            },
            Content = new StringContent ( "{\n  \"queryId\": \"#chew_test_parametro\",\n  \"parameters\": {\n    \"device_name\": \"GT09876X470591P\"  \n}\n}" )
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue ( "application/json" )
                }
            }
        };
        using (var response = await client.SendAsync(request))
        {
            //response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine(body);
        }
        
    }
   
}