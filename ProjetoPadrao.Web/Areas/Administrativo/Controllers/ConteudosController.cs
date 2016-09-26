using ProjetoPadrao.Dados.DAO;
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
    }
}