'use strict';
app.controller('TinyMceController', function ($scope) {
    $scope.tinymceModel = 'Initial content';

    $scope.getContent = function () {
        console.log('Editor content:', $scope.tinymceModel);
    };

    $scope.setContent = function () {
        $scope.tinymceModel = 'Time: ' + (new Date());
    };

    $scope.tinymceOptions = {
        height: 400,
        width: 950,
        fontsize_formats: "8pt 9pt 10pt 11pt 12pt 26pt 36pt",
        plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars code fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor colorpicker textpattern"
        ],
        toolbar1: "styleselect | fontselect | fontsizeselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | forecolor backcolor | link image media print ",

        image_advtab: true,
        file_browser_callback: RoxyFileBrowser,
        menubar: true,
        statusbar: true,
        media_strict: true,
        convert_urls: false,
        media_types: "flash=swf;shockwave=dcr;qt=mov,qt,mpg,mp3,mp4,mpeg;shockwave=dcr;wmp=avi,wmv,wm,asf,asx,wmx,wvx;rmp=rm,ra,ram",
    };

    function RoxyFileBrowser(field_name, url, type, win) {
        var roxyFileman = 'app/views/Common/fileman/index.html?integration=tinymce4';

        var cmsURL = roxyFileman;  // script URL - use an absolute path!
        if (cmsURL.indexOf("?") < 0) {
            cmsURL = cmsURL + "?type=" + type;
        }
        else {
            cmsURL = cmsURL + "&type=" + type;
        }
        cmsURL += '&input=' + field_name + '&value=' + win.document.getElementById(field_name).value;
        tinyMCE.activeEditor.windowManager.open({
            file: cmsURL,
            title: 'Roxy File Browser',
            width: 850, // Your dimensions may differ - toy around with them!
            height: 650,
            resizable: "yes",
            plugins: "media",
            inline: "yes", // This parameter only has an effect if you use the inlinepopups plugin!
            close_previous: "no"
        }, {
            window: win,
            input: field_name
        });
        return false;
    }
});