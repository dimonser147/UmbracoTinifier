(function($){
	'use strict';
		
	$("a.zoom:first[data-pretty^='prettyPhoto']").prettyPhoto({animation_speed:'normal',theme:'pp_default',slideshow:3000, autoplay_slideshow: false});
	$("a.zoom:gt(0)[data-pretty^='prettyPhoto']").prettyPhoto({animation_speed:'fast',slideshow:10000, hideflash: true});
		
})(jQuery);