$(function () {
    $('#OnlineUser').hover(function () {
        $('#OnlineUser').hide();
        $('#ConnectionStatus').show();
    })

    $('.closeHIDE').click(function () {
        $('#OnlineUser').show();
        $('#ConnectionStatus').hide();
    })
})