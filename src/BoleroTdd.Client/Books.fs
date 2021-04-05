module BoleroTddTemplate.Client.Books

open Bolero
open Bolero.Html
open Bolero.Remoting
open Elmish
open UseCases.Books

type Service =
    {
        getBooks: unit -> Async<Book[]>
        addBook: Book -> Async<unit>
        removeBookByIsbn: string -> Async<unit>
    }

    interface IRemoteService with
        member this.BasePath = "/books"

let adaptEffect service = function
    | None -> Cmd.none
    | RetrieveBooks msg -> Cmd.OfAsync.perform service.getBooks () msg  

type ViewTemplate = Template<"wwwroot/books.html">

let view model (username: string) signOut dispatch =
    ViewTemplate()
        .Reload(fun _ -> dispatch GetBooks)
        .Username(username)
        .SignOut(fun _ -> signOut())
        .Rows(cond model.books <| function
            | Option.None ->
                ViewTemplate.EmptyData().Elt()
            | Some books ->
                forEach books <| fun book ->
                    tr [] [
                        td [] [text book.title]
                        td [] [text book.author]
                        td [] [text (book.publishDate.ToString("yyyy-MM-dd"))]
                        td [] [text book.isbn]
                    ])
        .Elt()