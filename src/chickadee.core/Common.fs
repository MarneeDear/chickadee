namespace chickadee.core

[<AutoOpen>]
module Common =

    (*
    http://www.aprs.org/aprs11/SSIDs.txt
    SSID RECOMMENDATIONS:  It is very convenient to other mobile 
    operators or others looking at callsigns flashing by, to be able to 
    recognize some common applications at a glance.
    *)
    type SSID =
        | PrimaryStation 
        | Generic_1
        | Generic_2
        | Generic_3
        | Generic_4
        | Other
        | SpecialActivity
        | HumanPortable
        | SecondMainMobile
        | PrimaryMobile
        | InternetLink
        | Aircraft
        | TTDevices
        | WeatherStation
        | FullTimeDriver
        | Generic_15
        member this.ToId =
            match this with
            | PrimaryStation    -> 0
            | Generic_1         -> 1
            | Generic_2         -> 2
            | Generic_3         -> 3
            | Generic_4         -> 4
            | Other             -> 5
            | SpecialActivity   -> 6
            | HumanPortable     -> 7
            | SecondMainMobile  -> 8
            | PrimaryMobile     -> 9
            | InternetLink      -> 10
            | Aircraft          -> 11
            | TTDevices         -> 12
            | WeatherStation    -> 13
            | FullTimeDriver    -> 14
            | Generic_15        -> 15

    let getSSID ssid =
        match ssid with
        | 0     -> Some PrimaryStation
        | 1     -> Some Generic_1
        | 2     -> Some Generic_2
        | 3     -> Some Generic_3
        | 4     -> Some Generic_4
        | 5     -> Some Other
        | 6     -> Some SpecialActivity
        | 7     -> Some HumanPortable
        | 8     -> Some SecondMainMobile
        | 9     -> Some PrimaryMobile
        | 10    -> Some InternetLink
        | 11    -> Some Aircraft
        | 12    -> Some TTDevices
        | 13    -> Some WeatherStation
        | 14    -> Some FullTimeDriver
        | 15    -> Some Generic_15
        | _     -> None






