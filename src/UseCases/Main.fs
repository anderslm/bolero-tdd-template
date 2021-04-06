module UseCases.Main

type Page =
    | Home
    | Counter
    | Books
    
type Message =
    | SetPage of Page
    | CountingMsg of Counting.Message
    | BooksMsg of Books.Message
    | LoginMsg of Login.Message
    | Error of exn
    | ClearError
    
type Effect =
    | CountingEffect of Counting.Effect
    | BooksEffect of Books.Effect
    | LoginEffect of Login.Effect
    | BatchEffect of Effect list
    | MessageEffect of Message
    | None
    
type Model =
    {
        page: Page
        countingModel: Counting.Model
        booksModel: Books.Model
        loginModel: Login.Model
        error: string option
    }
    
let init =
    {
        page = Home
        countingModel = Counting.init
        booksModel = Books.init
        loginModel = Login.init
        error = Option.None
    }
    
let update model = function
    | SetPage page ->
        { model with page = page }, None
    | CountingMsg msg ->
        let m, e = Counting.update model.countingModel msg
        
        { model with countingModel = m }
        , CountingEffect e
    | BooksMsg msg ->
        let m, e = Books.update model.booksModel msg
        
        { model with booksModel = m }
        , BooksEffect e
    | LoginMsg msg ->
        let m, e = Login.update model.loginModel msg
        
        { model with loginModel = m }
        , BatchEffect
              [LoginEffect e
               match msg with
               | Login.RecvSignIn (Some _) -> MessageEffect <| BooksMsg Books.GetBooks
               | _ -> None]
    | Error exn ->
        { model with error = Some exn.Message }, None
    | ClearError ->
        { model with error = Option.None }, None