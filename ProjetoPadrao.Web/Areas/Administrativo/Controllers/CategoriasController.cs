﻿using ProjetoPadrao.Dados.DAO;
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
    }
}