module FsTestSample

open Xunit
open FsUnit.Xunit
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

let times = 1000
let timer = 1
[<Fact>]
let ``Testing with async.sleep``() = 
    let sw = Stopwatch()
    let poller = async {
        sw.Start()
        for i = 0 to times do
            do! Async.Sleep timer
        sw.Stop()
    }
    poller |> Async.RunSynchronously

    printfn "Async took %i" sw.ElapsedMilliseconds
    123

[<Fact>]
let ``Testing with task.delay``() = 
    let sw = Stopwatch()
    let poller = async {
        sw.Start()
        for i = 0 to times do
            do! Task.Delay timer |> Async.AwaitIAsyncResult |> Async.Ignore
        sw.Stop()
    }
    poller |> Async.RunSynchronously

    printfn "Async took %i" sw.ElapsedMilliseconds
    123

[<Fact>]
let ``Testing with thread.sleep``() = 
    let sw = Stopwatch()
    let poller = async {
        sw.Start()
        for i = 0 to times do
            Thread.Sleep timer
        sw.Stop()
    }
    poller |> Async.RunSynchronously

    printfn "Async took %i" sw.ElapsedMilliseconds
    123