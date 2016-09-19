using ProjetoPadrao.Dados.Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace ProjetoPadrao.WebEngine
{
    public class DbVirtualFile : VirtualFile
    {
        private Template _Template;
        private DateTime _CreationDate;

        public DbVirtualFile(string virtualPath, Template template) : base(virtualPath)
        {
            _Template = template;
            _CreationDate = DateTime.Now;
        }

        public override Stream Open()
        {
            Stream stream = new MemoryStream();

            StreamWriter writer = new StreamWriter(stream);
            writer.Write(_Template.HTML);
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public Template Template { get { return _Template; } }

        public DateTime CreationDate { get { return _CreationDate; } }
    }
}
