module UseCases.Tests.LoginTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open UseCases.Login

let model = init

[<Fact>]
let ``Can set username and password`` () =
    let username = "Some username"
    let password = "Some password"
    let model, _ = update model (SetUsername username)
    let model, _ = update model (SetPassword password)
    
    model.username |> should equal username
    model.password |> should equal password

[<Fact>]
let ``Creates effect to get signed in username`` () =
    let _, effect = update model GetSignedInAs
    
    effect |> should be (ofCase<@RetrieveSignedInAs@>)
    
[<Fact>]
let ``Can retrieve signed in username`` () =
    let username = "Username"
    let model, _ = update model (RecvSignedInAs <| Some username)
    
    model.signedInAs.Value |> should equal username
    
[<Fact>]
let ``Creates effect to sign in`` () =
    let _, effect = update model SendSignIn
    
    effect |> should be (ofCase<@SignIn@>)
    
[<Fact>]
let ``Can sign in`` () =
    let username = "Username"
    let model, _ = update model (RecvSignIn <| Some username)
    
    model.signedInAs.Value |> should equal username
    model.signInFailed |> should equal false
    
[<Fact>]
let ``Can fail to sign in`` () =
    let model, _ = update model (RecvSignIn Option.None)
    
    model.signedInAs.IsNone |> should equal true
    model.signInFailed |> should equal true
    
[<Fact>]
let ``Creates effect to sign out`` () =
    let _, effect = update model SendSignOut
    
    effect |> should be (ofCase<@SignOut@>)
    
[<Fact>]
let ``Can sign out`` () =
    let model, _ = update model RecvSignOut
    
    model.signedInAs.IsNone |> should equal true
    model.signInFailed |> should equal false
