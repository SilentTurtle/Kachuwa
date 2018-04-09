$(".dropdown").hover(function() {
     $(this).addClass('show');
}, function () { $(this).removeClass('show'); });
var openSearch = $('.open-search'), SearchForm = $('.full-search'), closeSearch = $('.close-search'); openSearch.on('click', function (event) {
    event.preventDefault();
    if (!SearchForm.hasClass('active'))
    { SearchForm.fadeIn(300, function() {
         SearchForm.addClass('active');
    }); }
});
loseSearch.on('click', function(event) {
     event.preventDefault(); SearchForm.fadeOut(300, function() {
         SearchForm.removeClass('active'); $(this).find('input').val('');
    });
}); var wow = new WOW({ mobile: false });
wow.init();
$(document).ready(function() {
     $('.wpb-mobile-menu').slicknav({ prependTo: '.navbar-header', parentTag: 'liner', allowParentLinks: true, duplicate: true, label: '', closedSymbol: '<i class="fa fa-angle-right"></i>', openedSymbol: '<i class="fa fa-angle-down"></i>', });
});
('#clients-scroller').owlCarousel({ items: 4, itemsTablet: 3, margin: 90, stagePadding: 90, smartSpeed: 450, itemsDesktop: [1199, 4], itemsDesktopSmall: [980, 3], itemsTablet: [768, 3], itemsTablet: [767, 2], itemsTabletSmall: [480, 2], itemsMobile: [479, 1], });
('#color-client-scroller').owlCarousel({ items: 4, itemsTablet: 3, margin: 90, stagePadding: 90, smartSpeed: 450, itemsDesktop: [1199, 4], itemsDesktopSmall: [980, 3], itemsTablet: [768, 3], itemsTablet: [767, 2], itemsTabletSmall: [480, 2], itemsMobile: [479, 1], });
('#testimonial-item').owlCarousel({ autoPlay: 5000, items: 3, itemsTablet: 3, margin: 90, stagePadding: 90, smartSpeed: 450, itemsDesktop: [1199, 4], itemsDesktopSmall: [980, 3], itemsTablet: [768, 3], itemsTablet: [767, 2], itemsTabletSmall: [480, 2], itemsMobile: [479, 1], }); $('#testimonial-dark').owlCarousel({ autoPlay: 5000, items: 3, itemsTablet: 3, margin: 90, stagePadding: 90, smartSpeed: 450, itemsDesktop: [1199, 4], itemsDesktopSmall: [980, 3], itemsTablet: [768, 3], itemsTablet: [767, 2], itemsTabletSmall: [480, 2], itemsMobile: [479, 1], }); $('#single-testimonial-item').owlCarousel({ singleItem: true, autoPlay: 5000, items: 1, itemsTablet: 1, margin: 90, stagePadding: 90, smartSpeed: 450, itemsDesktop: [1199, 4], itemsDesktopSmall: [980, 3], itemsTablet: [768, 3], itemsTablet: [767, 2], itemsTabletSmall: [480, 2], itemsMobile: [479, 1], stopOnHover: true, }); $("#image-carousel").owlCarousel({ autoPlay: 3000, items: 4, itemsDesktop: [1170, 3], itemsDesktopSmall: [1170, 3] }); $("#carousel-image-slider").owlCarousel({ navigation: false, slideSpeed: 300, paginationSpeed: 400, singleItem: true, pagination: false, autoPlay: 3000, }); $(document).ready(function () { $('#carousel-about-us').owlCarousel({ navigation: true, navigationText: ['<i class="fa fa-angle-left"></i>', '<i class="fa fa-angle-right"></i>'], slideSpeed: 800, paginationSpeed: 400, autoPlay: true, singleItem: true, pagination: false, items: 1, itemsCustom: false, itemsDesktop: [1199, 4], itemsDesktopSmall: [980, 3], itemsTablet: [768, 2], itemsTabletSmall: false, itemsMobile: [479, 1], }); }); $(function () { $('#portfolio').mixItUp(); }); $('testimonial-carousel').carousel(); $('a[data-slide="prev"]').click(function () { $('#testimonial-carousel').carousel('prev'); }); $('a[data-slide="next"]').click(function () { $('#testimonial-carousel').carousel('next'); }); jQuery(document).ready(function ($) { $('.counter').counterUp({ delay: 1, time: 800 }); }); $('.skill-shortcode').appear(function () { $('.progress').each(function () { $('.progress-bar').css('width', function () { return ($(this).attr('data-percentage') + '%') }); }); }, { accY: -100 }); var offset = 200; var duration = 500; $(window).scroll(function () { if ($(this).scrollTop() > offset) { $('.back-to-top').fadeIn(400); } else { $('.back-to-top').fadeOut(400); } }); $('.back-to-top').click(function (event) {
    event.preventDefault();
    $('html, body').animate({ scrollTop: 0 }, 600); return false;
})