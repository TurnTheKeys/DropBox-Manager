using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DropBox_Upload
{
    internal class DropBoxRefreshToken
    {
        private string refresh_token { get; set; } = string.Empty;
        private string scope { get; set; } = string.Empty;
        private string uid { get; set; } = string.Empty;
        private string account_id { get; set; } = string.Empty;
        private string app_secret { get; set; } = string.Empty;
        private string client_id { get; set; } = string.Empty;
    }

    internal class DropBoxToken
    {
        DropBoxRefreshToken RefreshToken = new DropBoxRefreshToken();

        private string FilePath = string.Empty;
        private string json = string.Empty;

        private DateTime TokenExpiryTime {  get; set; } = DateTime.Now;
        
        public DropBoxToken(string givenFilePath) {

            FilePath = givenFilePath;
            FileChecker();
            TokenValidation();
        }

        /// <summary>
        /// Validates entered file
        /// </summary>
        /// <returns>If file was sucessfully found</returns>
        private bool FileChecker()
        {
            if (File.Exists(FilePath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Validates token from given filepath to see if it exists and check if token information can be extracted
        /// </summary>
        /// <returns>If validation was successful</returns>
        private bool TokenValidation()
        {
            if (ExtractJSONInformation() == false)
            {
                string question = ("The retrieval of the refresh token information was unsuccessful, do you want to generate one?");
                string[] validOptions = {"y", "n"};

                string userInput = UserAnswer(question, validOptions);
            }
            else
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Gives user question and valid options, if the user chooses an available option, it returns, otherwise keeps asking question
        /// </summary>
        /// <param name="question">The question being asked to the user</param>
        /// <param name="validOptions">Options the users is allowed to select</param>
        /// <returns>Option the user chooses</returns>
        private string UserAnswer(string question, string[] validOptions)
        {
            string answer = string.Empty;
            while (true)
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
                if (validOptions.Contains(answer))
                {
                    break;
                }
                Console.WriteLine();
                Console.WriteLine($"'{answer}' is not a valid choice");
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
        private bool ExtractJSONInformation()
        {
            if (File.Exists(FilePath))
            {
                return false;
            }
            else
            {
                string json = ExtractText();
                RefreshToken = JsonConvert.DeserializeObject<DropBoxRefreshToken>(json);
                
            }
            return false;
        }

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
