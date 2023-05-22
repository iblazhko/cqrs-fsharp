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

// Marker interface for command DTOs
[<AllowNullLiteral>]
type CqrsEventDto =
    interface
        inherit CqrsDto
    end
