(function () {
    var tileUrl = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
    var tileAttribution = 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, <a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>';

    // Global export
    window.deliveryMap = {
        showOrUpdate: function (elementId, location, zoom, isSideBySide) {
            var elem = document.getElementById(elementId);
            if (!elem) {
                throw new Error('No element with ID ' + elementId);
            }

            // Initialize map if needed
            if (!elem.map) {
                elem.map = L.map(elementId).setView([location.lat, location.lon], zoom);
                L.tileLayer(tileUrl, { attribution: tileAttribution }).addTo(elem.map);

                location = L.circle([location.lat, location.lon], {
                    color: location.color,
                    fillOpacity: 0.05,
                    radius: 50,
                    id: 'location'
                });
                elem.map.addLayer(location);
            }

            if (isSideBySide === null) {
                return;
            }

            if (!isSideBySide) {
                elem.style.height = (window.innerHeight * 50 / 100) + "px";
            }
            else {
                elem.style.height = (window.innerHeight * 70 / 100) + "px";
            }
            elem.map.invalidateSize();
        },

        renderMarkers: function (elementId, markers) {
            var elem = document.getElementById(elementId);
            if (!elem) {
                throw new Error('No element with ID ' + elementId);
            }

            var map = elem.map;
            var layerGroup = L.layerGroup().addTo(map);

            var corridors = [];
            var options = {
                corridor: 1000, // meters
                className: 'route-corridor'
            };

            markers.forEach(element => setMarker(element, null, null, corridors));

            //corridors.forEach(element => setCorridors(element, null, null, options));

            function setMarker(m, index, array, corridors, options) {
                marker = L.circle([m.lat, m.lon], {
                    color: m.color,
                    fillOpacity: 0.05,
                    radius: 50,
                    id: 'marker'
                });
                marker.bindPopup('Sat: ' + String(m.satNo) + ', Quality: ' + String(m.quality));
                layerGroup.addLayer(marker);

                ////add to corridor
                //if (!corridors[m.satNo]) {
                //    corridors[m.satNo] = [];
                //}
                //if (m.alt > 100) {
                //    corridors[m.satNo].push(L.latLng(m.lat, m.lon));
                //}
            }

            //function setCorridors(c, index, array, options) {
            //    map.addLayer(L.corridor(c, options));
            //}
        },

        clearMarkers: function (elementId, location) {
            var elem = document.getElementById(elementId);
            if (!elem) {
                throw new Error('No element with ID ' + elementId);
            }

            var map = elem.map;
            map.eachLayer(function (layer) {
                if (layer.options.id === 'marker') {
                    map.removeLayer(layer);
                }
            });
        }
    };
})();
