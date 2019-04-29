$(() => {
    
    $(".categories").on('click', '.category', function () {
        const id = $(this).data('id');
        $.get('/home/index', { id: id });
    });
   
})