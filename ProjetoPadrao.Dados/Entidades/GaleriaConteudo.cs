//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjetoPadrao.Dados.Entidades
{
    using System;
    using System.Collections.Generic;
    
    public partial class GaleriaConteudo
    {
        public int IdGaleria { get; set; }
        public int IdConteudo { get; set; }
        public int Ordem { get; set; }
    
        public virtual Conteudo Conteudo { get; set; }
        public virtual Galeria Galeria { get; set; }
    }
}
