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
type ClientConnectingModel = { serverAddress: string }
type Model =
  | Index of IndexModel
  | Host of HostModel
  | ClientConnecting of ClientConnectingModel
  | Client of ClientModel
  static member Empty = Index { connectAddress= "" }
  
  
type IndexMsg =  ChangeAddress of string
 

type Connection = { clientAddr: string
                    snapshot: Counter.Model } 
type Msg =
    | Send of Counter.Msg
    | Receive of Counter.Msg
    | Host
    | Connect of string
    | Connected of Connection
    | Index of IndexMsg