(function($) {

    var kachuwaGrid = function() {

    	var commands = [];



		$(".kachuwa-grid").each(function(index, item) {
		    //attaching commands events
			var commands = $(item).find("a.command-link");
			commands.each(function(i, c) {
			    $(c).on("click", function() {
			        
			    })
			})

		})

    }();


}(jQuery))