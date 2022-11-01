$(function () {

    /**********************************************************************************************************/
    // Select product from specified category 

    $("#SelectCategory").on("change", function () {
        var url = $(this).val();

        if (url) {
            window.location = "/admin/shop/Products?catId=" + url;
        }
        return false;
    });

    /************************************************************************************************************/

    // Preview selected image 

    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $("img#imgpreview")
                    .attr("src", e.target.result)
                    .width(200)
                    .height(200);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    $("#imageUpload").change(function () {
        readURL(this);
    });

    /*****************************************************************************************************************/

    // Confirm page deletion 

    $("a.delete").click(function () {
        if (!confirm("Confirm product deletion")) return false;
    });

});