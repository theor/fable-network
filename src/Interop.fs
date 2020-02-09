module App.Interop

open Fable.Core


module Interop =
    type IInterop =
        abstract send: msg:Counter.Msg -> unit
        abstract callback: f:(Counter.Msg -> unit) -> unit
        abstract host: unit -> string
        abstract connect: serverAddr:string -> (*clientAddr*) string

    [<Import("*", "../public/alert.js")>]
    let instance: IInterop = jsNative