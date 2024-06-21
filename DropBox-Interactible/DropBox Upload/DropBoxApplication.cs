using DropBox_Upload;
using System.Linq;
using System.Runtime.CompilerServices;

class DropBoxApplication
{
    DropBoxToken? dropboxToken;
    public DropBoxApplication()
    {
        Console.WriteLine("Hello, this program allows for files to be downloaded or uploaded to DropBox");
        while (true)
        {
            ProgramMenu();
        }
    }

    /// <summary>
    /// Generates menu that can be interacted with.
    /// </summary>
    public void ProgramMenu()
    {
        string[] optionsAvaliable = { "Upload Refresh Token", "Generate Refresh Token", "Generate access token from refresh token" , "Print token details", "Upload File", "Download File" };
        string[] options = { "1", "2", "3", "4" };
        string optionSelected = UserAnswer(optionsAvaliable,options);

        switch (optionSelected)
        {
            case "1":
                Console.WriteLine("Please enter file path of the json token.");
                dropboxToken = new DropBoxToken(Console.ReadLine() ?? "");
                if (dropboxToken.TokenValidation() == false)
                {
                    Console.WriteLine($"The token was unable to be retrieved. If you would like like, you can select option 2, to generate the token");
                }
                else
                {
                    Console.WriteLine($"The token was able to read, please select option 4 to generate access token.");
                }
                break;
            case "2":
                Console.WriteLine("To retrieve token, you first need to setup your GitHub account.");
                Console.WriteLine("To do this, please follow the instructions here: https://github.com/TurnTheKeys/DropBox-Manager");
                Console.WriteLine("Please enter the app key:");
                string appKey = Console.ReadLine() ?? "";
                Console.WriteLine("Please enter the app Secret:");
                string appSecret = Console.ReadLine() ?? "";
                Console.WriteLine("Please enter the access code generated:");
                string accessCode = Console.ReadLine() ?? "";
                dropboxToken = new DropBoxToken("");

                if (dropboxToken.GetRefreshToken(appKey, appSecret, accessCode)){
                    Console.WriteLine("Token was successfully generated, where would you like to save the token?");
                    string saveFilePath = Console.ReadLine() ?? "";
                    dropboxToken.ConvertTokenJSON(saveFilePath);
                }
                break;
            case "4":
                if(CheckNullToken() == false) { break; }
                dropboxToken?.PrintToken();
                break;
            default:
                Console.WriteLine("Sorry, either it has yet to be implemented or it is not an option");
                break;
        }
    }

    /// <summary>
    /// Checks to see if the refresh token has been set up
    /// </summary>
    /// <returns>Returns true if the token has been setup, otherwise returns false</returns>
    public bool CheckNullToken()
    {
        if (dropboxToken == null)
        {
            Console.WriteLine("You've yet to setup the token! Please select option 1 first before continuing.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Gives user question and valid options, if the user chooses an available option, it returns, otherwise keeps asking question
    /// </summary>
    /// <param name="options">The option description</param>
    /// <param name="validOptions">Options the users is allowed to select</param>
    /// <returns>Option the user chooses</returns>
    private string UserAnswer(string[] options, string[] validOptions)
    {
        string answer = string.Empty;
        while (true)
        {
            Console.WriteLine("Please select an option from the menu");
            // Display options
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{i + 1}) {options[i]}");
            }

            //Accept Answer
            answer = Console.ReadLine() ?? string.Empty;

            //Validate Answer
            if (validOptions.Contains(answer))
            {
                break;
            }
            Console.WriteLine($"'{answer}' is not a valid choice");
        }
        return answer;
    }
}