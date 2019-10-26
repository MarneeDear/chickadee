namespace chickadee.core.Helpers

open System

     (* Base-91 Notation Two APRS data formats use base-91 notation: lat/long coordinates in
        compressed format (see Chapter 9) and the altitude in Mic-E format (see
        Chapter 10).
        Base-91 data is compressed into a short string of characters. All the
        characters are printable ASCII, with character codes in the range 33–124
        decimal (i.e. ! through |).
        To compute the base-91 ASCII character string for a given data value, the
        value is divided by progressively reducing powers of 91 until the remainder
        is less than 91. At each step, 33 is added to the modulus of the division
        process to obtain the corresponding ASCII character code.
        For example, for a data value of 12345678:
        12345678 / 913 = modulus 16, remainder 288542
        288542 / 912 = modulus 34, remainder 6988
        6988 / 911 = modulus 76, remainder 72
        The four ASCII character codes are thus 49 (i.e. 16+33), 67 (i.e. 34+33), 109
        (i.e. 76+33) and 105 (i.e. 72+33), corresponding to the ASCII string 1Cmi.
    *)



[<AutoOpen>]
module Compression = 
    open System.Text

(* 
    Lat/Long Encoding The values of YYYY and XXXX are computed as follows:
    YYYY is 380926 x (90 – latitude) [base 91]
            latitude is positive for north, negative for south, in degrees.
    XXXX is 190463 x (180 + longitude) [base 91]
    longitude is positive for east, negative for west, in degrees.

    For example, for a longitude of 72° 45' 00 west (i.e. -72.75 degrees), 
    the math is 190463 x (180 – 72.75) = 20427156.
    Because this is to base 91, it is     
    then necessary to progressively divide this value by reducing powers of 91,    
        to obtain the numerical values of X:
    20427156 / 913 = 27, remainder 80739
    80739 / 912 = 9, remainder 6210
    6210 / 911 = 68, remainder 22

    To obtain the corresponding ASCII characters, 33 is added to each of these
    values, yielding 60 (i.e. 27+33), 42, 101 and 55. From the ASCII Code Table
            (in Appendix 3), this corresponds to <*e7 for XXXX.

*)

    let compressedEncodedLongitude (dd:float) =
        let l0 = 190463.0 * (180.0 + dd)
        let l1 = l0 / (Math.Pow(91.0, 3.0)) //91 ^ 3
        let r1 = l0 % (Math.Pow(91.0, 3.0)) //91 ^ 3
        let l2 = r1 / (Math.Pow(91.0, 2.0)) //91 ^ 2
        let r2 = r1 % (Math.Pow(91.0, 2.0)) //91 ^ 2
        let l3 = r2 / (Math.Pow(91.0, 1.0)) //91 ^ 1
        let r3 = r2 % (Math.Pow(91.0, 1.0)) //91 ^ 1
        let t1 = (l1 + 33.0), (l2 + 33.0), (l3 + 33.0), (r3 + 33.0)
        let c (d:float) = 
            string (Convert.ToChar(d + 33.0))
            //(Encoding.UTF8.GetBytes(string (d + 33.0)))
        sprintf "%s%s%s%s" (c l1) (c l2) (c l3) (c r3)

    let compressedEncodedLatitude (dd:float) =
        let l0 = 380926.0 * (90.0 - dd)
        let l1 = l0 / (Math.Pow(91.0, 3.0)) //91 ^ 3
        let r1 = l0 % (Math.Pow(91.0, 3.0)) //91 ^ 3
        let l2 = r1 / (Math.Pow(91.0, 2.0)) //91 ^ 2
        let r2 = r1 % (Math.Pow(91.0, 2.0)) //91 ^ 2
        let l3 = r2 / (Math.Pow(91.0, 1.0)) //91 ^ 1
        let r3 = r2 % (Math.Pow(91.0, 1.0)) //91 ^ 1
        let t1 = (l1 + 33.0), (l2 + 33.0), (l3 + 33.0), (r3 + 33.0)
        let c (d:float) = 
            string (Convert.ToChar(d + 33.0))
            //(Encoding.UTF8.GetBytes(string (d + 33.0)))
        sprintf "%s%s%s%s" (c l1) (c l2) (c l3) (c r3)

        //to convert to ascii code (int of char)