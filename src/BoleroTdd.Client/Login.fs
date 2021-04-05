module BoleroTddTemplate.Client.Login

open Bolero
open Bolero.Html
open Bolero.Remoting
open Elmish
open UseCases.Login

type Service =
    {
        signIn : string * string -> Async<option<string>>
        getUsername : unit -> Async<string>
        signOut : unit -> Async<unit>
    }

    interface IRemoteService with
        member this.BasePath = "/login"

let adaptEffect service = function
    | RetrieveSignedInAs msg -> Cmd.OfAsync.perform service.getUsername () (msg << Some)
    | SignIn (username, password, msg) -> Cmd.OfAsync.perform service.signIn (username, password) msg
    | SignOut msg -> Cmd.OfAsync.perform service.signOut () (fun _ -> msg)
    | None -> Cmd.none

type ViewTemplate = Template<"wwwroot/login.html">

let view model dispatch =
    ViewTemplate()
        .Username(model.username, fun s -> dispatch <| SetUsername s)
        .Password(model.password, fun s -> dispatch <| SetPassword s)
        .SignIn(fun _ -> dispatch SendSignIn)
        .ErrorNotification(
            cond model.signInFailed <| function
            | false -> empty
            | true ->
                ViewTemplate.ErrorNotificationTemplate()
                    .HideClass("is-hidden")
                    .Text("Sign in failed. Use any username and the password \"password\".")
                    .Elt()
        )
        .Elt()