(function () {
    const toggle = document.querySelector("[data-nav-toggle]");
    const shell = document.querySelector(".shell");
    if (toggle && shell) {
        toggle.addEventListener("click", function () {
            const open = shell.classList.toggle("nav-open");
            toggle.setAttribute("aria-label", open ? "Close menu" : "Open menu");
        });
    }

    document.querySelectorAll("[data-line-table]").forEach(function (table) {
        const body = table.querySelector("tbody");
        const add = table.closest(".lines").querySelector("[data-add-line]");

        function money(value) {
            return value.toLocaleString("en-US", { style: "currency", currency: "USD" });
        }

        function updateAmounts() {
            body.querySelectorAll("tr").forEach(function (row) {
                const qty = parseFloat(row.querySelector("[data-qty]").value) || 0;
                const price = parseFloat(row.querySelector("[data-price]").value) || 0;
                row.querySelector("[data-amount]").textContent = money(qty * price);
            });
        }

        function reindex() {
            body.querySelectorAll("tr").forEach(function (row, index) {
                row.querySelectorAll("[name]").forEach(function (input) {
                    input.name = input.name.replace(/Lines\[\d+\]/, "Lines[" + index + "]");
                });
            });
        }

        function bindRemove(row) {
            const button = row.querySelector("[data-remove-line]");
            button.addEventListener("click", function () {
                if (body.querySelectorAll("tr").length === 1) {
                    row.querySelectorAll("input").forEach(function (input) { input.value = ""; });
                    updateAmounts();
                    return;
                }
                row.remove();
                reindex();
                updateAmounts();
            });
        }

        body.querySelectorAll("tr").forEach(bindRemove);
        body.addEventListener("input", updateAmounts);
        add.addEventListener("click", function () {
            const clone = body.querySelector("tr").cloneNode(true);
            clone.querySelectorAll("input").forEach(function (input) { input.value = ""; });
            clone.querySelectorAll(".field-error").forEach(function (error) { error.textContent = ""; });
            body.appendChild(clone);
            bindRemove(clone);
            reindex();
            updateAmounts();
        });
        updateAmounts();
    });
})();
