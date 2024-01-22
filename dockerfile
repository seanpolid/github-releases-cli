FROM bitnami/dotnet-sdk:6

# Setup directory for code
RUN mkdir -p /opt/bin/csharp/release

# Setup the release tool
RUN git clone https://github.com/seanpolid/github-releases-cli.git
WORKDIR github-releases-cli

RUN chmod +x release.sh
RUN cp release.sh /opt/bin/release

RUN dotnet publish
WORKDIR bin/Debug/net6.0/publish
RUN cp * /opt/bin/csharp/release
