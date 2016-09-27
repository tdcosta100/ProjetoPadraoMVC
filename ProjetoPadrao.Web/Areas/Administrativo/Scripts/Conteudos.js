jQuery(function ($)
{
    $(document).on("click", ".btn-adicionar-conteudo", function (event)
    {
        event.preventDefault();

        $("#modal-carregando").modal("show");

        $.ajax({
            type: "GET",
            url: Urls.Conteudos.Novo,
            data: {
                carregarFormulario: true,
                IdCategoria: $(this).parents(".modal").data("idCategoria")
            },
            success: function (data, status, xhr)
            {
                configurarFormularioConteudo("novo", data);
            },
            error: function (xhr, status, error)
            {
                ModalMessage("Erro", "Erro ao carregar o formulário do conteúdo.", "Ok");
            },
            complete: function (xhr, status)
            {
                $("#modal-carregando").modal("hide");
            }
        });
    });
});

function itemMenuConteudos(data)
{
    var instance = $.jstree.reference(data.reference);
    var node = instance.get_node(data.reference);

    carregarListaConteudos(node.data.dados.idCategoria, $.trim(node.text));
}

function carregarListaConteudos(idCategoria, nomeCategoria)
{
    $("#modal-carregando").modal("show");

    $.ajax({
        type: "GET",
        url: Urls.Conteudos.Index,
        data: { idCategoria: idCategoria },
        success: function (data, status, xhr)
        {
            configurarListaConteudos(data, idCategoria, nomeCategoria);
        },
        error: function (xhr, status, error)
        {
            ModalMessage("Erro", "Erro ao carregar a lista de conteúdos.", "Ok");
        },
        complete: function (xhr, status)
        {
            $("#modal-carregando").modal("hide");
        }
    });
}

function configurarListaConteudos(dados, idCategoria, nomeCategoria)
{
    if ($("#lista-conteudos").length)
    {
        $("#lista-conteudos .modal-body").html(dados);
    }
    else
    {
        var modal = Modal({
            attributes: {
                id: "#lista-conteudos"
            },
            title: "Conteúdos - <span class=\"nome-categoria\">" + nomeCategoria + "</span>",
            content: dados,
            show: false,
            buttons: [
                {
                    content: "Fechar",
                    attributes: {
                        "class": "btn btn-primary",
                        "data-dismiss": "modal"
                    }
                }
            ]
        });

        $(".modal-dialog", modal).addClass("modal-lg");

        modal.data({ idCategoria: idCategoria }).modal("show");
    }
}

function configurarFormularioConteudo(acao, dados)
{
    var modal = Modal({
        title: (acao == "novo") ? "Adicionar conteúdo" : "Editar conteúdo",
        content: dados,
        removeOnDismiss: false,
        show: false,
        buttons: [
            {
                content: "Cancelar",
                attributes: {
                    "class": "btn btn-danger",
                    "data-dismiss": "modal"
                }
            },
            {
                content: "Salvar",
                attributes: {
                    "class": "btn btn-success"
                },
                events: {
                    "click": function (event)
                    {
                        var modal = $(this).parents(".modal");

                        $("form", modal).submit();
                    }
                }
            }
        ]
    })
    .on("hide.bs.modal", fecharFormularioConteudo)
    .on("hidden.bs.modal", finalizarFormularioConteudo);

    $(".modal-dialog", modal).addClass("modal-lg");

    $.validator.unobtrusive.parse(modal);

    $("textarea", modal).each(function ()
    {
        CKEDITOR.replace(this, {
            on: {
                "change": function (event)
                {
                    event.editor.updateElement();
                }
            }
        });
    });

    $(":input[type=datetime]").datepicker({
        todayBtn: "linked",
        language: "pt-BR",
        autoclose: true,
        todayHighlight: true
    });

    $("#IdCategoria", modal).on("change", function ()
    {
        $(":input[name=IdIdioma]", modal).val($("#IdCategoria optgroup:has(option:selected)", modal).data("idIdioma"));
    });

    $("form", modal).data("validator").settings.submitHandler = function (form)
    {
        salvarFormularioCategoria.apply(modal, [form, acao]);
    };

    modal.modal("show");
}

function salvarFormularioConteudo(formulario, acao)
{
    var modal = this;

    var dados = {};

    $($(formulario).serializeArray()).each(function ()
    {
        if (!(this.name in dados))
        {
            dados[this.name] = this.value;
        }
    });

    $("#modal-carregando").modal("show");

    $.ajax({
        type: "POST",
        url: (acao == "novo") ? Urls.Conteudos.Novo : Urls.Conteudos.Editar,
        data: dados,
        success: function (data, status, xhr)
        {
            var idCategoria = $("#lista-conteudos").data("idCategoria");
            var nomeCategoria = $("#lista-conteudos .modal-title .nome-categoria").html();

            carregarListaConteudos(idCategoria, nomeCategoria);

            modal.data("closeConfirmed", true);
            modal.modal("hide");

            ModalMessage("Sucesso", "<p>Dados salvos com sucesso.</p>", "Ok");

        },
        error: function (xhr, status, error)
        {
            ModalMessage("Erro", "Erro ao salvar os dados do conteúdo.", "Ok");
        },
        complete: function (xhr, status)
        {
            $("#modal-carregando").modal("hide");
        }
    });
}

function fecharFormularioConteudo(event)
{
    var modal = this;

    if (!$(modal).data("closeConfirmed"))
    {
        event.preventDefault();

        ModalConfirmCancel("Confirmar cancelamento", "<p>Deseja realmente cancelar? As alterações serão perdidas.</p>", "Confirmar", "Voltar", function ()
        {
            $(modal).data("closeConfirmed", true);
            $(modal).modal("hide");
        });
    }
}

function finalizarFormularioConteudo()
{
    var modal = this;

    $("form", modal).each(function ()
    {
        var validator = $(this).data("validator");

        if (validator)
        {
            $(":input." + validator.settings.errorClass, this).removeClass(validator.settings.errorClass);
            $(":input." + validator.settings.pendingClass, this).removeClass(validator.settings.pendingClass);
            $(":input." + validator.settings.validClass, this).removeClass(validator.settings.validClass);
            $(".field-validation-error " + validator.settings.errorElement, this).remove();

            $(this).removeData("unobtrusiveValidation");
            validator.destroy();
        }

        for (var i in CKEDITOR.instances)
        {
            if ($(this).has(CKEDITOR.instances[i].element.$))
            {
                CKEDITOR.instances[i].destroy();
            }
        }

        $(":input[type=datetime]", modal).datepicker("destroy");
    });

    $(modal).removeData(["closeConfirmed"]);

    $(modal).remove();
}