# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["src/Grc.Web/Grc.Web.csproj", "src/Grc.Web/"]
COPY ["src/Grc.Application/Grc.Application.csproj", "src/Grc.Application/"]
COPY ["src/Grc.Application.Contracts/Grc.Application.Contracts.csproj", "src/Grc.Application.Contracts/"]
COPY ["src/Grc.Domain/Grc.Domain.csproj", "src/Grc.Domain/"]
COPY ["src/Grc.Domain.Shared/Grc.Domain.Shared.csproj", "src/Grc.Domain.Shared/"]
COPY ["src/Grc.EntityFrameworkCore/Grc.EntityFrameworkCore.csproj", "src/Grc.EntityFrameworkCore/"]
COPY ["src/Grc.HttpApi/Grc.HttpApi.csproj", "src/Grc.HttpApi/"]
COPY ["src/Grc.FrameworkLibrary.Domain/Grc.FrameworkLibrary.Domain.csproj", "src/Grc.FrameworkLibrary.Domain/"]
COPY ["src/Grc.FrameworkLibrary.Application/Grc.FrameworkLibrary.Application.csproj", "src/Grc.FrameworkLibrary.Application/"]
COPY ["src/Grc.FrameworkLibrary.Application.Contracts/Grc.FrameworkLibrary.Application.Contracts.csproj", "src/Grc.FrameworkLibrary.Application.Contracts/"]
COPY ["src/Grc.Assessment.Domain/Grc.Assessment.Domain.csproj", "src/Grc.Assessment.Domain/"]
COPY ["src/Grc.Risk.Domain/Grc.Risk.Domain.csproj", "src/Grc.Risk.Domain/"]
COPY ["src/Grc.Risk.Application/Grc.Risk.Application.csproj", "src/Grc.Risk.Application/"]
COPY ["src/Grc.Risk.Application.Contracts/Grc.Risk.Application.Contracts.csproj", "src/Grc.Risk.Application.Contracts/"]
COPY ["src/Grc.Evidence.Domain/Grc.Evidence.Domain.csproj", "src/Grc.Evidence.Domain/"]
COPY ["src/Grc.Evidence.Application/Grc.Evidence.Application.csproj", "src/Grc.Evidence.Application/"]
COPY ["src/Grc.Evidence.Application.Contracts/Grc.Evidence.Application.Contracts.csproj", "src/Grc.Evidence.Application.Contracts/"]

RUN dotnet restore "src/Grc.Web/Grc.Web.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/src/Grc.Web"
RUN dotnet build "Grc.Web.csproj" -c Release -o /app/build
RUN dotnet publish "Grc.Web.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000

ENTRYPOINT ["dotnet", "Grc.Web.dll"]
