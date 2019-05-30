global.$ = require('jquery');
require('webui-popover');

var api = require('./api');

$(function () {
    $('#tooltip').webuiPopover({
        width: 426,
        height: 138,
        placement: 'top',
        offsetTop: -48,
        offsetLeft: 500,
        animation:'pop'
    });

    $('#url').keyup(function (event) {
        if (event.keyCode === 13) {
            $('#submit').click();
        }
    });

    $('#submit').bind('click', function () {
        $('#link').hide();
        $('#error').hide();
        $('#spinner').show();

        api.shortenUrl($('#url').val())
          .then(function (shortUrl) {
            $('#link').attr('href', shortUrl).text(shortUrl);
            $('#link').show();
        }).catch(function (error) {
            $('#error').text(error);
            $('#error').show();
        }).finally(function() {
            $('#spinner').hide();
        });
    });
});
