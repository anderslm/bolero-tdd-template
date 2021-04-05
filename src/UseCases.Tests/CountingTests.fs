module Tests.CountingTests

open Xunit
open FsCheck.Xunit
open FsUnit.Xunit
open UseCases.Counting

let model = init

[<Fact>]
let ``Counting starts at 0`` () =
    model.counter |> should equal 0
    
[<Property>]
let ``Can set counter to any number`` (i : int) =
    let model, _ = update model (SetCounter i)
    
    model.counter |> should equal i
    
[<Property>]
let ``Counter is incremented by 1`` (i : int) =
    let model, _ = update model (SetCounter i)
    let model, _ = update model Increment
    
    model.counter |> should equal (i + 1)
    
[<Property>]
let ``Counter is decremented by 1`` (i : int) =
    let model, _ = update model (SetCounter i)
    let model, _ = update model Decrement
    
    model.counter |> should equal (i - 1)