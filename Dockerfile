# Hata ay�klama kapsay�c�n�z� �zelle�tirme ve Visual Studio�nun daha h�zl� hata ay�klama i�in g�r�nt�lerinizi derlemek �zere bu Dockerfile'� nas�l kulland��� hakk�nda bilgi edinmek i�in https://aka.ms/customizecontainer sayfas�na bak�n.

# Kapsay�c�lar� olu�turan veya �al��t�ran konak makinelerinin i�letim sistemine ba�l� olarak FROM deyiminde belirtilen g�r�nt�n�n de�i�tirilmesi gerekir.
# Daha fazla bilgi edinmek i�in https://aka.ms/containercompat sayfas�na bak�n

# Bu a�ama, VS'den h�zl� modda �al��t�r�ld���nda kullan�l�r (Hata ay�klama yap�land�rmas� i�in varsay�lan olarak ayarl�d�r)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Bu a�ama, hizmet projesini olu�turmak i�in kullan�l�r
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MidtermAPIGateway/MidtermAPIGateway.csproj", "MidtermAPIGateway/"]
RUN dotnet restore "./MidtermAPIGateway/MidtermAPIGateway.csproj"
COPY . .
WORKDIR "/src/MidtermAPIGateway"
RUN dotnet build "./MidtermAPIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Bu a�ama, son a�amaya kopyalanacak hizmet projesini yay�mlamak i�in kullan�l�r
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MidtermAPIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Bu a�ama �retimde veya VS'den normal modda �al��t�r�ld���nda kullan�l�r (Hata Ay�klama yap�land�rmas� kullan�lmazken varsay�lan olarak ayarl�d�r)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MidtermAPIGateway.dll"]