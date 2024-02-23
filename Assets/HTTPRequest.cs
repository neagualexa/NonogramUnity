using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class HTTPRequest
{
    public static async Task SendPuzzleMeaningRequest(string userGuess, string solution)
    {
        // Replace the URL with the actual endpoint you want to send the request to
        string apiUrl = "http://localhost:500/api/endpoint";

        // Create a JSON string with the specified format
        string jsonData = $"{{ \"user_guess\": \"{userGuess}\", \"solution\": \"{solution}\" }}";

        using (HttpClient client = new HttpClient())
        using (MultipartFormDataContent formData = new MultipartFormDataContent())
        {
            // Adding the string data to the FormData
            formData.Add(new StringContent(jsonData, Encoding.UTF8, "application/json"), "puzzleMeaning");

            // Sending the HTTP request
            HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

            // Checking if the request was successful (HTTP 200-299)
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request successful!");
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            }

            if (response.Content != null)
            {
                // Reading the response content
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
                return responseContent.ToLower().Contains("true");
            }

            return solution == userGuess
        }
    }
}
