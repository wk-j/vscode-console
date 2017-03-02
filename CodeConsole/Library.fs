module CodeConsole.Library

open System.Runtime.Serialization.Json
open System.Runtime.Serialization
open System.IO
open System.Text
open System
open CodeConsole.Formatter
open CodeConsole.Executor

[<DataContract>]
type Storage = {
    [<field: DataMember(Name = "openedPathsList")>]
    OpenedPathsList: PathInfo
} 

and [<DataContract>] PathInfo = {
    [<field: DataMember(Name="files")>]
    Files: string array
    [<field: DataMember(Name="folders")>]
    Folders: string array
}

let unjson<'t> (json: string) =
    use ms = new MemoryStream(UTF8Encoding.Default.GetBytes(json))
    let obj = DataContractJsonSerializer(typeof<'t>).ReadObject(ms)
    obj :?> 't

let readConfig() =
    let platform = Environment.OSVersion.Platform
    if platform = PlatformID.MacOSX || platform = PlatformID.Unix then
        let home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        let file = Path.Combine(home, "Library", "Application Support", "Code", "storage.json")
        match File.Exists file with
        | true ->
            let text = file |> File.ReadAllText
            let storage = unjson<Storage>(text)
            (storage)
        | false ->
            { OpenedPathsList = 
                { Files = Array.empty<string>
                  Folders = Array.empty<string> } 
            }
    else
        { OpenedPathsList = 
            { Files = Array.empty<string>
              Folders = Array.empty<string> } 
        }

let getFolder ans (options: (string*string) list) = 
    let ok, index = Int32.TryParse ans
    match ok with 
    | true ->
        if index <= options.Length then
            Some <| snd options.[index - 1] 
        else None
    | false ->
        None

let startConsole() =
    let storage = readConfig()
    let map x = 
        let info = DirectoryInfo(x)
        (info.Name, info.FullName)
    let options = 
        storage.OpenedPathsList.Folders 
        |> Array.map map 
        |> Array.truncate 20
        |> Array.toList

    let ans = readInput "Select project" options
    let folder = getFolder ans options
    match folder with
    | Some folder -> 
        executeCommand "code" (sprintf "\"%s\"" folder) 
        //startConsole()
    | None -> ()
        //startConsole()

