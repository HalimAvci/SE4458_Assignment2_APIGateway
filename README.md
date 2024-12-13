# Assignment 2 (Midterm API Gateway)

This project is an API Gateway solution built using ASP.NET Core and YARP (Yet Another Reverse Proxy). It serves as a gateway for managing communication between microservices, providing a single entry point for requests from clients.
The project can be containerized and run using Docker.


# Technologies Used
  - **ASP.NET Core 8.0**
  - **YARP (Yet Another Reverse Proxy)**
  - **Docker**
  - **NGINX (Reverse Proxy)**
  - **Visual Studio**


# Overview

This API Gateway acts as an entry point for all client requests. It routes the requests to the appropriate backend microservices, which handle specific business logic. YARP is used to simplify the reverse proxy implementation.

# YARP Configuration
YARP is used in this project to handle reverse proxying. Below is an example of how the YARP proxy is configured in the appsettings.json file:

```bash
   "ReverseProxy": {
       "Routes": {
           "se4458-midterm-route": {
               "ClusterId": "se4458-midterm-cluster",
               "Match": {
                   "Path": "se4458-midterm/{**catch-all}"
               },
               "Transforms": {
                   "PathPattern": "{**catch-all}"
               }
           }
       },
       "Clusters": {
           "SE4458-midterm-cluster": {
               "Destinations": {
                   "destination1": {
                       "Address": "http://se4458-midterm:5100"
                   }
               }
           }
       }
   }
```

# Program.cs Configuration
In the Program.cs file, the configuration for YARP reverse proxy is set up with the following lines:

```bash
var builder = WebApplication.CreateBuilder(args);

// Add reverse proxy to the services collection
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")); // Loads proxy routes from the configuration

var app = builder.Build();

// Map reverse proxy to handle incoming requests
app.MapReverseProxy();

app.Run();
```
- AddReverseProxy(): This method adds YARP to the services collection, which sets up the reverse proxy functionality.
- LoadFromConfig(): This loads the YARP configuration from the appsettings.json file, which contains the routes and clusters to forward requests to.
- MapReverseProxy(): This maps the proxy to the application's HTTP request pipeline, allowing it to process and forward incoming requests to the appropriate microservices.

# Docker Desktop Container

![Ekran görüntüsü 2024-12-14 011008](https://github.com/user-attachments/assets/c2931eb0-037a-4290-8e7b-f76bc98949ce)
