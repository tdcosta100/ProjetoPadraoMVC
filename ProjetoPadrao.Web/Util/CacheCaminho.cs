﻿using ProjetoPadrao.Dados.DAO;
using ProjetoPadrao.Dados.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetoPadrao.Web.Util
{
	public static class CacheCaminho
	{
		private static Dictionary<string, Tuple<int, string>> _CacheCaminho = new Dictionary<string, Tuple<int, string>>();

		public static Tuple<object, string> ObterObjetoCaminho(string caminho)
		{
			var segmentos = new Queue<string>(caminho.ToLower().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
			var caminhoNormalizado = string.Join("/", segmentos);
			var caminhoEncontrado = _CacheCaminho.ContainsKey(caminhoNormalizado);
			Tuple<object, string> resultado = null;

			if (!caminhoEncontrado)
			{
				var categoria = CategoriaDAO.Listar().FirstOrDefault(c => c.Ativa && !c.IdCategoriaPai.HasValue && c.URL == "home-pt-br");

				while (segmentos.Count > 0)
				{
					var segmento = segmentos.Dequeue();

					if (segmentos.Count == 0)
					{
						var conteudo = categoria.Conteudos.FirstOrDefault(c => c.Ativo && c.URL == segmento);

						if (conteudo != null)
						{
							_CacheCaminho[caminhoNormalizado] = new Tuple<int, string>(conteudo.IdConteudo, "conteudo");
							resultado = new Tuple<object, string>(conteudo, "conteudo");
							caminhoEncontrado = true;
						}
						else
						{
							categoria = categoria.Subcategorias.FirstOrDefault(sub => sub.Ativa && sub.URL == segmento);

							if (categoria != null)
							{
								_CacheCaminho[caminhoNormalizado] = new Tuple<int, string>(categoria.IdCategoria, "categoria");
								resultado = new Tuple<object, string>(categoria, "categoria");
								caminhoEncontrado = true;
							}
						}
					}
					else
					{
						categoria = categoria.Subcategorias.FirstOrDefault(sub => sub.Ativa && sub.URL == segmento);
					}

					if (categoria == null || caminhoEncontrado)
					{
						break;
					}
				}
			}

			if (caminhoEncontrado && resultado == null)
			{
				switch (_CacheCaminho[caminhoNormalizado].Item2)
				{
					case "categoria":
						resultado = new Tuple<object, string>(CategoriaDAO.BuscarPorChave(_CacheCaminho[caminhoNormalizado].Item1), "categoria");
						break;
					case "conteudo":
						resultado = new Tuple<object, string>(ConteudoDAO.BuscarPorChave(_CacheCaminho[caminhoNormalizado].Item1), "conteudo");
						break;
					default:
						break;
				}
			}

			return resultado;
		}

        public static string ObterCaminhoObjeto(int idObjeto, string tipoObjeto, bool absoluto = true)
        {
            var resultado = _CacheCaminho.Where(c => c.Value.Item1 == idObjeto && c.Value.Item2 == tipoObjeto).Select(c => c.Key).FirstOrDefault();

            if (resultado == null)
            {
                Categoria categoria = null;
                Stack<string> segmentos = new Stack<string>();

                switch (tipoObjeto)
                {
                    case "categoria":
                        categoria = CategoriaDAO.BuscarPorChave(idObjeto);

                        if (categoria == null)
                        {
                            return null;
                        }
                        break;
                    case "conteudo":
                        var conteudo = ConteudoDAO.BuscarPorChave(idObjeto);

                        if (conteudo != null)
                        {
                            categoria = conteudo.Categoria;
                            segmentos.Push(conteudo.URL);
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    default:
                        break;
                }

                while (categoria != null && categoria.CategoriaPai != null)
                {
                    segmentos.Push(categoria.URL);

                    categoria = categoria.CategoriaPai;
                }

                resultado = string.Join("/", segmentos);

				_CacheCaminho[resultado] = new Tuple<int, string>(idObjeto, tipoObjeto);
            }

            if (resultado != null)
            {
				resultado = string.Format("~/{0}", resultado);

                if (absoluto)
                {
                    var url = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);

                    resultado = url.Content(resultado);
                }
            }

            return resultado;
        }

		public static void LimparCache()
		{
			_CacheCaminho.Clear();
		}
	}
}
