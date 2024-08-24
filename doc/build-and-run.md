# Building and Running

## Prerequisites

- .NET 8.0 SDK: <https://dotnet.microsoft.com/download>
- PowerShell Core: <https://github.com/powershell/powershell>
- Docker with Docker Compose: <https://www.docker.com/>

### Development Environment

Development environment is a personal choice, here are some options:

- [JetBrains Rider](https://www.jetbrains.com/rider/)
- [Visual Studio](https://visualstudio.microsoft.com/)
- [Visual Studio Code](https://code.visualstudio.com/) with following
  extensions:
    - Ionide F# (`ionide.ionide-fsharp`)
    - PowerShell (`ms-vscode.powershell`)
    - Docker (`ms-azuretools.vscode-docker`)

## Build + Test

To build the solution and run tests:

```bash
./build.ps1
```

> NOTE. Command above is equivalent to
>
> ```bash
> ./build.ps1 -Target FullBuild
> ```

### Cleanup

To completely remove all the intermediate build files:

```bash
./build.ps1 Prune
```

To cleanup Docker containers and images, first make sure that the system
is stopped (see `DockerCompose.Stop` target below).

Then run command

```bash
./build.ps1 Prune.Docker
```

## Run

To run the solution:

```bash
./build.ps1 DockerCompose.Start
```

This will start Docker Compose project and will display combined console logs
from all the components involved. The system will stop when any of the
containers terminates. Use `Ctrl+C` to terminate manually.

```bash
./build.ps1 DockerCompose.StartDetached
```

This will run the Docker Compose project in detached mode;
use `docker logs` to inspect individual parts.

To stop a system running in the detached mode:

```bash
./build.ps1 DockerCompose.Stop
```