(*
helful guide to some of the design decision made here
//https://stackoverflow.com/questions/791706/how-do-i-customize-output-of-a-custom-type-using-printf

*)

    type WIDEnN =
        | WIDE11
        | WIDE21
        | WIDE22
        override this.ToString() =
            match this with 
            | WIDE11    -> "WIDE1-1"
            | WIDE21    -> "WIDE2-1"
            | WIDE22    -> "WIDE2-2"
        static member fromString p =
            match p with
            | "WIDE1-1" -> WIDE11
            | "WIDE2-1" -> WIDE21
            | "WIDE2-2" -> WIDE22
            | _         -> WIDE11 //Use this as the default

    //9 byte field
    //aka the UNPROTO path
    //http://wa8lmf.net/DigiPaths/index.htm#Recommended
    type Path =
        | WIDEnN of WIDEnN
        override this.ToString() =
            match this with
            | WIDEnN p ->   match p with
                            | WIDE11    -> WIDE11.ToString()
                            | WIDE21    -> WIDE21.ToString()
                            | WIDE22    -> WIDE22.ToString()

    //This is only a subset of the codes because I don't want to support all of them
    (*
    APPENDIX 2: THE APRS SYMBOL TABLES p. 105
    Also listed here: https://github.com/hessu/aprs-symbol-index/blob/master/symbols.csv
    *)
    type SymbolCode =
        | PoliceStation
        | Digipeater
        | Telephone
        | DXCluster
        | HFGateway
        | SmallAircraft
        | MobileSatelliteStation
        | Handicapped
        | Snowmobile
        | RedCross
        | BoyScouts
        | House 
        | RedX
        | RedDot
        | Fire
        | CampgroundTent
        | Motorcycle
        | RailroadEngine
        | Car
        | FileServer
        | HurricanePredictedPath
        | AidStation
        | BBS
        | Canoe
        | Eyeball
        | FarmVehicleTractor
        | GridSquare3x3
        | Hotel
        | TCPIPNetworkStation
        | School
        | PCUser
        | MacApple
        | NTSStation
        | Balloon
        | PoliceCar
        | RecreationalVehicle
        | SpaceShuttle
        | SSTV
        | Bus
        | ATVAmateurTelevision
        | WeatherServiceSite
        | Helicopter
        | Sailboat
        | WindowsFlag
        | Human
        | DFTriangle
        | MailboxPostoffice
        | LargeAircraft
        | WeatherStation
        | SatelliteDishAntenna
        | Ambulance
        | Bicycle 
        | IncidentCommandPost
        | FireStation
        | HorseEquestrian
        | FireTruck
        | Glider
        | Hospital
        | IOTAIslandsOnTheAir
        | Jeep 
        | Truck
        | Laptop
        | MicERepeater
        | NodeBlackBullsEye
        | EmergencyOperationsCenter
        | Dog
        | GridSquare2x2
        | RepeaterTower
        | ShipPowerBoat
        | TruckStop
        | SemiTrainerTruck18Wheeler
        | Van
        | WaterStation
        | XUnix
        | HouseYagiAntenna
        | Shelter
        //| Emergency
        member this.ToCode() =
            match this with
            | PoliceStation -> ('!',"BB")
            | Digipeater -> ('#', "BD")
            | Telephone -> ('$', "BE")
            | DXCluster -> ('%', "BF")
            | HFGateway -> ('&',"BG")
            | SmallAircraft -> (''',"BH")
            | MobileSatelliteStation -> ('(',"BI")
            | Handicapped -> (')',"BJ")
            | Snowmobile -> ('*',"BK")
            | RedCross -> ('+',"BL")
            | BoyScouts -> (',',"BM")
            | House  -> ('-',"BN")
            | RedX -> ('.',"BO")
            | RedDot -> ('/',"BP")
            | Fire -> (':',"MR")
            | CampgroundTent -> (';',"MS")
            | Motorcycle    -> ('<',"MT")
            | RailroadEngine -> ('=',"MU")
            | Car -> ('>',"MV")
            | FileServer -> ('?',"MW")
            | HurricanePredictedPath -> ('@',"MX")
            | AidStation -> ('A',"PA")
            | BBS -> ('B',"PB")
            | Canoe -> ('C',"PC")
            | Eyeball -> ('E',"PE")
            | FarmVehicleTractor -> ('F',"PF")
            | GridSquare3x3 -> ('G',"PG")
            | Hotel -> ('H',"PH")
            | TCPIPNetworkStation -> ('I',"PI")
            | School -> ('K',"PK")
            | PCUser -> ('L',"PL")
            | MacApple -> ('M',"PM")
            | NTSStation -> ('N',"PN")
            | Balloon       -> ('O',"PO")
            | PoliceCar -> ('P',"PP")
            | RecreationalVehicle -> ('R',"PR")
            | SpaceShuttle -> ('S',"PS")
            | SSTV -> ('T',"PT")
            | Bus -> ('U',"PU")
            | ATVAmateurTelevision -> ('V',"PV")
            | WeatherServiceSite -> ('W',"PW")
            | Helicopter -> ('X',"PX")
            | Sailboat -> ('Y',"PY")
            | WindowsFlag -> ('Z',"PZ")
            | Human -> ('[',"HS")
            | DFTriangle -> ('\\',"HT")
            | MailboxPostoffice -> (']',"HU")
            | LargeAircraft -> ('^',"HV")
            | WeatherStation -> ('_',"HW")
            | SatelliteDishAntenna -> ('`',"HX")
            | Ambulance -> ('a',"LA")
            | Bicycle       -> ('b',"LB")
            | IncidentCommandPost -> ('c',"LC")
            | FireStation -> ('d',"LD")
            | HorseEquestrian -> ('e',"LE")
            | FireTruck -> ('f',"LF")
            | Glider -> ('g',"LG")
            | Hospital      -> ('h',"LH")
            | IOTAIslandsOnTheAir -> ('i',"LI")
            | Jeep   -> ('j',"LJ")
            | Truck  -> ('k',"LK")
            | Laptop -> ('l',"LL")
            | MicERepeater -> ('m',"LM")
            | NodeBlackBullsEye -> ('n',"LN")
            | EmergencyOperationsCenter -> ('o',"LO")
            | Dog -> ('p',"LP")
            | GridSquare2x2 -> ('q',"LQ")
            | RepeaterTower -> ('r',"LR")
            | ShipPowerBoat -> ('s',"LS")
            | TruckStop -> ('t',"LT")
            | SemiTrainerTruck18Wheeler -> ('u',"LU")
            | Van -> ('v',"LV")
            | WaterStation -> ('w',"LW")
            | XUnix -> ('x',"LX")
            | HouseYagiAntenna -> ('y',"LY")
            | Shelter -> ('z',"LZ")
            //| Emergency -> 
        static member fromSymbol s =
            match s with
            | '!' -> Some PoliceStation
            | '#' -> Some Digipeater
            | '$' -> Some Telephone
            | '%' -> Some DXCluster
            | '&' -> Some HFGateway
            | ''' -> Some SmallAircraft
            | '(' -> Some MobileSatelliteStation
            | ')' -> Some Handicapped
            | '*' -> Some Snowmobile
            | '+' -> Some RedCross
            | ',' -> Some BoyScouts
            | '-' -> Some House
            | '.' -> Some RedX
            | '/' -> Some RedDot
            | ':' -> Some Fire
            | ';' -> Some CampgroundTent
            | '<' -> Some Motorcycle
            | '=' -> Some RailroadEngine
            | '>' -> Some Car
            | '?' -> Some FileServer
            | '@' -> Some HurricanePredictedPath
            | 'A' -> Some AidStation
            | 'B' -> Some BBS
            | 'C' -> Some Canoe
            | 'E' -> Some Eyeball
            | 'F' -> Some FarmVehicleTractor
            | 'G' -> Some GridSquare3x3
            | 'H' -> Some Hotel
            | 'I' -> Some TCPIPNetworkStation
            | 'K' -> Some School
            | 'L' -> Some PCUser
            | 'M' -> Some MacApple
            | 'N' -> Some NTSStation
            | 'O' -> Some Balloon
            | 'P' -> Some PoliceCar
            | 'R' -> Some RecreationalVehicle
            | 'S' -> Some SpaceShuttle
            | 'T' -> Some SSTV
            | 'U' -> Some Bus
            | 'V' -> Some ATVAmateurTelevision
            | 'W' -> Some WeatherServiceSite
            | 'X' -> Some Helicopter
            | 'Y' -> Some Sailboat
            | 'Z' -> Some WindowsFlag
            | '[' -> Some Human
            | '\\' -> Some DFTriangle
            | ']' -> Some MailboxPostoffice
            | '^' -> Some LargeAircraft
            | '_' -> Some WeatherStation
            | '`' -> Some SatelliteDishAntenna
            | 'a' -> Some Ambulance
            | 'b' -> Some Bicycle
            | 'c' -> Some IncidentCommandPost
            | 'd' -> Some FireStation
            | 'e' -> Some HorseEquestrian
            | 'f' -> Some FireTruck
            | 'g' -> Some Glider
            | 'h' -> Some Hospital
            | 'i' -> Some IOTAIslandsOnTheAir
            | 'j' -> Some Jeep
            | 'k' -> Some Truck
            | 'l' -> Some Laptop
            | 'm' -> Some MicERepeater
            | 'n' -> Some NodeBlackBullsEye
            | 'o' -> Some EmergencyOperationsCenter
            | 'p' -> Some Dog
            | 'q' -> Some GridSquare2x2
            | 'r' -> Some RepeaterTower
            | 's' -> Some ShipPowerBoat
            | 't' -> Some TruckStop
            | 'u' -> Some SemiTrainerTruck18Wheeler
            | 'v' -> Some Van
            | 'w' -> Some WaterStation
            | 'x' -> Some XUnix
            | 'y' -> Some HouseYagiAntenna
            | 'z' -> Some Shelter
            | _   -> None

    //9 byte field
    type CallSign = private CallSign of string          
    module CallSign =
        open System

        let create (s:string) = 
            match (s.Trim()) with
            | c when not (String.IsNullOrEmpty(c)) && c.Length < 10     -> Some (CallSign c)
            | _                                                         -> None // "Call Sign cannot be empty and must be 1 - 9 characters. See APRS 1.01."
        let value (CallSign s) = s.ToUpper() // MUST BE ALL CAPS        
    
    //5 APRS DATA IN THE AX.25 INFORMATION FIELD
    //APRS Data Type
    //Identifier
    //Every APRS packet contains an APRS Data Type Identifier (DTI). This
    //determines the format of the remainder of the data in the Information field, as
    //follows:
    //APRS Data Type Identifiers
    type APRSDataTypeIdentifier =
        | PositionReportWithoutTimeStampNoMessaging //!
        | PositionReportWithoutTimeStampWithMessaging //=
        | PositionReportWithTimeStampWithMessaging //@ //TODO not supported
        | PositionReportWithTimeStampNoMessaging // / //TODO not supported
        | UserDefined // {
        | Message // :
        override this.ToString() =
            match this with
            | PositionReportWithoutTimeStampNoMessaging     -> "!"
            | PositionReportWithoutTimeStampWithMessaging   -> "="
            | PositionReportWithTimeStampWithMessaging      -> "@"
            | PositionReportWithTimeStampNoMessaging        -> "/"
            | UserDefined                                   -> "{"
            | Message                                       -> ":"
            
    
    let getAPRSDataTypeIdentifier id =
        match id with
        | "!"   -> Some PositionReportWithoutTimeStampNoMessaging
        | "="   -> Some PositionReportWithoutTimeStampWithMessaging
        | "@"   -> Some PositionReportWithTimeStampWithMessaging
        | "/"   -> Some PositionReportWithTimeStampNoMessaging
        | "{"   -> Some UserDefined
        | ":"   -> Some Message
        | _     -> None
