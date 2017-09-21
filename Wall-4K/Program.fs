open FSharp.Data
open System.Text.RegularExpressions
open FSharp.Text.RegexProvider
open System.Net

type Wall = { Id: string; Url: string }
type thumbUrl = Regex< @".+\/(?<Id>\d+?)\/", noMethodPrefix = true >
let wc = new WebClient()
wc.Headers.Add("Cookie", "ota=%7B%22cookies-notice%22%3A%7B%22t%22%3A1%7D%7D; ota=%7B%22cookies-notice%22%3A%7B%22t%22%3A1%7D%7D; ec4a5c7eec253ec00a968727935b4765=1; wup_jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ3dXBfbWVtYmVyX2lkIjoiNDc0MzIiLCJ3dXBfbm90aWZpY2F0aW9ucyI6MCwibWVtYmVyIjp7IklEIjoiNDc0MzIiLCJVc2VybmFtZSI6InRqZG90IiwiZGF0ZV9yZWdpc3RlcmVkIjoiMTUwNjAxODIyNCIsIkVtYWlsIjoidGpkb3Qud2FsbHBhcGVydXBAbWFpbG51bGwuY29tIiwiQWN0aXZlIjoiMSIsIkxldmVsX2FjY2VzcyI6IjIiLCJhdmF0YXJfaWQiOiIwIiwicG9pbnRzIjoiMCIsInJhbmtpbmciOiIwIiwibGFzdF9hY3Rpdml0eSI6MTUwNjAxODIyNCwiaGFzX25vdGlmaWNhdGlvbnMiOiIwIiwiYmxvY2tlZF9ub3RpZmljYXRpb25zIjoiIiwid2FsbHBhcGVycyI6IjAiLCJmb2xsb3dlcnMiOiIwIiwiZm9sbG93aW5nIjoiMCJ9LCJleHAiOjE1MDg2MTAyMjR9.kN_tcNwiEa6vfn4G0NgfRG01KXtFvhwT0kMDW-a50KE; _ga=GA1.2.1547847158.1505940614; _gid=GA1.2.711001702.1505940614")

let downloadPage index =
    let mainUrl = sprintf "http://www.wallpaperup.com/resolution/4k_-_ultra_hd/%d" index
    printfn "Scanning: %s" mainUrl    
    let source = wc.DownloadString(mainUrl)
    let html = HtmlDocument.Parse(source)
    html.Descendants["img"]
        |> Seq.filter (fun x -> x.HasClass("thumb"))
        |> Seq.map (fun x -> x.AttributeValue("src"))
        |> Seq.map (fun url -> thumbUrl().Match(url).Id.Value)
        |> Seq.map (fun id -> { Id = id; Url = sprintf "http://www.wallpaperup.com/wallpaper/download/%s" id })
        |> Seq.iter (fun (wall) ->
                        printfn "Downloading: %s" wall.Url
                        wc.DownloadFile(wall.Url, (sprintf @"Walls\%s.jpg" wall.Id))
                    )
[<EntryPoint>]
let main argv =
    for i in 40 .. 100 do
         downloadPage i

    0 // return an integer exit code
