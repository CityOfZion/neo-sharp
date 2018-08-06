FROM microsoft/dotnet:2.1-runtime

RUN apt-get update && \
    apt-get install -y libgflags-dev libsnappy-dev zlib1g-dev libbz2-dev liblz4-dev libzstd-dev nuget && \
    rm -rf /var/lib/apt/lists/*

#ENTRYPOINT ["/bin/dotnet build --configuration Release -o /bin_files"]
ENTRYPOINT ["/bin/bash"]
