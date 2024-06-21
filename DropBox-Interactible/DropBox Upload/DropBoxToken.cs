using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

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

    internal class DropBoxToken
    {
        DropBoxRefreshToken RefreshToken = new DropBoxRefreshToken();

        private string FilePath = string.Empty;
        private string json = string.Empty;

        private DateTime TokenExpiryTime {  get; set; } = DateTime.Now;
        
        public DropBoxToken(string givenFilePath) {

            FilePath = givenFilePath;
            Console.WriteLine();
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
        /// Generates refresh token and related information for DropBox
        /// </summary>
        /// <returns>If function was successful</returns>
        private bool GetRefreshToken()
        {
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
            else
            {
                string json = ExtractText();
                RefreshToken = JsonConvert.DeserializeObject<DropBoxRefreshToken>(json);
                if (RefreshToken.AllFieldsFilled() == false)
                {
                    Console.WriteLine("Unable to properly read token");
                    return false;
                }
                return true;
            }
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
