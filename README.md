# CampusHaps

31927 Application Development with .NET Assignment 2

Group Members Seniru Basnayake 24796500 Aidan Ramen 24749855 Benjamin Walters 24971095

Planned Tech Stack: Front End: SvelteKit Back End: C# Database: MySQL External API:

Brief Project Description: Student Social Event Social Media

Operational Instructions: Github:

clone github repo
Zip File:

Unzip zip file

Test

# CampusHaps – How to Run

**Requirements:** Visual Studio 2022, .NET 9 SDK, Node.js 18+

**Backend**
1. Open `CampusHaps.sln`
2. Set `CampusHaps.Api` as the startup project and run it with the **https** profile
3. Swagger: https://localhost:7011/swagger (the database is created automatically)

**Frontend** (with the API running)
```
cd CampusHaps.Client
npm install
npm run dev
```
Open http://localhost:5173

**Tests:** Test → Test Explorer → Run All

**Note:** Register with a `.edu.au` email to log in. To reset the data, delete `campushaps.db` and rerun the API

# The GitHub Workflow
1. Before starting a session, do Git -> Pull
2. Add Code or changes
3. Then After your done go to: View -> Git Changes
4. Check your changed/added files and add a commit message
5. Click Commit All
6. Click Git -> Push

## How to open the Svelte Project (If you cant see it already)
File -> Open Folder -> select CampusHaps.Client



## Project Structure

CampusHaps.Api = Backend API

CampusHaps.Client = Svelte frontend

CampusHaps.Tests = NUnit tests

### Folders to use
- Models: Main application objects.
- Data: Database-related classes.
- Controllers: Handles API requests.
- Services: Contains application/business logic.
- Interfaces: Interfaces for services.
- DTOs: Objects used to send/receive data through the API.
- Extensions: Extension methods.
- CampusHaps.Tests: NUnit tests.
- CampusHaps.Client: Svelte frontend/UI.

