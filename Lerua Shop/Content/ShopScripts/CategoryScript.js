$(function () {
    /************************************************************************************ */
    //// add new category
    var newCatA = $("a#newcata"); /*Class add link*/
    var newCatTextInput = $("#newcatname"); /*class text input*/
    var ajaxText = $("span.ajax-text"); /*class load image*/
    var table = $("table#pages tbody"); /*class output table*/

    /* catch press key Enter */
    newCatTextInput.keyup(function (e) {
        if (e.keyCode == 13) {
            newCatA.click();
        }
    });

    /* Click function */
    newCatA.click(function (e) {
        e.preventDefault();

        var catName = newCatTextInput.val();

        if (catName.length < 3) {
            alert("Category name must be at least 3 characters long.");
            return false;
        }

        ajaxText.show();

        var url = "/admin/shop/AddNewCategory";

        $.post(url, { catName: catName }, function (data) {
            var response = data.trim();

            if (response == "titletaken") {
                ajaxText.html("<span class='alert alert-danger'>That title is taken!</span>");
                setTimeout(function () {
                    ajaxText.fadeOut("fast", function () {
                        ajaxText.html("<img src='/Content/images/ajax-loader.gif' height='50' />");
                    });
                }, 2000);
                return false;
            }
            else {
                if (!$("table#pages").length) {
                    location.reload();
                }
                else {
                    ajaxText.html("<span class='alert alert-success'>The category has been added!</span>");
                    setTimeout(function () {
                        ajaxText.fadeOut("fast", function () {
                            ajaxText.html("<img src='/Content/img/ajax-loader.gif' height='50' />");
                        });
                    }, 2000);

                    newCatTextInput.val("");

                    var toAppend = $("table#pages tbody tr:last").clone();
                    toAppend.attr("id", "id_" + data);
                    toAppend.find("#item_Name").val(catName);
                    toAppend.find("a.delete").attr("href", "/admin/shop/DeleteCategory/" + data);
                    table.append(toAppend);
                    table.sortable("refresh");
                }
            }
        });
    });

    /*************************************************************************************************************/

    //sorting script
    $("table#pages tbody").sortable(
        {
            items: "tr:not(.home)",
            placeholder: "ui-state-highlight",
            update: function () {
                var ids = $("table#pages tbody").sortable("serialize");
                var url = "/Admin/Shop/ReorderCategories";

                $.post(url, ids, function (data) {
                });
            }

        });

    /**************************************************************************************************************/

    //// confirm category deletion
    $("body").on("click", "a.delete", function () {
        if (!confirm("Confirm category deletion")) return false;
    });

    /*************************************************************************************************************/
});