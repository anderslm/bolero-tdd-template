module UseCases.Tests.BooksTests

open System
open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open UseCases.Books

let model = init

[<Fact>]
let ``Creates effect to retrieve books`` () =
    let _, effect = update model GetBooks
    
    effect |> should be (ofCase<@RetrieveBooks@>)
    
[<Fact>]
let ``Can retrieve books`` () =
    let books = [|
        {
            title = "Some title"
            author = "Some author"
            publishDate = DateTime.Now
            isbn = "Some isbn"
        }
    |]
    
    let model, _ = update model (GotBooks books)
    
    model.books.Value |> should equal books