module App.Router

open Browser
open Elmish.Navigation
open Elmish.UrlParser
open Fable.React.Props
open Elmish

open App.Types

let route: Parser<Route -> Route, Route> =
    oneOf
        [ map Route.Index top
          map Hosting (s "host")
          map Join (s "join" </> str) ]
        
let private toHash page =
    match page with
    | Route.Index -> "#/"
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
  | Some Route.Index -> Model.Empty, Cmd.none
  | Some Hosting -> Model.Empty, Cmd.ofMsg Msg.Host
  | Some (Join addr) -> Model.Empty, Cmd.ofMsg (Msg.Connect addr)
  | None -> Model.Empty, Navigation.modifyUrl "#"