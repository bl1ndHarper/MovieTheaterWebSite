let selectedMovie = null;

document.getElementById("searchMovieBtn").addEventListener("click", async function () {
    const query = document.getElementById("searchMovieInApi").value.trim();
    if (!query) return;

    const response = await fetch(`/api/admin/movies/search?query=${encodeURIComponent(query)}`);
    if (!response.ok) {
        const err = await response.json();
        alert(err.detail || "Помилка пошуку фільмів");
        return;
    }
    const movies = await response.json();

    const container = document.getElementById("foundMoviesDiv");
    container.innerHTML = "";

    movies.forEach(movie => {
        const div = document.createElement("div");
        div.className = "d-flex justify-content-between found-movie mt-3";
        div.innerHTML = `
            <span class="found-movie-title">${movie.title} (${movie.releaseDate})</span>
            <i class="fa-solid fa-plus addMovieIcon" data-id="${movie.tmdbId}" style="margin-top:5px; margin-right:5px"></i>
        `;
        container.appendChild(div);
    });

    document.querySelectorAll(".addMovieIcon").forEach(icon => {
        icon.addEventListener("click", async function () {
            const tmdbId = this.dataset.id;
            const res = await fetch(`/api/admin/movies/details/${tmdbId}`);
            if (!res.ok) {
                const err = await res.json();
                alert(err.detail || "Помилка завантаження деталей");
                return;
            }

            selectedMovie = await res.json();
            renderSelectedMovie(selectedMovie);
        });
    });
});

document.querySelector(".delete-time-btn").addEventListener("click", async function () {
    if (!selectedMovie) return alert("No movie was selected");

    const response = await fetch("/api/admin/movies/save", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(selectedMovie)
    });
    
    if (response.ok) {
        alert("Фільм збережено");
    } else {
        const err = await response.json();
        alert(err.detail || "Помилка під час збереження");
    }
});

function renderSelectedMovie(movie) {
    document.querySelector(".movie-thumb").src = `${movie.thumbnailUrl}`;
    document.querySelector(".session-movie-title").innerText = movie.title;
    document.querySelector(".movie-director").innerHTML = `<span>Режисер:</span> -`; // можна розширити
    document.querySelector(".movie-desc").innerText = movie.description;

    const genresDiv = document.querySelector(".tags-genres-div");
    genresDiv.innerHTML = movie.genres.map(genre =>
        `<div class="genre-tag text-light-emphasis small">${genre.genre.name}</div>`
    ).join("") + `<div class="rating-tag text-light-emphasis small">Рейтинг: ${movie.adult ? "18+" : "13+"}</div>`;

    const statsDiv = document.querySelector(".session-info");
    statsDiv.innerHTML = `
        <span class="stats-tag">${movie.adult ? "18+" : "13+"}</span>
        <span class="stats-tag">⭐ ${movie.imdbRating}/10 IMDb</span>
        <span class="stats-tag">${movie.duration} хв</span>
        <span class="stats-tag">${movie.releaseDate?.split("-")[0]}</span>
    `;
}
