# DropBox Manager (in-progress)
## Goal
- The goal is to make a program that allows for interaction with GitHub

## Intended Functions
- This will include ways to:
  - Open a JSON file to get token information
  - Generate tokens
  - Access and display directories of the GitHub account
  - Upload and Download Files on DropBox

## Currently Working on
- Generating refresh token

## Requirements
- Visual Studio package: Newtonsoft.Json
  - Will allow for token json to be read
- Dropbox Setup:
  - Need to create app to have access:
      - Create app: 
        - [Dropbox Developer Apps](https://www.dropbox.com/developers/apps)
      - Select options for app:
        - <img src="https://github.com/TurnTheKeys/DropBox-Manager/assets/166112225/9e465618-e614-4f48-a4ef-5ba0621d0834" width="400">
    - Modify permissions:
      - Click the permissions heading:
        - <img src="https://github.com/TurnTheKeys/DropBox-Manager/assets/166112225/2042b740-d35f-48e6-a0df-308e56d8c65d" width="300">
      - Tick these boxes:
        - <img src="https://github.com/TurnTheKeys/DropBox-Manager/assets/166112225/d9a1c065-dff0-4d3e-bb90-875ebf5b7357" width="400">
        - files.content.write
        - files.content.read
    - Record these key information somewhere (e.g. notepad) from the Settings header:
      - App Key
      - App Secret
  - Generate token to allow the app access in the application
    - Enter: `https://www.dropbox.com/oauth2/authorize?client_id=<APP_KEY>&token_access_type=offline&response_type=code`
    - Replace the [app key] with the app key generated when creating app
    - Click 'Allow'
    - Record the access code
