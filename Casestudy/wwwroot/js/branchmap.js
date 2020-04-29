// modal component
Vue.component("branchmodal", {
    template: "#modal-template",
    props: {
        lat: '',
        lng: '',
        details: []
    },
    methods: {
        async showModal() {
            try {
                let response = await fetch(`/GetBranches/${this.lat}/${this.lng}`);
                let data = await response.json();
                this.details = data;
                let myLatLng = new google.maps.LatLng(this.lat, this.lng);
                let map_canvas = document.getElementById("map");
                let options = {
                    zoom: 10, center: myLatLng, mapTypeId:
                        google.maps.MapTypeId.ROADMAP
                };
                let map = new google.maps.Map(map_canvas, options);
                let center = map.getCenter();
                let infowindow = new google.maps.InfoWindow({ content: "" });
                let idx = 0;
                this.details.map((detail) => {
                    idx = idx + 1;
                    let marker = new google.maps.Marker({
                        position: new google.maps.LatLng(detail.latitude, detail.longitude),
                        map: map,
                        animation: google.maps.Animation.DROP,
                        icon: `../images/marker${idx}.png`,
                        title: `Branch#${detail.id} ${detail.street}, ${detail.city},${detail.region}`,
                        html: `<div>Branch#${detail.id}<br/>${detail.street}, ${detail.city}<br/>${detail.distance.toFixed(2)} km</div>`
                    });
                    google.maps.event.addListener(marker, "click", () => {
                        infowindow.setContent(marker.html); // added .html to the marker object.
                        infowindow.close();
                        infowindow.open(map, marker);
                    });
                });
                map.setCenter(center);
                google.maps.event.trigger(map, "resize");
            } catch (error) {
                console.log(error.statusText);
            }
        }
    },
    mounted() { this.showModal(); }
});

const storemap = new Vue({
    el: '#branches',
    methods: {
        loadAndShowModal() {
            try {
                // A service for converting between an address to LatLng
                let geocoder = new google.maps.Geocoder();
                geocoder.geocode({ address: this.address }, (results, status) => {
                    if (status === google.maps.GeocoderStatus.OK) { // only if google gives us the OK
                        this.lat = results[0].geometry.location.lat();
                        this.lng = results[0].geometry.location.lng();
                        this.showModal = true;
                    }
                });
            } catch (error) {
                console.log(error);
            }
        }
    },
    data: {
        address: '',
        lat: '',
        lng: '',
        showModal: false
    }
});