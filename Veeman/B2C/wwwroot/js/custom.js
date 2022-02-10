$(function() {

	$(".dest-name, .fromto ").on("click", function() {
	$(".airport-ul").toggle();
	});	
	
	$(".airport-ul").on("click",function(){
		$('.airport-ul').hide();
	});
	

	  


	
		
		//$('.trip-type ul li').on('click', function() {
		//	$(this).addClass('sel').siblings().removeClass('sel');
		//});
		//$('.trip-type ul li:nth-child(2)').on('click', function() {
		//	$('.return .date').css('opacity','1');
		//});
});