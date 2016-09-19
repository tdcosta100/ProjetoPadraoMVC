using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoPadrao.Web.Areas.Administrativo.Models
{
    public class TemplateNovo
    {
        [Required]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Alias")]
        public string Alias { get; set; }

        [Required]
        [AllowHtml]
        [HiddenInput]
        [Display(Name = "Conteúdo")]
        public string HTML { get; set; }

        [Display(Name = "Conteúdo dinâmico (pré-processar no servidor)")]
        public bool ParseHTML { get; set; }

        [HiddenInput]
        [Display(Name = "Miniatura")]
        public int? IdArquivoImagemMiniatura { get; set; }
    }

    public class TemplateEditar : TemplateNovo
    {
        [Required]
        [HiddenInput]
        [Display(Name = "ID do Template")]
        public int IdTemplate { get; set; }
    }
}