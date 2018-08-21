<p align="center">
  Docker containers to build and run C# Node for the <a href="https://neo.org">NEO</a> blockchain.
</p>

# Overview

Using Docker Containers to build and run the Node is the best way to abstract and simulate a real clean environment.

Goals:
- Fast and easy way to build neo-sharp application.
- Run the Node in a small runtime container without SDK to simulate a non-dev environment.
- Build the base infrastructure to pave the path to running multiple tests after merging new Node code.
- Test the Node in different architectures with different runtimes.

# How to build (optional)

The Dockerfile will run in two stages. The fisrt stage called Builder will create a SDK container image (where we compile the Node) and then the second stage will create a Runtime container image (where it will run).

To build the container run the following command (the last 'dot' is important):<br>
`docker build -t neo-sharp-node:0.1_runtime .`

The last command cloned the neo-sharp git development branch and compiled the application with it.

If you want to build from the git master branch, you need to use the following command:<br>
`docker build --build-arg BRANCH=master -t neo-sharp-node:0.1_runtime .`

# Usage - Running the app

Now that we have the container image built with the compiled app published, we need to run it.

Run the following command if you've built the container in the last step or else skip to the next command:<br>
`docker run -p 8000:8000 -it --rm --name neo-sharp-node neo-sharp-node:0.1_runtime`

If you want to run the container using our pre-build images, do the following command:<br>
`docker run -p 8000:8000 -it --rm --name neo-sharp-node cityofzion/neo-sharp`

Last command will give you the neo-sharp console. All done ;)

If you want to play around with an image with full dotnet SDK, you can pull the SDK tag.<br>
`docker run -p 8000:8000 -it --rm --name neo-sharp-SDK cityofzion/neo-sharp:SDK`

# Contributing

Feel free to contribute to this project after reading the
[contributing guidelines](https://github.com/CityOfZion/neo-sharp/blob/master/CONTRIBUTING.md).

Before starting to work on a certain topic, create an new issue first, describing the feauture/topic you are going to implement. Please submit new pull requests to the `development` branch

# License

- Open-source [MIT](https://github.com/CityOfZion/neo-sharp/blob/master/LICENCE.md)

