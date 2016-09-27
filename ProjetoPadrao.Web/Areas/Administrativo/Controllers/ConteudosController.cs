using ProjetoPadrao.Dados.DAO;
using ProjetoPadrao.Dados.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoPadrao.Web.Areas.Administrativo.Controllers
{
    public class ConteudosController : Controller
    {
        public ActionResult Index(int? idCategoria)
        {
            return PartialView(ConteudoDAO.Listar().Where(c => !idCategoria.HasValue || c.IdCategoria == idCategoria).OrderByDescending(c => c.DataPublicacao ?? c.DataCriacao));
        }

        [HttpGet]
        public ActionResult Novo(bool? carregarFormulario, Models.ConteudoNovo model)
        {
            ModelState.Clear();

            return PartialView("NovoEditar", model);
        }

        [HttpPost]
        public ActionResult Novo(Models.ConteudoNovo model)
        {
            if (ModelState.IsValid)
            {
                var conteudo = new Conteudo
                {
                    Titulo = model.Titulo,
                    URL = model.URL,
                    Chamada = model.Chamada,
                    HTML = model.HTML,
                    DataPublicacao = model.DataPublicacao,
                    Ativo = model.Ativo,
                    IdCategoria = model.IdCategoria.Value,
                    IdTemplate = model.IdTemplate.Value,
                    IdArquivoImagemChamada = model.IdArquivoImagemChamada,
                    IdIdioma = model.IdIdioma.Value,
                    GrupoIdioma = new GrupoIdioma
                    {
                        TipoGrupoIdioma = "conteudo"
                    }
                };

                ConteudoDAO.Inserir(conteudo);

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Accepted);
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }
    }
}