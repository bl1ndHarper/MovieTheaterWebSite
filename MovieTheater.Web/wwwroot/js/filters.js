

const genreSelect = document.querySelector('[name="genre"]');
const ratingSelect = document.querySelector('[name="rating"]');
const durationSelect = document.querySelector('[name="duration"]');

const titleInput = document.getElementById('searchInput');
titleInput?.addEventListener('input', applyFilters);

const cards = document.querySelectorAll('.movie-session-card');

console.log(cards);

[genreSelect, ratingSelect, durationSelect].forEach(select => {
    select.addEventListener('change', applyFilters);
  });

  
  function applyFilters() {
  const genreSelect = document.querySelector('[name="genre"]');
  const ratingSelect = document.querySelector('[name="rating"]');
  const durationSelect = document.querySelector('[name="duration"]');

  const searchValue = titleInput?.value?.trim().toLowerCase();

  const selectedGenre = genreSelect?.value?.trim().toLowerCase();
  const selectedRating = ratingSelect?.value?.trim().toLowerCase();
  const selectedDuration = durationSelect?.value;

  cards.forEach(card => {
    const cardGenre = card.dataset.genre?.trim().toLowerCase();
    const cardRating = card.dataset.rating?.trim().toLowerCase();
    const cardDuration = parseInt(card.dataset.duration);

    const cardTitle = card.querySelector('.session-movie-title')?.textContent?.trim().toLowerCase() || "";
    const titleMatch = !searchValue || cardTitle.includes(searchValue);

    const genreMatch = !selectedGenre || selectedGenre === "будь-який" || selectedGenre === cardGenre;
    const ratingMatch = !selectedRating || selectedRating === "будь-який" || selectedRating === cardRating;

    let durationMatch = true;
    if (!selectedDuration || selectedDuration === "будь-який") {
     durationMatch = true;}
    else if (selectedDuration === "<90") durationMatch = cardDuration < 90;
    else if (selectedDuration === "90-120") durationMatch = cardDuration >= 90 && cardDuration <= 120;
    else if (selectedDuration === "120-150") durationMatch = cardDuration > 120 && cardDuration <= 150;
    else if (selectedDuration === ">150") durationMatch = cardDuration > 150;

    if (genreMatch && ratingMatch && durationMatch && titleMatch) {
        card.style.display = "";
    } else {
        card.style.display = "none";
    }
  });
}