$(function () {
        var welcomeSection = $('.welcome-section'),
            enterbutton = welcomeSection.find('.enter-button');

        setTimeout(function () {
            welcomeSection.removeClass('content-hidden');
        }, 500);
    })