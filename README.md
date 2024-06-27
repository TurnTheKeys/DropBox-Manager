# DropBox Manager (in-progress)

## Goal
- The goal is to make a program that can be used with a GitHub account.
- This includes generations of tokens in addition to exploring, downloading and uploading files

<details>
  <summary><h2>Intended Functions</h2></summary>

  - This will include ways to:
    - Open a JSON file to get token information
    - Generate tokens
    - Access and display directories of the GitHub account
    - Upload and Download Files on DropBox

</details>
<details>
  <summary><h2>Project Status</h2></summary>

  - Completed
    - Opening JSON token and outputting JSON token
    - Generating Refresh Token
    - Generating Access Token
    - Download of files (through filepath)
  - Currently Working on
    - Upload of files
    - Feature Manual
  - Working on Next:
    - Printing to console: Directory files and folders, Dropbox file ID

</details>
<details>
  <summary><h2>Requirements</h2></summary>
  - Visual Studio package: Newtonsoft.Json
    - Will allow for token json to be read
</details>

<details>
<summary><h2>Dropbox Setup</h2></summary>

- Need to create app to have access:
  - Create app: 
    - [Dropbox Developer Apps](https://www.dropbox.com/developers/apps)
  - Select options for app:
    - <img src="https://github.com/TurnTheKeys/DropBox-Manager/assets/166112225/9e465618-e614-4f48-a4ef-5ba0621d0834" width="400">
- Modify permissions:
  - Click the permissions heading:
    - <img src="https://github.com/TurnTheKeys/DropBox-Manager/assets/166112225/2042b740-d35f-48e6-a0df-308e56d8c65d" width="300">
  - Tick these boxes:
    - <img src="https://github.com/TurnTheKeys/DropBox-Manager/assets/166112225/8331ff01-c86a-4da8-a21f-35b1c91a7a3c" width="400">
    - files.metadata.write
    - files.content.write
    - files.content.read
- Record these key information somewhere (e.g. notepad) from the Settings header:
  - App Key
  - App Secret
- Generate token to allow the app access in the application:
  - Enter: `https://www.dropbox.com/oauth2/authorize?client_id=<APP_KEY>&token_access_type=offline&response_type=code`
  - Replace the `<APP_KEY>` with the app key generated when creating the app
  - Click 'Allow'
  - Record the access code

</details>


