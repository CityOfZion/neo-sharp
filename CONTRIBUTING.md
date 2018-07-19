# Contributing
Thank you for the interest in contributing to neo-sharp.  To contribute, please adhere to the following guidelines:

## Issue Tracking:

* Check github for any open issues or feature requests before making a request.

* Check the [Trello Board](https://trello.com/b/WwSwxpB7/city-of-zion-neo-sharp) to see the current progress of an issue or feature, and verify someone is already working on this issue before beginning work.

## Testing
It's part of the goal project to reach a point where all the code running has a Unit Test that cover it. Let's face it, it's not because a project has a good test coverage mean that there is no bug. In the other end, a code without test it's a huge code-smell. When there is a change (due a bug or improvement), how do we know that everything still working as expected?
Testing is important to retro-testing previous features, mean that future changes aren't breaking the current features.

The threadhold to archive is around 80% test coverage and this value is not 100% because there is code that will not be testable, like: DTO objects and Third-Party calls.

### How to write Unit Tests?
The methodoly used for Unit Tests should be the AAA that can be found int this link (https://msdn.microsoft.com/en-us/library/hh694602.aspx).

## Best Patterns and Practices to follow:
Another goal of the neo-sharp team is to grow together and help everyone become better developers than when they started working on the project.  We will continue to curate a list of resources on best patterns and practices, anti-patterns, bad habits, or any other quick reads that could be beneficial to increase the quality of code contributors of the project:

#### General Coding Practices and Conventions:
Neo-sharp aims to adhere to Microsft C# .NET best practices and industry standard design patterns.  Please reference the following resources and familiarize yourself with recommended best practices.  

* [C# Gettting Started](https://github.com/dotnet/training-tutorials/tree/master/content/csharp/getting-started)
This resource was a good crash course on some of the basics in C# if you are new to the language or if you could use a quick refresher.

* [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
This is a quick resource to demonstrate code commenting, general formatting, and a quick overview.

* [.NET Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/)
This is a comprehensive resource that enumerates everything from naming conventions, to proper exception handlling, all the way to when to use certain collections and types.

* [Patterns and Anti-patterns](https://github.com/dotnet/training-tutorials/blob/master/content/csharp/getting-started/patterns-antipatterns.md)
This is a good overview of some of the more popular design patterns (and anti-patterns) with examples

#### Anti-patterns and bad habits
We've all done it at some point.  Here you can find some links to resources on patterns and practices that could get you in trouble as a developer.

* [9 Anti-patterns Every Programmer Should Be Aware Of](https://sahandsaba.com/nine-anti-patterns-every-programmer-should-be-aware-of-with-examples.html)
This is a good quick list of some more general concepts that often hang developers and teams up from making good progress. 

* [Code Smells](https://blog.codinghorror.com/code-smells/)
This was a nice concise list of some common code smells that developers will encounter.  

## To Contribute a Pull Request:
* Please make sure you write unit tests and those tests pass locally before issuing a pull request.  Code that does not compile and pass CI testing will not be merged.

*  Open a pull request to the [development branch](https://github.com/CityOfZion/neo-sharp/tree/development), and reference the relevant issue(s) in Trello or Github (if relevant)

* Ask the community for feedback and review if you are implementing a new design pattern, implementing a new module, or making any change(s) that may affect the overall architecture or design considerations of the project

* After receiving feedback or approval, <b>squash your commits</b> and add a <b>detailed</b> commit message enumerating the features that were implemented, the bug that was addressed, or a <b>detailed description</b> of the changes that were implemented.

* All commit messages must be in English
