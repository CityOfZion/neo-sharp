<p align="center">
<img
    src="http://res.cloudinary.com/vidsy/image/upload/v1503160820/CoZ_Icon_DARKBLUE_200x178px_oq0gxm.png"
    width="125px"
  >
</p>

<h1 align="center">neo-sharp</h1>

<p align="center">      
  <a href="https://travis-ci.org/CityOfZion/neo-sharp">
    <img src="https://travis-ci.org/CityOfZion/neo-sharp.svg?branch=master">
  </a>      
</p>

<p align="center">
  <b>C#</b> Node for the <a href="https://neo.org">NEO</a> blockchain.
</p>

# Overview

neo-sharp is a new node implementation created with the intention of using modern software engineering practices to improve code quality, testability, modularity, and readability without sacrificing speed of execution.

neo-sharp is not aimed at adding new features.

# Projects

* Application - Composition Route and entry point
	* Client - Console based client
* Core - Neo datastructures, Configuration, Logging
	* Persistence - Database based persistence
	* Network - Neo p2p protocol

# Short term TODO

Feedback on structure, particularly with regard to the dependency injection patterns and the intended goal of modularity between projects. Once satisfied, move onto implementation of network protocol.

# Contributing

Feel free to contribute to this project after reading the
[contributing guidelines](https://github.com/CityOfZion/neo-go/blob/master/CONTRIBUTING.md).

Before starting to work on a certain topic, create an new issue first, describing the feauture/topic you are going to implement. Please submit new pull requests to the `development` branch

# License

- Open-source [MIT](https://github.com/CityOfZion/neo-sharp/blob/master/LICENCE.md)