# MapAPI

## TODO

- [ ] Authentication
- [ ] Swagger
- [ ] Docker compose
- [ ] Hangfire for background tasks

## Run Development

### VS Code

* Ctrl + Shift + P to run tasks
    * Tasks: Run Build Task (also Ctrl + Shift + B)
    * Tasks: Run Test Task
    * Tasks: Run Task
        * select build, run or test
        * If running, choose Tasks: Terminate Running Task to stop task

* Debug
    * Debug menu and start .NET Core Launch (web)

### Shell

```sh
# Run API
$ dotnet run -p src/MapApi/
# Execute tests
$ dotnet test test/MapApi.Tests/
# Run API with watch (run from src/MapApi)
$ dotnet watch run
```

### Postgres

```sh
# Postgres
# Get Postgres image
$ docker pull postgres
# Run image with map-db database
$ docker run --name map-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=pwd -e POSTGRES_DB=map-db -d -p 5432:5432 postgres
# Own docker for unit test
$ docker run --name map-postgres-ut -e POSTGRES_USER=mapuser -e POSTGRES_PASSWORD=pwd -e POSTGRES_DB=map-db-ut -d -p 5433:5432 postgres
```

## API

```sh
$ curl http://localhost:5000/api/map/location
$ curl http://localhost:5000/api/map/markers
$ curl -H "Content-Type: application/json" -X POST -d '{ "mapId": "", "lat": 60.170,"long": 24.940, "description": "hello 44"}' http://localhost:5000/api/markers
$ curl -H "Content-Type: application/json" -X DELETE -d '{ "mapId": "", "lat": 60.170,"long": 24.940, "description": "hello 44"}' http://localhost:5000/api/markers
```

## Run Production with Docker

TODO: Docker compose:
```sh
# In project root
$ docker-compose up
```

Or start MapApi docker:
```sh
# Build in src/MapApi
$ docker build -t web .
# Run
$ docker run -it -p 5000:5000 web
# Remeber to run postgres docker
```

## Links
* [Core debug](https://github.com/OmniSharp/omnisharp-vscode/blob/master/debugger.md)
* [Core unit test](https://docs.microsoft.com/en-us/dotnet/articles/core/testing/unit-testing-with-dotnet-test)
* [Docker & AWS](https://medium.com/trafi-tech-beat/running-net-core-on-docker-c438889eb5a#.2csx1do7r)
* [Marten](http://jasperfx.github.io/marten/)

## License

Licensed under the [MIT](LICENSE) License.