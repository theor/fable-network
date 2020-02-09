module App.Interop

open App.Types
open Fable.Core
open Fable.Core.JS

module Interop =
    type IInterop =
        abstract send: msg:Counter.Msg -> unit
        abstract subscribe: dispatchFunction:(Counter.Msg -> unit) -> unit
        abstract saveSnapshot: Counter.Model -> unit
        abstract host: unit -> string
        abstract connect: serverAddr:string -> Promise<Connection>

    [<Import("*", "../public/alert.js")>]
    let instance: IInterop = jsNative