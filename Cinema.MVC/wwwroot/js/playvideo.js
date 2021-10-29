$(document).ready(function () {

    var $videoSrc;
    $('.video-btn').click(function () {
        $videoSrc = $(this).data("src");
    });

    $('.play-trailer').click(function () {
        $videoSrc = $(this).data("src");
    });
    
    $('#myModal').on('shown.bs.modal', function (e) {
        $("#video").attr('src', $videoSrc && $videoSrc + "?autoplay=1&amp;modestbranding=1&amp;showinfo=0");
    })

    
    $('#myModal').on('hide.bs.modal', function (e) {
        $("#video").attr('src', $videoSrc);
    })

    window.addEventListener('click', e => {
        const videoBtn = e.target.closest('.video-btn')
        if (videoBtn) {
            console.log('Video-btn');
            const dataSrc = videoBtn.dataset.src
            if (!dataSrc) {
                e.stopPropagation()
            }
        }
    }, { capture: true })

});