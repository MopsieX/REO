var SendMessageClick = function (username,ele) {
    window.sendInGameMessage(username);
    if (chatMessageMethod === 'CopyToClipboard') {
        var position = $(ele).offset();
        window.CreateToolTip("Copied to clipboard", position.left, Math.max(0, position.top - $(ele).height() - 2));
    }
}
var insertMessageButtons = function () {
    if (typeof insertedButtons !== 'undefined') {
        /*Only insert buttons 1 time. Quickly navigating to another page while the current page is still loading causes did-finish-load event to fire twice.*/
        return;
    }
    insertedButtons = true;
    if ($(".table.tablesorter") != null) {
        var offer = $('.table.tablesorter > tbody').find('tr');/*get all offers*/
        offer.each(function (index) {
            var poster = $(this).find('a[href*="/offers-by/"]');/*get the username of the person that posted the offer*/
            var msgButton = $('<input/>').attr({ type: 'button', name: 'message', class: 'sendMessage', value: 'Message', onclick: 'SendMessageClick("' + poster.text() + '",this); ' });/*create a new button to message the poster*/
            var newTd = $('<td>');/*create a new td for the button*/
            $(newTd).append(msgButton);/*insert the button into the td*/
            $(this).append($(newTd));/*insert the td into the row*/
        });
    }
}
insertMessageButtons();