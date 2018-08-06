<p align="center">
  Docker containers to build and run C# Node for the <a href="https://neo.org">NEO</a> blockchain.
</p>

# Overview

Using Docker Containers to build and run the Node is the best way to abstract and simulate a real clean environment.

Goals:
- Fast and easy build neo-shap application.
- Run the Node in a small runtime container without SDK to simulate a non-dev environment.
- Build the base insfrastructure to pave the path to running multiple tests after merging new Node code.
- Test the Node in different architectures with different runtimes.

# Usage

First we need to build the SDK container image (where we compile the Node) and then the Runtime container image (where it will run)

To build the SDK container move to the folder:

`cd sdk_container`

And then build the sdk container (the last 'dot' is important):

`docker build -t neo-sharp:0.1_sdk .`

Now to build the runtime move to the folder:

`cd ../runtime_container`

And then run the build command

`docker build -t neo-sharp:0.1_runtime .`

Now that we have the container images built, we need to run each in sequence.
First will be the SDK container, because it will create a shared folder and compile the Node there.

`docker run -it -v neo-sharp-builds:/neo-sharp-builds --name neo-sharp-sdk neo-sharp:0.1_sdk`

last command will compile the Application and exit to prompt (if no errors exceptions) and then You should start the App running the runtime container.

`docker run -it -v neo-sharp-builds:/neo-sharp-builds --name neo-sharp-runtime neo-sharp:0.1_runtime`

Last command will give You neo-sharp console. All done ;)

# Contributing

Feel free to contribute to this project after reading the
[contributing guidelines](https://github.com/CityOfZion/neo-sharp/blob/master/CONTRIBUTING.md).

Before starting to work on a certain topic, create an new issue first, describing the feauture/topic you are going to implement. Please submit new pull requests to the `development` branch

Also, there is a [Trello board](https://trello.com/b/WwSwxpB7/city-of-zion-neo-sharp) to show the status of the project.

# License

- Open-source [MIT](https://github.com/CityOfZion/neo-sharp/blob/master/LICENCE.md)