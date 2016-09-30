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
				Util.CacheCaminho.LimparCache();

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

                var pilhaArvore = new Stack<IEnumerator<Categoria>>();

                pilhaArvore.Push((new List<Categoria> { categoria }).GetEnumerator());

                do
                {
                    var currentEnumerator = pilhaArvore.Peek();

                    if (currentEnumerator.MoveNext())
                    {
                        if (currentEnumerator.Current.Subcategorias.Any())
                        {
                            foreach (var subcategoria in currentEnumerator.Current.Subcategorias)
                            {
                                subcategoria.IdIdioma = currentEnumerator.Current.IdIdioma;
                            }

							foreach (var conteudo in currentEnumerator.Current.Conteudos)
							{
								conteudo.IdIdioma = currentEnumerator.Current.IdIdioma;
							}

                            pilhaArvore.Push(currentEnumerator.Current.Subcategorias.GetEnumerator());
                        }
                    }
                    else
                    {
                        pilhaArvore.Pop();
                    }
                } while (pilhaArvore.Count > 0);

                CategoriaDAO.SalvarAlteracoesPendentes();
				Util.CacheCaminho.LimparCache();

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Accepted);
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public ActionResult Organizar(IEnumerable<Models.CategoriaOrganizar> models)
        {
            if (ModelState.IsValid && models != null)
            {
                var lookupModels = models.ToLookup(m => m.IdCategoriaPai);
                var idCategorias = models.Select(m => m.IdCategoria.Value).ToArray();
                var categorias = CategoriaDAO.ListarTracking().Where(c => idCategorias.Contains(c.IdCategoria)).ToDictionary(c => c.IdCategoria, c => c);

                if (categorias.Count == models.Count())
                {
                    var pilhaArvore = new Stack<IEnumerator<Models.CategoriaOrganizar>>();

                    pilhaArvore.Push(lookupModels[null].GetEnumerator());

                    do
                    {
                        var currentEnumerator = pilhaArvore.Peek();

                        if (currentEnumerator.MoveNext())
                        {
                            var subModels = lookupModels[currentEnumerator.Current.IdCategoria];
                            var categoria = categorias[currentEnumerator.Current.IdCategoria.Value];

                            categoria.Ordem = currentEnumerator.Current.Ordem.Value;

                            foreach (var conteudo in categoria.Conteudos)
                            {
                                conteudo.Idioma = categoria.Idioma;
                            }

                            categoria.Subcategorias.Clear();

                            if (subModels.Any())
                            {
                                foreach (var subcategoria in subModels.OrderBy(s => s.Ordem).Select(s => categorias[s.IdCategoria.Value]))
                                {
                                    subcategoria.CategoriaPai = categoria;
                                    subcategoria.Idioma = categoria.Idioma;

                                    categoria.Subcategorias.Add(subcategoria);
                                }

                                pilhaArvore.Push(lookupModels[currentEnumerator.Current.IdCategoria].GetEnumerator());
                            }
                        }
                        else
                        {
                            pilhaArvore.Pop();
                        }
                    } while (pilhaArvore.Count > 0);

                    CategoriaDAO.SalvarAlteracoesPendentes();
					Util.CacheCaminho.LimparCache();

                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Accepted);
                }
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }
    }
}