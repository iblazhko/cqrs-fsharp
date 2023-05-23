namespace CQRS.DTO

// Marker interface for DTOs
[<AllowNullLiteral>]
type CqrsDto =
    interface
    end

// Marker interface for command DTOs
[<AllowNullLiteral>]
type CqrsCommandDto =
    interface
        inherit CqrsDto
    end

// Marker interface for event DTOs
[<AllowNullLiteral>]
type CqrsEventDto =
    interface
        inherit CqrsDto
    end

// Marker interface for aggregate state DTOs
[<AllowNullLiteral>]
type CqrsStateDto =
    interface
        inherit CqrsDto
    end
