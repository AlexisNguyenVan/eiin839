function contractsRetrieved() {
    var json = JSON.parse(this.responseText);
    for (var i = 0; i < json.length; i++) {
        var obj = json[i];
        console.log(obj);
        var option =document.createElement("option");
        option.setAttribute("value", obj.name);
        document.getElementById("contracts").appendChild(option);
    }
}

function retrieveAllContracts()
{
    var key = document.getElementById("apiKey").value;
    var url = "https://api.jcdecaux.com/vls/v3/contracts?apiKey=" + key;
    var req = new XMLHttpRequest();
    req.open("get", url);
    req.setRequestHeader("Accept", "application/json");
    req.onload = contractsRetrieved;
    req.send();
    
}

function getDistanceFrom2GpsCoordinates(lat1, lon1, lat2, lon2) {
    // Radius of the earth in km
    var earthRadius = 6371;
    var dLat = deg2rad(lat2 - lat1);
    var dLon = deg2rad(lon2 - lon1);
    var a =
            Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(deg2rad(lat1)) * Math.cos(deg2rad(lat2)) *
                Math.sin(dLon / 2) * Math.sin(dLon / 2)
        ;
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = earthRadius * c; // Distance in km
    return d;
}

function deg2rad(deg) {
    return deg * (Math.PI / 180)
}



function getClosestStation() {
    var key = document.getElementById("apiKey").value;
    var contract = document.getElementById("contract").value;
    var url = "https://api.jcdecaux.com/vls/v3/stations?contract="+contract+"&apiKey="+key;
    var req = new XMLHttpRequest();
    req.open("get", url);
    req.setRequestHeader("Accept", "application/json");
    req.send();
    req.onload = function () {
        var json = JSON.parse(req.response);
        var lat = document.getElementById("lat");
        var lon = document.getElementById("long");
        console.log(json[0]);
        var min = getDistanceFrom2GpsCoordinates(json[0].position.latitude, json[0].position.longitude, lat, lon);
        var closest = json[0];
        for (var i = 1; i < json.length; i++) {
            var dist = getDistanceFrom2GpsCoordinates(json[i].position.latitude, json[i].position.longitude, lat, lon);
            if (dist < min) {
                min = dist;
                closest = json[i];
            }
        }
        document.getElementById("closest").value = closest.name;
    }
}