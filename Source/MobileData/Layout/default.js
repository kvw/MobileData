$(window).bind("orientationchange", function () {
    //if (window.orientation === undefined) {
    //    $("body").addClass("portrait");
    //    return;
    //}

    //if (window.orientation == 0) {
    //    $("body").addClass("portrait");
    //}
    //else {
    //    $("body").removeClass("portrait");
    //}

    if (event.type == "orientationchange")
        window.scrollTo(0, window._scrollTop === 1 ? 0 : 1);
});


$(function () {
    if (!navigator.userAgent.match(/iPad|iPhone|iPod|android|webOS|Windows Phone OS|IEMobile/i)) {
        $("body").addClass("desktop");
    }

    var win = window;
    var doc = win.document;

    // If there's a hash, or addEventListener is undefined, stop here
    if (!location.hash && win.addEventListener) {
        //scroll to 1
        window.scrollTo(0, 1);
        win._scrollTop = 1;
        var getScrollTop = function() {
            return win.pageYOffset || doc.compatMode === "CSS1Compat" && doc.documentElement.scrollTop || doc.body.scrollTop || 0;
        };

        //reset to 0 on bodyready, if needed
        var bodycheck = setInterval(function () {
            if (doc.body) {
                clearInterval(bodycheck);
                win._scrollTop = getScrollTop();
                win.scrollTo(0, win._scrollTop === 1 ? 0 : 1);
            }
        }, 15);

        win.addEventListener("load orientationchange", function () {
            setTimeout(function () {
                //at load, if user hasn't scrolled more than 20 or so...
                if (getScrollTop() < 20) {
                    //reset to hide addr bar at onload
                    win.scrollTo(0, win._scrollTop === 1 ? 0 : 1);
                }
            }, 0);
        });
    }
});
