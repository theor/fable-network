module App.Router

open Browser
open Elmish.Navigation
open Elmish.UrlParser
open Fable.React.Props
open Elmish

open App.Types

let route: Parser<Route -> Route, Route> =
    oneOf
        [ map Index top
          map Hosting (s "host")
          map Join (s "join" </> str) ]
        
let private toHash page =
    match page with
    | Index -> "#/"
    | Hosting -> "#host"
    | Join s -> sprintf "#join/%s" s
        
let href route =
    Href (toHash route)

let modifyUrl route =
    route |> toHash |> Navigation.modifyUrl

let newUrl route =
    route |> toHash |> Navigation.newUrl

let modifyLocation route =
    window.location.href <- toHash route

let urlUpdate (result: Option<Route>) model =
  printfn "url %O" result
  match result with
  | Some Index -> {model with route = Index}, Cmd.none
  | Some Hosting -> {model with route = Hosting}, Cmd.none
  | Some (Join addr) -> {model with route = (Join addr)}, Cmd.none//, Cmd.ofMsg (Msg.Connect addr)
  | None -> ({model with route = Index}, Navigation.modifyUrl "#")