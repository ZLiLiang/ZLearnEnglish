#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Z.IdentityService.WebAPI/Z.IdentityService.WebAPI.csproj", "Z.IdentityService.WebAPI/"]
RUN dotnet restore "Z.IdentityService.WebAPI/Z.IdentityService.WebAPI.csproj"
COPY . .
WORKDIR "/src/Z.IdentityService.WebAPI"
RUN dotnet build "Z.IdentityService.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Z.IdentityService.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Z.IdentityService.WebAPI.dll"]