FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EDI315Api/EDI315Api.csproj", "EDI315Api/"]
RUN dotnet restore "EDI315Api/EDI315Api.csproj"
COPY . .
WORKDIR "/src/EDI315Api"
RUN dotnet build "EDI315Api.csproj" 

FROM build AS publish
RUN dotnet publish "EDI315Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EDI315Api.dll"]
