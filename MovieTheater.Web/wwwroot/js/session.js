let sessionData = null;
let sessionDate = document.getElementById("sessionDate").dataset.date;

// логіка обирання часу сеансу
document.querySelectorAll(".session-time-window").forEach(block => {
    block.addEventListener("click", () => {
      document.querySelectorAll(".session-time-window").forEach(el => el.classList.remove("selected"));

      block.classList.add("selected");

      sessionData = {
        time: block.dataset.time,
        hall: block.dataset.hall,
        seats: block.dataset.seats
      };

    updateBookingButton();
    });
  });

function updateBookingButton() {
  const btn = document.getElementById("bookingBtn");
  if (sessionData && selectedSeats.size > 0) {
    btn.classList.remove("disabled");
  } else {
    btn.classList.add("disabled");
  }
}

// ціни місць
const standardPrice = 150;
const premiumPrice = 250;

// букви звичайних рядів
const rows = ["A","B","C","D","E","F","G","H","I"];
// скільки звичайних місць в ряду
const seatsPerRow = 12;
// зайнято (і звичайні і преміум)
const occupiedSeats = ["A7", "B3", "B12","C4", "J5"];
// для збереження обраних місць
const selectedSeats = new Set();
// букви преміум рядів
const premiumRows = ["J"]
// преміум місць має бути в два рази менше в ряду
// або треба якось переписати стиль для них (поки хз як зробити адаптовану width у них)
const seatsPerPremiumRow = 6;


const layoutDiv = document.querySelector(".seats-layout");


// генерую звичайні місця

rows.forEach(row => {
  const rowDiv = document.createElement("div");
  rowDiv.className = "seat-row";

  for (let i = 1; i <= seatsPerRow; i++) {
    const seatId = `${row}${i}`;
    const seat = document.createElement("div");
    seat.className = "seat";
    seat.dataset.id = seatId;

    if (occupiedSeats.includes(seatId)) {
      seat.classList.add("occupied");
    }

    seat.textContent = i;
    seat.addEventListener("click", () => {
      if (seat.classList.contains("occupied")) return;

      seat.classList.toggle("selected");

      if (selectedSeats.has(seatId)) {
        selectedSeats.delete(seatId);
      } else {
        selectedSeats.add(seatId);
      }
      
      updateBookingSummary();
      updateBookingButton();

    });

    rowDiv.appendChild(seat);
  }

  const rowLabel = document.createElement("span");
  rowLabel.className = "row-label";
  rowLabel.textContent = row;
  rowDiv.prepend(rowLabel);

  layoutDiv.appendChild(rowDiv);
});

// генерую преміум місця


premiumRows.forEach(row => {
  const rowDiv = document.createElement("div");
  rowDiv.className = "seat-row";

  for (let i = 1; i <= seatsPerPremiumRow; i++) {
    const seatId = `${row}${i}`;
    const seat = document.createElement("div");
    seat.className = "premium-seat";
    seat.dataset.id = seatId;

    if (occupiedSeats.includes(seatId)) {
      seat.classList.add("occupied");
    }

    seat.textContent = i;
    seat.addEventListener("click", () => {
      if (seat.classList.contains("occupied")) return;

      seat.classList.toggle("selected");

      

      if (selectedSeats.has(seatId)) {
        selectedSeats.delete(seatId);
      } else {
        selectedSeats.add(seatId);
      }

      updateBookingSummary();
      updateBookingButton();

    });

    rowDiv.appendChild(seat);
  }

  const rowLabel = document.createElement("span");
  rowLabel.className = "row-label";
  rowLabel.textContent = row;
  rowDiv.prepend(rowLabel);

  layoutDiv.appendChild(rowDiv);
});

// оновлювати дані про бронювання внизу сторінки


function updateBookingSummary() {
  const countSpan = document.getElementById("selected-count");
  const priceSpan = document.getElementById("total-price");

  let total = 0;

  selectedSeats.forEach(id => {
    const rowLetter = id[0];
    if (premiumRows.includes(rowLetter)) {
      total += premiumPrice;
    } else {
      total += standardPrice;
    }
  });

  countSpan.textContent = selectedSeats.size;
  priceSpan.textContent = total;
}

// відкриття модального вікна

document.querySelector(".booking-btn").addEventListener("click", openBookingModal);

function openBookingModal() {
  
// якщо не обрано час та місце модальне не відкриється
  if (!sessionData || selectedSeats.size === 0) {
    return; 
  }

  const sessionInfo = `Сеанс ${sessionData.time}, ${sessionDate}, ${sessionData.hall}`;

  const listEl = document.getElementById("selected-seats-list");
  const modalTotal = document.getElementById("modal-total");

  listEl.innerHTML = ""; 

  let total = 0;
  selectedSeats.forEach(id => {
    const row = id[0];
    const number = id.slice(1);
    const price = premiumRows.includes(row) ? premiumPrice : standardPrice;
    total += price;

    const li = document.createElement("li");
    li.textContent = `Ряд ${row}, місце ${number} — ${price}₴`;
    listEl.appendChild(li);
  });

  document.getElementById("session-info").textContent = sessionInfo;
  modalTotal.textContent = total;

  const modal = new bootstrap.Modal(document.getElementById("bookingModal"));
  modal.show();
}