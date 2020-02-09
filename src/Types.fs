module App.Types

type Route =
    | Index
    | Hosting
    | Join of string
    
type SessionModel = {
    counter: Counter.Model }
with
    static member Empty = {counter = Counter.init() }

    
type IndexModel = {connectAddress: string}
type HostModel = { serverAddress: string
                   session: SessionModel }
type ClientModel = { serverAddress: string
                     clientAddress: string
                     session: SessionModel }

type Model =
  | Index of IndexModel
  | Host of HostModel
  | Client of ClientModel
  static member Empty = Index { connectAddress= "" }
  
  
type IndexMsg =  ChangeAddress of string
  
type Msg =
    // | First of Counter.Msg
    | Send of Counter.Msg
    | Receive of Counter.Msg
    | Host
    | Connect of string
    | Index of IndexMsg