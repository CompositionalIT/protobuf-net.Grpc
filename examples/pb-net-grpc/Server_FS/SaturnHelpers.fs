module SaturnExtensions

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Server.Kestrel.Core

type Saturn.Application.ApplicationBuilder with
    /// Provides overrides for using custom endpoints.
    [<CustomOperation "endpoint_config">]
    member __.UseEndpoints(state:Saturn.Application.ApplicationState, endpointConfig) =
        { state with
            AppConfigs =
                fun app -> app.UseEndpoints (fun endpoint -> endpointConfig endpoint |> ignore)
                :: state.AppConfigs }

    /// Turns on basic routing.
    [<CustomOperation "use_routing">]
    member __.UseRouting (state:Saturn.Application.ApplicationState) =
        { state with
            AppConfigs =
                fun app -> app.UseRouting()
                :: state.AppConfigs }

    /// Provides a hook for working with the web host environment.
    [<CustomOperation "webhost_config">]
    member __.WebHostEnvironmentConfig (state:Saturn.Application.ApplicationState, config) =
        { state with
            AppConfigs =
                fun app ->
                    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
                    config app env
                :: state.AppConfigs }

    /// Turns on the developer exception page, if the environment is in development mode.
    [<CustomOperation "use_developer_exceptions">]
    member this.ActivateDeveloperExceptions (state:Saturn.Application.ApplicationState) =
        let config (app:IApplicationBuilder) (env:IWebHostEnvironment) =
            if env.IsDevelopment() then app.UseDeveloperExceptionPage()
            else app
        this.WebHostEnvironmentConfig(state, config)

    /// Turns on the developer exception page, if the environment is in development mode.
    [<CustomOperation "use_http2">]
    member this.UseHttp2 (state:Saturn.Application.ApplicationState, portNumber) =
        let config (webHostBuilder:IWebHostBuilder) =
            webHostBuilder
               .ConfigureKestrel(fun options ->
                    options.ListenLocalhost(portNumber, fun listenOptions ->
                    listenOptions.Protocols <- HttpProtocols.Http2))
        this.HostConfig(state, config)