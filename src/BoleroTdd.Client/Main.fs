module BoleroTddTemplate.Client.Main

open Elmish
open Bolero
open Bolero.Html
open Bolero.Remoting
open Bolero.Remoting.Client
open Bolero.Templating.Client
open UseCases
open UseCases.Main

let router = Router.infer SetPage (fun model -> model.page)

type MainTemplate = Template<"wwwroot/main.html">

let menuItem (model: Model) (page: Page) (text: string) =
    MainTemplate.MenuItem()
        .Active(if model.page = page then "is-active" else "")
        .Url(router.Link page)
        .Text(text)
        .Elt()

let view model dispatch =
    MainTemplate()
        .Menu(concat [
            menuItem model Home "Home"
            menuItem model Counter "Counter"
            menuItem model Data "Download data"
        ])
        .Body(
            cond model.page <| function
            | Home -> MainTemplate.Home().Elt()
            | Counter -> Counting.view model.countingModel (dispatch << CountingMsg)
            | Data ->
                cond model.loginModel.signedInAs <| function
                | Some username -> Books.view model.booksModel username (fun () -> dispatch <| LoginMsg Login.SendSignOut) (dispatch << BooksMsg)
                | Option.None -> Login.view model.loginModel (dispatch << LoginMsg)
        )
        .Error(
            cond model.error <| function
            | Option.None -> empty
            | Some err ->
                MainTemplate.ErrorNotification()
                    .Text(err)
                    .Hide(fun _ -> dispatch ClearError)
                    .Elt()
        )
        .Elt()

type MyApp() =
    inherit ProgramComponent<Model, Message>()

    override this.Program =
        let bookService = this.Remote<Books.Service>()
        let loginService = this.Remote<Login.Service>()
        
        let rec adaptEffect = function
            | BooksEffect e ->
                Books.adaptEffect bookService e |> Cmd.map BooksMsg
            | CountingEffect e ->
                Counting.adaptEffect e |> Cmd.map CountingMsg
            | LoginEffect e ->
                Login.adaptEffect loginService e |> Cmd.map LoginMsg
            | BatchEffect es ->
                es
                |> List.map adaptEffect
                |> Cmd.batch
            | MessageEffect msg ->
                Cmd.ofMsg msg
            | None -> Cmd.none
        
        let update message model =
            let model, effect = update model message
            
            model, adaptEffect effect
        Program.mkProgram (fun _ -> init, Cmd.ofMsg (LoginMsg Login.GetSignedInAs)) update view
        |> Program.withRouter router
#if DEBUG
        |> Program.withHotReload
#endif
