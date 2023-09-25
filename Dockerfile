#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN apt-get update && apt-get install -y curl
WORKDIR /app
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/SpendManagement.Identity.API/SpendManagement.Identity.API.csproj", "SpendManagement.Identity.API/"]
COPY ["src/SpendManagement.Identity.IoC/SpendManagement.Identity.IoC.csproj", "SpendManagement.Identity.IoC/"]
COPY ["src/SpendManagement.Identity.Application/SpendManagement.Identity.Application.csproj", "SpendManagement.Identity.Application/"]
COPY ["src/SpendManagement.Identity.Data/SpendManagement.Identity.Data.csproj", "SpendManagement.Identity.Data/"]
RUN dotnet restore "SpendManagement.Identity.API/SpendManagement.Identity.API.csproj"
COPY . .

RUN dotnet build "src/SpendManagement.Identity.API/SpendManagement.Identity.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/SpendManagement.Identity.API/SpendManagement.Identity.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

#RUN dotnet tool install --global dotnet-ef
#ENV PATH="$PATH:/root/.dotnet/tools"
#RUN dotnet ef database update --context IdentityDataContext -s "src\SpendManagement.Identity.API"


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpendManagement.Identity.API.dll"]