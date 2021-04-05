module UseCases.Books

open System

type Message =
    | GetBooks
    | GotBooks of Book[]

and Book =
    {
        title: string
        author: string
        publishDate: DateTime
        isbn: string
    }
    
type Effect =
    | RetrieveBooks of (Book array -> Message)
    | None
    
type Model =
    {
        books : Book array option
    }

let init =
    {
        books = Option.None
    }

let update model = function
    | GetBooks ->
        model, RetrieveBooks GotBooks
    | GotBooks books ->
        { model with books = Some books }, None