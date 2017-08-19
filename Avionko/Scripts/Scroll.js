/*handling button clicks and redirections to various divs */






$(document).ready(function () {
    scrollToResults();
});




function scrollToResults() {


    $('html, body').animate({ scrollTop: $('#searchResults').offset().top }, 2000);
    return false;
}


function scrollToTop() {

    $('html, body').animate({ scrollTop: $('#page-top').offset().top }, 2000);
    return false;
}



