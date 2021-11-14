$(function() {

	$(".dest-name, .fromto ").on("click", function() {
	$(".airport-ul").toggle();
	});	
	
	$(".airport-ul").on("click",function(){
		$('.airport-ul').hide();
	});
	

	$(".passenger, .travel-type, .psg-class").on("click", function() {
		$(".passenger-bx").addClass("active");
	  });
	  $(".psg-done").on("click", function() {
		  $(".passenger-bx").removeClass("active");
	  });
	  

	  $('.morefare-btn').on('click', function() {
		$(this).hide();
		$(this).siblings('.hide-fare-btn').show();
		$(this).parent('.select-btn').siblings('.option-price-bx').toggle();
		$(this).parent('.select-btn').siblings('.flight-detail-bx').hide();
		});

		$('.hide-fare-btn').on('click', function() {
		$(this).hide();
		$(this).siblings('.morefare-btn').show();
		$(this).parent('.select-btn').siblings('.option-price-bx').toggle();

		});
		
		$('.flight-detail-btn').on('click', function() {
	   
		$(this).parent('.select-btn').siblings('.flight-detail-bx').toggle();
		$(this).parent('.select-btn').siblings('.option-price-bx').hide();
		$(this).siblings('.hide-fare-btn').hide();
		$(this).siblings('.morefare-btn').show();
		});


		$('.trip-type ul li').on('click', function() {
			$(this).addClass('sel').siblings().removeClass('sel');
		});
		$('.trip-type ul li:nth-child(2)').on('click', function() {
			$('.return .date').css('opacity','1');
		});
});