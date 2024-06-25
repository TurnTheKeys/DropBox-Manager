using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBox_Upload
{
    internal class DropBoxExplorerClass
    {
        private HTTPPostRequest HTTPPostRequest = new HTTPPostRequest();
        List<string> Directory = new List<string>();

        private const int uploadChunkSize = 150 * 1024 * 1024; // 150 MiB

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
        /// Downloads specified file from dropbox filepath to the given local filepath.
        /// </summary>
        /// <param name="token">The token for access to the DropBox Account</param>
        /// <param name="dropBoxDownloadFilePath">The location of the file to be downloaded from</param>
        /// <param name="saveToLocalFilePath">The location of where the file will be download to</param>
        /// <returns>Returns true if the file was succefully downloaded, otherwise, returns false</returns>
        public bool DownloadFile (DropBoxToken token, string dropBoxDownloadFilePath, string saveToLocalFilePath)
        {
            return false;
        }
    }
}
