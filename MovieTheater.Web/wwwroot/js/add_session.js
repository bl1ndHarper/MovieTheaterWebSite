let selectedMovieId = null; 
let selectedMovieTitleDisplay = '';

async function loadExistingMovies() {
    try {
        const response = await fetch("/api/movies/all"); 
        if (!response.ok) {
            throw new Error(`HTTP помилка! статус: ${response.status}`);
        }
        return response.json(); // Отримаємо List<MovieListItemDto>
    } catch (error) {
        console.error("Помилка при завантаженні існуючих фільмів:", error);
        return [];
    }
}


async function loadHalls() {
    try {

        const response = await fetch("/api/admin/halls");
        
        if (!response.ok) {
            throw new Error(`HTTP помилка! статус: ${response.status}`);
        }
        
        return response.json();
        
    } catch (error) {
        console.error("Детальна помилка:", error);
        console.error("Стек помилки:", error.stack);
        return [];
    }
}

document.addEventListener('DOMContentLoaded', async function() {
    const maxBlocks = 5;
    let blocksCount = 0;
    let halls = []; 
    let existingMovieOptions = [];

    try {
        halls = await loadHalls();
        console.log("Завантажені зали:", halls);
    } catch (error) {
        console.error("Невийшло завантажити зали:", error);
        halls = [];
    }

    try {
        existingMovieOptions = await loadExistingMovies();
        console.log("Завантажені існуючі фільми для списку:", existingMovieOptions);
        const movieSelect = document.getElementById("existingMovieSelect");
        existingMovieOptions.forEach(movie => {
            const option = document.createElement("option");
            option.value = movie.id;
            option.innerText = `${movie.title} (${movie.releaseYear})`;
            movieSelect.appendChild(option);
        });
    } catch (error) {
        console.error("Не вийшло завантажити існуючі фільми для списку:", error);
        existingMovieOptions = [];
    }

     document.getElementById("existingMovieSelect").addEventListener("change", function() {
        const selectElement = this;
        selectedMovieId = selectElement.value ? parseInt(selectElement.value) : null;
        selectedMovieTitleDisplay = selectElement.options[selectElement.selectedIndex].text;

        const selectedMovieDisplaySpan = document.getElementById("selectedMovieDisplay");
        if (selectedMovieId) {
            selectedMovieDisplaySpan.innerText = `Обраний фільм: ${selectedMovieTitleDisplay}`;
            selectedMovieDisplaySpan.style.display = "inline";
        } else {
            selectedMovieDisplaySpan.innerText = "";
            selectedMovieDisplaySpan.style.display = "none";
        }
        console.log("Обраний ID фільму:", selectedMovieId);
    });

    const timeContainer = document.querySelector(".session-time-add-window");
    document.querySelector(".session-time-add-window").addEventListener("click", () => {
        if (blocksCount >= maxBlocks) {
            alert("Можна додати не більше 5 сеансів на день.");
            return;
        }
        const block = document.createElement("div");
        block.className = "session-time-admin-window mt-3";

        const timeInput = `<input class="change-time-input form-control" type="time" step="60">`;
        const hallSelect = `<select class="dropdown-select">${halls.map(h => `<option value="${h.id}">${h.name}</option>`).join("")}</select>`;
        const deleteBtn = `<button class="delete-time-btn mt-2">Видалити</button>`;

        block.innerHTML = `
            ${timeInput}
            <div class="dropdown-div hall-dropdown mt-3" style=width:100%;>
              ${hallSelect}
              <div class="d-flex justify-content-center mt-3">${deleteBtn}</div>
            </div>
        `;

        // видалити блок часу
        block.querySelector(".delete-time-btn").addEventListener("click", () => {
            block.remove();
            blocksCount--;
        });

        timeContainer.before(block);
        blocksCount++;
    });

    document.getElementById("AddSession").addEventListener("click", async function () {
        //  чи обрано фільм
        if (selectedMovieId === null) {
            return alert("Будь ласка, оберіть фільм зі списку, перш ніж додавати сеанси!");
        }

        try {
            const sessions = [];
            // збираю усі вибрані дати
            const selectedDateSpans = Array.from(document.querySelectorAll('#dateContainer [data-date]'));
            // збираю усі блоки з часом та залою
            const sessionTimeBlocks = Array.from(document.querySelectorAll(".session-time-admin-window"));

            if (selectedDateSpans.length === 0) {
                return alert("Будь ласка, оберіть хоча б одну дату для сеансу.");
            }
            if (sessionTimeBlocks.length === 0) {
                return alert("Будь ласка, додайте хоча б один час сеансу.");
            }

            // по кожній даті 
            selectedDateSpans.forEach(dateSpan => {
                const date = dateSpan.dataset.date; // формат "dd.mm"

                // для кожної дати створюю сеанси для усіх обраних часів та залів
                sessionTimeBlocks.forEach(block => {
                    const timeInput = block.querySelector("input[type='time']");
                    const hallSelect = block.querySelector("select");

                    if (!timeInput || !hallSelect) {
                        console.warn("Помилка: Не знайдено input для часу або select для зали в одному з блоків сеансу.", block);
                        return; 
                    }

                    const time = timeInput.value; // час у форматі "HH:mm"
                    const hallId = hallSelect.value;

                    // ще перевірка на заповненість даних
                    if (!time || !hallId || !date) {
                        console.warn(`Помилка: не всі дані для сеансу заповнені. Дата: ${date}, Час: ${time}, ID Зали: ${hallId}`);
                        return;
                    }

                    //  дата + час
                    const [day, month] = date.split('.');
                    const currentYear = new Date().getFullYear(); // поточний рік

                    // створюю об'єкт Date. місяці з 0 йдуть
                    const sessionDateTime = new Date(currentYear, parseInt(month) - 1, parseInt(day));
                    
                    const [hours, minutes] = time.split(':');
                    sessionDateTime.setHours(parseInt(hours), parseInt(minutes), 0, 0); 

                    // валідність створеної дати
                    if (isNaN(sessionDateTime.getTime())) {
                        console.error(`Помилка: Не вдалося створити валідну дату/час з '${date}' та '${time}'.`);
                        return;
                    }

                    const startTime = sessionDateTime.toISOString(); // в ISO рядок для бекенду

                    sessions.push({
                        startTime: startTime,
                        hallId: parseInt(hallId),
                        movieId: selectedMovieId // айді фільму, вибраного з select
                    });
                });
            });
            
            // ще перевірка
            if (sessions.length === 0) {
                return alert("Не вдалося сформувати жодного сеансу. Перевірте, чи всі поля заповнені та дати/часи коректні.");
            }

            console.log("Сеанси для відправки:", JSON.stringify(sessions, null, 2));

            //  дані на API
            const response = await fetch("/api/admin/sessions/create", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(sessions)
            });

            const responseText = await response.text(); //відповідь як текст спочатку

            // обробка відповіді від API
            if (response.ok) {
                alert("Сеанси успішно додано!");
                // перезавантажити сторінку 
                location.reload(); 
            } else {
                // логування помилки 
                let errorMessage = `Помилка при створенні сеансів (HTTP ${response.status}).`;
                const contentType = response.headers.get("content-type");

                if (contentType && contentType.includes("application/json") && responseText.trim().length > 0) {
                    try {
                        const errorData = JSON.parse(responseText);
                        errorMessage = errorData.detail || errorData.message || errorMessage;
                    } catch (jsonError) {
                        console.error("Помилка парсингу JSON відповіді помилки:", jsonError);
                        errorMessage += " Не вдалося розпарсити деталі помилки.";
                    }
                } else if (responseText.trim().length > 0) {
                     errorMessage += ` Відповідь сервера: ${responseText}`;
                }
                alert(errorMessage);
                console.error("Деталі помилки:", responseText);
            }
        //теж  логування помилки але для всього блоку
        } catch (error) {
            console.error("Загальна помилка при додаванні сеансів:", error);
            alert("Сталася неочікувана помилка при спробі додати сеанси.");
        }
    });
});


