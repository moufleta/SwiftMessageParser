# SWIFT MT799 Message Parser Web API

## Introduction

Welcome to my SWIFT MT799 Message Parser Web API! This API is designed to parse SWIFT MT799 messages and store them in a database. In this README, I will provide you with all the necessary information to get started.

## Getting Started

### Database Initialization

When you run the application for the first time, the database initializer class will check if a `.db` file exists. If no database file is found, the initializer will automatically set up the database for you. Here's what you need to do:

1. Open the `appsettings.json` file.

2. Locate the `"ConnectionStrings"` section.

3. Update the `"DefaultConnection"` value with the path to your desired local database file. By default, it's set to `"Data Source=localdatabase.db"`.

### Configuring NLog

To configure the NLog logging settings for your local directory, follow these steps:

1. Locate the `nlog.config` file in the project directory.

2. Open the `nlog.config` file using a text editor of your choice.

3. Look for the `<target>` element that specifies the file path where log files will be saved. It should look something like this:

```xml
<target xsi:type="File" name="file" fileName="C:\Logs\yourlog.log" />

### Running the Application

To run my SWIFT MT799 Message Parser Web API:

1. Make sure you have configured the database connection in `appsettings.json` as mentioned above.

2. Build and run the application using your preferred method (e.g., Visual Studio, `dotnet run`, etc.).

3. The API will be hosted locally, and you can access it via `http://localhost:portNumber`. Replace `portNumber` with the actual port number your API is running on (e.g., `http://localhost:5000`).

## Sample SWIFT MT799 Message File

I have included a sample SWIFT MT799 message file named `success.txt` in the root directory of the project. You can use this file to test the API. Send a SWIFT MT799 message to the API and verify the parsing process.

## API Endpoints

Here is the main API endpoint:

- `/api/message/insert` - Insert and parse a SWIFT MT799 message.

For more details on how to use this endpoint, please refer to the API documentation.

## API Documentation

For comprehensive information on how to use my SWIFT MT799 Message Parser Web API and its endpoints, please check out the API documentation. You can access it at `http://localhost:portNumber/swagger` when the API is running.

## Contact

If you have any questions, encounter issues, or want to provide feedback, please feel free to contact me at [katrin.lilova@gmail.com].

Thank you for using my SWIFT MT799 Message Parser Web API!
