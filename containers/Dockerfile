## Builder stage
# use official Microsoft dotnet container
FROM microsoft/dotnet:latest as builder

# argument to choose type of git source (default is development)
ARG BRANCH=development

# create folders structure and clone neo-sharp repo
RUN mkdir /repo && \
    cd /repo && \
    git clone --recursive -b $BRANCH https://github.com/CityOfZion/neo-sharp.git && \
    cd /repo/neo-sharp/src/NeoSharp.Application && \
    dotnet publish --configuration Release --output /neo-sharp-builds


## Runtime stage
# use official Microsoft dotnet container
FROM microsoft/dotnet:2.1-runtime as runtime

# install rocksdb dependencies and delete temp packages after to clean the most garbase possible
RUN apt-get update && \
    apt-get install -y libgflags-dev libsnappy-dev zlib1g-dev libbz2-dev liblz4-dev libzstd-dev && \
    rm -rf /var/lib/apt/lists/* && \
    mkdir /home/neo-sharp

# copy app from builder stage
COPY --from=builder /neo-sharp-builds/* /home/neo-sharp/

# workdir
WORKDIR /home/neo-sharp

EXPOSE 8000/tcp

# first command to run when container is started will start app
ENTRYPOINT ["/usr/bin/dotnet", "NeoSharp.Application.dll"]
