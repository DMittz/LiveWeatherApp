window.geoInterop = {
    getCurrentPosition: function () {
        return new Promise(function (resolve, reject) {
            if (!navigator.geolocation) {
                reject("Geolocation not supported");
                return;
            }
            navigator.geolocation.getCurrentPosition(function (pos) {
                resolve({
                    Latitude: pos.coords.latitude,
                    Longitude: pos.coords.longitude
                });
            }, function (err) {
                reject(err && err.message ? err.message : err);
            }, { enableHighAccuracy: true, timeout: 10000 });
        });
    }
};