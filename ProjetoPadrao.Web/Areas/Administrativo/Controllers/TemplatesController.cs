using ProjetoPadrao.Dados.DAO;
using ProjetoPadrao.Dados.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProjetoPadrao.Web.Areas.Administrativo.Controllers
{
    public class TemplatesController : Controller
    {
        public ActionResult Index()
        {
            return View(TemplateDAO.Listar().OrderByDescending(t => t.IdTemplate));
        }

        public ActionResult Novo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Novo(Models.TemplateNovo model)
        {
            if (ModelState.IsValid)
            {
                Template template = new Template
                {
                    Nome = model.Nome,
                    Alias = model.Alias,
                    HTML = model.HTML,
                    ParseHTML = model.ParseHTML,
                    IdArquivoImagemMiniatura = model.IdArquivoImagemMiniatura
                };

                TemplateDAO.Inserir(template);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Editar(int? id)
        {
            if (id.HasValue)
            {
                Template template = TemplateDAO.BuscarPorChave(id.Value);

                if (template != null)
                {
                    var model = new Models.TemplateEditar
                    {
                        IdTemplate = template.IdTemplate,
                        Nome = template.Nome,
                        Alias = template.Alias,
                        HTML = template.HTML,
                        ParseHTML = template.ParseHTML,
                        IdArquivoImagemMiniatura = template.IdArquivoImagemMiniatura
                    };

                    return View(model);
                }
            }

            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Editar(Models.TemplateEditar model)
        {
            if (ModelState.IsValid)
            {
                Template template = TemplateDAO.BuscarPorChave(model.IdTemplate);

                if (template != null)
                {
                    template.Nome = model.Nome;
                    template.Alias = model.Alias;
                    template.HTML = model.HTML;
                    template.ParseHTML = model.ParseHTML;
                    template.IdArquivoImagemMiniatura = model.IdArquivoImagemMiniatura;

                    TemplateDAO.SalvarAlteracoesPendentes();

                    ProjetoPadrao.WebEngine.DbVirtualFileManager.RemoveTemplate(template.IdTemplate);

                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }
    }
}