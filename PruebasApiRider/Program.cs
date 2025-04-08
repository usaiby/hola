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


    public static async Task QueryDevicesAsync1(string token)
    {
        var url = "https://santander-es.api.eu.nexthink.cloud/api/v1/nql/execute";


        // Construimos el objeto JSON del cuerpo
        var bodyObject = new
        {
            queryId = "#chew_test_query"/*,
            parameters = new
            {
               parameterName1 = "parameterValue1",
                parameterName2 = "parameterValue2"
            }*/
        };

        var jsonBody = JsonSerializer.Serialize ( bodyObject );
        var content = new StringContent ( jsonBody, Encoding.UTF8, "application/json" );

        
            // Cabeceras
            client2.DefaultRequestHeaders.Accept.Clear ();
            client2.DefaultRequestHeaders.Accept.Add ( new MediaTypeWithQualityHeaderValue("application/json") );
            client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/csv"));
            client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine( client2.DefaultRequestHeaders);

            // Petición POST
            var response = await client2.PostAsync(url, content);

            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Status code: {response.StatusCode}");
            Console.WriteLine("Respuesta:");
            Console.WriteLine(responseString);
        
    }
    
    public static async Task QueryDevicesAsync2(string token)
    {

        var url = "https://santander-es.api.eu.nexthink.cloud/api/v1/nql/execute";

        // JSON del cuerpo de la consulta
        
        var queryObject = new
        {
            queryId = "#chew_test_query",
            parameters = new
            {
                devicen = "fasdfaksd",
                usuario = "jdoe",
                fecha = "2025-04-07"
            }
        };
        
        var jsonBody = JsonSerializer.Serialize(queryObject);
        Console.WriteLine(jsonBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        // Encabezados de autorización
        client2.DefaultRequestHeaders.Clear();
        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine("---");
        Console.WriteLine(client2.DefaultRequestHeaders.Authorization);
        Console.WriteLine("---");
        try
        {
            var response = await client2.PostAsync(url, content);
           // response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("✅ Respuesta de la consulta:");
            Console.WriteLine(responseBody);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Error en la solicitud: {ex.Message}" + ex.StackTrace + ex.HttpRequestError);
        }
    }
}