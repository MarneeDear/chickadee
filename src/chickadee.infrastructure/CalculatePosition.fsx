open System.IO
open System

(*   

    /092345z4903.50N/07201.75W>Test1234

    POSTION example
    4903.50N/07201.75W

    LATTITUDE
    4903.50N is 49 degrees 3 minutes 30 seconds north.
    01234567
    49 degress
    3 minutes
    30 seconds

    LONGITUDE
    07201.75W is 72 degrees 1 minute 45 seconds west.
    012345678
    72 degrees
    1 minues
    45 seconds

    32° 20' 21.3318

*)

let nsew hem =
    match hem with
    | "N" -> 1.0
    | "S" -> -1.0
    | "W" -> -1.0
    | "E" -> 1.0
    | _ -> 0.0

nsew "W"

//let lat = "4903.50N"
let lat = "3220.35N"
let lat_deg = float (lat.Substring(0,2).TrimStart([|'0'|]))
let lat_min = float (lat.Substring(2, 2).TrimStart([|'0'|]))
let lat_sec = float (lat.Substring(4,3))
let lat_hem = lat.Substring(7,1)

//let decimal = deg + (min/60) + (sec * 60)

let latResult = (lat_deg + (lat_min/60.0) + (lat_sec * 60.0)/3600.0) * (nsew lat_hem)
//printfn "LAT RESULT %f" latResult

//07201.75W
//012345678
let lon = "07201.75W"
let lon_deg = float (lon.Substring(0,3).TrimStart([|'0'|]))
let lon_min = float (lon.Substring(3, 2).TrimStart([|'0'|]))
let lon_sec = float (lon.Substring(5,3))
let lon_hem = lon.Substring(8,1)
nsew lon_hem

let lonResult = 
    (lon_deg + (lon_min/60.0) + (lon_sec * 60.0)/3600.0) * (nsew lon_hem)
