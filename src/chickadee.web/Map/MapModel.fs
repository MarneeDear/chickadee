namespace AprsMap

module Models =

    open chickadee.core
    
    type IconMapping =
        {
            Symbol : SymbolCode
            Icon : string
        }

    let icons =
        [
            {Symbol = SymbolCode.PoliceStation; Icon = "policestation.png"}
            {Symbol = SymbolCode.Digipeater; Icon = "digipeater.png"}
            {Symbol = SymbolCode.Telephone; Icon = "telephone.png"}
            {Symbol = SymbolCode.DXCluster; Icon = "dx-cluster.png"}
            {Symbol = SymbolCode.HFGateway; Icon = "hf-gateway.png"}
            {Symbol = SymbolCode.SmallAircraft; Icon = "small-aircraft.png"}
            {Symbol = SymbolCode.MobileSatelliteStation; Icon = "mobile-satellite-station.png"}
            {Symbol = SymbolCode.Handicapped; Icon = "handicapped.png"}
            {Symbol = SymbolCode.Snowmobile; Icon = "snowmobile.png"}
            {Symbol = SymbolCode.RedCross; Icon = "red-cross.png"}
            {Symbol = SymbolCode.BoyScouts; Icon = "boy-scouts.png"}
            {Symbol = SymbolCode.House; Icon = "house.png"}
            {Symbol = SymbolCode.RedX; Icon = "red-x.png"}
            {Symbol = SymbolCode.RedDot; Icon = "red-dot.png"}
            {Symbol = SymbolCode.Fire; Icon = "fire.png"}
            {Symbol = SymbolCode.CampgroundTent; Icon = "campground-test.png"}
            {Symbol = SymbolCode.Motorcycle; Icon = "motorcycle.png"}
            {Symbol = SymbolCode.RailroadEngine; Icon = "railroad-engine.png"}
            {Symbol = SymbolCode.Car; Icon = "car.png"}
            {Symbol = SymbolCode.FileServer; Icon = "file-server.png"}
            {Symbol = SymbolCode.HurricanePredictedPath; Icon = "hurricane-predicted-path.png"}
            {Symbol = SymbolCode.AidStation; Icon = "aid-station.png"}
            {Symbol = SymbolCode.BBS; Icon = "bbs.png"}
            {Symbol = SymbolCode.Canoe; Icon = "canoe.png"}
            {Symbol = SymbolCode.Eyeball; Icon = "eyeball.png"}
            {Symbol = SymbolCode.FarmVehicleTractor; Icon = "farm-vehicle-tractor.png"}
            {Symbol = SymbolCode.GridSquare3x3; Icon = "grid-square-3-x-3.png"}
            {Symbol = SymbolCode.Hotel; Icon = "hotel.png"}
            {Symbol = SymbolCode.TCPIPNetworkStation; Icon = "tcp-ip-network-station.png"}
            {Symbol = SymbolCode.School; Icon = "school.png"}
            {Symbol = SymbolCode.PCUser; Icon = "pc-user.png"}
            {Symbol = SymbolCode.MacApple; Icon = "mac-apple.png"}
            {Symbol = SymbolCode.NTSStation; Icon = "nts-station.png"}
            {Symbol = SymbolCode.Balloon; Icon = "balloon.png"}
            {Symbol = SymbolCode.PoliceCar; Icon = "police-car.png"}
            {Symbol = SymbolCode.RecreationalVehicle; Icon = "recreational-vehicle.png"}
            {Symbol = SymbolCode.SpaceShuttle; Icon = "space-shuttele.png"}
            {Symbol = SymbolCode.SSTV; Icon = "sstv.png"}
            {Symbol = SymbolCode.Bus; Icon = "bus.png"}
            {Symbol = SymbolCode.ATVAmateurTelevision; Icon = "amatuer-television.png"}
            {Symbol = SymbolCode.WeatherServiceSite; Icon = "weather-service-site.png"}
            {Symbol = SymbolCode.Helicopter; Icon = "helicopter.png"}
            {Symbol = SymbolCode.Sailboat; Icon = "sailboat.png"}
            {Symbol = SymbolCode.WindowsFlag; Icon = "windows-flag.png"}
            {Symbol = SymbolCode.Human; Icon = "human.png"}
            {Symbol = SymbolCode.DFTriangle; Icon = "df-triangle.png"}
            {Symbol = SymbolCode.MailboxPostoffice; Icon = "mailbox-postoffice.png"} 
            {Symbol = SymbolCode.LargeAircraft; Icon = "large-aircraft.png"}
            {Symbol = SymbolCode.WeatherStation; Icon = "weather-station.png"}
            {Symbol = SymbolCode.SatelliteDishAntenna; Icon = "satellite-dish-antenna.png"}
            {Symbol = SymbolCode.Ambulance; Icon = "ambulance.png"}
            {Symbol = SymbolCode.Bicycle; Icon = "bicycle.png"}
            {Symbol = SymbolCode.IncidentCommandPost; Icon = "incident-command-post.png"}
            {Symbol = SymbolCode.FireStation; Icon = "fire-station.png"}
            {Symbol = SymbolCode.HorseEquestrian; Icon = "horse-equestrian.png"}
            {Symbol = SymbolCode.FireTruck; Icon = "fire-truck.png"}
            {Symbol = SymbolCode.Glider; Icon = "glider.png"}
            {Symbol = SymbolCode.Hospital; Icon = "hospital.png"}
            {Symbol = SymbolCode.IOTAIslandsOnTheAir; Icon = "iota-islands-on-the-air.png"}
            {Symbol = SymbolCode.Jeep; Icon = "jeep.png"}
            {Symbol = SymbolCode.Truck; Icon = "truck.png"}
            {Symbol = SymbolCode.Laptop; Icon = "laptop.png"}
            {Symbol = SymbolCode.MicERepeater; Icon = "mic-e-repeater.png"}
            {Symbol = SymbolCode.NodeBlackBullsEye; Icon = "node-black-bullseye.png"}
            {Symbol = SymbolCode.EmergencyOperationsCenter; Icon = "emergency-operations-center.png"}
            {Symbol = SymbolCode.Dog; Icon = "dog.png"}
            {Symbol = SymbolCode.GridSquare2x2; Icon = "grid-square-2-x-2.png"}
            {Symbol = SymbolCode.RepeaterTower; Icon = "repeater-tower.png"}
            {Symbol = SymbolCode.ShipPowerBoat; Icon = "ship-power-boat.png"}
            {Symbol = SymbolCode.TruckStop; Icon = "truck-stop.png"}
            {Symbol = SymbolCode.SemiTrainerTruck18Wheeler; Icon = "semi-trailer-18-wheeler.png"}
            {Symbol = SymbolCode.Van; Icon = "van.png"}
            {Symbol = SymbolCode.WaterStation; Icon = "water-station.png"}
            {Symbol = SymbolCode.XUnix; Icon = "x-unix.png"}
            {Symbol = SymbolCode.HouseYagiAntenna; Icon = "house-yagi-antenna.png"}
            {Symbol = SymbolCode.Shelter; Icon = "shelter.png"}
        ]
