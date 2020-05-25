namespace Settings

module Model =
    [<CLIMutable>]
    type UserSettings =
        {
            CallSign : string
            LocationLatitude : float
            LocationLongitude : float
        }
