$(document).ready(function () {

   
   
    $('.accordion-collapse').on('show.bs.collapse', function () {
        $(this).closest("table")
            .find(".accordion-collapse.show")
            .not(this)
            .collapse('toggle');
    })


    $(document).on('click', '.product-item-remove', function (e) {
        e.preventDefault();
        let productId = $(this).attr('data-productId');
        fetch('/Basket/DeleteBasket/' + productId).then(res => {
            return res.text();
        }).then(data => {
            if (data != null) {
                $('.minicart-inner-content').html(data)
                fetch('/basket/GetBasketCount')
                    .then(res => {
                        return res.json();
                    }).then(data => {
                        $('.quantity').text(data);
                    });
                fetch('/Basket/GetBasketForBasket/').then(res => {
                    return res.text();
                }).then(data => {
                    $('.cart-section').html(data)
                    const Toast = Swal.mixin({
                        toast: true,
                        position: 'bottom-end',
                        showConfirmButton: false,
                        timer: 3000,
                        timerProgressBar: false,
                        didOpen: (toast) => {
                            toast.addEventListener('mouseenter', Swal.stopTimer)
                            toast.addEventListener('mouseleave', Swal.resumeTimer)
                        }
                    })

                    Toast.fire({
                        icon: 'success',
                        title: 'Məhsul Səbətdən silindi'
                    })
                })

            }

        })
    })

    

    $('.addToBasket').click(function (e) {
        e.preventDefault();
        let productId = $(this).data('id')

        console.log(productId)

        fetch('/basket/addbasket?id=' + productId)
            .then(res => {
                return res.text();
            }).then(data => {
                console.log(data)

                $('.minicart-inner-content').html(data);
                fetch('/basket/GetBasketCount')
                    .then(res => {
                        return res.json();
                    }).then(data => {
                        $('.quantity').text(data);
                    });
            })
       
    })

    $('.searchInput').keyup(function () {
        let search = $(this).val();
        console.log(search)

        if (search.length >= 3) {
            fetch('product/search?search=' + search)
                .then(res => {
                    return res.text();
                }).then(data => {
                    console.log(data)
                    $('#searchbody').html(data)
                });
        }
        else {
            $('#searchbody').html('')
        }



    })

    $('.addToWishList').click(function (e) {
        e.preventDefault();
        let url = $(this).attr('href');
        fetch(url).then(res => {
            return res.text()
        })
            .then(data => {
                $('.wishListTable').html(data);
            })
    })
    $(document).on('click', '.removeWishList', function (e) {
        e.preventDefault();

        let url = $(this).attr('href');
        console.log(url)
        fetch(url).then(res => {
            return res.text();
        })
            .then(data => {
                $('.wishListTable').html(data);
            })
    })

    $('.rangeFilter').click(function (e) {
        e.preventDefault();

        let val1 = $('#lower').val();
        let val2 = $('#upper').val();
        let val = val1 + "-" + val2
        console.log(val)
        let categoryId = $(this).attr('data-categoryId')
        let pageIndex = $(this).attr('data-pageIndexId')
        

        fetch('/shop/getshopfilter?range=' + val + '&categoryId=' + categoryId + '&pageIndex=' + pageIndex)
            .then(res => {
                
                return res.text();
            })
            .then(data => {
                
                $('.shop-area-section').html(data)
            });

    })

    



    $(document).on('click', '.catogoryfilter', function (e) {
        e.preventDefault();
        let categoryId = $(this).attr('data-categoryId')
        let pageIndex = $(this).attr('data-pageIndexId')
        let priceRange = $(this).attr('data-priceRange')

        console.log(categoryId)

        

        fetch("/shop/getShopFilter?categoryId=" + categoryId + "&pageIndex=" + pageIndex + "&range=" + priceRange)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shopproductlist').html(data);
            });

    })

    $(document).on('click', '.page-link', function (e) {
        e.preventDefault();


        let categoryId = $(this).attr('data-categoryId')
        let pageIndex = $(this).attr('data-pageIndexId')
        let priceRange = $(this).attr('data-priceRange')

        console.log(categoryId)



        fetch("/shop/getShopFilter?categoryId=" + categoryId + "&pageIndex=" + pageIndex + "&range=" + priceRange)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shopproductlist').html(data);
            });
    })


   


    $(".productModal").click(function (e) {
        e.preventDefault();

        let url = $(this).attr('href');
        console.log(url);
        fetch(url).then(res => {
            return res.text();
        })
            .then(data => {
                $('.modal-content').html(data);
                $('.modal-carousel').slick({
                    slidesToShow: 1,
                    arrows: false,
                });
            })

    })

    $(document).on('click', '.addAddress', function (e) {
        e.preventDefault()

        $('.addressContainer').addClass('d-none');
        $('.addressForm').removeClass('d-none');
    })

    $(document).on('click', '.editAddress', function (e) {
        e.preventDefault();
        $('.addressContainer').addClass('d-none');
        $('.editAddressForm').removeClass('d-none');
    })
})

$(document).on('click', '.plus-btn', function () {

    let productId = $(this).attr('data-id')
    console.log(productId)
    fetch("/basket/IncreaseCount?productId=" + productId)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.cart-area').html(data)
        });

})

$(document).on('click', '.minus-btn', function () {

    let productId = $(this).attr('data-id')
    console.log(productId)
    fetch("/basket/DecreaseCount?productId=" + productId)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.cart-area').html(data)
        });



})


$(document).on('click', '.removeProdInCart', function () {
    

    let productId = $(this).attr('data-id');
    console.log(productId)
    fetch("/basket/DeleteCart?id="  +productId).then(res => {
        return res.text();
    })
        .then(data => {
            $('.cart-area').html(data);
        })
})