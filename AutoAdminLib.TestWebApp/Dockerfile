FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-image

WORKDIR /backend
EXPOSE 5000
EXPOSE 5001

COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o /published --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /published
COPY --from=build-image /published .
ENV ASPNETCORE_URLS="http://0.0.0.0:5000"
ENTRYPOINT ["dotnet", "AutoAdmin.dll"]
