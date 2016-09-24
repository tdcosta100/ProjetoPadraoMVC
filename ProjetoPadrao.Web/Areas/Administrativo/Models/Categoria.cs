using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoPadrao.Web.Areas.Administrativo.Models
{
    public class CategoriaNovo
    {
        [Required]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "URL")]
        public string URL { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Conteúdo")]
        public string HTML { get; set; }

        [Required]
        [Display(Name = "Ativa")]
        public bool Ativa { get; set; }

		[Required]
		[Display(Name = "Categoria Pai")]
        public int? IdCategoriaPai { get; set; }

        [Required]
        [Display(Name = "Template")]
        public int? IdTemplate { get; set; }

        [Required]
        [Display(Name = "Idioma")]
        public int? IdIdioma { get; set; }
    }

    public class CategoriaEditar : CategoriaNovo
    {
        [Required]
        [HiddenInput]
        [Display(Name = "ID da Categoria")]
        public int? IdCategoria { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "ID do Grupo de Idiomas")]
        public int? IdGrupoIdioma { get; set; }
    }

    public class CategoriaOrganizar
    {
        [Required]
        public int? IdCategoria { get; set; }

        public int? IdCategoriaPai { get; set; }

        [Required]
        public int? Ordem { get; set; }
    }
}