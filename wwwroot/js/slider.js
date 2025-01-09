$(document).ready(function () {
    $('.slider').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        dots: true,
        arrows: true,
        prevArrow: '<button type="button" class="slick-prev">&lt;</button>',
        nextArrow: '<button type="button" class="slick-next">&gt;</button>'
    });

    lightbox.option({
        'resizeDuration': 200,
        'wrapAround': true
    });
});