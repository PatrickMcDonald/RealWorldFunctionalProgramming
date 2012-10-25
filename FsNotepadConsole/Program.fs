// Learn more about F# at http://fsharp.net

open Module1

let testIt() = log {
    do! logMessage "starting"
    do! write("Enter name: ")
    let! name = read()
//    if System.String.IsNullOrEmpty(res) then System.Environment.Exit(0)
    return "Hello " + name + "!"
}

let (Log(res, logs)) = testIt()

if res = "Hello !" then System.Environment.Exit(0)

printfn "%s %A" res logs

let (Log(res2, logs2)) = testIt()

printfn "%s %A" res2 logs2

System.Console.ReadKey(true) |> ignore

