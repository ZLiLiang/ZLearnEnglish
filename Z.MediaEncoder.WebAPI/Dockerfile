#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Z.MediaEncoder.WebAPI/Z.MediaEncoder.WebAPI.csproj", "Z.MediaEncoder.WebAPI/"]
RUN dotnet restore "Z.MediaEncoder.WebAPI/Z.MediaEncoder.WebAPI.csproj"
COPY . .
WORKDIR "/src/Z.MediaEncoder.WebAPI"
RUN dotnet build "Z.MediaEncoder.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Z.MediaEncoder.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Z.MediaEncoder.WebAPI.dll"]