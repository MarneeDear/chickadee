namespace Map

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine

    let index (otp:string) (clientId:string) =
        let content = [
            div [] [
                p [] [encodedText "MAP GOES HERE"]
                input [_type "hidden"; _id "otp"; _value otp]
                input [_type "hidden"; _id "clientId"; _value clientId]
            ]
            div [_id "map"; _style "width: 100%; height: 400px"] []
            link [_rel "stylesheet"; _href "https://atlas.microsoft.com/sdk/javascript/mapcontrol/2/atlas.min.css"]
            script [_src "https://atlas.microsoft.com/sdk/javascript/mapcontrol/2/atlas.min.js"] []
            script [_src "/map.js"] []
            //script [] [
            //    rawText """
            //        var resolveFunc = null;
            //        var rejectFunc = null;
            //        function tokenResolver() {
            //            if (this.readyState === 4 && this.status === 200) {
            //                resolveFunc(this.responseText);
            //            } else if (this.status !== 200) {
            //                rejectFunc(this.responseText);
            //            }
            //        }

            //        var map = new atlas.Map("map", {
            //            center: [-122.33, 47.64],
            //            zoom: 12,
            //            language: "en-US",
            //            authOptions: {
            //                authType: "anonymous",
            //                clientId: "390a7f01-5faa-46c8-a193-c08258334fcb",
            //                getToken: function (resolve, reject, map) {
            //                    var xhttp = new XMLHttpRequest();
            //                    var url = "/api/token";
            //                    resolveFunc = resolve;
            //                    rejectFunc = reject;
            //                    xhttp.open("GET", url, true);
            //                    xhttp.setRequestHeader("X-API-Key", "HelloWorld");
            //                    xhttp.onreadystatechange = tokenResolver;
            //                    xhttp.send();
            //                }
            //            }
            //        });
            //        map.events.add("tokenacquired", function () {
            //            console.log("token acquired");
            //        });
            //        map.events.add("error", function (err) {
            //            console.log(err.error);
            //        });
            //    """
            //]
        ]
        App.layout content
