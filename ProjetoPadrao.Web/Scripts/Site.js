jQuery(function ($)
{
    $(".modal").on("hidden.bs.modal", function (event)
    {
        $(this).removeClass("modal-stack");

        if($(".modal.modal-stack").length)
        {
            $("body").addClass("modal-open");
        }
    });

    $(".modal").on("show.bs.modal", function (event)
    {
        if ($(this).hasClass("modal-stack"))
        {
            return;
        }

        $(this).addClass("modal-stack").css("z-index", 1040 + 10 * $(".modal-stack").length);

        setTimeout(function ()
        {
            $(".modal-backdrop").not(".modal-backdrop-stack").css("z-index", 1035 + 10 * ($(".modal-backdrop.modal-backdrop-stack").length + 1)).addClass("modal-backdrop-stack");
        }, 50);
    });
});

function Modal(options)
{
	var defaults = {
		id: "",
		title: "",
		backdrop: true,
		keyboard: true,
		show: true,
		content: "",
		buttons: []
	};

	var settings = $.extend({}, defaults, options);
}