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

$(function(){
    $('.panel-heading').click(function(e) {
        $('.panel-heading').removeClass('tab-collapsed');
        var collapsCrnt = $(this).find('.collapse-controle').attr('aria-expanded');
        if (collapsCrnt != 'true') {
            $(this).addClass('tab-collapsed');
        }
    });
})



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