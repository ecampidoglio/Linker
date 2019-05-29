var $ = require('jquery');

module.exports.shortenUrl = function (url) {
    return new Promise(function (resolve, reject) {
        $.post(
            '/link',
            { url: url }
        ).done(function (data, status, response) {
            var shortUrl = response.getResponseHeader('Location');
            resolve(shortUrl);
        }).fail(function (data, status, error) {
            reject(error);
        });
    });
}
