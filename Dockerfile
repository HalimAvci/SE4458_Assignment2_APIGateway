# Hata ayýklama kapsayýcýnýzý özelleþtirme ve Visual Studio’nun daha hýzlý hata ayýklama için görüntülerinizi derlemek üzere bu Dockerfile'ý nasýl kullandýðý hakkýnda bilgi edinmek için https://aka.ms/customizecontainer sayfasýna bakýn.

# Kapsayýcýlarý oluþturan veya çalýþtýran konak makinelerinin iþletim sistemine baðlý olarak FROM deyiminde belirtilen görüntünün deðiþtirilmesi gerekir.
# Daha fazla bilgi edinmek için https://aka.ms/containercompat sayfasýna bakýn

# Bu aþama, VS'den hýzlý modda çalýþtýrýldýðýnda kullanýlýr (Hata ayýklama yapýlandýrmasý için varsayýlan olarak ayarlýdýr)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Bu aþama, hizmet projesini oluþturmak için kullanýlýr
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MidtermAPIGateway/MidtermAPIGateway.csproj", "MidtermAPIGateway/"]
RUN dotnet restore "./MidtermAPIGateway/MidtermAPIGateway.csproj"
COPY . .
WORKDIR "/src/MidtermAPIGateway"
RUN dotnet build "./MidtermAPIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Bu aþama, son aþamaya kopyalanacak hizmet projesini yayýmlamak için kullanýlýr
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MidtermAPIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Bu aþama üretimde veya VS'den normal modda çalýþtýrýldýðýnda kullanýlýr (Hata Ayýklama yapýlandýrmasý kullanýlmazken varsayýlan olarak ayarlýdýr)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MidtermAPIGateway.dll"]