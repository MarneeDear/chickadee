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

function GetMap() {
    //Initialize a map instance.
    map = new atlas.Map('map', {
        center: [-73.985708, 40.75773],
        zoom: 12,
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
    });

    //Wait until the map resources are ready.
    map.events.add('ready', function () {

        //Load the custom image icon into the map resources.
        map.imageSprite.add('my-custom-icon', '/image/map/showers.png').then(function () {

            //Create a data source and add it to the map.
            datasource = new atlas.source.DataSource();
            map.sources.add(datasource);

            //Create a point feature and add it to the data source.
            datasource.add(new atlas.data.Feature(new atlas.data.Point([-73.985708, 40.75773]), {
                temperature: 64
            }));

            //Add a layer for rendering point data as symbols.
            map.layers.add(new atlas.layer.SymbolLayer(datasource, null, {
                iconOptions: {
                    //Pass in the id of the custom icon that was loaded into the map resources.
                    image: 'my-custom-icon',

                    //Optionally scale the size of the icon.
                    size: 0.5
                },
                textOptions: {
                    //Convert the temperature property of each feature into a string and concatenate "°F".
                    textField: ['concat', ['to-string', ['get', 'temperature']], '°F'],

                    //Offset the text so that it appears on top of the icon.
                    offset: [0, -2]
                }
            }));
        });
    });
}