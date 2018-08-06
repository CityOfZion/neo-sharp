# use official Microsoft dotnet container
FROM microsoft/dotnet:2.1-runtime

# install rocksdb dependencies and delete temp packages after to clean the most garbase possible
RUN apt-get update && \
    apt-get install -y libgflags-dev libsnappy-dev zlib1g-dev libbz2-dev liblz4-dev libzstd-dev && \
    rm -rf /var/lib/apt/lists/*

# workdir
WORKDIR /neo-sharp-builds

# first command to run when container is started will start app
ENTRYPOINT ["/usr/bin/dotnet", "NeoSharp.Application.dll"]
