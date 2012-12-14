function(ownerModelId, messageboxText, caption, button, icon, allowHtml, callback) {
    var Cancel = '<button data-result="' + System_Windows_MessageBoxResult.Cancel + '">Cancel</button>';
    var OK = '<button data-result="' + System_Windows_MessageBoxResult.OK + '">OK</button>';
    var Yes = '<button data-result="' + System_Windows_MessageBoxResult.Yes + '">Yes</button>';
    var No = '<button data-result="' + System_Windows_MessageBoxResult.No + '">No</button>';

    switch (icon) {
        default:
        case System_Windows_MessageBoxImage.None:
            icon = "warning";
            break;
        case System_Windows_MessageBoxImage.Error:
        case System_Windows_MessageBoxImage.Hand:
        case System_Windows_MessageBoxImage.Stop:
            icon = "error";
            break;
        case System_Windows_MessageBoxImage.Exclamation:
        case System_Windows_MessageBoxImage.Warning:
            icon = "warning";
            break;
        case System_Windows_MessageBoxImage.Question:
        case System_Windows_MessageBoxImage.Asterisk:
        case System_Windows_MessageBoxImage.Information:
            icon = "success";
            break;
    }

    var buttons;
    switch (button) {
        default:
        case System_Windows_MessageBoxButton.OK:
            buttons = [ OK ];
            break;
        case System_Windows_MessageBoxButton.OKCancel:
            buttons = [ OK, Cancel ];
            break;
        case System_Windows_MessageBoxButton.YesNoCancel:
            buttons = [ Yes, No, Cancel ];
            break;
        case System_Windows_MessageBoxButton.YesNo:
            buttons = [ Yes, No ];
            break;
    }

    if (caption)
        caption = "<h3>" + caption + "</h3>";
    else
        caption = "";
    
    if (!window.kine_zIndex)
        window.kine_zIndex = 9000000;

    var small = caption == "" && messageboxText.length <= 40 && buttons.length == 1;
    var modalClass = small ? "smallmodal" : "modal";
    var modal = $('<table class="k-' + modalClass + '-background" style="z-index: ' + (window.kine_zIndex--) + '"><tr>' +
        '<td class="k-' + modalClass + '-wrapper">' +
            '<div class="k-' + modalClass + ' ' + icon + '">' +
                '<div class="k-' + modalClass + '-body">' + caption + '<p>' + messageboxText + '</p></div>' +
                '<div class="k-' + modalClass + '-footer buttons-' + buttons.length + '">' + buttons.join('') + '</div>' +
              '</div>' +
        '</td>' +
    '</tr></table>');

    var handler = function(button) {
        callback($(button).data("result"));
        if (navigator.userAgent.match(/Windows Phone OS|IEMobile/i)) {
            modal.hide(function() {
                modal.remove();
            });
        }
        else {
            modal.fadeOut(function() {
                modal.remove();
            });
        }
    };
    modal.find(".k-" + modalClass + "-footer button").click(function () { handler(this); });
    if (small) {
        modal.click(function() { handler(buttons[0]); });
    }

    modal.hide();
    $("#portrait").append(modal);
    if (navigator.userAgent.match(/Windows Phone OS|IEMobile/i)) {
        modal.show();
    }
    else {
        modal.fadeIn();
    }
}