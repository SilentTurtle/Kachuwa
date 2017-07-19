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
    var kachuwaForm = function() {

        var init = function() {

            $(".kachuwa-form").find("input:checkbox").on("change",
                function() {
                    console.log(this);
                    $(this).val($(this).is(":checked"));
                });

            $(".kachuwa-tags").tagsinput();
            $(".date").datepicker({ format: 'mm/dd/yyyy'});
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
        }();
    }();

}(jQuery))