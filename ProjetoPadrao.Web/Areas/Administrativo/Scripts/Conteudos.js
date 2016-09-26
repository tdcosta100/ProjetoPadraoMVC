jQuery(function ($)
{
    $(document).on("click", ".btn-adicionar-conteudo", function ()
    {

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
            configurarListaConteudos(data, nomeCategoria);
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

function configurarListaConteudos(dados, nomeCategoria)
{
    var modal = Modal({
        title: "Conteúdos - " + nomeCategoria,
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

    modal.modal("show");
}
