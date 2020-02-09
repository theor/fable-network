module App.Types

type Route =
    | Index
    | Hosting
    | Join of string

type Model = 
  { route: Route
    connectAddress: string
    model: Counter.Model }
  static member Empty = { route=Index
                          connectAddress = ""
                          model = 0 }
  
  
type Msg =
    // | First of Counter.Msg
    | Send of Counter.Msg
    | Receive of Counter.Msg
    | Host
    | Connect of string
    | ChangeAddress of string