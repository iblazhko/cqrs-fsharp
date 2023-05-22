module CQRS.Configuration.Infrastructure

open System
open System.Text
open SettingStringBuilder
open CQRS.Configuration.UrlPart

[<AllowNullLiteral>]
type EndpointSettings() =
    member val Scheme: string = String.Empty with get, set
    member val Host: string = String.Empty with get, set
    member val Port: int = 0 with get, set

    static member Empty = EndpointSettings(Scheme = String.Empty, Host = String.Empty, Port = 0)

    override this.ToString() =
        $"{this.Scheme}://{this.Host}:{this.Port}"


type MartenDbSettings() =
    member val Endpoint: EndpointSettings = null with get, set
    member val Username: string = String.Empty with get, set
    member val Password: string = String.Empty with get, set
    member val Database: string = String.Empty with get, set

    member val ConnectionOptions: string = String.Empty with get, set

    member self.getConnectionString() =
        $"Host={self.Endpoint.Host};Port={self.Endpoint.Port};Database={self.Database};Username={self.Username};Password={self.Password};{self.ConnectionOptions}"

    static member Empty =
        MartenDbSettings(
            Endpoint = EndpointSettings.Empty,
            Username = String.Empty,
            Password = String.Empty,
            ConnectionOptions = String.Empty
        )

    override this.ToString() =
        StringBuilder()
        |> appendSettingValue this.Endpoint (nameof this.Endpoint)
        |> appendSettingValue this.Username (nameof this.Username)
        |> appendSettingRedactedValue this.Password (nameof this.Password)
        |> appendSettingValue this.Database (nameof this.Database)
        |> fun b -> b.ToString()


type RabbitMqSettings() =
    member val Endpoint: EndpointSettings = null with get, set
    member val Username: string = String.Empty with get, set
    member val Password: string = String.Empty with get, set
    member val VirtualHost: string = String.Empty with get, set

    member self.getAmqpUrl() =
        $"amqp://{toUrlPart self.Username}:{toUrlPart self.Password}@{self.Endpoint.Host}:{self.Endpoint.Port}/{self.VirtualHost}"

    member self.getRabbitMqUrl() =
        $"rabbitmq://{self.Endpoint.Host}:{self.Endpoint.Port}/{self.VirtualHost}"

    static member Empty =
        RabbitMqSettings(
            Endpoint = EndpointSettings.Empty,
            Username = String.Empty,
            Password = String.Empty,
            VirtualHost = String.Empty
        )

    override this.ToString() =
        StringBuilder()
        |> appendSettingValue this.Endpoint (nameof this.Endpoint)
        |> appendSettingValue this.Username (nameof this.Username)
        |> appendSettingRedactedValue this.Password (nameof this.Password)
        |> appendSettingValue this.VirtualHost (nameof this.VirtualHost)
        |> fun b -> b.ToString()


type MassTransitSettings() =
    member val RabbitMq: RabbitMqSettings = RabbitMqSettings.Empty with get, set
    static member Empty = MassTransitSettings(RabbitMq = RabbitMqSettings.Empty)

    override this.ToString() =
        StringBuilder()
        |> appendSettingSection this.RabbitMq (nameof this.RabbitMq)
        |> fun b -> b.ToString()


type InfrastructureStartupSettings() =
    member val WaitOnStartup: bool = true with get, set
    member val RetryDelay: TimeSpan = TimeSpan.Zero with get, set
    member val RetryCount: int = 0 with get, set

    static member Empty =
        InfrastructureStartupSettings(WaitOnStartup = true, RetryDelay = TimeSpan.Zero, RetryCount = 0)

    override this.ToString() =
        StringBuilder()
        |> appendSettingValue this.WaitOnStartup (nameof this.WaitOnStartup)
        |> appendSettingValue this.RetryDelay (nameof this.RetryDelay)
        |> appendSettingValue this.RetryCount (nameof this.RetryCount)
        |> fun b -> b.ToString()


type LoggingSettings() =
    member val Level: string = String.Empty with get, set
    static member Empty = LoggingSettings(Level = String.Empty)

    override this.ToString() =
        StringBuilder()
        |> appendSettingValue this.Level (nameof this.Level)
        |> fun b -> b.ToString()
