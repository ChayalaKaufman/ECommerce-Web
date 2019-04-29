$(() => {
    $("#add-to-cart").on('click', function () {
        const product = {
            productId : $("#add-to-cart").data('id'),
            quantity : $("#quantity option:selected").val()
        }
        $.post('/home/addToCart', product, function (p) {
            $(".modal").modal();
        })
    })

    $(".link").on('click', function () {
        $(".modal").modal('hide');
    })
})