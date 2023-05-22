# CQRS Benchmark

> Benchmark tests are **WIP**

## Build & run from CI process

Automated build is implemented via Docker.

Build target `DockerCompose.Benchmark` builds the test in Docker image,
and runs the tests in Docker Compose environment.

To run the benchmark tests:

```bash
./build.ps1 DockerCompose.Benchmark
```
