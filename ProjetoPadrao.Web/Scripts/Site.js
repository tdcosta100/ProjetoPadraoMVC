jQuery(function ($)
{
    $(document).on("click", "a[href='#']", function (event)
    {
        event.preventDefault();
    });

    $(document).on("hidden.bs.modal", ".modal", function (event)
    {
        $(this).removeClass("modal-stack");

        if($(".modal.modal-stack").length)
        {
            $("body").addClass("modal-open");
        }
    });

    $(document).on("show.bs.modal", ".modal", function (event)
    {
        if ($(this).hasClass("modal-stack"))
        {
            return;
        }

        var highestZIndex = 1040;

        $(".modal-stack").each(function ()
        {
            var zIndex = parseInt($(this).css("zIndex"));
            
            if (zIndex > highestZIndex)
            {
                highestZIndex = zIndex;
            }
        });

        $(this).addClass("modal-stack").css("z-index", highestZIndex + 10);

        setTimeout(function ()
        {
            $(".modal-backdrop").not(".modal-backdrop-stack").css("z-index", highestZIndex + 5).addClass("modal-backdrop-stack");
        }, 50);
    });
});

function Modal(options)
{
    var defaults = {
        attributes: {
            "id": null,
            "class": "modal fade",
            "tabindex": -1,
            "role": "dialog",
            "aria-labelledby": null
        },
		title: "",
		content: "",
		initialize: true,
        removeOnDismiss: true,
		backdrop: true,
		keyboard: true,
		show: true,
		buttons: []
	};

	var buttonDefaults = {
	    content: "",
	    attributes: {
	        "id": null,
            "type": "button",
	        "class": "btn btn-primary",
	        "aria-pressed": null,
            "data-dismiss": null,
	        "data-toggle": null,
	        "data-target": null
	    },
	    events: {
	        "click": function (event)
	        {

	        }
	    }
	};

	var settings = $.extend({}, defaults, options);

	var modalElement = $("#modal-template").clone().attr(settings.attributes);

	$(".modal-title", modalElement).html(settings.title);
	$(".modal-body", modalElement).html(settings.content);

	$(settings.buttons).each(function ()
	{
        var buttonSettings = $.extend(true, {}, buttonDefaults, this)

        var button = $("<button />").html(buttonSettings.content).attr(buttonSettings.attributes).on(buttonSettings.events).appendTo($(".modal-footer", modalElement)   );
	});

	if (settings.removeOnDismiss)
	{
	    modalElement.on("hidden.bs.modal", function ()
	    {
	        $(this).remove();
	    });
	}

	if (settings.initialize)
	{
	    modalElement.appendTo("body").modal({
	        backdrop: settings.backdrop,
	        keyboard: settings.keyboard,
            show: settings.show
	    });
	}

	return modalElement;
}

function ModalMessage(title, content, buttonContent)
{
    return Modal({
        title: title,
        content: content,
        buttons: [
            {
                content: buttonContent,
                attributes: {
                    "class": "btn btn-success",
                    "data-dismiss": "modal"
                }
            }
        ]
    });
}

function ModalConfirmCancel(title, content, confirmContent, cancelContent, clickConfirm, clickCancel)
{
    return Modal({
        title: title,
        content: content,
        buttons: [
            {
                content: cancelContent,
                attributes: {
                    "class": "btn btn-danger",
                    "data-dismiss": "modal"
                },
                events: {
                    click: clickCancel || function() {}
                }
            },
            {
                content: confirmContent,
                attributes: {
                    "class": "btn btn-success",
                    "data-dismiss": "modal"
                },
                events: {
                    click: clickConfirm || function() {}
                }
            }
        ]
    });
}