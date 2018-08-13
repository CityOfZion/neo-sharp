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
  <a href="https://codecov.io/gh/CityOfZion/neo-sharp">
    <img src="https://codecov.io/gh/CityOfZion/neo-sharp/branch/master/graph/badge.svg" />
  </a>
  <a href="https://github.com/CityOfZion/neo-sharp/blob/master/LICENSE">
    <img src="https://img.shields.io/badge/license-MIT-blue.svg">
  </a>

</p>

<p align="center">
  <b>C#</b> Node for the <a href="https://neo.org">NEO</a> blockchain.
</p>

# Overview

Neo-sharp is a new core and node implementation of NEO with three main project goals:

- To break interdependencies in NEO and allow a plug-able modular design to be used by node implementations. The modules will consist in: Network, VM, Persistence and Consensus.

- To create testing infrastructure for both specification of behavior and benchmark of modifications.

- To develop an initial fully compatible version and one new experimental high performance module for each component.

This project is not about adding new features to NEO, it is about establishing and adhering coding best practcies, implementing proper design patterns, and ensuring the code is testable with a focus on unit test code coverage.

# Projects

* Application - Composition Route and entry point
	* Client - Console based client
* BinarySerialization - Binary serialization and deserialization of NEO entities
* Consensus - NEO Consensus implementation (TODO)
* Core - Configuration Management, Dependency Injection Framework, Logging Management and Models
	* Cryptography - all crypto based functions
	* Network - Neo p2p protocol
* DI - Dependency Injection framework
	* SimpleInjector - Module to implement Simple Injector as the DI framework
* Logging - Logging framework implementation
	* NLog - Logging implementation using NLog
* Persistence - Disk and in-memory based data storage / persistence
	* RocksDB - RocksDB Repository Implementation
	* RedisDB - Redis Repository Implementation
* SmartContract - NEO Smart Contract execution engine (TODO)
* VM - Interop assembly for HyperVM / VM integration (TODO)
* Wallet - NEO wallet implementation (TODO)

# Architecture 
<img src="https://trello-attachments.s3.amazonaws.com/5abf13a56a3d403651be77b1/5aca3d04b38bd3a13691eb14/4c583187ab49b7502c4a663cbdc61882/Client-Usage-Diagram.PNG">

<img src="https://trello-attachments.s3.amazonaws.com/5abf13a56a3d403651be77b1/5aca3cf96cd4c979a002adf7/a6e3922803a5dd7cf888fe8f43f5ffd5/Node-Usage-Diagram.PNG">

<img src="https://trello-attachments.s3.amazonaws.com/5abf13a56a3d403651be77b1/5aca3d04b38bd3a13691eb14/4c583187ab49b7502c4a663cbdc61882/Client-Usage-Diagram.PNG">
# Contributing

Feel free to contribute to this project after reading the
[contributing guidelines](https://github.com/CityOfZion/neo-sharp/blob/master/CONTRIBUTING.md).

Before starting to work on a certain topic, create an new issue first, describing the feature/topic you are going to implement. Please submit new pull requests to the `development` branch.

# License

- Open-source [MIT](https://github.com/CityOfZion/neo-sharp/blob/master/LICENCE.md)
