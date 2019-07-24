open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open ProtoBuf.Grpc.Server
open Saturn
open SaturnExtensions
open Server_FS

/// Add Code First GRPC
let configureServices (services: IServiceCollection) =
    services.AddCodeFirstGrpc()
    services

/// Configures the GRPC endpoints
let configureEndpoints (endpoints:Routing.IEndpointRouteBuilder) =
    endpoints.MapGrpcService<MyCalculator>()


/// This is a hack and shouldn't be required.
let routes = router { get "/" (Giraffe.ResponseWriters.text "Hello") }

let app = application {
    use_routing
    use_router routes
    use_developer_exceptions
    use_http2 10042
    service_config configureServices
    endpoint_config configureEndpoints
}

run app
