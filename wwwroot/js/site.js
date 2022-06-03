// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.querySelector('.hamburger-button').addEventListener('click', function () {
    document.querySelector('.animated-icon').classList.toggle('open');
    if(document.querySelector('.animated-icon').classList.contains('open')) {
        document.querySelector('.overlay').classList.remove('fade-out')
        document.querySelector('.overlay').classList.add('fade-in')
    }
    else {
        document.querySelector('.overlay').classList.remove('fade-in')
        document.querySelector('.overlay').classList.add('fade-out')
    }
});