// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

let checkInField = null;
let checkOutField = null;
let priceField = null;


document.addEventListener("DOMContentLoaded", (event) => {
    priceField = document.getElementById("totalPrice");
    checkInField = document.getElementById("checkIn");
    checkOutField = document.getElementById("checkOut");

    checkInField.addEventListener("change", validateDates);
    checkOutField.addEventListener("change", validateDates);

    updatePrice();

});

function updatePrice() {
    if (checkInField != null && checkOutField != null) {
        let checkInDate = new Date(checkInField.value);
        let checkOutDate = new Date(checkOutField.value);
        let difference = checkOutDate.getTime() - checkInDate.getTime();
        let totalDays = Math.ceil(difference / (1000 * 3600 * 24));

        let roomSelect = document.getElementById("roomSelect");
        let roomOption = roomSelect.options[roomSelect.selectedIndex].text;

        let dailyPrice = roomOption.slice(1, roomOption.search("/"));

        let totalPrice = totalDays * dailyPrice;
        if (!isNaN(totalPrice)) {
            priceField.value = "$" + totalPrice;

            let hiddenPriceField = document.getElementById("Price");
            hiddenPriceField.value = totalPrice;
        }

    }
}

function validateDates() {
    if (checkInField != null && checkOutField != null) {
        let currentDate = new Date();

        let checkInDate = new Date(checkInField.value);
        let checkOutDate = new Date(checkOutField.value);

        let dateDifference = (checkOutDate.getTime() - checkInDate.getTime()) / 1000 / 3600;

        let infoBar = document.getElementById("infoBar");
        if (checkInDate >= checkOutDate) {
            infoBar.innerHTML = "Check-in date must be before check-out date.";
        }
        else if (checkInDate < currentDate.getTime() || checkOutDate < currentDate.getTime()) {
            infoBar.innerHTML = "Check-in and check-out dates must be in the future.";
        }
        else if (dateDifference < 8) {
            infoBar.innerHTML = "Reservation must be at least 8 hours long.";
        }
        else {
            infoBar.innerHTML = "";
        }
    }
}