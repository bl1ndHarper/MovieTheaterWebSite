/*
const movies = [
  {
    title: "Pulp Fiction",
    genre: "Трілер-крімінал",
    length: "2:28",
    rating: 4.5,
    image: "https://m.media-amazon.com/images/I/81UTs3sC5hL._UF1000,1000_QL80_.jpg"
  },
  {
    title: "Guardians of the Galaxy",
    genre: "Трілер-крімінал",
    length: "2:28",
    rating: 2.2,
    image: "https://m.media-amazon.com/images/I/71lbFfxfMtL._UF894,1000_QL80_.jpg"
  },
  {
    title: "Snatch",
    genre: "Трілер-крімінал",
    length: "2:38",
    rating: 5.0,
    image: "https://m.media-amazon.com/images/M/MV5BMjI0MjYzODA1NF5BMl5BanBnXkFtZTgwNTQxNzI2MTI@._V1_FMjpg_UX1000_.jpg"
  },
  {
    title: "American Gagnster",
    genre: "Трілер-крімінал",
    length: "2:28",
    rating: 4.5,
    image: "https://m.media-amazon.com/images/I/71tEMmFxo7L._UF894,1000_QL80_.jpg"
  },
  {
    title: "Pulp Fiction",
    genre: "Трілер-крімінал",
    length: "2:28",
    rating: 4.5,
    image: "https://m.media-amazon.com/images/I/81UTs3sC5hL._UF1000,1000_QL80_.jpg"
  },
  {
    title: "Pulp Fiction",
    genre: "Трілер-крімінал",
    length: "2:28",
    rating: 4.5,
    image: "https://m.media-amazon.com/images/I/81UTs3sC5hL._UF1000,1000_QL80_.jpg"
  },
];
*/

const slider = document.getElementById("movie-slider");
const movies = slider.children;

let currentIndex = 0;

let currentSlide = 1;
let totalSlides = Math.ceil(movies.length / 3)

function renderSlider() {
  slider.innerHTML = "";

    const visible = movies.slice(currentIndex, currentIndex + 3);
  visible.forEach(movie => {
    const fullStars = Math.floor(movie.rating);
    const hasHalf = movie.rating % 1 >= 0.5;
    const emptyStars = 5 - fullStars - (hasHalf ? 1 : 0);

    const starsHTML =
      `<i class="fa-solid fa-star" style="color: #FFD43B;"></i>`.repeat(fullStars) +
      (hasHalf ? `<i class="fa-solid fa-star-half-stroke" style="color: #FFD43B;"></i>` : "") +
      `<i class="fa-regular fa-star" style="color: #FFD43B;"></i>`.repeat(emptyStars);

    const cardHTML = `
      <div class="col-lg-4 col-md-6">
        <div class="new-movie-card h-100">
          <div class="card-img-container">
            <img class="card-img-top" src="${movie.image}">
          </div>
          <div class="card-text-content">
            <div class="movie-title-div">
              <p class="movie-title">${movie.title}</p>
            </div>
            <div class="genre-length-info-div row">
              <p class="movie-genre col text-start">${movie.genre}</p>
              <p class="movie-length col text-end">${movie.length}</p>
            </div>
            <div class="star-rating">${starsHTML}</div>
            <div class="movie-button-div">
              <a href="#" class="movie-button">Детальніше</a>
            </div>
          </div>
        </div>
      </div>
    `;

    slider.insertAdjacentHTML("beforeend", cardHTML);
  });
}

function nextSlide() {
  if (currentIndex + 3 < movies.length) {
    currentIndex += 3;
    currentSlide++
    renderSlider();
    updateNavButtons();
  }
}

function prevSlide() {
  if (currentIndex - 3 >= 0) {
    currentIndex -= 3;
    currentSlide--

    renderSlider();
    updateNavButtons();
  }
}

function updateNavButtons() {
  const prevBtn = document.querySelector('.btn-prev');
  const nextBtn = document.querySelector('.btn-next');

  if (currentIndex === 0) {
    prevBtn.classList.add('disabled');
    prevBtn.setAttribute('disabled', true);
  } else {
    prevBtn.classList.remove('disabled');
    prevBtn.removeAttribute('disabled');
  }

  if (currentIndex + 3 >= movies.length) {
    nextBtn.classList.add('disabled');
    nextBtn.setAttribute('disabled', true);
  } else {
    nextBtn.classList.remove('disabled');
    nextBtn.removeAttribute('disabled');
  }
}

//renderSlider();
updateNavButtons();