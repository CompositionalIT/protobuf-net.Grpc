open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Server.Kestrel.Core
open ProtoBuf.Grpc.Server
open Saturn
open SaturnExtensions
open Server_FS

/// Add Code First GRPC
let configureServices (services: IServiceCollection) =
    services.AddCodeFirstGrpc()
    services

/// Configure the host with basic listening options
let configureHost (webHostBuilder:IWebHostBuilder) =
    webHostBuilder
       .ConfigureKestrel(fun options ->
            options.ListenLocalhost(10042, fun listenOptions ->
            listenOptions.Protocols <- HttpProtocols.Http2))

/// Configures the GRPC endpoints
let configureEndpoints (endpoints:Routing.IEndpointRouteBuilder) =
    endpoints.MapGrpcService<MyCalculator>() |> ignore

/// This is a hack and shouldn't be required.
let routes = router { get "/" (Giraffe.ResponseWriters.text "Hello") }

let app = application {
    url "http://localhost"
    use_router routes
    host_config configureHost
    service_config configureServices
    use_developer_exceptions
    use_routing
    endpoint_config configureEndpoints
}

run app