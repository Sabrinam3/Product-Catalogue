
const Mixin = {
    methods: {
        formatDate(date) {
            let d;
            date === undefined
                ? d = new Date()
                : d = new Date(Date.parse(date));
            let _day = d.getDate();
            let _month = d.getMonth() + 1;
            let _year = d.getFullYear();
            let _hour = d.getHours();
            let _min = d.getMinutes();
            if (_min < 10) { _min = "0" + _min; }
            return _year + "-" + _month + "-" + _day + " " + _hour + ":" + _min;
        },
    }
};

// register modal component
Vue.component("modal", {
    template: "#modal-template",
    props: {
        order: {},
        details: {}
    },
    mixins: [Mixin]
});

const app9 = new Vue({
    el: '#orders',
    methods: {
        async getOrders() {
            try {
                this.status = 'Loading... ';
                let response = await fetch(`/GetOrders`);
                if (!response.ok)
                    throw new Error(`Status - ${response.status}, Text -${response.statusText}`);
                let data = await response.json();
                this.orders = data;
                this.status = '';
            } catch (error) {
                this.status = error;
                console.log(error);
            }
        },
        async loadAndShowModal() {
            try {
                this.modalStatus = "Loading Details..";
                this.showModal = true;
                let response = await fetch(`/GetOrderDetails/${this.orderForModal.id}`);
                this.detailsForModal = await response.json();
                this.modalStatus = "";
            } catch (error) {
                console.log(error.statusText);
            }
        },
    },
    mixins: [Mixin],
    mounted() { this.getOrders(); },
    data: {
        orders: [],
        showModal: false,
        orderForModal: {},
        detailsForModal: {},
        status: "",
        modalStatus: ""
    }
});

Vue.filter('toCurrency', function (value) {
    if (typeof value !== "number") {
        return value;
    }
    const formatter = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        minimumFractionDigits: 2
    });
    return formatter.format(value);
});

