const { ipcRenderer } = require('electron')

window.sendInGameMessage = function (username, message) {
    ipcRenderer.sendToHost('sendInGameMessage', username, message);
}
window.CreateToolTip = function (text, x, y) {
    var msgButton = $('<span/>').attr({ name: 'tooltip' });
    msgButton.css('zIndex', '999');
    msgButton.css('left', x);
    msgButton.css('top', y);
    msgButton.css('position', 'absolute');
    msgButton.text(text);
    $("body").append(msgButton);
}