module UseCases.Counting

type Message =
    | Increment
    | Decrement
    | SetCounter of int

type Effect =
    | None

type Model =
    {
        counter: int
    }
    
let init =
    {
        counter = 0
    }

let update model = function
    | Increment ->
        { model with counter = model.counter + 1 }, None
    | Decrement ->
        { model with counter = model.counter - 1 }, None
    | SetCounter value ->
        { model with counter = value }, None