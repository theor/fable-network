module App

#if DEBUG
open Elmish.Debug
#endif

(**
 The famous Increment/Decrement ported from Elm.
 You can find more info about Elmish architecture and samples at https://elmish.github.io/
*)

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props

open Fable.Core.JsInterop // needed to call interop tools
open Fable.Import
open Fable.Core

// MODEL

type Model = Counter.Model

module Alert = 
  type IAlert =
    abstract send : msg:Counter.Msg -> unit
    abstract callback : f:(Counter.Msg -> unit) -> unit
    abstract someString: string

  [<Import("*", "../public/alert.js")>]
  let mylib: IAlert = jsNative


type Msg =
// | First of Counter.Msg
| Send of Counter.Msg
| Receive of Counter.Msg

let init() : Model * Cmd<Msg> = 

  (Counter.init(), Cmd.none)

let log () = Fable.Core.JS.console.log("From F#")

// Alert.mylib.triggerAlert (Counter.Msg.Increment)
// Alert.mylib.callback log
// UPDATE

let update (msg:Msg) (model:Model): (Model * Cmd<Msg>) =
    Fable.Core.JS.console.log("update", msg)
    match msg with
    | Send(m) ->
      Alert.mylib.send(m)
      (model,Cmd.none)
    | Receive(m) -> (Counter.update m model, Cmd.none)
    // | First(m) -> (Counter.update m (fst model), (snd model))
    // | Second(m) -> ((fst model), Counter.update m (snd model))
    // | Networked(m) -> 
    // | Second(m) -> model - 1

// VIEW (rendered with React)

let view (model:Model) (dispatch: (Msg -> unit)) =
  div []
      // [ Counter.view (fst model) (fun m -> dispatch(First(m)))
      [ Counter.view model (Send >> dispatch)
      ]

let sub (m:Model): Cmd<Msg> =
  let sub (dispatch: Msg -> unit) =
    let transmit msg = dispatch (Receive msg)
    transmit |> Alert.mylib.callback
  Cmd.ofSub sub

// App
Program.mkProgram init update view
|> Program.withSubscription sub
|> Program.withReactSynchronous "elmish-app"
|> Program.withConsoleTrace
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
