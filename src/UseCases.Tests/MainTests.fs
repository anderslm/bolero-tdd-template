module UseCases.Tests.MainTests

open FsUnit.Xunit
open FsUnit.CustomMatchers
open Xunit
open UseCases
open UseCases.Main

let model = init

[<Fact>]
let ``Can change page`` () =
    let model, _ = update model (SetPage Counter)
    
    model.page |> should be (ofCase<@Counter@>)
    
[<Fact>]
let ``Can register an error`` () =
    let ex = "Some error"
    let model, _ = update model (Error <| exn ex)
    
    model.error.Value |> should equal ex
    
[<Fact>]
let ``Can clear error`` () =
    let model, _ = update model (Error <| exn "Some error")
    let model, _ = update model ClearError
    
    model.error.IsNone |> should equal true
    
[<Fact>]
let ``Forwards message to counting component`` () =
    let model, effect = update model (CountingMsg <| Counting.SetCounter 42)
    
    model.countingModel.counter |> should equal 42
    effect |> should be (ofCase<@CountingEffect@>)
    
[<Fact>]
let ``Forwards message to books component`` () =
    let model, effect = update model (BooksMsg <| Books.GotBooks [||])
    
    model.booksModel.books.Value |> should equal [||]
    effect |> should be (ofCase<@BooksEffect@>)

[<Fact>]
let ``Forwards message to login component`` () =
    let username = "Some username"
    let model, effect = update model (LoginMsg <| Login.SetUsername username)
    
    model.loginModel.username |> should equal username
    match effect with
    | BatchEffect effects ->
        effects.Head |> should be (ofCase<@LoginEffect@>)
    | _ -> failwith "Expected a batch of effects; the first being from the login component"
    
[<Fact>]
let ``Gets books when signing in`` () =
    let _, effect = update model (LoginMsg <| Login.RecvSignIn (Some "User"))
    
    match effect with
    | BatchEffect effects ->
        match effects.[1] with
        | MessageEffect msg ->
            match msg with
            | BooksMsg msg ->
                msg |> should be (ofCase<@Books.GetBooks@>)
            | _ -> failwith "Expected a message to get books"
        | _ -> failwith "Expected a message to the books component"
    | _ -> failwith "Expected a batch of effects; one from the component and another for the books component"