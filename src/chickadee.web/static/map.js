var resolveFunc = null;
var rejectFunc = null;
function tokenResolver() {
    if (this.readyState === 4 && this.status === 200) {
        resolveFunc(this.responseText);
    } else if (this.status !== 200) {
        rejectFunc(this.responseText);
    }
}

var map = new atlas.Map("map", {
    center: [-122.33, 47.64],
    zoom: 12,
    language: "en-US",
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
map.events.add("tokenacquired", function () {
    console.log("token acquired");
});
map.events.add("error", function (err) {
    console.log(err.error);
});
