$(document).ready(function () {

    $(document).on('click', '.deleteImage', function (e) {

        e.preventDefault();
        let url = $(this).attr('href');
        //let imageId = $(this).attr('data-imageId');

        fetch(url)
            .then(res => {
                if (res.ok) {
                    return res.text()
                } else {
                    alert("Yanlis emeliyyat")
                    return
                }
            })
            .then(data => {
                $('.productImages').html(data)
            })

        /*   console.log(url + "?imageid= " + imageid)*/
    })
})