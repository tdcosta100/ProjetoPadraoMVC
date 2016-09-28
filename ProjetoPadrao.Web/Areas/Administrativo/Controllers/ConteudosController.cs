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
            ViewBag.Idiomas = IdiomaDAO.Listar();
            ViewBag.Templates = TemplateDAO.Listar();

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
                    DataCriacao = DateTime.Now,
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

        [HttpGet]
        public ActionResult Editar(int? id)
        {
            if (id.HasValue)
            {
                var conteudo = ConteudoDAO.BuscarPorChave(id.Value);

                if (conteudo != null)
                {
                    var model = new Models.ConteudoEditar
                    {
                        IdConteudo = conteudo.IdConteudo,
                        Titulo = conteudo.Titulo,
                        URL = conteudo.URL,
                        Chamada = conteudo.Chamada,
                        HTML = conteudo.HTML,
                        DataPublicacao = conteudo.DataPublicacao,
                        Ativo = conteudo.Ativo,
                        IdCategoria = conteudo.IdCategoria,
                        IdTemplate = conteudo.IdTemplate,
                        IdArquivoImagemChamada = conteudo.IdArquivoImagemChamada,
                        IdGrupoIdioma = conteudo.IdGrupoIdioma,
                        IdIdioma = conteudo.IdIdioma
                    };

                    ViewBag.Idiomas = IdiomaDAO.Listar();
                    ViewBag.Templates = TemplateDAO.Listar();

                    return PartialView("NovoEditar", model);
                }
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public ActionResult Editar(Models.ConteudoEditar model)
        {
            if (ModelState.IsValid)
            {
                var conteudo = ConteudoDAO.BuscarPorChave(model.IdConteudo.Value);

                if (conteudo != null)
                {
                    conteudo.Titulo = model.Titulo;
                    conteudo.URL = model.URL;
                    conteudo.Chamada = model.Chamada;
                    conteudo.HTML = model.HTML;
                    conteudo.DataModificacao = DateTime.Now;
                    conteudo.DataPublicacao = model.DataPublicacao;
                    conteudo.Ativo = model.Ativo;
                    conteudo.IdCategoria = model.IdCategoria.Value;
                    conteudo.IdTemplate = model.IdTemplate.Value;
                    conteudo.IdArquivoImagemChamada = model.IdArquivoImagemChamada;
                    conteudo.IdIdioma = model.IdIdioma.Value;
                    conteudo.IdGrupoIdioma = model.IdGrupoIdioma.Value;

                    ConteudoDAO.SalvarAlteracoesPendentes();

                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Accepted);
                }
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }
    }
}