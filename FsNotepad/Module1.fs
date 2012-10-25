#if INTERACTIVE
#else
module Module1
#endif

type Logging<'a> =
    | Log of 'a * string list

type LoggingBuilder() =
    member this.Bind(Log(value, list1), f) =
        let (Log(newValue, list2)) = f(value)
        Log(newValue, list1 @ list2)

//    member this.Combine expr1 expr2 =
//        expr1
//        expr2
//
//    member this.Delay f =
//        f
  
    member this.Return value =
        Log(value, [])
  
    member this.Zero() = Log((), [])

let log = LoggingBuilder()

let logMessage msg =
    Log((), [msg])

let write(s) = log {
    do! logMessage("writing: " + s)
    printf "%s" s
}

let read() = log {
    do! logMessage("reading")
    let input = System.Console.ReadLine()
    do! logMessage ("User entered: " + input)
    return input
}

