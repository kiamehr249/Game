// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function showMessage(sets) {
    new Noty({
        text: sets.text,
        type: sets.type,
        theme: 'sunset',
        progressBar: true,
        timeout: 4000
    }).show();
}





