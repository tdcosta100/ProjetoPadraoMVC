jQuery(function ($)
{
    $("#btn-adicionar-categoria").on("click", function ()
    {
        carregarFormularioCategoria("novo");
    });

    $("#btn-organizar-categorias").on("click", function ()
    {
        setTimeout(function ()
        {
            if ($("#btn-organizar-categorias").hasClass("active"))
            {
                $("#btn-adicionar-categoria").prop("disabled", true);
                $("#btn-organizar-categorias").hide();
                $("#btn-organizar-categorias-salvar, #btn-organizar-categorias-cancelar").show();
            }
            else
            {
                $("#btn-adicionar-categoria").prop("disabled", false);
                $("#btn-organizar-categorias").show();
                $("#btn-organizar-categorias-salvar, #btn-organizar-categorias-cancelar").hide();
            }
        }, 0);
    });

    $("#btn-organizar-categorias-cancelar").on("click", function ()
    {
        ModalConfirmCancel("Confirmar cancelamento", "<p>Tem certeza de que deseja cancelar? Todas as alterações serão perdidas.</p>", "Confirmar", "Voltar", function ()
        {
            $("#btn-organizar-categorias").click();
            recriarArvoreCategorias();
        });
    });

    $("#btn-organizar-categorias-salvar").on("click", salvarArvoreCategorias);

    $("#btn-organizar-categorias-salvar, #btn-organizar-categorias-cancelar").hide();

    carregarArvoreCategorias();
});

function carregarArvoreCategorias()
{
    $(".arvore-categorias-template").clone().removeClass("arvore-categorias-template").addClass("arvore-categorias").insertBefore(".arvore-categorias-template").show();

    $(".arvore-categorias").jstree({
        core: {
            check_callback: function (operation, node, node_parent, node_position, more)
            {
                return $("#btn-organizar-categorias").hasClass("active");
            }
        },
        plugins: [
            "contextmenu",
            "dnd",
            "types"
        ],
        contextmenu: {
            items: {
                adicionarSubcategoria: {
                    label: "Adicionar subcategoria",
                    action: function (data)
                    {
                        var instance = $.jstree.reference(data.reference);
                        var node = instance.get_node(data.reference);

                        carregarFormularioCategoria("novo", {
                            IdCategoriaPai: node.data.dados.idCategoria,
                            IdIdioma: node.data.dados.idIdioma
                        });
                    },
                    _disabled: function ()
                    {
                        return $("#btn-organizar-categorias").hasClass("active");
                    }
                },
                editar: {
                    label: "Editar",
                    action: function (data)
                    {
                        var instance = $.jstree.reference(data.reference);
                        var node = instance.get_node(data.reference);

                        carregarFormularioCategoria("editar", {
                            IdCategoria: node.data.dados.idCategoria
                        });
                    },
                    _disabled: function ()
                    {
                        return $("#btn-organizar-categorias").hasClass("active");
                    }
                },
                excluir: {
                    label: "Excluir",
                    action: function (data)
                    {
                        console.log([this, arguments]);
                    },
                    _disabled: function (data)
                    {
                        var instance = $.jstree.reference(data.reference);
                        var node = instance.get_node(data.reference);

                        return $("#btn-organizar-categorias").hasClass("active") || (node.type == "home");
                    }
                },
                conteudos: {
                    label: "Conteúdos",
                    separator_before: true,
                    action: function (data)
                    {
                        if (typeof(itemMenuConteudos) === "function")
                        {
                            itemMenuConteudos.apply(this, arguments);
                        }
                    },
                    _disabled: function (data)
                    {
                        var instance = $.jstree.reference(data.reference);
                        var node = instance.get_node(data.reference);

                        return $("#btn-organizar-categorias").hasClass("active") || (node.type == "home");
                    }
                }
            }
        },
        dnd: {
            copy: false,
            drag_selection: false,
            inside_pos: "last"
        },
        types: {
            "#": {
                max_children: 1,
                valid_children: ["home"]
            },
            home: {
                icon: "glyphicon glyphicon-home",
                valid_children: ["categoria"]
            },
            categoria: {
                icon: "glyphicon glyphicon-folder-close",
                valid_children: ["categoria", "documento"]
            },
            documento: {
                icon: "glyphicon glyphicon-file"
            }
        }
    });
}

