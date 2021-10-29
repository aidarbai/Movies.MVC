const source = document.getElementById("source");
const search = document.getElementById("menu-search");
const results = search.querySelector(".dropdown-menu");

if (source) {
    const inputHandler = function (e) {
        const keyword = e.target.value.toString().trim();
        if (keyword.length >= 3) {
            getJson("/Home/GetAjaxSearchMovie/" + keyword);
        }
    };

    source.addEventListener("keyup", inputHandler);
    source.addEventListener("focus", inputHandler);

    async function getJson(url) {
        fetch(url)
        .then(response => response.json())
        .then(result => renderResults(result))
        .catch(error => console.error(error));
    }

    function renderResults(json) {
        if (json.length > 0) {
            results.innerHTML = "";

            json.forEach((e) => {
                results.innerHTML += `
            
                <a href="/film/${e.id}" class="nav-link d-flex flex-row align-items-center w-100 h-100 mb-2 p-2 user-menu-link">`
                    + (e.poster_small_path
                        ? `<img src="${e.poster_small_path}" class="poster-image"/>`
                        : '<div class="no-image"></div>'
                    )
                    + `<span class="movie-title ml-2">${e.title}</span>
                </a>
            
        `
            });
        }
        else {
            results.innerHTML = `<span class="movie-title ml-2">Not found</span>`;
        }

        $('#dropdownResultButton').dropdown('show');

    }
}