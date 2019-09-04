# HackerNewsTest
This repo contains my solution for the Runpath test.

The solution is a .Net Core WebApi app and xunit test library.

# Usage instructions
To compile and run the WebApi project extract the project or clone the repo, and from the solution file directory run this:

```ps
dotnet run --project JsonPlaceholder
```

The API can be called using a suitable tool such as curl (e.g. from a bash shell):

```ps
curl --insecure --request GET https://localhost:5001/api/photos
```

You can also use numerous other methods, such as Powershell (Invoke-webrequest), or navigate with a web browser, but you will most likely get certificate errors. It is quite possible to overcome the errors, but using ```curl``` is quite straightforward. 

## Libraries

### The web project

The application uses the following libraries:

- Newtonsoft.Json : Excellent library for handling Javascript.

### The tests

I built the app using TDD, which is a strong preference of mine (although I'm used to dialing it back if the company I'm working for cannot be persuaded on the issue). Due to time constraints there are two aspects not covered by the test suite:

- the [```PlaceholderFetcher```](https://github.com/jamie-davis/RunpathTest/blob/master/JsonPlaceholder/PlaceholderOperations/PlaceholderFetcher.cs) class which uses a ```HttpClient``` to communicate with "http://jsonplaceholder.typicode.com". If better coverage was required, I would wrap ```HttpClient``` and provide the class with a fake. For the test, I made a pragmatic decision not to go this far.

- the main [controller](https://github.com/jamie-davis/RunpathTest/blob/master/JsonPlaceholder/Controllers/PhotosController.cs). It is possible to run tests directly against controllers but it is complex and requires a mocking framework.

The following libraries are used by the test suite:

- xunit : My favourite unit testing framework. It's a bit tidier than NUnit, and I'm not an MSTest fan.
- FluentAssertions : This gives much more readable and flexible assertion than the built in xunit ones. It's also compatible with MSTest and NUnit so it can help standardise the assertion code when moving between test frameworks.
- TestConsole : I wrote this library and I use it here for its support of approval style testing. If you've never encountered approval testing before, I recently wrote about it [here](https://jamie-davis.github.io/the-open-closed-dev/approval-style-testing/).
