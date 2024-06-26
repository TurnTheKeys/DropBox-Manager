﻿using DropBox_Upload;
using System.Linq;
using System.Runtime.CompilerServices;

class DropBoxApplication
{
    DropBoxToken? dropboxToken;
    DropBoxExplorerClass? dropBoxExplorerClass;
    public DropBoxApplication()
    {
        Console.WriteLine("Hello, this program allows for files to be downloaded or uploaded to DropBox");
        dropBoxExplorerClass = new DropBoxExplorerClass();
        while (true)
        {
            ProgramMenu();
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Generates menu with program options.
    /// </summary>
    public void ProgramMenu()
    {
        string[] optionsAvaliable = { "Upload Refresh Token", "Generate Refresh Token", "Generate new access token from refresh token" , "Print token details", "Upload File", "Download File from file path" };
        string[] options = { "1", "2", "3", "4", "5", "6" };
        string optionSelected = UserAnswer(optionsAvaliable,options);

        switch (optionSelected)
        {
            case "1":
                OpenJSONToken();
                break;
            case "2":
                GenerateRefreshTokens();
                break;
            case "3":
                GenerateNewAccessToken();
                break;
            case "4":
                PrintTokenInformation();
                break;
            case "5":
                UploadFileFilePath();
                break;
            case "6":
                DownloadFileFilePath();
                break;
            default:
                Console.WriteLine("Sorry, either it has yet to be implemented or it is not an option");
                break;
        }
    }

    /// <summary>
    /// Downloads file from given DropBox file path
    /// </summary>
    public void DownloadFileFilePath()
    {
        if (dropboxToken == null || dropBoxExplorerClass == null)
        {
            Console.WriteLine("You've yet to setup the token! Please select option 1 to read a token first or selecting option 2 to generate a new one before trying again.");
            return;
        }
        Console.WriteLine("Please enter the filepath or id of the file to be downloaded:");
        string fileReference = Console.ReadLine() ?? "";
        Console.WriteLine("Please enter the filepath for where the file will be saved to:");
        string downloadToFilePath = Console.ReadLine() ?? "";
        Console.WriteLine("Please enter the name you wish to give to the file along with extention (e.g. file.txt):");
        string fileName = Console.ReadLine() ?? "";
        dropBoxExplorerClass.DownloadFileDropBox(dropboxToken, fileReference, downloadToFilePath, fileName);
    }

    /// <summary>
    /// Uploads file from given file to DropBox
    /// </summary>
    public void UploadFileFilePath()
    {
        if (dropboxToken == null || dropBoxExplorerClass == null)
        {
            Console.WriteLine("You've yet to setup the token! Please select option 1 to read a token first or selecting option 2 to generate a new one before trying again.");
            return;
        }
        Console.WriteLine("Please enter the filepath of the file to be uploaded:");
        string uploadFrom = Console.ReadLine() ?? "";
        string fileName = Path.GetFileName(uploadFrom);
        Console.WriteLine("Please enter the filepath for where the file will be saved to in DropBox:");
        string uploadToFilePath = Console.ReadLine() ?? "";
        string uploadToFilePathWhole = uploadToFilePath + $"/{fileName}";
        dropBoxExplorerClass.UploadToDropBox(dropboxToken, uploadFrom, uploadToFilePathWhole);
    }

    /// <summary>
    /// Generates new access token if the dropboxToken refresh token has been loaded already
    /// </summary>
    public void GenerateNewAccessToken()
    {
        if (dropboxToken == null)
        {
            Console.WriteLine("You've yet to setup the token! Please select option 1 to read a token first or selecting option 2 to generate a new one before trying again.");
            return;
        }

        if (dropboxToken.GenerateToken("AccessTokenRefresh", dropboxToken.FillTokenDetails()))
        {
            Console.WriteLine("The access token was succesfully generated.");
            return;
        }
        Console.WriteLine("The access token was unsuccesfully generated.");
        return;
    }

    /// <summary>
    /// Commences function of printing token information
    /// </summary>
    public void PrintTokenInformation()
    {
        if (CheckNullToken() == false) { return; }
        dropboxToken?.PrintToken();
    }

    /// <summary>
    /// Commences function of reading refresh token from given filepath, if successfully read, generates new access token
    /// </summary>
    public void OpenJSONToken()
    {
        Console.WriteLine("Please enter file path of the json token.");
        dropboxToken = new DropBoxToken(Console.ReadLine() ?? "");
        if (!dropboxToken.TokenValidation())
        {
            Console.WriteLine($"The token was unable to be retrieved. If you would like like, you can select option 2, to generate the token");
        }
        else
        {
            GenerateNewAccessToken();
        }
    }

    /// <summary>
    /// Generates refresh token through given user prompts, also generates access token
    /// </summary>
    public void GenerateRefreshTokens()
    {
        Console.WriteLine("To retrieve token, you first need to setup your GitHub account.");
        Console.WriteLine("To do this, please follow the instructions here: https://github.com/TurnTheKeys/DropBox-Manager under Dropbox Setup");
        Console.WriteLine("Please enter the app key:");
        string appKey = Console.ReadLine() ?? "";
        Console.WriteLine("Please enter the app Secret:");
        string appSecret = Console.ReadLine() ?? "";
        Console.WriteLine("Please enter the access code generated:");
        string accessCode = Console.ReadLine() ?? "";
        dropboxToken = new DropBoxToken("");

        string[] dropboxTokenDetails = new string[] { accessCode, appKey, appSecret };


        if (dropboxToken.GenerateToken("RefreshTokenRefresh", dropboxTokenDetails) == true)
        {
            Console.WriteLine("Token was successfully generated, where would you like to save the token?");
            string saveFilePath = Console.ReadLine() ?? "";
            dropboxToken.ConvertTokenToJSON(saveFilePath, "RefreshToken", "RefreshToken");
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
            Console.WriteLine("You've yet to setup the token! Please select option 1 to read a token first or selecting option 2 to generate a new one before trying again.");
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