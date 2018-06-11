(function($) {
	'use strict';
	jQuery('.appear').appear();
	var runOnce = true;
	jQuery(".stats").on("appear", function(data) {
		var counters = {};
		var i = 0;
		if (runOnce){
			jQuery('.counter-number').each(function(){
				counters[this.id] = $(this).html();
				i++;
			});
			jQuery.each( counters, function( i, val ) {
			//console.log(i + ' - ' +val);
				jQuery({countNum: 0}).animate({countNum: val}, {
					duration: 3000,
					easing:'linear',
					step: function() {
						jQuery('#'+i).text(Math.floor(this.countNum));
					}
				});
			});
			runOnce = false;
		}
	});
})(jQuery);