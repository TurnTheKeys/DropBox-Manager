using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBox_Upload
{
    internal class HTTPPostRequest
    {
        public (bool success, string responseBody) PostRequest(string websiteURL, Dictionary<string, string> parameters)
        {
            return AsyncDropBoxConnection(websiteURL, parameters).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Updates class's response with response from given DropBoxURL and parameters
        /// </summary>
        /// <param name="DropBoxURL"></param>
        /// <param name="parameters">Parameters to be sent through dropbox url</param>
        /// <returns>Returns tuple. The 'success' and 'responseBody'. For success, returns true if the attempt was successful, otherwise returns false. "responseBody" holds the response.</returns>
        private async Task<(bool success, string responseBody)> AsyncDropBoxConnection(string DropBoxURL, Dictionary<string, string> parameters)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, DropBoxURL)
                {
                    Content = new FormUrlEncodedContent(parameters)
                };
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Log status code and reason phrase
                    //Console.WriteLine($"Status Code: {response.StatusCode}");
                    //Console.WriteLine($"Reason Phrase: {response.ReasonPhrase}");

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine($"Error Response: {errorResponse}");
                        return (false, "Error");
                    }

                    string responseBody = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine($"Response Body: {responseBody}");

                    return (true, responseBody);
                }
            }

            catch (HttpRequestException e)
            {
                //Console.WriteLine($"Request error: {e.Message}");
                return (false, "Error");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Exception: {ex.Message}");
                return (false, "Error");
            }

        }
    }
}
