module CodeConsole.Library

open System.Runtime.Serialization.Json
open System.Runtime.Serialization
open System.IO
open System.Text
open System

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
        let text = Path.Combine(home, "Library", "Application Support", "Code", "storage.json") |> File.ReadAllText
        let storage = unjson<Storage>(text)
        (storage)
    else
        { OpenedPathsList = 
            { Files = Array.empty<string>
              Folders = Array.empty<string> } 
        }

let startConsole() =
    let s = readConfig()
    Console.WriteLine(Environment.OSVersion.Platform)
    Console.WriteLine(s.OpenedPathsList.Folders.Length)
    ()

