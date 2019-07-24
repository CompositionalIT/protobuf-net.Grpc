open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Server.Kestrel.Core
open ProtoBuf.Grpc.Server
open Saturn
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

let configureApp (app:IApplicationBuilder) =
     let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
     if env.IsDevelopment() then app.UseDeveloperExceptionPage() |> ignore
     
     app.UseRouting()
        .UseEndpoints(fun endpoints -> endpoints.MapGrpcService<MyCalculator>() |> ignore)
        |> ignore

     app

/// This is a hack and shouldn't be required.
let routes = router { get "/" (Giraffe.ResponseWriters.text "Hello") }

let app = application {
    url "http://localhost"
    use_router routes
    app_config configureApp
    host_config configureHost
    service_config configureServices
}

run app