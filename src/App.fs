module App

(**
 The famous Increment/Decrement ported from Elm.
 You can find more info about Elmish architecture and samples at https://elmish.github.io/
*)

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props

// MODEL

type Model = Counter.Model * Counter.Model

type Msg =
| First of Counter.Msg
| Second of Counter.Msg

let init() : Model = (Counter.init(), Counter.init())

// UPDATE

let update (msg:Msg) (model:Model) =
    match msg with
    | First(m) -> (Counter.update m (fst model), (snd model))
    | Second(m) -> ((fst model), Counter.update m (snd model))
    // | Second(m) -> model - 1

// VIEW (rendered with React)

let view (model:Model) (dispatch: (Msg -> unit)) =

  div []
      // [ Counter.view (fst model) (fun m -> dispatch(First(m)))
      [ Counter.view (fst model) (First >> dispatch)
        Counter.view (snd model) (Second >> dispatch) ]

// App
Program.mkSimple init update view
|> Program.withReactSynchronous "elmish-app"
|> Program.withConsoleTrace
|> Program.run
