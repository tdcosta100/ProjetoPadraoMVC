using ProjetoPadrao.Dados.DAO;
using ProjetoPadrao.Dados.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoPadrao.Web.Controllers
{
    public class ConteudoController : Controller
    {
		private static Dictionary<string, Tuple<int, string>> _PathCache = new Dictionary<string, Tuple<int, string>>();

		public ActionResult Index(string path)
        {
			if (path == null)
			{
				var categoria = CategoriaDAO.Listar().FirstOrDefault(c => c.Ativa && !c.IdCategoriaPai.HasValue && c.URL == "home-pt-br");

				return View(string.Format("Templates/{0}", categoria.Template.Alias), categoria);
			}
			else
			{
				var objetoCaminho = Util.CacheCaminho.ObterObjetoCaminho(path);

				if (objetoCaminho != null)
				{
					switch (objetoCaminho.Item2)
					{
						case "categoria":
							var categoria = objetoCaminho.Item1 as Categoria;
							return View(string.Format("Templates/{0}", categoria.Template.Alias), categoria);
						case "conteudo":
							var conteudo = objetoCaminho.Item1 as Conteudo;
							return View(string.Format("Templates/{0}", conteudo.Template.Alias), conteudo);
						default:
							break;
					}
				}
			}

            return HttpNotFound();
        }
    }
}