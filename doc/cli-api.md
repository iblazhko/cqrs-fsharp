# Interacting with the system using API

Assuming that the application is running as described in the
[“Build and Run”](./build-and-run.md) document, we can use either
`CQRS.CLI` project or a generic API client to interact with the system.

## CLI

In command prompt, navigate to `src/CQRS.CLI`. From there, you can run

```bash
dotnet run -- <CQRS.CLI command>
```

Examples:

### Basic help

```bash
dotnet run -- --help
```

```txt
Usage: cqrs.cli [command] [options]

Options:
  -?|-h|--help   Show help information.

Commands:
  add-items
  create
  deactivate
  get-inventory
  remove-items
  rename
```

### Command help

`dotnet run -- <command> --help`, e.g.

```bash
dotnet run -- create --help
```

### Create a new inventory

```bash
dotnet run -- create --name "ABC-123"
```

Output:

```json
{
  "entityId": "1c9beca4-fc8f-4241-99ab-b10f0dc7aa3c",
  "messageId": "6605a0ed-edb1-4098-8f43-6de7361306d8",
  "correlationId": "40f8b0ac-bc8a-4980-a65f-c6092c7295cf",
  "causationId": "00000000-0000-0000-0000-000000000000",
  "timestamp": "2023-05-23T15:17:37.3239368+00:00"
}
```

Note the `entityId` value (`1c9beca4-fc8f-4241-99ab-b10f0dc7aa3c` in the output
above), you will need it to provide input for following commands.

### View current state of an inventory

```bash
dotnet run -- get --id "1c9beca4-fc8f-4241-99ab-b10f0dc7aa3c"
```

Output:

```json
{
  "inventoryId": "1c9beca4-fc8f-4241-99ab-b10f0dc7aa3c",
  "name": "ABC-123",
  "stockQuantity": 0,
  "isActive": true
}
```

### Add items to an inventory

```bash
dotnet run -- add-items --id "1c9beca4-fc8f-4241-99ab-b10f0dc7aa3c" --count 5
```

### Remove items from an inventory

```bash
dotnet run -- remove-items --id "1c9beca4-fc8f-4241-99ab-b10f0dc7aa3c" --count 2
```

### Deactivate an inventory

Request will only be processed when the inventory is empty, i.e. has
`"stockQuantity": 0`. Once an inventory is deactivated, it will not be possible
to add/remove items or rename it.

```bash
dotnet run -- deactivate --id "1c9beca4-fc8f-4241-99ab-b10f0dc7aa3c"
```

## CURL or Postman

Instead of using CLI you can call API endpoints directly using CURL, Postman,
httpie etc.

```bash
curl -X POST http://localhost:17322/api/v1/inventories \
   -H 'Content-Type: application/json' \
   -d '{"Name": "ABC-123"}'

curl -X GET  http://localhost:17322/api/v1/inventories/60460fd4-80c9-46a6-808f-10b2bd85c656

curl -X POST http://localhost:17322/api/v1/inventories/60460fd4-80c9-46a6-808f-10b2bd85c656/add/10

curl -X POST http://localhost:17322/api/v1/inventories/60460fd4-80c9-46a6-808f-10b2bd85c656/remove/5

curl -X POST http://localhost:17322/api/v1/inventories/60460fd4-80c9-46a6-808f-10b2bd85c656/deactivate
```
