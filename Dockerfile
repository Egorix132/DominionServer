## Deployment Image ##
# USE Runtime base image
FROM mcr.microsoft.com/dotnet/runtime:6.0

EXPOSE 3000

# Copy the source to /app
WORKDIR /app
COPY ./PreBuild .
CMD ["dotnet", "Dominion.SocketIoServer.dll"]