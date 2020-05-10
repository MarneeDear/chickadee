//var resolveFunc = null;
//var rejectFunc = null;
//function tokenResolver() {
//    if (this.readyState === 4 && this.status === 200) {
//        resolveFunc(this.responseText);
//    } else if (this.status !== 200) {
//        rejectFunc(this.responseText);
//    }
//}

//var map = new atlas.Map("map", {
//    center: [-110.911789, 32.253460],
//    zoom: 10,
//    language: "en-US",
//    authOptions: {
//        authType: "anonymous",
//        clientId: document.getElementById("clientId").value,
//        getToken: function (resolve, reject, map) {
//            var xhttp = new XMLHttpRequest();
//            var url = "/api/token";
//            resolveFunc = resolve;
//            rejectFunc = reject;
//            xhttp.open("GET", url, true);
//            xhttp.setRequestHeader("X-API-Key", document.getElementById("otp").value);
//            xhttp.onreadystatechange = tokenResolver;
//            xhttp.send();
//        }
//    }
//});
//map.events.add("tokenacquired", function () {
//    console.log("token acquired");
//});
//map.events.add("error", function (err) {
//    console.log(err.error);
//});


var map, datasource;
var resolveFunc = null;
var rejectFunc = null;
function tokenResolver() {
    if (this.readyState === 4 && this.status === 200) {
        resolveFunc(this.responseText);
    } else if (this.status !== 200) {
        rejectFunc(this.responseText);
    }
}

function GetMapData() {
    var xhttp = new XMLHttpRequest();
    var data = {};
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            // Typical action to be performed when the document is ready:
            //console.log(xhttp.responseText);
            data = JSON.parse(xhttp.response);
        }
    };
    xhttp.open("GET", "/api/map_data", false);
    xhttp.send();
    console.log(data);
    return data;
}

function GetMap() {
    //Initialize a map instance.

    var map_data = GetMapData();
    console.log(map_data);

    map = new atlas.Map('map', {
        center: [-110.911789, 32.253460],
        zoom: 10,
        view: 'Auto',

    authOptions: {
        authType: "anonymous",
        clientId: document.getElementById("clientId").value,
        getToken: function (resolve, reject, map) {
            var xhttp = new XMLHttpRequest();
            var url = "/api/token";
            resolveFunc = resolve;
            rejectFunc = reject;
            xhttp.open("GET", url, true);
            xhttp.setRequestHeader("X-API-Key", document.getElementById("otp").value);
            xhttp.onreadystatechange = tokenResolver;
            xhttp.send();
        }
    }
    //authOptions: {
    //    authType: 'subscriptionKey',
    //    subscriptionKey: 'BLAHBLAHFISHCAKES'
    //}
    });

    //var img1 = new Image();
    //img1.src = "/image/map/aprs-symbols-24-0.png";
    //img1.style = "clip-path: inset(0px 362px 121px 0px)";
    //img1.innerHTML = '<i style="background-position: -0px -0px;"></i>'

    map_data.forEach(addMapEvent);

    //Wait until the map resources are ready.
    function addMapEvent(item, index) {
        console.log("ITEM");
        console.log(item);
        var icon = 'aprs-icon-' + String(index);
        console.log(icon);
        map.events.add('ready', function () {

            //Load the custom image icon into the map resources.
            //map.imageSprite.add('my-custom-icon', '/image/map/aprs-symbols/aprs_small/image_part_001.png').then(function () {
            map.imageSprite.add(icon, item["Image"]).then(function () {
                //'/image/map/showers.png').then(function () {

                //Create a data source and add it to the map.
                datasource = new atlas.source.DataSource();
                map.sources.add(datasource);

                //Create a point feature and add it to the data source.
                //datasource.add(new atlas.data.Feature(new atlas.data.Point([-110.911789, 32.253460]), {
                datasource.add(new atlas.data.Feature(new atlas.data.Point(item["Location"]), {
                    //temperature: 64
                    //callsign: 'KG7SIO'
                    callsign: item["CallSign"]
                }));

                //Add a layer for rendering point data as symbols.
                map.layers.add(new atlas.layer.SymbolLayer(datasource, null, {
                    iconOptions: {
                        //Pass in the id of the custom icon that was loaded into the map resources.
                        image: icon,

                        //Optionally scale the size of the icon.
                        size: 0.5
                    },
                    textOptions: {
                        //Convert the temperature property of each feature into a string and concatenate "°F".
                        //textField: ['concat', ['to-string', ['get', 'temperature']], '°F'],
                        textField: ['get', 'callsign'],
                        //Offset the text so that it appears on top of the icon.
                        offset: [0, -2]
                    }
                }));
            });
        });
    }
}