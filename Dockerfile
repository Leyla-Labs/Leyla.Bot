FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Main/Main.csproj", "Main/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Logs/Logs.csproj", "Logs/"]
COPY ["Spam/Spam.csproj", "Spam/"]
RUN dotnet restore "Main/Main.csproj"
COPY . .
WORKDIR "/src/Main"
RUN dotnet build "Main.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Main.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Main.dll"]
