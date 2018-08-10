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

# Usage

The Dockerfile will run in two stages. The fisrt stage called Builder will create a SDK container image (where we compile the Node) and then the second stage will create a Runtime container image (where it will run).

To build the container run the following command (the last 'dot' is important):

`docker build -t neo-sharp-node:0.1_runtime .`

The last command cloned the neo-sharp git development branch and compiled the application with it.
If you want to use the git master branch You need to use the following command:

`docker build --build-arg BRANCH=master -t neo-sharp-node:0.1_runtime .`

Now that we have the container image built with the compiled app published, we need to run it.

`docker run -it -rm --name neo-sharp-node neo-sharp-node:0.1_runtime`

Last command will give you the neo-sharp console. All done ;)

# Contributing

Feel free to contribute to this project after reading the
[contributing guidelines](https://github.com/CityOfZion/neo-sharp/blob/master/CONTRIBUTING.md).

Before starting to work on a certain topic, create an new issue first, describing the feauture/topic you are going to implement. Please submit new pull requests to the `development` branch

Also, there is a [Trello board](https://trello.com/b/WwSwxpB7/city-of-zion-neo-sharp) to show the status of the project.

# License

- Open-source [MIT](https://github.com/CityOfZion/neo-sharp/blob/master/LICENCE.md)
