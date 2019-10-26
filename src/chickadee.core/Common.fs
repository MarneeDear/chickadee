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
    type SymbolCode =
        | House 
        | Bicycle 
        | Balloon 
        | Hospital
        | Jeep 
        | Truck
        | Motorcycle
        | Jogger
        member this.ToChar() =
            match this with
            | House         -> '-'
            | Bicycle       -> 'b'
            | Balloon       -> 'O'
            | Hospital      -> 'h'
            | Jeep          -> 'j'
            | Truck         -> 'k'
            | Motorcycle    -> '<'
            | Jogger        -> '['
        static member fromSymbol s =
            match s with
            | '-' -> Some House
            | 'b' -> Some Bicycle
            | 'O' -> Some Balloon
            | 'h' -> Some Hospital
            | 'j' -> Some Jeep
            | 'k' -> Some Truck
            | '<' -> Some Motorcycle
            | '[' -> Some Jogger
            | _   -> None

    //let getSymbolCode symbol =
    //    match symbol with
    //    | '-' -> Some House
    //    | 'b' -> Some Bicycle
    //    | 'O' -> Some Balloon
    //    | 'h' -> Some Hospital
    //    | 'j' -> Some Jeep
    //    | 'k' -> Some Truck
    //    | '<' -> Some Motorcycle
    //    | '[' -> Some Jogger
    //    | _   -> None

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
