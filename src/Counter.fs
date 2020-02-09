module Counter

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props

// MODEL

type Model = int

type Msg =
| Increment
| Decrement
| Sync of Model

let init() : Model = 0

// UPDATE

let update (msg:Msg) (model:Model) =
    match msg with
    | Increment -> model + 1
    | Decrement -> model - 1
    | Sync snapshot -> snapshot

// VIEW (rendered with React)

let view (model:Model) dispatch =

  div []
      [ label [] [ str "as" ]
        button [ OnClick (fun _ -> dispatch Increment) ] [ str "+" ]
        div [] [ str (string model) ]
        button [ OnClick (fun _ -> dispatch Decrement) ] [ str "-" ] ]
