var kachuwagrid;
(function ($) {

    var kachuwaGrid = function () {
        var ajaxCall = function (url, param, successFx, error) {
            $.ajax({
                type: "POST",
                //contentType: "application/json; charset=utf-8",
                async: false,
                url: url,
                data: param,
                success: successFx,
                error: error
            });
        };
        var commands = [];

        $(".kachuwa-grid").find("tbody>tr").each(function (i, x) {
            var $tr = $(x);
            $tr.data('kachuwa-item', $tr.data('item'));

        });
        //$(".kachuwa-grid").each(function(index, item) {
        //    //attaching commands events
        //    var commands = $(item).find("a.command-link");
        //    commands.each(function(i, c) {
        //        $(c).on("click",
        //            function() {

        //            });
        //    });

        //});

        kachuwagrid = {};

        kachuwagrid.delete = function (elem, options) {

            if (confirm("Are you sure want to delete?")) {
                var api = $(elem).attr('callbackApi');
                if (typeof (options) == 'object') {
                    alert('please extend the  kachuwagrid.delete api');
                } else {
                    ajaxCall(api,
                        { id: options },
                        function (response) {

                            window.location.reload();

                        },
                        function (response) { });
                }

            }
        }


    }();
    var kachuwaForm = function () {

        var init = function () {
            $.getScript("/resources/tinymce/tinymce.js",
                function (data, textStatus, jqxhr) {
                    console.log(data); // Data returned
                    console.log(textStatus); // Success
                    console.log(jqxhr.status); // 200
                    console.log("tinemcy loaded.");
                    tinymce.init({
                        selector: ".editor", //'#txtQuestion',  // change this value according to your HTML
                        plugins: [
                            'advlist autolink link image lists charmap print preview hr anchor pagebreak spellchecker',
                            'searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking',
                            'save table contextmenu directionality emoticons template paste textcolor'
                        ],
                        setup: function (ed) {
                            ed.on('change', function (e) {
                                $("#" + tinyMCE.activeEditor.id).html(ed.getContent());
                            });
                        },
                        cleanup: false,
                        valid_elements: '+*[*]',
                        entity_encoding: "raw",
                        extended_valid_elements: '@[name|params|source|page|pagesize|rowtotal|api|component],component[name|params],markdown[source],pagination[page|pagesize|rowtotal|api],plugin[component|name|params]',
                        custom_elements: 'component,markdown,module,pagination,plugin,theme',
                        // content_css: '/Themes/Shared/M2/css/theme.min.css',
                        toolbar:
                            'fontsizeselect | insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image | print preview media fullpage | forecolor backcolor emoticons',
                        fontsize_formats: '8pt 10pt 12pt 14pt 18pt 24pt 36pt',
                        file_picker_callback: function (callback, value, meta) {
                            if (meta.filetype == 'image')
                                $('#files').click();
                        }
                    });

                    $('#files').change(function () {
                        var fileUpload = $("#files").get(0);
                        var files = fileUpload.files;
                        var data = new FormData();
                        for (var i = 0; i < files.length; i++) {
                            data.append(files[i].name, files[i]);
                            data.append("fdr", "editor");
                        }

                        $.ajax({
                            url: "/api/v1/file/ajaxupload",
                            type: "POST",
                            data: data,
                            processData: false,
                            contentType: false,
                            success: function (response) {
                                var message = response.Data;
                                var img = "<img style = 'height:80px;width:80px' src = '" + message.savedFilePath + "' />";
                                tinyMCE.activeEditor.execCommand("mceInsertContent", true, img);
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                //if fails
                            }
                        });
                    });
                });
            $(".kachuwa-form").find("input:checkbox").on("change",
                function () {
                    console.log(this);
                    $(this).val($(this).is(":checked"));
                });
            //$.getScript("/resources/", function (data, textStatus, jqxhr) {
            //    console.log(data); // Data returned
            //    console.log(textStatus); // Success
            //    console.log(jqxhr.status); // 200
            //    console.log("Load was performed.");
            //});
            if ($(".kachuwa-tags").length > 0)
                $(".kachuwa-tags").tagsinput();
            if ($(".date").length > 0)
                $(".date").datepicker({ format: 'mm/dd/yyyy' });
            if ($(".datetimepicker").length > 0)
                $(".datetimepicker").datetimepicker({
                    //locale: 'ru',
                    //icons: {
                    //    time: "fa fa-clock-o",
                    //    date: "fa fa-calendar",
                    //    up: "fa fa-arrow-up",
                    //    down: "fa fa-arrow-down"
                    //},
                    // viewMode: 'years'
                });
            // "<script src='~/resources/tinymce/tinymce.js'></script>
           
        }();
    }();

}(jQuery))