FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EDI315Payment/EDI315Payment.csproj", "EDI315Payment/"]
RUN dotnet restore "EDI315Payment/EDI315Payment.csproj"
COPY . .  
WORKDIR "/src/EDI315Payment"  
RUN dotnet build "EDI315Payment.csproj" 

FROM build AS publish
RUN dotnet publish "EDI315Payment.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EDI315Payment.dll"]
