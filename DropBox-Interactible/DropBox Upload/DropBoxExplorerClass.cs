using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Headers;

namespace DropBox_Upload
{

    internal class DropBoxExplorerClass
    {

        private HTTPPostRequest HTTPPostRequest = new HTTPPostRequest();
        List<string> Directory = new List<string>();

        private const int uploadChunkSize = 150 * 1024 * 1024; // 150 MiB
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Starts and finishes file upload session to DropBox
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="saveWhere"></param>
        /// <param name="token"></param>
        /// <returns>Returns true if the upload was successful, otherwise, returns false</returns>
        public bool FileUploadSession(string filePath, string saveWhere, DropBoxToken token)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("The file was unable to be found, please check filepath");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Commences DropBox Upload session using given dropbox token
        /// </summary>
        /// <param name="token">The token for the dropbox account</param>
        /// <returns>Returns the id of the dropbox upload session, otherwise, returns false</returns>
        public string UploadSessionStart (DropBoxToken token)
        {
            string UploadSessionID = "";
            return UploadSessionID;
        }

        /// <summary>
        /// Uploads additional data to the upload session
        /// </summary>
        /// <param name="token">The token for accesss to the DropBox Account</param>
        /// <param name="uploadSessionID">The upload session the data is to be uploaded to.</param>
        /// <param name="appendedData">The data to be uploaded</param>
        /// <param name="offset">The data offset</param>
        /// <returns>Returns true if the data was succesfully appended to the upload session, otherwise, returns false</returns>
        public bool UploadSessionAppend (DropBoxToken token, string uploadSessionID, string appendedData, int offset)
        {
            return false; 
        }


        /// <summary>
        /// Finishes of the upload session
        /// </summary>
        /// <param name="token">The token for accesss to the DropBox Account</param>
        /// <param name="uploadSessionID">The upload session the data is to be uploaded to.</param>
        /// <param name="appendedData">The data to be uploaded</param>
        /// <param name="offset">The data offset</param>
        /// <param name="uploadFilePath">The location in the dropbox account the file is to be uploaded to</param>
        /// <returns>Returns true if the upload session was succfully finished, otherwise, returns false</returns>
        public bool UploadSessionFinish (DropBoxToken token, string uploadSessionID, string appendedData, int offset, string uploadFilePath)
        {
            return false;
        }

        /// <summary>
        /// Starts download process
        /// </summary>
        /// <param name="token">The dropbox token for the dropbox account to download from</param>
        /// <param name="dropBoxDownloadFilePath">The filepath or ID of the file to be downloaded from DropBox</param>
        /// <param name="saveToLocalFilePath">The location of where the file will be download to</param>
        /// <returns>Returns true if the file was succefully downloaded, otherwise, returns false</returns>
        public bool DownloadFileDropBox(DropBoxToken token, string dropBoxDownloadFilePath, string saveToLocalFilePath, string fileName)
        {
            if (DownloadFileProcess(token, dropBoxDownloadFilePath, saveToLocalFilePath, fileName).GetAwaiter().GetResult() == true)
            {
                Console.WriteLine("The file was successfully downloaded!");
                return true;
            }
            Console.WriteLine("The file was successfully downloaded!");
            return false;
        }

        /// <summary>
        /// Downloads specified file from Dropbox filepath to the given local filepath.
        /// </summary>
        /// <param name="token">The token for access to the Dropbox Account</param>
        /// <param name="dropBoxDownloadFilePath">The filepath or ID of the file to be downloaded from Dropbox</param>
        /// <param name="saveToLocalFilePath">The location where the file will be downloaded to</param>
        /// <param name="fileName">The filename to save the downloaded file as</param>
        /// <returns>Returns true if the file was successfully downloaded, otherwise, returns false</returns>
        public async Task<bool> DownloadFileProcess(DropBoxToken token, string dropBoxDownloadFilePath, string saveToLocalFilePath, string fileName)
        {
            var accessTokenRetrieve = token.RetrieveAccessToken();
            string normalizedDropBoxDownloadFilePath = dropBoxDownloadFilePath.Replace("\\", "/");
            string dropBoxDownloadFilePathConverted = JsonSerializer.Serialize(new { path = normalizedDropBoxDownloadFilePath.StartsWith("/") ? normalizedDropBoxDownloadFilePath : "/" + normalizedDropBoxDownloadFilePath });
            Console.WriteLine($"The Dropbox URL used is: {dropBoxDownloadFilePathConverted}");
            if (!accessTokenRetrieve.success)
            {
                Console.WriteLine("Unable to download file: Failed to retrieve access token.");
                return false;
            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/download");
            request.Headers.Add("Authorization", $"Bearer {accessTokenRetrieve.accessToken}");
            request.Headers.Add("Dropbox-API-Arg", dropBoxDownloadFilePathConverted);

            try
            {
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error Response: {errorResponse}");
                        return false;
                    }

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(Path.Combine(saveToLocalFilePath, fileName), FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        await stream.CopyToAsync(fileStream);
                    }

                    Console.WriteLine($"File successfully downloaded to: {Path.Combine(saveToLocalFilePath, fileName)}");
                    return true;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }

    }
}
