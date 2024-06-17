using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBox_Upload
{
    internal class KeyInformation
    {
        private string[] keyJSONInformation { get; set; } = new string[0];
        private string RefreshToken {  get; set; } = string.Empty;
        private string AppKey {  get; set; } = string.Empty;
        private string AppSecret { get; set; } = string.Empty;
        private string AppKeySecret {  get; set; } = string.Empty;

        private string AccessCode { get; set; } = string.Empty;
        private DateTime TokenExpiryTime {  get; set; } = DateTime.Now;
        
        public KeyInformation(string filePath) {
        
        }

        /// <summary>
        /// Validates token to see if it exists and check if token information can be extracted
        /// </summary>
        /// <returns>If validation was successful</returns>
        private bool TokenValidation()
        {
            if (ExtractJSONToken() == false)
            {
                Console.WriteLine("The retrieval of the refresh token information was unsuccessful, do you want to generate one? (y/n)");
                string userInput = Console.ReadLine() ?? string.Empty;
            }
            return false;
        }


        private string UserAnswer(string question, string[] validOptions, ref string answer)
        {
            //Display Question
            Console.WriteLine($"{question} (");
            for (int i = 0; i < validOptions.Length; i++)
            {
                Console.Write($"{validOptions[i]}");
                if (i < (validOptions.Length - 1))
                {
                    Console.Write("/ ");
                }
            }
            Console.Write("):");

            //Accept Answer
            answer = Console.ReadLine() ?? string.Empty;

            //Validate Answer
            if (!validOptions.Contains(answer))
            {
                Console.WriteLine("Sorry, that was not a valid option");
                UserAnswer(question, validOptions, ref answer);
            }

            return answer;
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
        private bool ExtractJSONToken()
        {
            return false;
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
