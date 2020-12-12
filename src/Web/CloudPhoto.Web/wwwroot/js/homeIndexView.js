var stickyNavBarHeight;

$(document).ready(
    function () {
        var element = document.getElementById("myDivHeader");
        element.classList.add("myHeader");
        stickyNavBarHeight = element.offsetHeight;

        $('nav').removeClass('navbar-light');
        $('nav').removeClass('my-nav-color');
    });

$(window).on("scroll", function () {
    if ($(window).scrollTop()) {
        $('nav').addClass('fixed-top');
    }
    else {
        $('nav').removeClass('fixed-top');
        $('nav').removeClass('navbar-light');
        $('nav').removeClass('my-nav-color');

        //var element = document.getElementById("myDivHeader");
        //element.classList.add("myHeader");
    }



    if (window.pageYOffset >= stickyNavBarHeight) {
        $('nav').addClass('navbar-light');
        $('nav').addClass('my-nav-color');

        //var element = document.getElementById("myDivHeader");
        //element.classList.remove("myHeader");
    }
});