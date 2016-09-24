using ProjetoPadrao.Dados.DAO;
using ProjetoPadrao.Dados.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoPadrao.Web.Areas.Administrativo.Controllers
{
    public class CategoriasController : Controller
    {
        public ActionResult Index()
        {
            return View(CategoriaDAO.Listar());
        }

        public ActionResult ArvoreCategorias()
        {
            var lookupCategorias = CategoriaDAO.Listar().ToLookup(c => c.IdCategoriaPai);

            foreach (var categoria in CategoriaDAO.Listar().ToList())
            {
                categoria.Subcategorias = lookupCategorias[categoria.IdCategoria].OrderBy(c => c.Ordem).ToList();
            }

            var arvoreCategorias = lookupCategorias[null].OrderBy(c => c.Ordem);

            return PartialView(arvoreCategorias);
        }

		[HttpGet]
        public ActionResult Novo(bool? carregarFormulario, Models.CategoriaNovo model)
        {
            ViewBag.Idiomas = IdiomaDAO.Listar();
            ViewBag.Templates = TemplateDAO.Listar();

			ModelState.Clear();

            return PartialView("NovoEditar", model);
        }

        [HttpPost]
        public ActionResult Novo(Models.CategoriaNovo model)
        {
            if (ModelState.IsValid)
            {
                Categoria categoria = new Categoria
                {
                    Nome = model.Nome,
                    URL = model.URL,
                    HTML = model.HTML,
                    Ativa = model.Ativa,
                    IdCategoriaPai = model.IdCategoriaPai,
                    IdTemplate = model.IdTemplate.Value,
                    IdIdioma = model.IdIdioma.Value,
                    Ordem = CategoriaDAO.Listar().Where(c => c.IdCategoriaPai == model.IdCategoriaPai).Count() + 1,
                    GrupoIdioma = new GrupoIdioma
                    {
                        TipoGrupoIdioma = "categoria"
                    }
                };

                CategoriaDAO.Inserir(categoria);

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Accepted);
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }

        [HttpGet]
        public ActionResult Editar(int? IdCategoria)
        {
            if (IdCategoria.HasValue)
            {
                var categoria = CategoriaDAO.BuscarPorChave(IdCategoria.Value);

                if (categoria != null)
                {
                    var model = new Models.CategoriaEditar
                    {
                        IdCategoria = categoria.IdCategoria,
                        Nome = categoria.Nome,
                        URL = categoria.URL,
                        HTML = categoria.HTML,
                        Ativa = categoria.Ativa,
                        IdCategoriaPai = categoria.IdCategoriaPai,
                        IdTemplate = categoria.IdTemplate,
                        IdGrupoIdioma = categoria.IdGrupoIdioma,
                        IdIdioma = categoria.IdIdioma
                    };

                    ModelState.Clear();

                    ViewBag.Idiomas = IdiomaDAO.Listar();
                    ViewBag.Templates = TemplateDAO.Listar();

                    return PartialView("NovoEditar", model);
                }
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public ActionResult Editar(Models.CategoriaEditar model)
        {
            if (model.IdCategoria.HasValue)
            {
                var categoria = CategoriaDAO.BuscarPorChave(model.IdCategoria.Value);

                categoria.Nome = model.Nome;
                categoria.URL = model.URL;
                categoria.HTML = model.HTML;
                categoria.Ativa = model.Ativa;
                categoria.IdCategoriaPai = model.IdCategoriaPai;
                categoria.IdTemplate = model.IdTemplate.Value;
                categoria.IdGrupoIdioma = model.IdGrupoIdioma.Value;
                categoria.IdIdioma = model.IdIdioma.Value;

                CategoriaDAO.SalvarAlteracoesPendentes();

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Accepted);
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public ActionResult Organizar(IEnumerable<Models.CategoriaOrganizar> models)
        {
            if (ModelState.IsValid && models != null)
            {
                var lookupModels = models.ToLookup(m => m.IdCategoria);
                var idCategorias = lookupModels.Where(m => m.Key.HasValue).Select(m => m.Key.Value).ToArray();

                var categorias = CategoriaDAO.ListarTracking().Where(c => idCategorias.Contains(c.IdCategoria));

                if (categorias.Count() == models.Count())
                {
                    foreach (var categoria in categorias.ToList())
                    {
                        var model = lookupModels[categoria.IdCategoria].FirstOrDefault();

                        categoria.IdCategoriaPai = model.IdCategoriaPai;
                        categoria.Ordem = model.Ordem.Value;
                    }

                    CategoriaDAO.SalvarAlteracoesPendentes();

                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Accepted);
                }
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }
    }
}