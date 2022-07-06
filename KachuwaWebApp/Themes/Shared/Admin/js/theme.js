$(function () {
    $('.hamburger').click(function () {
        $('.side-menu').toggleClass('disappear');
        $(function () {
            setTimeout(function () {
                $('.main-content').toggleClass('fullwidth');
            }, 200);
        });
    });
});

$(function () {
    $('.panel-heading').click(function (e) {
        $('.panel-heading').removeClass('tab-collapsed');
        var collapsCrnt = $(this).find('.collapse-controle').attr('aria-expanded');
        if (collapsCrnt != 'true') {
            $(this).addClass('tab-collapsed');
        }
    });
});

$(function () {
    //$('.side-menu').niceScroll();
});