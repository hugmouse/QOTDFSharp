open System
open System.Net
open System.Text
open System.Net.Sockets
open System.Threading.Tasks

let IP = "0.0.0.0"
let PORT = "17"

let rec forever func =
    func ()
    forever func

type System.Random with
    member this.GetItem(items: _ []) = items.[this.Next(items.Length)]

let MessagesOfTheDay =
    [| "Learn something new today!\n"
       "Just learn more things\n" |]

let writeToSocket (socket: Socket) =
    let stream = new NetworkStream(socket)
    let r = System.Random()
    let message = Encoding.UTF8.GetBytes(r.GetItem(MessagesOfTheDay))
    printfn $"[writeToSocket] sending random quote of the day to {socket.RemoteEndPoint.ToString()}"

    try
        stream.Write(message, 0, message.Length)
    with
    | e -> printfn $"[writeToSocket] An exception occured while perfoming stream.write: {e}"

    stream.Dispose()
    socket.Dispose()

let startListening ip port =
    let ip = IPAddress.Parse IP
    let listener = new TcpListener(localaddr = ip, port = port)
    listener.Start()
    printfn "[startListening] Listening on %A:%A" ip port

    forever
    <| fun () -> listener.Server.Accept() |> writeToSocket

[<EntryPoint>]
let main argv =
    startListening "0.0.0.0" 8080
    0xDEAFBEEF
