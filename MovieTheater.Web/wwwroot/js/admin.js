document.addEventListener("DOMContentLoaded", function () {
    const dropdown = document.getElementById("hallsDropdown");
    const sectors = document.querySelectorAll(".sectors");

    function updateVisibleHall() {
        const selectedHallId = dropdown.value;

        sectors.forEach(div => {
            div.style.display = div.dataset.hallId === selectedHallId ? "block" : "none";
        });

        const hiddenInput = document.querySelector("form input[name='hallId']");
        if (hiddenInput) hiddenInput.value = selectedHallId;
    }

    dropdown.addEventListener("change", updateVisibleHall);
    updateVisibleHall();

    document.querySelectorAll(".update-price-btn").forEach(button => {
        button.addEventListener("click", async function () {
            const row = this.closest(".sector-row");
            const sectorId = row.dataset.sectorId;
            const priceInput = row.querySelector(".sector-price");
            const price = parseFloat(priceInput.value).toFixed(2);

            try {
                const res = await fetch(`/api/admin/hall/${dropdown.value}/sector/${sectorId}/price`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ sectorId: parseInt(sectorId), price: parseFloat(price) })
                });

                if (!res.ok) {
                    const error = await res.json();
                    alert("Помилка: " + error.detail);
                } else {
                    priceInput.value = price;
                    alert("Ціну оновлено");
                }
            } catch (err) {
                alert("Помилка при оновленні: " + err.message);
            }
        });
    });

    let sectorToDelete = null;

    document.querySelectorAll(".delete-sector-btn").forEach(button => {
        button.addEventListener("click", function () {
            const hallId = dropdown.value;
            const sectorId = this.closest(".sector-row").dataset.sectorId;
            const sectorName = this.dataset.sectorName;

            sectorToDelete = {
                hallId,
                sectorId,
                row: this.closest(".sector-row")
            };

            document.getElementById("modalSectorName").textContent = `"${sectorName}"`;
            const modal = new bootstrap.Modal(document.getElementById("confirmDeleteModal"));
            modal.show();
        });
    });

    document.getElementById("confirmDeleteBtn").addEventListener("click", async function () {
        if (!sectorToDelete) return;

        try {
            const res = await fetch(`/api/admin/hall/${sectorToDelete.hallId}/sector/${sectorToDelete.sectorId}`, {
                method: "DELETE"
            });

            if (!res.ok) {
                const error = await res.json();
                alert("Помилка: " + error.detail);
            } else {
                sectorToDelete.row.remove();
            }
        } catch (err) {
            alert("Помилка при видаленні: " + err.message);
        }

        const modal = bootstrap.Modal.getInstance(document.getElementById("confirmDeleteModal"));
        modal.hide();

        sectorToDelete = null;
    });
});
