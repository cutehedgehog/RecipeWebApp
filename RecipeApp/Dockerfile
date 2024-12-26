
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ./RecipeApp/*.csproj ./
RUN dotnet restore

COPY ./RecipeApp.Test/*.csproj ./
RUN dotnet restore

COPY ./RecipeApp/ ./
RUN dotnet publish -c Release -o out

COPY ./RecipeApp.Test/ ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "RecipeApp.dll"]