module App

#if DEBUG
open Elmish.Debug
#endif

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Elmish.Navigation

// MODEL
open App.Types
open Browser.Types
open Fable.React

// UPDATE

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    Fable.Core.JS.console.log ("update", msg)
    match msg with
    | Send(m) ->
        App.Interop.Interop.instance.send (m)
        (model, Cmd.none)
    | Receive(m) -> ({ model with model = Counter.update m model.model}, Cmd.none)
    | Host ->
        let hostAddr = App.Interop.Interop.instance.host()
        printfn "F# host addr %s" hostAddr
        { model with route = Route.Hosting }, App.Router.newUrl Route.Hosting
    | Connect addr ->
        let clientAddr = App.Interop.Interop.instance.connect(addr)
        eprintfn "F# client addr %s" clientAddr
        { model with route = Route.Join addr }, App.Router.newUrl (Route.Join addr)
    | ChangeAddress newAddr -> { model with connectAddress = newAddr }, Cmd.none
        
        
// | First(m) -> (Counter.update m (fst model), (snd model))
// | Second(m) -> ((fst model), Counter.update m (snd model))
// | Networked(m) ->
// | Second(m) -> model - 1

// VIEW (rendered with React)

let view (model: Model) (dispatch: Msg -> unit) =
    let indexLink = div [] [ a [ App.Router.href Route.Index ] [ Helpers.str "Index" ] ]
    printfn "VIEW %O" model
    match model.route with
    | Index -> div [] [
            button [ OnClick (fun _ -> dispatch Host) ] [ Helpers.str "Host" ]
            div [] [
                label [] [ Helpers.str "Server address" ]
                input [ Value model.connectAddress
                        OnChange (fun ev -> ChangeAddress ((ev.target :?> HTMLInputElement).value) |> dispatch)
                ]
                button [ OnClick (fun _ -> dispatch (Connect model.connectAddress)) ] [ Helpers.str "Connect" ]
            ]
        ]
    | Hosting -> div [] [
            indexLink
            label [] [ Helpers.str "Join address:" ]
            a [ Route.Join "asd" |> App.Router.href ] [ Helpers.str "Index" ]
        ]
    | _ -> div [] [
        indexLink
        Helpers.str (string model.route)
    ]
//    div []
//        // [ Counter.view (fst model) (fun m -> dispatch(First(m)))
//        [ Counter.view model.model (Send >> dispatch) ]

let sub (m: Model): Cmd<Msg> =
    let sub (dispatch: Msg -> unit) =
        let transmit msg = dispatch (Receive msg)
        transmit |> App.Interop.Interop.instance.subscribe
    Cmd.ofSub sub



let init route: Model * Cmd<Msg> =
    printfn "INIT"
    match route with
    | None -> Model.Empty, Cmd.none
    | Some r ->
        let cmd = match r with
                  | Route.Index -> Cmd.none
                  | Route.Hosting -> Cmd.ofMsg Host
                  | Route.Join addr -> Connect addr |> Cmd.ofMsg
        { Model.Empty with route = r }, cmd
//    App.Router.urlUpdate route Model.Empty

// App
Program.mkProgram init update view
|> Program.withSubscription sub
|> Program.withReactSynchronous "elmish-app"
|> Program.toNavigable (UrlParser.parseHash App.Router.route) App.Router.urlUpdate
|> Program.withConsoleTrace
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
