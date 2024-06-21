using DropBox_Upload;
using System.Linq;
using System.Runtime.CompilerServices;

class DropBoxApplication
{
    DropBoxToken? dropboxToken;
    public DropBoxApplication()
    {
        Console.WriteLine("Hello, this program allows for files to be downloaded or uploaded to DropBox");
        Console.WriteLine();
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
        Console.WriteLine();
        string[] optionsAvaliable = { "Upload Refresh Token", "Generate Refresh Token","Validate and upload token json information" , "Print token details", "Upload File", "Download File" };
        string[] options = { "1", "2", "3", "4" };
        string optionSelected = UserAnswer(optionsAvaliable,options);
        Console.WriteLine();

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
                    Console.WriteLine($"The token was able to read.");
                }
                break;
            case "2":

            case "3":
                dropboxToken.PrintToken();
                break;
            default:
                Console.WriteLine("Sorry, either it has yet to be implemented or it is not an option");
                break;
        }
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
            Console.WriteLine();
            Console.WriteLine($"'{answer}' is not a valid choice");
        }
        return answer;
    }
}