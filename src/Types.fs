module App.Types

type Route =
    | Index
    | Hosting
    | Join of string

type Model = 
  { route: Route
    model: Counter.Model }
  static member Empty = { route=Index; model = 0 }
  
  
type Msg =
    // | First of Counter.Msg
    | Send of Counter.Msg
    | Receive of Counter.Msg
    | Host
    | Connect of string