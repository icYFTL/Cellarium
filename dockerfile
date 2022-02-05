FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /opt/build

COPY ./Cellarium.sln ./
COPY ./Cellarium/ ./Cellarium/

RUN dotnet restore Cellarium.sln