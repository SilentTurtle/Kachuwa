function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function eraseCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}

$(function () {
    var x = getCookie("am-x-h");
    if (x=="1") {
        $('.side-menu').toggleClass('disappear');
        $(function() {
            setTimeout(function() {
                $('.main-content').toggleClass('fullwidth');
                },
                200);
        });
    } else {
        setCookie("am-x-h","0",10);
    }
    $('.hamburger').click(function () {
        $('.side-menu').toggleClass('disappear');
        if ($('.side-menu').hasClass("disappear")) {
            setCookie("am-x-h", "1", 10);
        } else {
            setCookie("am-x-h", "0", 10);
        }
        $(function() {
		  setTimeout(function() {
              $('.main-content').toggleClass('fullwidth');  
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

