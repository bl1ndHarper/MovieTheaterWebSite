let selectedSessionId = null;
let sessionDate = document.getElementById("sessionDateSelect")?.value;
const selectedSeats = new Set();
const layoutDiv = document.querySelector(".seats-layout");

// Зберігатимемо інформацію про сектори/ціни
let sectorsInfo = {};

function updateBookingButton() {
    const btn = document.getElementById("bookingBtn");
    if (selectedSessionId && selectedSeats.size > 0) {
        btn.classList.remove("disabled");
    } else {
        btn.classList.add("disabled");
    }
}

function updateBookingSummary() {
    const countSpan = document.getElementById("selected-count");
    const priceSpan = document.getElementById("total-price");

    let total = 0;
    selectedSeats.forEach(id => {
        const sector = id.split(":")[0];
        total += sectorsInfo[sector] || 0;
    });

    countSpan.textContent = selectedSeats.size;
    priceSpan.textContent = total;
}

async function loadSeats(sessionId) {
    selectedSeats.clear();
    layoutDiv.innerHTML = "";
    const res = await fetch(`/Sessions/${sessionId}/Seats`);
    const data = await res.json();

    sectorsInfo = {};
    const rows = {};

    // формуємо rows та зберігаємо ціну секторів
    data.forEach(seat => {
        const rowLetter = seat.label[0];
        if (!rows[rowLetter]) rows[rowLetter] = [];
        rows[rowLetter].push(seat);

        const sector = seat.sectorName;
        if (!sectorsInfo[sector]) sectorsInfo[sector] = seat.price;
    });

    // малюємо ряди в алфавітному порядку (від A до Z)
    Object.keys(rows)
        .sort((a, b) => a.charCodeAt(0) - b.charCodeAt(0))
        .forEach(rowLetter => {
            const rowSeats = rows[rowLetter].sort((a, b) => {
                const aNum = parseInt(a.label.substring(1));
                const bNum = parseInt(b.label.substring(1));
                return aNum - bNum;
            });

            const rowDiv = document.createElement("div");
            rowDiv.className = "seat-row";

            const rowLabel = document.createElement("span");
            rowLabel.className = "row-label";
            rowLabel.textContent = rowLetter;
            rowDiv.appendChild(rowLabel);

            rowSeats.forEach(seat => {
                const sector = seat.sectorName;

                const seatEl = document.createElement("div");
                seatEl.className = sectorsInfo[sector] >= 200 ? "premium-seat" : "seat";
                seatEl.dataset.id = `${sector}:${seat.label}`;
                seatEl.textContent = seat.label.substring(1); // лише число

                if (seat.status === 1) {
                    seatEl.classList.add("occupied");
                }

                seatEl.addEventListener("click", () => {
                    if (seatEl.classList.contains("occupied")) return;
                    seatEl.classList.toggle("selected");

                    const seatId = seatEl.dataset.id;
                    if (selectedSeats.has(seatId)) {
                        selectedSeats.delete(seatId);
                    } else {
                        selectedSeats.add(seatId);
                    }

                    updateBookingSummary();
                    updateBookingButton();
                });

                rowDiv.appendChild(seatEl);
            });

            layoutDiv.appendChild(rowDiv);
        });

    // підписуємо ціни після побудови рядів (щоб не порушити порядок)

    for (const sector in sectorsInfo) {
        const label = document.createElement("p");
        label.className = "text-light font-secondary mt-2";
        label.textContent = `${sector} — ${sectorsInfo[sector]}₴`;
        layoutDiv.appendChild(label);
    }
}


// Обробка натискання на час сеансу

const sessionBlocks = document.querySelectorAll(".session-time-window");
sessionBlocks.forEach(block => {
    block.addEventListener("click", () => {
        sessionBlocks.forEach(b => b.classList.remove("selected"));
        block.classList.add("selected");
        selectedSessionId = block.dataset.sessionid;
        loadSeats(selectedSessionId);
        updateBookingButton();
    });
});

// Кнопка бронювання

document.getElementById("bookingBtn")?.addEventListener("click", openBookingModal);

function openBookingModal() {
    if (!selectedSessionId || selectedSeats.size === 0) return;

    const listEl = document.getElementById("selected-seats-list");
    const modalTotal = document.getElementById("modal-total");
    const sessionInfoEl = document.getElementById("session-info");

    listEl.innerHTML = "";
    let total = 0;
    selectedSeats.forEach(fullId => {
        const [sector, label] = fullId.split(":");
        const price = sectorsInfo[sector] || 0;
        total += price;
        const li = document.createElement("li");
        li.textContent = `Сектор ${sector}, місце ${label} — ${price}₴`;
        listEl.appendChild(li);
    });

    sessionInfoEl.textContent = `Сеанс ${sessionDate}, ID: ${selectedSessionId}`;
    modalTotal.textContent = total;

    const modal = new bootstrap.Modal(document.getElementById("bookingModal"));
    modal.show();
}