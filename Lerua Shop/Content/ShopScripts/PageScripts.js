﻿$(function () {
    
    //sorting script
    $("table#pages tbody").sortable(
        {
            items: "tr:not(.home)",
            placeholder: "ui-state-highlight",
            update: function () {
                var ids = $("table#pages tbody").sortable("serialize");
                var url = "/Admin/Pages/ReorderPages";

                $.post(url, ids, function (data) {
                });
            }

        });
});