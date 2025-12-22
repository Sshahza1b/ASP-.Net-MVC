$(document).ready(function () {
    // --- 1. SEARCH FUNCTIONALITY ---
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
                    if (response.data && response.data.length > 0) {
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

    // --- 2. LOGIN FUNCTIONALITY ---
    $("#loginForm").on("submit", function (e) {
        e.preventDefault();

        // IDs check karein (Modal HTML mein yehi honi chahiye)
        let email = $("#lEmail").val() || $("#loginEmail").val();
        let password = $("#lPassword").val() || $("#loginPassword").val();
        let btn = $(this).find('button[type="submit"]');

        btn.prop("disabled", true).text("Checking...");

        $.ajax({
            url: '/Customer/Account/Login',
            type: 'POST',
            data: { email: email, password: password },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    setTimeout(() => { window.location.reload(); }, 1000);
                } else {
                    toastr.error(response.message);
                    btn.prop("disabled", false).text("Login to Account");
                }
            },
            error: function () {
                toastr.error("Server error! Check if DB is updated.");
                btn.prop("disabled", false).text("Login to Account");
            }
        });
    });

    // --- 3. REGISTER FUNCTIONALITY ---
    $("#registerForm").on("submit", function (e) {
        e.preventDefault();

        let email = $("#rEmail").val();
        let password = $("#rPassword").val();
        let btn = $(this).find('button[type="submit"]');

        btn.prop("disabled", true).text("Creating Account...");

        $.ajax({
            url: '/Customer/Account/Register', // URL bilkul sahi honi chahiye
            type: 'POST',
            data: { email: email, password: password },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    setTimeout(() => { window.location.reload(); }, 1000);
                } else {
                    // Yahan Identity ke asli errors (like Password weak) nazar ayenge
                    toastr.error(response.message);
                    btn.prop("disabled", false).text("Create Account");
                }
            },
            error: function () {
                toastr.error("Server error! Make sure database tables exist.");
                btn.prop("disabled", false).text("Create Account");
            }
        });
    });

    // Hide search results when clicking outside
    $(document).on("click", function (e) {
        if (!$(e.target).closest(".search-box").length) {
            $("#searchResults").addClass("d-none");
        }
    });
});

function logoutUser() {
    $.ajax({
        url: '/Customer/Account/Logout',
        type: 'POST',
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                // Foran reload ya redirect karein
                window.location.href = '/';
            }
        },
        error: function (xhr) {
            console.log(xhr.responseText);
            toastr.error("Logout failed! Error: " + xhr.status);
        }
    });
}