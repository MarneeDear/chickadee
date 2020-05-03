module CommonTests

open Expecto
open chickadee.core.Common
open FSharp.Data

type APRSSymbols = CsvProvider<"../../data/primarysymbols.tsv">

[<Tests>]
let SymbolCodeTests =
    let symbols = new APRSSymbols()

    //let symbolCode sym =
    //    SymbolCode.fromSymbol sym

    let getSymbolChar (symbolCode:SymbolCode) = 
        let sym, _ = symbolCode.ToCode()
        sym

    let getDSTCall (symbolCode:SymbolCode) = 
        let _, dstcall = symbolCode.ToCode()
        dstcall

    let symbolDataFromTSV code =
        symbols.Rows |> Seq.find (fun r -> r.CODE = code)

    let symbolTest testSym testDSTCall testDesc (testSymbolCode:SymbolCode) =
        let sym = getSymbolChar testSymbolCode
        let dstcall = getDSTCall testSymbolCode
        let data = symbolDataFromTSV (sprintf "/%c" sym)
        Expect.equal (SymbolCode.fromSymbol sym) (Some testSymbolCode) (sprintf "From Symbol should have been %s" testDesc)
        Expect.equal data.DESCRIPTION testDesc (sprintf "TSV description should be %s" testDesc)
        Expect.equal (sym) testSym (sprintf "%s code should be %c" testDesc testSym)
        Expect.equal dstcall testDSTCall (sprintf "%s DSTCALL should be %s" testDesc testDSTCall)
        Expect.equal dstcall data.DSTCALL (sprintf "%s CSV DSTCALL should be %s" testDesc testDSTCall)


    testList "Symbol Code Tests" [
        testCase "Police station" <| fun _ ->
            symbolTest '!' "BB" "Police station" SymbolCode.PoliceStation
        testCase "Digipeater" <| fun _ ->
            symbolTest '#' "BD" "Digipeater" SymbolCode.Digipeater
        testCase "Telephone" <| fun _ ->
            symbolTest '$' "BE" "Telephone" SymbolCode.Telephone
        testCase "DX cluster" <| fun _ ->
            symbolTest '%' "BF" "DX cluster" SymbolCode.DXCluster
        testCase "HF gateway" <| fun _ ->
            symbolTest '&' "BG" "HF gateway" SymbolCode.HFGateway
        testCase "Small aircraft" <| fun _ ->
            symbolTest ''' "BH" "Small aircraft" SymbolCode.SmallAircraft
        testCase "Mobile satellite station" <| fun _ ->
            symbolTest '(' "BI" "Mobile satellite station" SymbolCode.MobileSatelliteStation
        testCase "Wheelchair, handicapped" <| fun _ ->
            symbolTest ')' "BJ" "Wheelchair, handicapped" SymbolCode.Handicapped
        testCase "Snowmobile" <| fun _ ->
            symbolTest '*' "BK" "Snowmobile" SymbolCode.Snowmobile
        testCase "Red Cross" <| fun _ ->
            symbolTest '+' "BL" "Red Cross" SymbolCode.RedCross
        testCase "Boy Scouts" <| fun _ ->
            symbolTest ',' "BM" "Boy Scouts" SymbolCode.BoyScouts
        testCase "House" <| fun _ -> 
            symbolTest '-' "BN" "House" SymbolCode.House
        testCase "Red X" <| fun _ ->
            symbolTest '.' "BO" "Red X" SymbolCode.RedX 
        testCase "Red dot" <| fun _ ->
            symbolTest '/' "BP" "Red dot" SymbolCode.RedDot 
        testCase "Fire" <| fun _ ->
            symbolTest ':' "MR" "Fire" SymbolCode.Fire 
        testCase "Campground, tent" <| fun _ ->
            symbolTest ';' "MS" "Campground, tent" SymbolCode.CampgroundTent 
        testCase "Motorcycle" <| fun _ ->
            symbolTest '<' "MT" "Motorcycle" SymbolCode.Motorcycle
        testCase "Railroad engine" <| fun _ ->
            symbolTest '=' "MU" "Railroad engine" SymbolCode.RailroadEngine
        testCase "Car" <| fun _ ->
            symbolTest '>' "MV" "Car" SymbolCode.Car
        testCase "File server" <| fun _ ->
            symbolTest '?' "MW" "File server" SymbolCode.FileServer
        testCase "Hurricane predicted path" <| fun _ ->
            symbolTest '@' "MX" "Hurricane predicted path" SymbolCode.HurricanePredictedPath
        testCase "Aid station" <| fun _ ->
            symbolTest 'A' "PA" "Aid station" SymbolCode.AidStation
        testCase "BBS" <| fun _ ->
            symbolTest 'B' "PB" "BBS" SymbolCode.BBS
        testCase "Canoe" <| fun _ ->
            symbolTest 'C' "PC" "Canoe" SymbolCode.Canoe
        testCase "Eyeball" <| fun _ ->
            symbolTest 'E' "PE" "Eyeball" SymbolCode.Eyeball
        testCase "Farm vehicle, tractor" <| fun _ ->
            symbolTest 'F' "PF" "Farm vehicle, tractor" SymbolCode.FarmVehicleTractor
        testCase "Grid square, 3 by 3" <| fun _ ->
            symbolTest 'G' "PG" "Grid square, 3 by 3" SymbolCode.GridSquare3x3
        testCase "Hotel" <| fun _ ->
            symbolTest 'H' "PH" "Hotel" SymbolCode.Hotel
        testCase "TCP/IP network station" <| fun _ ->
            symbolTest 'I' "PI" "TCP/IP network station" SymbolCode.TCPIPNetworkStation
        testCase "School" <| fun _ ->
            symbolTest 'K' "PK" "School" SymbolCode.School
        testCase "PC user" <| fun _ ->
            symbolTest 'L' "PL" "PC user" SymbolCode.PCUser
        testCase "Mac apple" <| fun _ ->
            symbolTest 'M' "PM" "Mac apple" SymbolCode.MacApple
        testCase "NTS station" <| fun _ ->
            symbolTest 'N' "PN" "NTS station" SymbolCode.NTSStation
        testCase "Balloon" <| fun _ ->
            symbolTest 'O' "PO" "Balloon" SymbolCode.Balloon
        testCase "Police car" <| fun _ ->
            symbolTest 'P' "PP" "Police car" SymbolCode.PoliceCar
        testCase "Recreational vehicle" <| fun _ ->
            symbolTest 'R' "PR" "Recreational vehicle" SymbolCode.RecreationalVehicle
        testCase "Space Shuttle" <| fun _ ->
            symbolTest 'S' "PS" "Space Shuttle" SymbolCode.SpaceShuttle
        testCase "SSTV" <| fun _ ->
            symbolTest 'T' "PT" "SSTV" SymbolCode.SSTV
        testCase "Bus" <| fun _ ->
            symbolTest 'U' "PU" "Bus" SymbolCode.Bus
        testCase "ATV, Amateur Television" <| fun _ ->
            symbolTest 'V' "PV" "ATV, Amateur Television" SymbolCode.ATVAmateurTelevision
        testCase "Weather service site" <| fun _ ->
            symbolTest 'W' "PW" "Weather service site" SymbolCode.WeatherServiceSite
        testCase "Helicopter" <| fun _ ->
            symbolTest 'X' "PX" "Helicopter" SymbolCode.Helicopter
        testCase "Sailboatt" <| fun _ ->
            symbolTest 'Y' "PY" "Sailboat" SymbolCode.Sailboat
        testCase "Windows flag" <| fun _ ->
            symbolTest 'Z' "PZ" "Windows flag" SymbolCode.WindowsFlag
        testCase "Human" <| fun _ ->
            symbolTest '[' "HS" "Human" SymbolCode.Human
        testCase "DF triangle" <| fun _ ->
            symbolTest '\\' "HT" "DF triangle" SymbolCode.DFTriangle
        testCase "Mailbox, post office" <| fun _ ->
            symbolTest ']' "HU" "Mailbox, post office" SymbolCode.MailboxPostoffice
        testCase "Large aircraft" <| fun _ ->
            symbolTest '^' "HV" "Large aircraft" SymbolCode.LargeAircraft
        testCase "Weather station" <| fun _ ->
            symbolTest '_' "HW" "Weather station" SymbolCode.WeatherStation
        testCase "Satellite dish antenna" <| fun _ ->
            symbolTest '`' "HX" "Satellite dish antenna" SymbolCode.SatelliteDishAntenna
        testCase "Ambulance" <| fun _ ->
            symbolTest 'a' "LA" "Ambulance" SymbolCode.Ambulance
        testCase "Bicycle" <| fun _ ->
            symbolTest 'b' "LB" "Bicycle" SymbolCode.Bicycle
        testCase "Incident command post" <| fun _ ->
            symbolTest 'c' "LC" "Incident command post" SymbolCode.IncidentCommandPost
        testCase "Fire station" <| fun _ ->
            symbolTest 'd' "LD" "Fire station" SymbolCode.FireStation
        testCase "Horse, equestrian" <| fun _ ->
            symbolTest 'e' "LE" "Horse, equestrian" SymbolCode.HorseEquestrian
        testCase "Fire truck" <| fun _ ->
            symbolTest 'f' "LF" "Fire truck" SymbolCode.FireTruck
        testCase "Glider" <| fun _ ->
            symbolTest 'g' "LG" "Glider" SymbolCode.Glider
        testCase "Hospital" <| fun _ ->
            symbolTest 'h' "LH" "Hospital" SymbolCode.Hospital
        testCase "IOTA, islands on the air" <| fun _ ->
            symbolTest 'i' "LI" "IOTA, islands on the air" SymbolCode.IOTAIslandsOnTheAir
        testCase "Jeep" <| fun _ ->
            symbolTest 'j' "LJ" "Jeep" SymbolCode.Jeep
        testCase "Truck" <| fun _ ->
            symbolTest 'k' "LK" "Truck" SymbolCode.Truck
        testCase "Laptop" <| fun _ ->
            symbolTest 'l' "LL" "Laptop" SymbolCode.Laptop
        testCase "Mic-E repeater" <| fun _ ->
            symbolTest 'm' "LM" "Mic-E repeater" SymbolCode.MicERepeater
        testCase "Node, black bulls-eye" <| fun _ ->
            symbolTest 'n' "LN" "Node, black bulls-eye" SymbolCode.NodeBlackBullsEye
        testCase "Emergency operations center" <| fun _ ->
            symbolTest 'o' "LO" "Emergency operations center" SymbolCode.EmergencyOperationsCenter
        testCase "Dog" <| fun _ ->
            symbolTest 'p' "LP" "Dog" SymbolCode.Dog
        testCase "Grid square, 2 by 2" <| fun _ ->
            symbolTest 'q' "LQ" "Grid square, 2 by 2" SymbolCode.GridSquare2x2
        testCase "Ship, power boat" <| fun _ ->
            symbolTest 's' "LS" "Ship, power boat" SymbolCode.ShipPowerBoat
        testCase "Truck stop" <| fun _ ->
            symbolTest 't' "LT" "Truck stop" SymbolCode.TruckStop
        testCase "Semi-trailer truck, 18-wheeler" <| fun _ ->
            symbolTest 'u' "LU" "Semi-trailer truck, 18-wheeler" SymbolCode.SemiTrainerTruck18Wheeler
        testCase "Van" <| fun _ ->
            symbolTest 'v' "LV" "Van" SymbolCode.Van
        testCase "Water station" <| fun _ ->
            symbolTest 'w' "LW" "Water station" SymbolCode.WaterStation
        testCase "X / Unix" <| fun _ ->
            symbolTest 'x' "LX" "X / Unix" SymbolCode.XUnix
        testCase "House, yagi antenna" <| fun _ ->
            symbolTest 'y' "LY" "House, yagi antenna" SymbolCode.HouseYagiAntenna
        testCase "Shelter" <| fun _ ->
            symbolTest 'z' "LZ" "Shelter" SymbolCode.Shelter
    ]

[<Tests>]
let CallSignTests =
    testList "Call Sign Tests" [
        testCase "Fail to create a too short call sign" <| fun _ ->
            Expect.isNone (CallSign.create "") "Call Sign is too short but was not caught"
        testCase "Fail to create too long call sign" <| fun _ ->
            Expect.isNone (CallSign.create "1234567890") "Call Sign is too long but was not caught"
        testCase "Can create a call sign with 1 to 9 characters" <| fun _ ->
            Expect.equal (CallSign.value((CallSign.create "KG7SIO").Value)) "KG7SIO" "Call Sign was not created"
    ]

[<Tests>]
let PathTests =
    testList "Path Tests" [
        testCase "WIDE11 return WIDE1-1" <| fun _ ->
            Expect.equal (WIDE11.ToString()) "WIDE1-1" "Did not return the expected string representation"
        testCase "WIDE21 return WIDE2-1" <| fun _ ->
            Expect.equal (WIDE21.ToString()) "WIDE2-1" "Did not return the expected string representation"
        testCase "WIDE22 return WIDE2-2" <| fun _ ->
            Expect.equal (WIDE22.ToString()) "WIDE2-2" "Did not return the expected string representation"
    ]