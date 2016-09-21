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

        public ActionResult Novo()
        {
            return View();
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
                    IdTemplate = model.IdTemplate,
                    IdIdioma = model.IdIdioma,
                    GrupoIdioma = new GrupoIdioma
                    {
                        TipoGrupoIdioma = "categoria"
                    }
                };

                CategoriaDAO.Inserir(categoria);

                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}