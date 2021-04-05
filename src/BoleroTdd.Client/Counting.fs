module BoleroTddTemplate.Client.Counting

open Bolero
open Elmish
open UseCases.Counting

let adaptEffect = function
    | None -> Cmd.none

type ViewTemplate = Template<"wwwroot/counting.html">

let view model dispatch =
    ViewTemplate()
        .Decrement(fun _ -> dispatch Decrement)
        .Increment(fun _ -> dispatch Increment)
        .Value(model.counter, fun v -> dispatch <| SetCounter v)
        .Elt()