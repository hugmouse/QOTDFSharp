open System
open System.Net
open System.Text
open System.Net.Sockets

type Random with
    member this.GetItem items =
        Array.length items |> this.Next |> Array.get items

let writeToSocket (msgs: string []) (socket: Socket) : Socket =
    use stream = new NetworkStream(socket)
    let r = Random()
    let message = r.GetItem msgs |> Encoding.UTF8.GetBytes
    printfn $"[writeToSocket] sending random quote of the day to {socket.RemoteEndPoint.ToString()}"

    try
        stream.Write(message, 0, message.Length)
    with
    | e -> printfn $"[writeToSocket] An exception occured while performing stream.write: {e}"

    socket

let startListening (msgs: string []) (ip: string) (port: int) =
    let ip = IPAddress.Parse ip
    let listener = TcpListener(localaddr = ip, port = port)
    listener.Start()
    printfn $"[startListening] Listening on {ip}:{port}"

    while true do
        let socket =
            listener.Server.Accept() |> writeToSocket msgs

        try
            socket.Shutdown(SocketShutdown.Both)
        with
        | e -> printfn $"An exception occured while trying to close the socket: {e}"

        socket.Close()

[<EntryPoint>]
let main argv =
    let MessagesOfTheDay =
        [| "Learn something new today!\n"
           "Just learn more things\n" |]

    let IP = "0.0.0.0"
    let PORT = 17

    startListening MessagesOfTheDay IP PORT
    0xDEAFBEEF
