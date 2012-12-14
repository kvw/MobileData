function Kinepolis_MobileTicketing_ViewModels_ShellViewModel_loaded(view, model) {
    if (navigator.userAgent.match(/MSIE [^1]/i)) {
        setInterval(
            function () {
                var spinner = $(view).find("#busy .spinner");
                spinner.animate({ rotate: '+=359deg' }, 1000);
            },
            1000
        );
    }
}
