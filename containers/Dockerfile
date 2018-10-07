## Builder stage
# use official Microsoft dotnet container
FROM microsoft/dotnet:latest as builder

# arguments to choose branch and source repo (defaults are development and CoZ/neo-sharp)
ARG BRANCH="development"
ARG SOURCE_REPO="https://github.com/CityOfZion/neo-sharp.git"

# make builds faster. Caching packages on a temporary build machine is a waste of time
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

# opt-out telemetry
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# create folders structure and clone neo-sharp repo
RUN mkdir /repo && \
    cd /repo && \
    git clone --recursive -b $BRANCH $SOURCE_REPO && \
    cd /repo/neo-sharp/src/NeoSharp.Application && \
    dotnet publish --configuration Release --output /home/neo-sharp

# run additional compilation of rocksdb shared lib for ARM architecture
RUN export arch=$(uname -m);if [ "$arch" = "armv7l" ]; then apt-get update && \
    apt-get install -y libgflags-dev libsnappy-dev zlib1g-dev libbz2-dev liblz4-dev libzstd-dev git build-essential && \
    git clone https://github.com/facebook/rocksdb.git && \
    cd rocksdb && \
    make shared_lib && \
    cp librocksdb.so /home/neo-sharp/;fi

## Runtime stage
# use official Microsoft dotnet container
FROM microsoft/dotnet:2.1-runtime as runtime

# arguments to choose what network to used (defaults to mainnet)
ARG NEO_NETWORK="mainnet"

# make builds faster. Caching packages on a temporary build machine is a waste of time
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

# opt-out telemetry
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# install rocksdb dependencies and delete temp packages after to clean the most garbase possible
RUN apt-get update && \
    apt-get install -y libgflags-dev libsnappy-dev zlib1g-dev libbz2-dev liblz4-dev libzstd-dev && \
    rm -rf /var/lib/apt/lists/* && \
    mkdir /home/neo-sharp

# copy app from builder stage
COPY --from=builder /home/neo-sharp/* /home/neo-sharp/

# Set 
RUN cp /home/neo-sharp/appsettings.$NEO_NETWORK.json /home/neo-sharp/appsettings.json

# workdir
WORKDIR /home/neo-sharp

EXPOSE 8000/tcp 10332/tcp

# default first command to run when container is started will start app
CMD ["/usr/bin/dotnet", "NeoSharp.Application.dll","network start","rpc start"]
