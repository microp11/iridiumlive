(function () {
    var tileUrl = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
    var tileAttribution = 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, <a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>';

    // Global export
    window.deliveryMap = {
        showOrUpdate: function (elementId, location, zoom, markers) {
            var elem = document.getElementById(elementId);
            if (!elem) {
                throw new Error('No element with ID ' + elementId);
            }

            // Initialize map if needed
            if (!elem.map) {
                elem.map = L.map(elementId).setView([location.lat, location.lon], zoom);
                elem.map.addedMarkers = [];
                L.tileLayer(tileUrl, { attribution: tileAttribution }).addTo(elem.map);
            }

            var map = elem.map;

            location = L.circle([location.lat, location.lon], {
                color: location.color,
                fillOpacity: 0.5,
                radius: 50,
                id: 'location'
            });
            map.addLayer(location);

            if (map.addedMarkers.length !== markers.length) {
                // Markers have changed, so reset
                map.addedMarkers.forEach(marker => marker.removeFrom(map));
                map.addedMarkers = markers.map(m => {
                    return L.marker([m.y, m.x]).bindPopup(m.description).addTo(map);
                });

                //// Auto-fit the view
                //var markersGroup = new L.featureGroup(map.addedMarkers);
                //map.fitBounds(markersGroup.getBounds().pad(0.3));

                //// Show applicable popups. Can't do this until after the view was auto-fitted.
                //markers.forEach((marker, index) => {
                //    if (marker.showPopup) {
                //        map.addedMarkers[index].openPopup();
                //    }
                //});
            } else {
                // Same number of markers, so update positions/text without changing view bounds
                markers.forEach((marker, index) => {
                    animateMarkerMove(
                        map.addedMarkers[index].setPopupContent(marker.description),
                        marker,
                        4000);
                });
            }

            elem.style.height = window.innerHeight / 2 + "px";
            map.invalidateSize();
        },

        renderMarkers: function (elementId, markers) {
            var elem = document.getElementById(elementId);
            if (!elem) {
                throw new Error('No element with ID ' + elementId);
            }

            var map = elem.map;
            var layerGroup = L.layerGroup().addTo(map);

            markers.forEach(setMarker);

            function setMarker(m, index, array) {
                marker = L.circle([m.lat, m.lon], {
                    color: m.color,
                    fillOpacity: 0.5,
                    radius: 50,
                    id: 'marker'
                });
                layerGroup.addLayer(marker);
            }
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

    function animateMarkerMove(marker, coords, durationMs) {
        if (marker.existingAnimation) {
            cancelAnimationFrame(marker.existingAnimation.callbackHandle);
        }

        marker.existingAnimation = {
            startTime: new Date(),
            durationMs: durationMs,
            startCoords: { x: marker.getLatLng().lng, y: marker.getLatLng().lat },
            endCoords: coords,
            callbackHandle: window.requestAnimationFrame(() => animateMarkerMoveFrame(marker))
        };
    }

    function animateMarkerMoveFrame(marker) {
        var anim = marker.existingAnimation;
        var proportionCompleted = (new Date().valueOf() - anim.startTime.valueOf()) / anim.durationMs;
        var coordsNow = {
            x: anim.startCoords.x + (anim.endCoords.x - anim.startCoords.x) * proportionCompleted,
            y: anim.startCoords.y + (anim.endCoords.y - anim.startCoords.y) * proportionCompleted
        };

        marker.setLatLng([coordsNow.y, coordsNow.x]);

        if (proportionCompleted < 1) {
            marker.existingAnimation.callbackHandle = window.requestAnimationFrame(
                () => animateMarkerMoveFrame(marker));
        }
    }
})();
