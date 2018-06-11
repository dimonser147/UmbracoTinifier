(function($) {
	'use strict';
      $('.flexslider').flexslider({
        animation: "slide",
		directionNav: true,
		controlNav: true,
		slideshow: true, 
      });
	  
      $('.imac-device').flexslider({
        animation: "slide",
		directionNav: false,
		controlNav: false,
		slideshow: true, 
		pausePlay: true, 
		mousewheel: true,
      });
})(jQuery);