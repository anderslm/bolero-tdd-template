module UseCases.Login

type Message =
    | SetUsername of string
    | SetPassword of string
    | GetSignedInAs
    | RecvSignedInAs of string option
    | SendSignIn
    | RecvSignIn of string option
    | SendSignOut
    | RecvSignOut
    
type Effect =
    | RetrieveSignedInAs of (string option -> Message)
    | SignIn of string * string * (string option -> Message)
    | SignOut of Message
    | None
    
type Model =
    {
        username: string
        password: string
        signedInAs: string option
        signInFailed: bool
    }
    
let init =
    {
        username = ""
        password = ""
        signedInAs = Option.None
        signInFailed = false
    }
    
let update model = function
    | SetUsername s ->
        { model with username = s }, None
    | SetPassword s ->
        { model with password = s }, None
    | GetSignedInAs ->
        model, RetrieveSignedInAs RecvSignedInAs
    | RecvSignedInAs username ->
        { model with signedInAs = username }, None
    | SendSignIn ->
        model, SignIn (model.username, model.password, RecvSignIn)
    | RecvSignIn username ->
        { model with signedInAs = username; signInFailed = Option.isNone username }, None
    | SendSignOut ->
        model, SignOut RecvSignOut
    | RecvSignOut ->
        { model with signedInAs = Option.None; signInFailed = false }, None