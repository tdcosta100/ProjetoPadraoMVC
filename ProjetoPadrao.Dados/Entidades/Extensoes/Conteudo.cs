namespace ProjetoPadrao.Dados.Entidades
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ConteudoMetadata))]
    public partial class Conteudo
    {
    }

    public class ConteudoMetadata
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public Nullable<System.DateTime> DataPublicacao { get; set; }
    }
}
