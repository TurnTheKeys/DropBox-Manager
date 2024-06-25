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
using static System.Net.WebRequestMethods;
using System.Windows.Markup;

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
        public string token_type { get; set; } = string.Empty;

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

        /// <summary>
        /// Updates expiry time of the access token
        /// </summary>
        /// <param name="seconds">The number of seconds until the access code expires</param>
        private void UpdateExpiryTime(int seconds)
        {
            DateTime currentTime = DateTime.Now;
            expires_time = currentTime.AddSeconds(seconds);
        }

        /// <summary>
        /// Checks to see if access token is okay to use
        /// </summary>
        /// <returns>Returns trus if the access token is filled</returns>
        public bool OkayToUse()
        {
            if (access_token == string.Empty)
            {
                return false;
            }
            return true;
        }
    }
    internal class DropBoxToken
    {
        private DropBoxRefreshToken RefreshToken = new DropBoxRefreshToken();
        private DropBoxAccessToken AccessToken = new DropBoxAccessToken();
        private HTTPPostRequest HTTPPostRequest = new HTTPPostRequest();

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

        /// <summary>
        /// Generates token based option type
        /// </summary>
        /// <param name="retrievalType">Type of token being retrieved. Available:"RefreshTokenRefresh", "AccessTokenRefresh" </param>
        /// /// <param name="RefreshTokenDetails">Details of the refresh token to be used </param>
        /// <returns>Returns true if the token was successfully refreshed, otherwise returns false</returns>
        public bool GetToken (string retrievalType, string[] RefreshTokenDetails)
        {
            switch (retrievalType)
            {
                case "RefreshTokenRefresh":
                    {  
                        return (GenerateRefreshToken(RefreshTokenDetails[0], RefreshTokenDetails[1], RefreshTokenDetails[2]));
                    }
                case "AccessTokenRefresh":
                    {
                        return (RefreshAccessToken());
                    }
                default:
                    Console.WriteLine("Incorrect token retrieval type.");
                    return false;

            }
        }

        /// <summary>
        /// Fills in stored refresh token details
        /// </summary>
        /// <returns>necessary refresh token details stored as a string array</returns>
        public string[] FillTokenDetails()
        {
            return new string[] {RefreshToken.refresh_token, RefreshToken.account_id, RefreshToken.app_secret};
        }

        /// <summary>
        /// Attempts to generate access token from stored refresh token
        /// </summary>
        /// <returns>Returns true if access token was succefully generated</returns>
        private bool RefreshAccessToken()
        {
            var parameters = new Dictionary<string, string>
                {
                    {"grant_type", "refresh_token" },
                    {"refresh_token", RefreshToken.refresh_token},
                    {"client_id", RefreshToken.account_id },
                    {"client_secret", RefreshToken.app_secret}
                };
            string dropBoxURL = "https://api.dropbox.com/oauth2/token";
            var attemptConnection = HTTPPostRequest.PostRequest(dropBoxURL, parameters);
            if (attemptConnection.success)
            {
                var tokensInformation = JObject.Parse(attemptConnection.responseBody);
                string access_token = tokensInformation["access_token"]?.ToString() ?? "";
                string access_token_expiry = tokensInformation["expires_in"]?.ToString() ?? "";
                AccessToken.UpdateInformation(access_token, access_token_expiry);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Generates new refresh token from given information
        /// </summary>
        /// <param name="accessCode">Access code for dropbox account</param>
        /// <param name="appKey">App key for dropbox account</param>
        /// <param name="appSecret">App secret for dropbox account</param>
        /// <returns></returns>
        private bool GenerateRefreshToken(string accessCode, string appKey, string appSecret)
        {
            var parameters = new Dictionary<string, string>
                {
                    { "code", accessCode },
                    { "grant_type", "authorization_code" },
                    { "client_id", appKey },
                    { "client_secret", appSecret }
                };

            string dropBoxURL = "https://api.dropbox.com/oauth2/token";
            var attemptConnection = HTTPPostRequest.PostRequest(dropBoxURL, parameters);
            if (attemptConnection.success)
            {
                var tokensInformation = JObject.Parse(attemptConnection.responseBody);
                UpdateBothTokens(tokensInformation, appKey, appSecret);
                Console.WriteLine("Refresh Token Generation was successful");
                return true;
            }
            Console.WriteLine("Refresh Token Generation was unsuccessful");
            return false;
        }
 


        /// <summary>
        /// Updates both refresh token and access token based on given information
        /// </summary>
        /// <param name="tokensInformation">Response from response of cURL</param>
        /// <param name="appKey">App Id of Dropbox account</param>
        /// <param name="appSecret">App secret of Dropbox account</param>
        private void UpdateBothTokens(JObject tokensInformation, string appKey, string appSecret)
        {
            RefreshToken.refresh_token = tokensInformation["refresh_token"]?.ToString() ?? "";
            RefreshToken.scope = tokensInformation["scope"]?.ToString() ?? "";
            RefreshToken.uid = tokensInformation["uid"]?.ToString() ?? "";
            RefreshToken.account_id = appKey ?? "";
            RefreshToken.app_secret = appSecret.ToString() ?? "";
            RefreshToken.client_id = tokensInformation["account_id"]?.ToString() ?? "";
            AccessToken.UpdateInformation(tokensInformation["access_token"]?.ToString() ?? "", tokensInformation["expires_in"]?.ToString() ?? "0");
        }

        /// <summary>
        /// Converts stored token information into a json
        /// </summary>
        /// <param name="filePath">path to store json file</param>
        /// <param name="fileName">What to label the file</param>
        /// <param name="tokenType">The token type, Available options: "RefreshToken", "AccessToken"</param>
        /// <returns>Returns true if the JSON was successfully generated, otherwise, returns false</returns>
        public bool ConvertTokenToJSON(string filePath, string fileName, string tokenType)
        {
            string jsonConvert = String.Empty;
            switch (tokenType)
            {
                case ("RefreshToken"):
                    jsonConvert = System.Text.Json.JsonSerializer.Serialize(RefreshToken, new JsonSerializerOptions { WriteIndented = true });
                    break;
                case ("AccessToken"):
                    jsonConvert = System.Text.Json.JsonSerializer.Serialize(AccessToken, new JsonSerializerOptions { WriteIndented = true });
                    break;
                default:
                    Console.WriteLine("Wrong token type in ConvertTokenToJson");
                    return false;

            }
            string wholeFilePath = filePath + $"\\{fileName}.json";
            System.IO.File.WriteAllTextAsync(wholeFilePath, jsonConvert);
            if (System.IO.File.Exists(wholeFilePath))
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
            if (!System.IO.File.Exists(FilePath))
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
                Console.WriteLine("Refresh Token details:");
                Console.WriteLine($"The stored values for token are:");
                Console.WriteLine($"Refresh Token: {RefreshToken.refresh_token}");
                Console.WriteLine($"Account Id: {RefreshToken.account_id}");
                Console.WriteLine($"Scope: {RefreshToken.scope}");
                Console.WriteLine($"uid: {RefreshToken.uid}");
                Console.WriteLine($"Account Id: {RefreshToken.account_id}");
                Console.WriteLine($"App secret: {RefreshToken.app_secret}");
                Console.WriteLine($"Client Id: {RefreshToken.client_id}");
            }
            if (AccessToken.OkayToUse())
            {
                Console.WriteLine("Access Token details:");
                Console.WriteLine($"Access token: {AccessToken.access_token}");
                Console.WriteLine($"Expiry time: {AccessToken.expires_time}");
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
                using (StreamReader reader = System.IO.File.OpenText(FilePath))
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
        /// Checks to see if the access token can still be used
        /// </summary>
        /// <returns>Returns true if the access token hasn't expired, otherwise, return false</returns>
        public bool AccessTokenActive()
        {
            return (AccessToken.ExpiryCheck());
        }

    }
}
