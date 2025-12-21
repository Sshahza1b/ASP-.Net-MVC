$(document).ready(function () {
    $("#navSearchInput").on("keyup", function () {
        let query = $(this).val();
        let resultsDiv = $("#searchResults");

        if (query.length > 2) {
            $.ajax({
                url: '/Customer/Home/Search',
                type: 'GET',
                data: { query: query },
                success: function (response) {
                    resultsDiv.empty().removeClass("d-none");
                    if (response.data.length > 0) {
                        response.data.forEach(item => {
                            resultsDiv.append(`
                                <a href="/Customer/Home/Details?productId=${item.id}" class="d-flex align-items-center p-2 border-bottom text-decoration-none text-dark hover-bg-light">
                                    <img src="${item.imageUrl}" style="width:40px; height:50px; object-fit:cover;" class="rounded me-2">
                                    <div>
                                        <div class="fw-bold small">${item.title}</div>
                                        <div class="text-muted" style="font-size:0.7rem;">${item.author} - $${item.price}</div>
                                    </div>
                                </a>
                            `);
                        });
                    } else {
                        resultsDiv.append('<div class="p-3 text-muted small">No products found.</div>');
                    }
                }
            });
        } else {
            resultsDiv.addClass("d-none").empty();
        }
    });

    // Hide results when clicking outside
    $(document).on("click", function (e) {
        if (!$(e.target).closest(".search-box").length) {
            $("#searchResults").addClass("d-none");
        }
    });
});

$(document).ready(function () {
    $("#loginForm").on("submit", function (e) {
        e.preventDefault(); // Page refresh roknay ke liye

        let email = $("#loginEmail").val();
        let password = $("#loginPassword").val();
        let btn = $("#btnLoginSubmit");
        let spinner = $("#loginSpinner");
        let errorDiv = $("#loginError");

        // UI Changes
        btn.prop("disabled", true);
        spinner.removeClass("d-none");
        errorDiv.addClass("d-none");

        $.ajax({
            url: '/Customer/Account/Login', // Aapka controller action
            type: 'POST',
            data: { email: email, password: password },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    // 1 second baad page reload karo taake login state update ho jaye
                    setTimeout(() => { location.reload(); }, 1000);
                } else {
                    errorDiv.text(response.message).removeClass("d-none");
                    btn.prop("disabled", false);
                    spinner.addClass("d-none");
                }
            },
            error: function () {
                toastr.error("Something went wrong. Please try again.");
                btn.prop("disabled", false);
                spinner.addClass("d-none");
            }
        });
    });
});