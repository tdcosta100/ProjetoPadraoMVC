using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoPadrao.Web.Areas.Administrativo.Models
{
    public class ConteudoNovo
    {
        [Required]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required]
        [Display(Name = "URL")]
        public string URL { get; set; }

        [Display(Name = "Chamada")]
        public string Chamada { get; set; }

        [Display(Name = "Imagem da Chamada")]
        public int? IdArquivoImagemChamada { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Conteúdo")]
        public string HTML { get; set; }

        [Display(Name = "Data de Publicação")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? DataPublicacao { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }

        [Required]
        [Display(Name = "Categoria")]
        public int? IdCategoria { get; set; }

        [Required]
        [Display(Name = "Template")]
        public int? IdTemplate { get; set; }

        [Required]
        [Display(Name = "Idioma")]
        public int? IdIdioma { get; set; }
    }

    public class ConteudoEditar : ConteudoNovo
    {
        [Required]
        [Display(Name = "ID do Conteúdo")]
        public int? IdConteudo { get; set; }

        [Required]
        [Display(Name = "ID do Grupo de Idiomas")]
        public int? IdGrupoIdioma { get; set; }
    }
}