function recriarArvoreCategorias()
{
    $(".arvore-categorias").jstree("destroy").remove();

    carregarArvoreCategorias();
}

function salvarArvoreCategorias()
{
    ModalConfirmCancel("Confirmar alterações", "<p>Tem certeza de que deseja salvar? As alterações não poderão ser desfeitas.</p>", "Confirmar", "Voltar", function ()
    {
        var categoriasOrganizar = $(
            $('.arvore-categorias')
            .jstree(true)
            .get_json('#', {
                no_li_attr: true,
                no_a_attr: true,
                flat: true
            })
        )
        .map(function ()
        {
            var categoriaPai = $(".arvore-categorias").jstree(true).get_node(this.parent);

            return {
                IdCategoria: this.data.dados.idCategoria,
                IdCategoriaPai: (categoriaPai.id != "#") ? categoriaPai.data.dados.idCategoria : null,
                Ordem: $.inArray(this.id, categoriaPai.children) + 1
            };
        }).get();

        $("#modal-carregando").modal("show");

        $.ajax({
            type: "POST",
            url: Urls.Categorias.Organizar,
            data: JSON.stringify(categoriasOrganizar),
            contentType: "application/json",
            success: function (data, status, xhr)
            {
                $("#btn-organizar-categorias").click();

                $(".arvore-categorias-template").empty().load(Urls.Categorias.ArvoreCategorias, function (response, status, xhr)
                {
                    recriarArvoreCategorias();
                });
            },
            error: function (xhr, status, error)
            {

            },
            complete: function (xhr, status)
            {
                $("#modal-carregando").modal("hide");
            }
        })
    });
}

function carregarFormularioCategoria(acao, dados)
{
    acao = acao || "novo";
    dados = dados || {};
    
    $("#modal-carregando").modal("show");

    $.ajax({
        type: "GET",
        url: (acao == "novo") ? Urls.Categorias.Novo : Urls.Categorias.Editar,
        data: $.param($.extend({ carregarFormulario: true }, dados)),
        success: function (data, status, xhr)
        {
            configurarFormularioCategoria(data, acao);
        },
        error: function (xhr, status, error)
        {
            ModalMessage("Erro", "Erro ao carregar o formulário da categoria.", "Ok");
        },
        complete: function (xhr, status)
        {
            $("#modal-carregando").modal("hide");
        }
    });
}

function configurarFormularioCategoria(dados, acao)
{
    var modal = Modal({
        title: (acao == "novo") ? "Adicionar categoria" : "Editar categoria",
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
    .on("hide.bs.modal", fecharFormularioCategoria)
    .on("hidden.bs.modal", finalizarFormularioCategoria);

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

    $("#IdCategoriaPai", modal).on("change", function ()
    {
        $(":input[name=IdIdioma]", modal).val($("#IdCategoriaPai optgroup:has(option:selected)", modal).data("idIdioma"));
    });

    $("form", modal).data("validator").settings.submitHandler = function (form)
    {
        salvarFormularioCategoria.apply(modal, [form, acao]);
    };

    modal.modal("show");
}

function salvarFormularioCategoria(formulario, acao)
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
        url: (acao == "novo") ? Urls.Categorias.Novo : Urls.Categorias.Editar,
        data: dados,
        success: function (data, status, xhr)
        {
            $(".arvore-categorias-template").empty().load(Urls.Categorias.ArvoreCategorias, function (response, status, xhr)
            {
                recriarArvoreCategorias();
            });

            modal.data("closeConfirmed", true);
            modal.modal("hide");

            ModalMessage("Sucesso", "<p>Dados salvos com sucesso.</p>", "Ok");

        },
        error: function (xhr, status, error)
        {

        },
        complete: function (xhr, status)
        {
            $("#modal-carregando").modal("hide");
        }
    });
}

function fecharFormularioCategoria(event)
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

function finalizarFormularioCategoria()
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
    });

    $(modal).removeData(["closeConfirmed"]);

    $(modal).remove();
}