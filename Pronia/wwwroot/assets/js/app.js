//count
$(document).ready(function() {
  






  
$('.main-slider-active').slick({
  fade: true,
  autoplay: true,
  speed: 3000,
  arrows:false,
  
});

$('.owl-carousel').slick({
  slidesToShow: 4,
  arrows:false,
});



$('.testimonial-carousel').slick({
  slidesToShow: 4,
  arrows:false,
})

$('.brand-carousel').slick({
  autoplay: true,
  speed: 1000,
  slidesToShow: 5,
  slidesToScroll: 1,
  focusOnSelect: true,
  centerMode:false,
  infinite:true,
  arrows:false
  
});
$('.blog-carousel').slick({
  slidesToShow: 3,
  arrows:false,
});

//$('.modal-carousel').slick({
//  slidesToShow: 1,
//  arrows:false,
//});

$('.count').counterUp({
  delay:10,
  time:1200
});

});

$(document).ready(function(){
  $('.slider-for').slick({
      slidesToShow: 1,
      slidesToScroll: 1,
      arrows: false,
      fade: true,
      asNavFor: '.slider-nav'
    });
    $('.slider-nav').slick({
      slidesToShow: 3,
      slidesToScroll: 1,
      asNavFor: '.slider-for',
      centerMode: true,
      focusOnSelect: true,
      centerMode:false,
      infinite:true,
      arrows:false,
      
    });
  });

//search
  $(document).ready(function() {
    $("#search-icon").click(function() {
      $("#search-modal").fadeIn();
    });
  
    $("#close").click(function() {
      $("#search-modal").fadeOut();
    });
  });
  
  $(document).ready(function() {
    $('.productModal').click(function() {
      $('.modal').modal('show');
    });

    
  });

  
  //range filter
  var lowerSlider = document.querySelector('#lower');
  var  upperSlider = document.querySelector('#upper');
  
  document.querySelector('#two').value=upperSlider.value;
  document.querySelector('#one').value=lowerSlider.value;
  
  var  lowerVal = parseInt(lowerSlider.value);
  var upperVal = parseInt(upperSlider.value);
  
  upperSlider.oninput = function () {
      lowerVal = parseInt(lowerSlider.value);
      upperVal = parseInt(upperSlider.value);
  
      if (upperVal < lowerVal + 4) {
          lowerSlider.value = upperVal - 4;
          if (lowerVal == lowerSlider.min) {
          upperSlider.value = 4;
          }
      }
      document.querySelector('#two').value=this.value
  };
  
  lowerSlider.oninput = function () {
      lowerVal = parseInt(lowerSlider.value);
      upperVal = parseInt(upperSlider.value);
      if (lowerVal > upperVal - 4) {
          upperSlider.value = lowerVal + 4;
          if (upperVal == upperSlider.max) {
              lowerSlider.value = parseInt(upperSlider.max) - 4;
          }
      }
      document.querySelector('#one').value=this.value
  };







 






  
 

