# use official Microsoft dotnet container
FROM microsoft/dotnet:latest

# create folders structure and clone neo-sharp repo
RUN mkdir /repo && \
    mkdir /neo-sharp-builds && \
    cd /repo && \
    git clone --recursive https://github.com/CityOfZion/neo-sharp.git  

# create docker volume to share to other containers
VOLUME neo-sharp-builds

# workdir
WORKDIR /repo/neo-sharp/src/NeoSharp.Application

# the command to build and publish the app when container runs
ENTRYPOINT ["/usr/bin/dotnet", "publish", "--configuration", "Release", "--output", "/neo-sharp-builds"]
