namespace CQRS.DTO

// Marker interfaces for DTOs

[<AllowNullLiteral>]
type CqrsDto =
    interface
    end

[<AllowNullLiteral>]
type CqrsCommandDto =
    interface
        inherit CqrsDto
    end

[<AllowNullLiteral>]
type CqrsEventDto =
    interface
        inherit CqrsDto
    end

[<AllowNullLiteral>]
type CqrsStateDto =
    interface
        inherit CqrsDto
    end
