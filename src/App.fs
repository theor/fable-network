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

let updateIndex (msg: IndexMsg) (model: IndexModel): IndexModel * Cmd<IndexMsg> =
    match msg with
    | ChangeAddress newAddr -> { model with connectAddress = newAddr }, Cmd.none
let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    Fable.Core.JS.console.error ("update", msg)
    match msg with
    | Send(m) ->
        App.Interop.Interop.instance.send (m)
        (model, Cmd.none)
    | Receive(m) ->
        let model = match model with
                    | Client c -> Client { c with session = { c.session with counter = Counter.update m c.session.counter } }  
                    | Model.Host c -> Model.Host { c with session = { c.session with counter = Counter.update m c.session.counter } }
                    | _ -> failwith "Not possible"
        model, Cmd.none
    | Host ->
        let serverAddress = App.Interop.Interop.instance.host()
        printfn "F# host addr %s" serverAddress
        Model.Host { serverAddress = serverAddress; session= SessionModel.Empty },
        App.Router.modifyUrl Route.Hosting
    | Connect serverAddress ->
        let clientAddr = App.Interop.Interop.instance.connect(serverAddress)
        eprintfn "F# client addr %s" clientAddr
        Model.Client {  serverAddress = serverAddress; clientAddress = clientAddr; session=SessionModel.Empty },
        App.Router.modifyUrl (Route.Join serverAddress)
    | Index indexMsg ->
        let (Model.Index(indexModel)) = model
        let m,c = updateIndex indexMsg indexModel
        Model.Index m, Cmd.map Msg.Index c
        
// | First(m) -> (Counter.update m (fst model), (snd model))
// | Second(m) -> ((fst model), Counter.update m (snd model))
// | Networked(m) ->
// | Second(m) -> model - 1

// VIEW (rendered with React)

let view (model: Model) (dispatch: Msg -> unit) =
    let indexLink = div [] [ a [ App.Router.href Route.Index ] [ Helpers.str "Index" ] ]
    printfn "VIEW %O" model
    match model with
    | Model.Index index -> div [] [
            button [ OnClick (fun _ -> dispatch Host) ] [ Helpers.str "Host" ]
            div [] [
                label [] [ Helpers.str "Server address" ]
                input [ Value index.connectAddress
                        OnChange (fun ev -> ChangeAddress ((ev.target :?> HTMLInputElement).value) |> Msg.Index |> dispatch)
                ]
                button [ OnClick (fun _ -> dispatch (Connect index.connectAddress)) ] [ Helpers.str "Connect" ]
            ]
        ]
    | Model.Host host -> div [] [
            indexLink
            label [] [ Helpers.str "HOSTING Join address:" ]
            a [ Route.Join host.serverAddress |> App.Router.href ] [ Helpers.str "host.serverAddress" ]
            div [] [ Counter.view host.session.counter (Send >> dispatch) ]
        ]
    | Model.Client client -> div [] [
        indexLink
        Helpers.str (sprintf "JOINING %s" client.clientAddress)
        br []
        Helpers.str (sprintf "server %s" client.serverAddress)
        div [] [ Counter.view client.session.counter (Send >> dispatch) ]
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
//    match route with
//    | None -> Model.Empty, Cmd.none
//    | Some r ->
//        let cmd = match r with
//                  | Route.Index -> Cmd.none
//                  | Route.Hosting -> Cmd.ofMsg Host
//                  | Route.Join addr -> Connect addr |> Cmd.ofMsg
//        Model.Empty, cmd
    App.Router.urlUpdate route Model.Empty

// App
Program.mkProgram init update view
|> Program.withSubscription sub
|> Program.withReactSynchronous "elmish-app"
|> Program.toNavigable (UrlParser.parseHash App.Router.route) App.Router.urlUpdate
// |> Program.withConsoleTrace
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
