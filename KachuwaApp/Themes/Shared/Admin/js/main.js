$(function () {
    $('.hamburger').click(function () {
        $('.side-menu').toggleClass('disappear');
        $(function() {
		  setTimeout(function() {
		    $('.side-body').toggleClass('fullwidth');  
		  }, 200);
		});              
    });
});

$(function() {
    $('.panel-heading').click(function(e) {
        $('.panel-heading').removeClass('tab-collapsed');
        var collapsCrnt = $(this).find('.collapse-controle').attr('aria-expanded');
        if (collapsCrnt != 'true') {
            $(this).addClass('tab-collapsed');
        }
    });
});

$(function () {
    //$('.side-menu').niceScroll({
    //    cursorcolor: "green", // change cursor color in hex
    //    cursoropacitymin: 0, // change opacity when cursor is inactive (scrollabar "hidden" state), range from 1 to 0
    //    cursoropacitymax: 1, // change opacity when cursor is active (scrollabar "visible" state), range from 1 to 0
    //    cursorwidth: "5px", // cursor width in pixel (you can also write "5px")
    //    cursorborder: "1px solid #fff", // css definition for cursor border
    //    cursorborderradius: "5px", // border radius in pixel for cursor
    //    zindex: "auto",// | [number], // change z-index for scrollbar div
    //    scrollspeed: 60, // scrolling speed
    //    mousescrollstep: 40, // scrolling speed with mouse wheel (pixel)
    //    touchbehavior: false, // DEPRECATED!! use "touchemulate"
    //    emulatetouch: false, // enable cursor-drag scrolling like touch devices in desktop computer
    //    hwacceleration: true, // use hardware accelerated scroll when supported
    //    boxzoom: false, // enable zoom for box content
    //    dblclickzoom: true, // (only when boxzoom=true) zoom activated when double click on box
    //    gesturezoom: true, // (only when boxzoom=true and with touch devices) zoom activated when pinch out/in on box
    //    grabcursorenabled: true ,// (only when touchbehavior=true) display "grab" icon
    //    autohidemode: true, // how hide the scrollbar works, possible values: true | // hide when no scrolling"cursor" | 
    //    // only cursor hiddenfalse | // do not hide,
    //    //"leave" | // hide only if pointer leaves content
    //   // "hidden" | // hide always
    //    //"scroll", // show only on scroll          
    //    background: "", // change css for rail background
    //    iframeautoresize: true, // autoresize iframe on load event
    //    cursorminheight: 32, // set the minimum cursor height (pixel)
    //    preservenativescrolling: true, // you can scroll native scrollable areas with mouse, bubbling mouse wheel event
    //    railoffset: false, // you can add offset top/left for rail position
    //    bouncescroll: false, // (only hw accell) enable scroll bouncing at the end of content as mobile-like 
    //    spacebarenabled: true, // enable page down scrolling when space bar has pressed
    //    railpadding: { top: 0, right: 0, left: 0, bottom: 0 }, // set padding for rail bar
    //    disableoutline:
    //        true, // for chrome browser, disable outline (orange highlight) when selecting a div with nicescroll
    //    horizrailenabled: true, // nicescroll can manage horizontal scroll
    //   // railalign: right, // alignment of vertical rail
    //    //railvalign: bottom, // alignment of horizontal rail
    //    enabletranslate3d: true, // nicescroll can use css translate to scroll content
    //    enablemousewheel: true, // nicescroll can manage mouse wheel events
    //    enablekeyboard: true, // nicescroll can manage keyboard events
    //    smoothscroll: true, // scroll with ease movement
    //    sensitiverail: true, // click on rail make a scroll
    //    enablemouselockapi: true, // can use mouse caption lock API (same issue on object dragging)
    //    cursorfixedheight: false, // set fixed height for cursor in pixel
    //    hidecursordelay: 400, // set the delay in microseconds to fading out scrollbars
    //    directionlockdeadzone: 6, // dead zone in pixels for direction lock activation
    //    nativeparentscrolling: true, // detect bottom of content and let parent to scroll, as native scroll does
    //    enablescrollonselection: true, // enable auto-scrolling of content when selection text
    //    cursordragspeed: 0.3, // speed of selection when dragged with cursor
    //    rtlmode: "auto", // horizontal div scrolling starts at left side
    //    cursordragontouch: false, // drag cursor in touch / touchbehavior mode also
    //    oneaxismousemode:
    //        "auto", // it permits horizontal scrolling with mousewheel on horizontal only content, if false (vertical-only) mousewheel don't scroll horizontally, if value is auto detects two-axis mouse
    //    scriptpath: "", // define custom path for boxmode icons ("" => same script path)
    //    preventmultitouchscrolling: true, // prevent scrolling on multitouch events
    //    disablemutationobserver: false, // force MutationObserver disabled,
    //    enableobserver:
    //        true, // enable DOM changing observer, it tries to resize/hide/show when parent or content div had changed
    //    scrollbarid: false // set a custom ID for nicescroll bars 
    //});
    //$('body,.side-menu').niceScroll();
    $('.side-menu').niceScroll();
});



$(document).ready(function() {
    "use strict";
    $.fn.peity.defaults.pie = {
        delimiter: null,
        fill: ["#40babd", "#f5f5f5", "#92d1d2"],
        height: null,
        radius: 16,
        width: null
    }, $.fn.peity.defaults.donut = {
        delimiter: null,
        fill: ["#40babd", "#f5f5f5", "#92d1d2"],
        height: null,
        innerRadius: null,
        radius: 16,
        width: null
    }, $.fn.peity.defaults.line = {
        delimiter: ",",
        fill: "#92d1d2",
        height: 32,
        max: null,
        min: 0,
        stroke: "#40babd",
        strokeWidth: 2,
        width: 120
    }, $.fn.peity.defaults.bar = {
        delimiter: ",",
        fill: ["#40babd"],
        height: 32,
        max: null,
        min: 0,
        padding: .1,
        width: 64
    }, $(".peity-pie").peity("pie"), 
    	$(".peity-donut").peity("donut"), 
    	$(".peity-line").peity("line"), 
    	$(".peity-bar").peity("bar");
    var t = $(".peity-updating-chart").peity("line", {
        width: 800,
        height: 78
    });
    setInterval(function() {
        var i = Math.round(10 * Math.random()),
            e = t.text().split(",");
        e.shift(), e.push(i), t.text(e.join(",")).change()
    }, 1e3),
     $(".peity-pie-data-attributes").peity("pie"), 
     $(".peity-donut-data-attributes").peity("donut"), 
     $(".peity-line-data-attributes").peity("line"), 
     $(".peity-bar-data-attributes").peity("bar"),
     $(window).resize(function() {
        $(".peity-full-width").peity()
    })
});