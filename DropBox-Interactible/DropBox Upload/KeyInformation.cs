using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DropBox_Upload
{
    internal class KeyInformation
    {
        private string FilePath = string.Empty;
        private string[] KeyJSONInformation { get; set; } = new string[0];
        private string RefreshToken {  get; set; } = string.Empty;
        private string AppKey {  get; set; } = string.Empty;
        private string AppSecret { get; set; } = string.Empty;
        private string AppKeySecret {  get; set; } = string.Empty;

        private string AccessCode { get; set; } = string.Empty;
        private DateTime TokenExpiryTime {  get; set; } = DateTime.Now;
        
        public KeyInformation(string filePath) {

            filePath = filePath ?? string.Empty;
            TokenValidation();
        }

        /// <summary>
        /// Validates token to see if it exists and check if token information can be extracted
        /// </summary>
        /// <returns>If validation was successful</returns>
        private bool TokenValidation()
        {
            if (ExtractJSONToken() == false)
            {
                string question = ("The retrieval of the refresh token information was unsuccessful, do you want to generate one?");
                string[] validOptions = {"y", "n"};

                string userInput = UserAnswer(question, validOptions);

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
