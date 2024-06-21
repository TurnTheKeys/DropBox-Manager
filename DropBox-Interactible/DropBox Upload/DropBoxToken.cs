using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace DropBox_Upload
{
    internal class DropBoxRefreshToken
    {
        public string refresh_token { get; set; } = string.Empty;
        public string scope { get; set; } = string.Empty;
        public string uid { get; set; } = string.Empty;
        public string account_id { get; set; } = string.Empty;
        public string app_secret { get; set; } = string.Empty;
        public string client_id { get; set; } = string.Empty;

        public bool AllFieldsFilled()
        {
            if (refresh_token == string.Empty || scope == string.Empty || uid == string.Empty || account_id == string.Empty || account_id == string.Empty || app_secret == string.Empty || client_id == string.Empty)
            {
                return false;
            }
            return true;
        }
    }

    internal class DropBoxAccessToken
    {
        public string access_token { get; set; } = string.Empty;
        public DateTime expires_time { get; set; } = DateTime.Now;

        /// <summary>
        /// Updates the token information
        /// </summary>
        /// <param name="access_token_given">The access token represented as a string</param>
        /// <param name="expiry_time">The seconds left till expiry</param>
        public void UpdateInformation(string access_token_given, string expiry_time)
        {
            access_token = access_token_given;
            UpdateExpiryTime(int.Parse(expiry_time));
        }


        /// <summary>
        /// Checks to see if access token is still fresh
        /// </summary>
        /// <returns>Returns true if access token is still fresh, otherwise returns false</returns>
        public bool ExpiryCheck()
        {
            DateTime currentTime = DateTime.Now;
            if (currentTime > expires_time)
            {
                return false;
            }
            return true;
        }

        public void UpdateExpiryTime(int seconds)
        {
            DateTime currentTime = DateTime.Now;
            expires_time = currentTime.AddSeconds(seconds);
        }
    }
    internal class DropBoxToken
    {
        private DropBoxRefreshToken RefreshToken = new DropBoxRefreshToken();
        private DropBoxAccessToken AccessToken = new DropBoxAccessToken();

        private string FilePath = string.Empty;
        private string json = string.Empty;
        private static readonly HttpClient client = new HttpClient();

        private DateTime TokenExpiryTime {  get; set; } = DateTime.Now;
        
        public DropBoxToken(string givenFilePath) {

            FilePath = givenFilePath;
        }


        /// <summary>
        /// Validates token from given filepath to see if it exists and check if token information can be extracted
        /// </summary>
        /// <returns>If validation was successful</returns>
        public bool TokenValidation()
        {
            if (ExtractJSONInformation() == false)
            {
                return false;
            }
                return true;
        }

        // <summary>
        /// Generates refresh token and related information for DropBox
        /// </summary>
        /// <returns>If function was successful</returns>
        public bool GetRefreshToken(string accessCode, string appKey, string appSecret)
        {
            try
            {
                bool result = GetRefreshTokenAsync(accessCode, appKey, appSecret).GetAwaiter().GetResult();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks to see if access token is still active, if it is no longer active, generate a new access token
        /// </summary>
        /// <returns>Returns true if the access token was succesfully generated, otherwise, return false</returns>
        public bool GetAccessToken()
        {
            if (RefreshToken == null)
            {
                Console.WriteLine("The refresh token has yet to be set up properly");
                return false;
            }
            bool accessTokenAttemptRenewal = GetAccessTokenAsync().GetAwaiter().GetResult(); ;
            return true;
        }

        public async Task<bool> GetAccessTokenAsync()
        {
            Console.WriteLine("Attempting to get new access token");
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    {"grant_type", "refresh_token" },
                    {"refresh_token", RefreshToken.refresh_token},
                    {"client_id", RefreshToken.account_id },
                    {"client_secret", RefreshToken.app_secret}
                };

                Console.WriteLine($"Client_id: {parameters["client_id"]}");
                Console.WriteLine($"Client_secret: {parameters["client_secret"]}");
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropbox.com/oauth2/token")
                {
                    Content = new FormUrlEncodedContent(parameters)
                };

                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Log status code and reason phrase
                    Console.WriteLine($"Status Code: {response.StatusCode}");
                    Console.WriteLine($"Reason Phrase: {response.ReasonPhrase}");

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error Response: {errorResponse}");
                        return false;
                    }

                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Body: {responseBody}");

                    var tokensInformation = JObject.Parse(responseBody);
                    //AccessToken.UpdateInformation(tokensInformation[])
                    return true;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                Console.WriteLine();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Attempts to use generate the refresh token
        /// </summary>
        /// <param name="accessCode">Access code generated for app from DropBox</param>
        /// <param name="appKey">The app key of the app on DropBox</param>
        /// <param name="appSecret">The app secret of the app on DropBox</param>
        /// <returns>If function was successful</returns>
        public async Task<bool> GetRefreshTokenAsync(string accessCode, string appKey, string appSecret)
        {
            Console.WriteLine("Your entered details are:");
            Console.WriteLine($"App key: {appKey}");
            Console.WriteLine($"appSecret: {appSecret}");
            Console.WriteLine($"accessCode: {accessCode}");
            Console.WriteLine();
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "code", accessCode },
                    { "grant_type", "authorization_code" },
                    { "client_id", appKey },
                    { "client_secret", appSecret }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropbox.com/oauth2/token")
                {
                    Content = new FormUrlEncodedContent(parameters)
                };

                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Log status code and reason phrase
                    Console.WriteLine($"Status Code: {response.StatusCode}");
                    Console.WriteLine($"Reason Phrase: {response.ReasonPhrase}");

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error Response: {errorResponse}");
                        return false;
                    }

                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Body: {responseBody}");

                    var tokensInformation = JObject.Parse(responseBody);
                    UpdateTokenInformation(tokensInformation, appKey, appSecret);

                    return true;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                Console.WriteLine();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Updates both refresh token and access token based on given information
        /// </summary>
        /// <param name="tokensInformation">Response from response of cURL</param>
        /// <param name="appKey">App Id of Dropbox account</param>
        /// <param name="appSecret">App secret of Dropbox account</param>
        private void UpdateTokenInformation(JObject tokensInformation, string appKey, string appSecret)
        {
            RefreshToken.refresh_token = tokensInformation["refresh_token"]?.ToString() ?? "";
            RefreshToken.scope = tokensInformation["scope"]?.ToString() ?? "";
            RefreshToken.uid = tokensInformation["uid"]?.ToString() ?? "";
            RefreshToken.account_id = tokensInformation["account_id"]?.ToString() ?? "";
            RefreshToken.app_secret = appSecret.ToString() ?? "";
            RefreshToken.client_id = appKey ?? "";
            AccessToken.UpdateInformation(tokensInformation["access_token"]?.ToString() ?? "", tokensInformation["expires_in"]?.ToString() ?? "0");
        }

        /// <summary>
        /// Converts token information into a json so that it can be reused
        /// </summary>
        /// <param name="filePath">file path to which the json will be saved</param>
        /// <returns></returns>
        public bool ConvertTokenJSON(string filePath)
        {
            string jsonConvert = System.Text.Json.JsonSerializer.Serialize(RefreshToken, new JsonSerializerOptions { WriteIndented = true });
            string wholeFilePath = filePath + "\\RefreshToken.json";
            File.WriteAllTextAsync(wholeFilePath, jsonConvert);
            if (File.Exists(wholeFilePath))
            {
                Console.WriteLine($"Succesfully created token at: {wholeFilePath}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Extract token information from specified file location
        /// </summary>
        /// <returns>Whether or not the token information retrieval was successful</returns>
        private bool ExtractJSONInformation()
        {
            if (!File.Exists(FilePath))
            {
                return false;
            }
            string jsonRead = ExtractText();
            if (!string.IsNullOrEmpty(jsonRead))
            {
                try
                {
                    var tokenHeld = JsonConvert.DeserializeObject<DropBoxRefreshToken>(jsonRead);
                    if (tokenHeld != null && tokenHeld.AllFieldsFilled())
                    {
                        RefreshToken = tokenHeld;
                    }
                    return true;
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    Console.WriteLine($"Unable to read token, Json Exception: {ex}");
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to read token, Exception: {ex}");
                    return false;
                }
            }
            return false;
        }


        /// <summary>
        /// Prints out token information, used for testing purposes
        /// </summary>
        public void PrintToken()
        {
            if (RefreshToken.AllFieldsFilled() && RefreshToken != null)
            {
                Console.WriteLine($"The stored values for token are:");
                Console.WriteLine($"Refresh Token: {RefreshToken.refresh_token}");
                Console.WriteLine($"Account Id: {RefreshToken.account_id}");
                Console.WriteLine($"Scope: {RefreshToken.scope}");
                Console.WriteLine($"uid: {RefreshToken.uid}");
                Console.WriteLine($"Account Id: {RefreshToken.account_id}");
                Console.WriteLine($"App secret: {RefreshToken.app_secret}");
                Console.WriteLine($"Client Id: {RefreshToken.client_id}");
            }
            else
            {
                Console.WriteLine("The token has yet to be entered properly");
            }
        }


        /// <summary>
        /// Attempts to extract text from the class's file path
        /// </summary>
        /// <returns></returns>
        private string ExtractText()
        {
            string extractedText = string.Empty;
            try
            {
                using (StreamReader reader = File.OpenText(FilePath))
                {
                    extractedText = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("File could not be read: " + e.Message);
            }
            return extractedText;
        }


        /// <summary>
        /// Exports JSON location to a given location
        /// </summary>
        /// <returns>Whether or not the JSON was successfully created</returns>
        private bool ExportJSONToken()
        {
            return false; 
        } 

        /// <summary>
        /// Generates access token to be used for dropbox, also updates expiry time of access code
        /// </summary>
        /// <returns>If the access token was succesfully generated</returns>
        private bool GenerateAccessToken()
        {
            return false;
        }
    }
}
