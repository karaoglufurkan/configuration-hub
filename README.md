# ConfigurationHub

This repository contains a class library project called **ConfigurationHub**, designed to provide streamlined access to application configurations via Redis. Additionally, it includes a demo Web API project that allows you to list and modify configuration values through REST endpoints.

## Features

- Centralized configuration storage using Redis.
- In-memory caching with configurable refresh intervals.
- Easy-to-use dependency injection setup.
- REST API demo project for managing configurations.

## Getting Started with ConfigurationHub

### Installation

To use ConfigurationHub in your project, register the `ConfigurationReader` through dependency injection:

```csharp
services.AddConfigurationReader("redis-connection-string", "applicationName", 1000);
```

Alternatively, create an instance directly:


```csharp
var configurationReader = new ConfigurationReader("redis-connection-string", "applicationName", 1000);
```

### Parameters

- **Redis Connection String**: Specifies the connection string for the Redis server where configuration data is stored.
- **Application Name**: Identifies the specific application’s configuration data to be accessed and modified.
- **Refresh Interval**: Defines the interval, in milliseconds, at which ConfigurationHub will refresh in-memory data from Redis. This helps ensure up-to-date configurations without frequently querying Redis.

### Example Usage

The **ConfigurationHub** library simplifies retrieving configuration values. Here’s an example of how to retrieve values in your application:

```csharp
var configValue = configurationReader.GetValue<string>("YourConfigKey");
var configValue = configurationReader.GetValue<int>("YourConfigKey2");
```

### Running the Demo Project

To get the demo project up and running locally, follow these steps:

1. Make sure Docker is installed on your machine.

2. Run the following command in your terminal:

```bash
docker compose up
```

3. Once the setup completes, access the demo project's Swagger UI to interact with the API:

```
http://localhost:80/swagger
```

Through the Swagger UI, you can test the API endpoints, list existing configurations, and modify values.