$(() => {

    $("#quantity").on('change', function () {
        const quantity = $("#quantity option:selected").val();
        const productId = $("#quantity").data('product-id');

        $.post('/home/editCart', { quantity, productId }, function () {
            clearCartAndPopulate();
        })
    })

    function clearCartAndPopulate() {
        $(".cart").remove();
        $(".original").append('<div class="container cart"></div>')
        $.get('/home/getCartAjax', function (result) {
            result.forEach(p => {
                $(".cart").append(`<div class="row">
                <div class="col-md-9">
                    <div class="col-sm-4 col-lg-4 col-md-4">
                        <div class="thumbnail">
                            <img src="/images/uploads/${p.product.image}" style="width: 800px;" alt="">
                            <div class="caption">
                                <h4 class="pull-right">$${p.product.price}</h4>
                                <h4><a href="/home/viewProduct?id=${p.product.id}">${p.product.name}</a></h4>
                                <h4>quantity: ${p.quantity}</h4>
                                    <h4>select new quantity</h4>
                                    <select id="quantity" data-product-id="${p.product.id}" name="quantity">
                                        <option value="1">1</option>
                                        <option value="2">2</option>
                                        <option value="3">3</option>
                                        <option value="4">4</option>
                                        <option value="5">5</option>
                                        <option value="6">6</option>
                                        <option value="7">7</option>
                                        <option value="8">8</option>
                                        <option value="9">9</option>
                                        <option value="10">10</option>
                                    </select>
                            </div>
                            <h4>Total:${((p.quantity * p.product.price).ToString("C"))}</h4>
                            <button class="btn-link remove" data-product-id="${p.product.id}">Remove</button>
                        </div>
                    </div>
                </div>
            </div>`);
            });
        });
    }

    $(".cart").on('click', '.remove', function () {
        console.log('hi');
        const productId = $(this).data('product-id');
        $.post('/home/RemoveFromCart', {productId: productId}, function () {
            clearCartAndPopulate();
        });
    })

})