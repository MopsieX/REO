var Settings = Settings || (function () {
    const { ipcRenderer } = require('electron');
    var head = null;
    return {
        Init: function () {
            head = document.getElementsByTagName('head')[0];
        },
        ApplyStyle: function (cssFile) {
            ipcRenderer.send('SetSettingsStyle', cssFile);
            this.LoadStyle(cssFile);
        },
        LoadStyle: function (cssFile) {
            $('#stylesheet').remove();
            if (cssFile === 'default') {
                return;
            }
            var myPath = "css/settings/" + cssFile;
            $.when($.get(myPath))
                .done(function (data) {
                    var msgButton = $('<link>').attr({ type: 'text/css', id: 'stylesheet', rel: 'stylesheet', href: 'data:text/css;charset=UTF-8,' + encodeURIComponent(data) });
                    $(head).append(msgButton);
                });

        },
        SetChatMessageMethod: function () {
            ipcRenderer.send('SetChatMessageMethod', $("input[type='radio'][name='chatmessagesendmethod']:checked").val());
        }
    };
}());
