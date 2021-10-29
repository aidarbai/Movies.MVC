$(document).ready(function () {
    // Swiper slider
    $('.my-swiper').slick({
        infinite: true,
        slideMargin: 10,
        useCSS: false,
        slidesToShow: 4,
        slidesToScroll: 2,
        dots: true,
        responsive: [
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
                }
            },
            {
                breakpoint: 576,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    });

    // Movie rating circle
    $('.circle').circleProgress({
        fill: { gradient: ['darkblue', 'lightblue'] },
        startAngle: -Math.PI / 2,
        size: 50,
        thickness: 5,
        animation: false
    })

    // Logout
    //$("#logout-link").click(() => $("#logout-link").closest("form").submit());
    //$("#logout-link").click(function () { $(this).closest("form").submit() });
    $("#logout-link").click(e => $(e.currentTarget).closest("form").submit());

    // Activate tooltip
    $('[data-toggle="tooltip"]').tooltip();


    // Passing data to edit modal window on manage movies page
    $(document).on("click", '.edit', function (e) {
        var id = $(this).data('id');
        var title = $(this).data('title');
        var overview = $(this).data('overview');
        var youtube = $(this).data('youtube');

        $(".movie_id").val(id);
        $(".movie_title").val(title);
        $(".movie_overview").val(overview);
        $(".movie_youtube").val(youtube);
    });

    // Passing data to delete modal window on manage movies page
    $(document).on("click", '.delete', function (e) {
        var id = $(this).data('id');
        $(".movie_id").val(id);
    });

    // Resizing textarea in edit movie modal window
    autosize($("#editMovieModal .movie_overview"));

    $('#editMovieModal').on('shown.bs.modal', function () {
        autosize.update($("#editMovieModal .movie_overview"));
    })

    // Showing spinner while uploading new movies to database
    function displayBusyIndicator() {
        document.getElementById("loading").style.display = "block";
    }

    $("#upload-movies-btn").click(() => displayBusyIndicator());

    // Select/Deselect checkboxes

    $("input[type=checkbox]:checked").each(function () {
        if ($(this).is(":checked")) {
            $(".chkCheckBoxId").prop("checked", true)
        }
        else {
            $(".chkCheckBoxId").prop("checked", false)
        }
    });

    $('#checkBoxAll').click(function () {
        if ($(this).is(":checked")) {
            $(".chkCheckBoxId").prop("checked", true)
        } else {
            $(".chkCheckBoxId").prop("checked", false)
        }
    });

    // bind this event to multi delete button
    $(".multi-delete").click(function () {
        var modal = $("#multiDeleteMovieModal").find(".modal-body");
        $(".checkbox").each(function (index) {
            var articleId = $(this).val();
            if ($(this).is(":checked")) {
                $(modal).append("<input name='articlesArray' value='" + articleId + "'  type='hidden' />")
            }
        });
    });

    // Passing data to edit user modal window on manage users page
    $(document).on("click", '.edit-user', function (e) {
        var userId = $(this).data('id');
        var email = $(this).data('email');
        var firstname = $(this).data('name');
        var lastname = $(this).data('lastname');
        var roles = $(this).data('roles').split(', ');
        var disabledUser = $(this).data('disabled');

        console.log(disabledUser);
        
        $(".user_id").val(userId);
        $(".user_email").val(email);
        $(".user_firstname").val(firstname);
        $(".user_lastname").val(lastname);

        $(".user-roles > .form-check-input").each(function (index) {
            var role = $(this).val();
            $(this).prop('checked', roles.includes(role));
        });
        
        $(".user-dis-checkbox").val(disabledUser);
        $(".user-dis-checkbox").prop('checked', disabledUser === "True");
        
    });

    // Passing data to delete modal window on manage users page
    $(document).on("click", '.delete-user', function (e) {
        var id = $(this).data('id');
        $(".user_id").val(id);
    });

    // Sending vote for movie
    $(".vote-form .vote").click(function () {
        var data = $(this).attr('var');
        $('.vote-form .sendvote').attr("value", data);
        $(this).closest('form').submit();
    });

    // Hiding ajaxsearch input
    if ($("#movie-search").length) {
        $("#source").addClass("d-none");
    }

});