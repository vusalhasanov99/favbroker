// Site nav ---------------
const nav = document.getElementById("site-menu");
const header = document.getElementById("top");

window.addEventListener("scroll", function () {
    // nav.classList.add("nav-sticky");
    header.classList.add("nav-sticky");
    document.querySelector("main").classList.add("pt-scroll");
});

function navToggle() {
    const btn = document.getElementById("menuBtn");
    const nav = document.getElementById("menu");

    btn.classList.toggle("open");
    nav.classList.toggle("flex");
    nav.classList.toggle("hidden");
    header.classList.toggle("h-screen");
    document.body.classList.toggle("overflow-hidden");
}

const dropDowns = document.querySelectorAll(".menu-dropdown");

dropDowns.forEach((drpdwn) => {
    const dropDwnBtn = drpdwn.querySelector(".dropdown-btn");
    const dropDwnSvg = dropDwnBtn.querySelector("svg");
    const dropDwnBox = dropDwnBtn.nextElementSibling;

    dropDwnBtn.addEventListener("click", () => {
        if (dropDwnBox.classList.contains("hidden")) {
            dropDowns.forEach((drpdwn) => {
                const dropDwnBtn = drpdwn.querySelector(".dropdown-btn");
                dropDwnBtn.querySelector("svg").classList.remove("rotate-180");
                dropDwnBtn.nextElementSibling.classList.remove("flex");
                dropDwnBtn.nextElementSibling.classList.add("hidden");
            });
            dropDwnBox.classList.toggle("hidden");
            dropDwnBox.classList.toggle("flex");
            dropDwnSvg.classList.toggle("rotate-180");
        } else {
            dropDwnBox.classList.toggle("hidden");
            dropDwnBox.classList.toggle("flex");
            dropDwnSvg.classList.toggle("rotate-180");
        }
    });
});
// Country select --------------
const countryBox = document.querySelector(".country-box");
const countrySelect = document.querySelector(".country-select");
const selectedCountry = document.querySelector(".selected-country");
const countryArrow = document.querySelector(".select-arrow");
const countryList = document.querySelector(".country-list");
const countryListEach = document.querySelectorAll(".country-list li");
const countryBtn = document.querySelector(".country-btn");
function listToggle() {
    countryList.classList.toggle("c-list-open");
    countryList.classList.toggle("c-list-close");
    countryBox.classList.toggle("bg-slate-50");
    countryArrow.classList.toggle("rotate-180");
}

if (countryBox) {
    countrySelect.addEventListener("click", () => listToggle());

    countryListEach.forEach((list) => {
        list.addEventListener("click", () => {
            listToggle();
            selectedCountry.innerHTML = list.dataset.name;
            countryBtn.dataset.link = list.dataset.link;
            countryBtn.disabled = false;
        });
    });

    countryBtn.addEventListener("click", () => {
        window.location = countryBtn.dataset.link;
    });
    window.addEventListener("click", (e) => {
        if (
            !e.target.closest("div.country-box") &&
            countryList.classList.contains("c-list-open")
        ) {
            listToggle();
        }
    });
}