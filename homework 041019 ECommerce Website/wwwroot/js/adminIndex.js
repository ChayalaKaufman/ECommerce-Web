$(() => {


    $("#add-product").on('click', function () {

        $("#product-modal").modal('hide');
    });

    //function isFormValid() {
    //    const productName = $("#product-name").val();
    //    const description = $("#description").val();
    //    const price = $("#price").val();
    //    const categoryId = $(".category-select option:selected").val();
    //    return productName && description && price && categoryId !== "0";
    //}

    //function SetButtonValidity() {
    //    const productName = $("#product-name").val();
    //    const description = $("#description").val();
    //    const price = $("#price").val();
    //    const categoryId = $(".category-select option:selected").val();
    //    $("#add-product").prop('disabled', !isFormValid());
    //}

    $("#add-prod").on('click', function () {
        $("#product-name").val('');
        $("#description").val('');
        $("#price").val('');
        //**make default selection
        //$("#category-select").select(0);
        $("#product-modal").modal();
    });
    
    //$(".input").on('keyup', setButtonValidity);
    //$("#category-select").on('change', SetButtonValidity);
    
});