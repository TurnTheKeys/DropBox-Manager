using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DropBox_Upload
{
    internal class HTTPPostRequest
    {
        /// <summary>
        /// Updates class's response with response from given DropBoxURL and parameters
        /// </summary>
        /// <param name="websiteURL">The url for request to be sent through</param>
        /// <param name="parameters">Parameters to be sent through dropbox url</param>
        /// <returns>Returns tuple. The 'success' and 'responseBody'. For success, returns true if the attempt was successful, otherwise returns false. "responseBody" holds the response.</returns>
        public (bool success, string responseBody) PostRequestParameters(string websiteURL, Dictionary<string, string> parameters)
        {
            return AsyncDropBoxConnectionParameters(websiteURL, parameters).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Updates class's response with response from given DropBoxURL and parameters
        /// </summary>
        /// <param name="DropBoxURL">The url for request to be sent through</param>
        /// <param name="parameters">Parameters to be sent through dropbox url</param>
        /// <returns>Returns tuple. The 'success' and 'responseBody'. For success, returns true if the attempt was successful, otherwise returns false. "responseBody" holds the response.</returns>
        private async Task<(bool success, string responseBody)> AsyncDropBoxConnectionParameters(string DropBoxURL, Dictionary<string, string> parameters)
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
                Console.WriteLine($"Request error in AsyncDropBoxConnection: {e.Message}");
                return (false, "Error");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AsyncDropBoxConnection: {ex.Message}");
                return (false, "Error");
            }
        }

        /// <summary>
        /// Generates and post request using given headers
        /// </summary>
        /// <param name="websiteURL">URL for the request to be sent through</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public (bool success, string responseBody) PostRequestHeaders(string websiteURL, Dictionary<string, string> headers, HttpContent content)
        {
            return AsyncDropBoxConnectionHeaders(websiteURL, headers, content).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Updates class's response with response from given DropBoxURL and parameters
        /// </summary>
        /// <param name="DropBoxURL"></param>
        /// <param name="headers">Parameters to be sent through dropbox url</param>
        /// /// <param name="content">Contents to be sent through request</param>
        /// <returns>Returns tuple. The 'success' and 'responseBody'. For success, returns true if the attempt was successful, otherwise returns false. "responseBody" holds the response.</returns>
        private async Task<(bool success, string responseBody)> AsyncDropBoxConnectionHeaders(string DropBoxURL, Dictionary<string, string> headers, HttpContent content)
        {
            var requestGenerated = RequestGenerate(DropBoxURL, headers, content);
            if (!requestGenerated.success || requestGenerated.request == null)
            {
                Console.WriteLine("The request could not be submitted.");
                return (false, "Error generating request");
            }
            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.SendAsync(requestGenerated.request);

                    // Log status code and reason phrase
                    Console.WriteLine($"Status Code: {response.StatusCode}");
                    Console.WriteLine($"Reason Phrase: {response.ReasonPhrase}");

                    if (!response.IsSuccessStatusCode || response.ReasonPhrase == "Bad Request")
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error Response: {errorResponse}");
                        return (false, "Error");
                    }

                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Body : {responseBody}");

                    // Extract session ID from the response
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    string sessionId = jsonResponse.GetProperty("session_id").GetString() ?? string.Empty;

                    return (true, sessionId);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return (false, "Error");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UploadSessionStart: {ex.Message}");
                return (false, "Error");
            }
        }

        /// <summary>
        /// Generates Request based on given headers and url
        /// </summary>
        /// <param name="URL">URL for request to be sent through</param>
        /// <param name="headers"></param>
        /// <returns>returns true and filled request if successful, otherwise returns false and an empty request</returns>
        private (bool success, HttpRequestMessage? request) RequestGenerate (string URL, Dictionary<string, string> headers, HttpContent content)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, URL);
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
                request.Content = content;
                return (true, request);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Issue with generating request to {URL} with headers {string.Join(", ", headers.Select(h => $"{h.Key}: {h.Value}"))}: {e}");
                return (false, null);
            } 
        }
    }
}
