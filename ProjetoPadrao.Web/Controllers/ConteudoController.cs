using ProjetoPadrao.Dados.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoPadrao.Web.Controllers
{
    public class ConteudoController : Controller
    {
        public ActionResult Index(string path)
        {
			var categoria = CategoriaDAO.Listar().FirstOrDefault(c => !c.IdCategoriaPai.HasValue && c.URL == "home-pt-br");

			if (path == null)
			{
				return View(string.Format("Templates/{0}", categoria.Template.Alias), categoria);
			}
			else
			{
				var segments = new Stack<string>(path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Reverse());

				while (segments.Count > 0)
				{
					if (segments.Count == 1)
					{
						var conteudo = categoria.Conteudos.FirstOrDefault(c => c.URL == segments.Peek());

						if (conteudo != null)
						{
							return View(string.Format("Templates/{0}", conteudo.Template.Alias), conteudo);
						}
						else
						{
							categoria = categoria.Subcategorias.FirstOrDefault(sub => sub.URL == segments.Pop());

							if (categoria != null)
							{
								return View(string.Format("Templates/{0}", categoria.Template.Alias), categoria);
							}
						}
					}
					else
					{
						categoria = categoria.Subcategorias.FirstOrDefault(sub => sub.URL == segments.Pop());
					}

					if (categoria == null)
					{
						break;
					}
				}
			}

            return HttpNotFound();
        }
    }
}