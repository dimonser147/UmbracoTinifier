jQuery(document).ready(function($) {
	
	$('.options_toggle').bind('click', function() {
		if($('#panel').css('left') == '0px'){
			$('#panel').stop(false, true).animate({left:'-230px'}, 400, 'easeOutExpo');
		}else {
			$('#panel').stop(false, true).animate({left:'0px'}, 400, 'easeOutExpo');
		}	
	});

	
	$('#accent_color').ColorPicker({
		onSubmit: function(hsb, hex, rgb, el) {
			$(el).val(hex);
			$(el).ColorPickerHide();
		},
		onBeforeShow: function () {
			$(this).ColorPickerSetColor(this.value);
		},
		onChange: function (hsb, hex, rgb) {
			$('#accent_color').val(hex);
			$('#accent_color').css('backgroundColor', '#' + hex);
			accentColorUpdate(hex);
		}
	})
	.bind('keyup', function(){
		$(this).ColorPickerSetColor(this.value);
	});

	
function accentColorUpdate(hex){

	hex = '#'+hex;

	$('#custom_styles').html('<style>'+
		'a, a:focus, a:hover, a:active, .navbar-default .navbar-nav li a:hover, .navbar-default .navbar-nav li a.selected, .navbar-default .navbar-nav .active a, .navbar-default .navbar-nav .dropdown.active a, .navbar-default .navbar-nav .active a:hover, .navbar-default .navbar-nav .dropdown.active a:hover, .navbar-default .navbar-nav .active a:focus, .navbar-default .navbar-nav .dropdown.active a:focus, .icon-counter:hover, .social-network a:hover, .social-network a:focus, .social-network a:active, .pe-feature, .accordion-heading a:hover i, .counter-number, .pricing-head.popular .pricing-price, .validation { color:'+ hex +'; }' +
		'.btn-primary, .btn-primary:hover, .btn-primary:focus, .btn-primary:active, .open > .dropdown-toggle.btn-primary, .subscribe-button, .navbar-default .navbar-toggle:hover .icon-bar, .feature-box:hover .pe-feature, .accordion-heading a i, .accordion-heading a:hover, .flex-direction-nav .flex-next:hover, .flex-direction-nav .flex-prev:hover, .flex-control-paging li a:hover, .flex-control-paging li a.flex-active, .pricing-head.popular, .owl-theme .owl-controls .owl-buttons div.owl-prev:hover, .owl-theme .owl-controls .owl-buttons div.owl-next:hover, #toTopHover, .log-tabs li a, .log-tabs li a:hover, .log-tabs li a:focus, .log-tabs li a:active, .options_toggle_holder { background-color:'+ hex +';}' +
		'.btn-primary, .btn-primary:hover, .btn-primary:focus, .btn-primary:active, .btn-primary.active, .open > .dropdown-toggle.btn-primary, .navbar-default .navbar-nav li a:hover, .navbar-default .navbar-nav li a.selected, .navbar-default .navbar-nav .active a, .navbar-default .navbar-nav .dropdown.active a, .navbar-default .navbar-nav .active a:hover, .navbar-default .navbar-nav .dropdown.active a:hover, .navbar-default .navbar-nav .active a:focus, .navbar-default .navbar-nav .dropdown.active a:focus, .subscribe-button, .testimoni-avatar:hover, .icon-counter:hover, .form-control:focus, ul.listForm li .form-control:focus, .navbar-default .navbar-toggle:hover, .pe-feature, .accordion-heading a i, .accordion-heading a:hover { border-color:'+ hex +';}'+
		'</style>');
}

function bodybgColorUpdate(hex){
	$('body').css('background', '#'+hex);
}
	
});