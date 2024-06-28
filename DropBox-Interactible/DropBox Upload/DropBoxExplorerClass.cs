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

        private const int uploadChunkSize = 150 * 1024 * 1024; // upload chunk size limit set by Dropbox is 150 MiB
        private static readonly HttpClient client = new HttpClient();

        public async Task<bool> UploadToDropBox(DropBoxToken accessToken, string filePath, string saveWhere)
        {
            // Remember that filepaths are formatted like so '/backups/'
            // filePath needs to also include the file it's being saved as, like '/backups/example.sql'

            // filePath is where the file that needs to be uploaded is located
            // saveWhere is where the file is to be saved on DropBox

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File does not exist: {filePath}");
                return false;
            }

            string uploadSessionId = await UploadSessionStart(accessToken.RetrieveAccessToken().accessToken);

            if (uploadSessionId == null)
            {
                Console.WriteLine($"Failed to commence upload session for file: {filePath}");
                return false;
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                long fileSize = fileStream.Length;
                long offset = 0;

                while (offset < fileSize)
                {
                    long remainingBytes = fileSize - offset;
                    int bytesToRead = (int)Math.Min(uploadChunkSize, remainingBytes);
                    byte[] buffer = new byte[bytesToRead];
                    int bytesRead = await fileStream.ReadAsync(buffer, 0, bytesToRead);

                    if (remainingBytes <= uploadChunkSize)
                    {
                        var result = await UploadSessionFinish(accessToken.RetrieveAccessToken().accessToken, uploadSessionId, buffer, offset, saveWhere);
                        if (result)
                        {
                            Console.WriteLine("File uploaded successfully.");
                            return result;
                        }
                        else
                        {
                            Console.WriteLine("Failed to finish upload session.");
                        }
                        break;
                    }
                    else
                    {
                        bool success = await UploadSessionAppend(accessToken.RetrieveAccessToken().accessToken, uploadSessionId, buffer, offset);
                        if (!success)
                        {
                            Console.WriteLine("Failed to append data to upload session.");
                            break;
                        }
                        offset += bytesRead;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Commences DropBox Upload session using given dropbox token
        /// </summary>
        /// <param name="token">The token for the dropbox account</param>
        /// <returns>Returns the id of the dropbox upload session, otherwise, returns false</returns>
        private async Task<string> UploadSessionStart (string accessToken)
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
        private async Task<bool> UploadSessionAppend (string accessToken, string uploadSessionId, byte[] data, long offset)
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
        private async Task<bool> UploadSessionFinish (string accessToken, string uploadSessionId, byte[] data, long offset, string saveWhere)
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
