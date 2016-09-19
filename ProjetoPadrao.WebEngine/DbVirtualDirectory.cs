using ProjetoPadrao.Dados.Entidades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace ProjetoPadrao.WebEngine
{
    public class DbVirtualDirectory : VirtualDirectory
    {
        private IEnumerable<DbVirtualDirectory> _Directories;
        private IEnumerable<DbVirtualFile> _Files;

        public DbVirtualDirectory(string virtualPath, IEnumerable<DbVirtualDirectory> directories, IEnumerable<DbVirtualFile> files) : base(virtualPath)
        {
            _Directories = directories;
            _Files = files;
        }

        public override IEnumerable Children { get { return _Directories.AsEnumerable<VirtualFileBase>().Concat(_Files.AsEnumerable<VirtualFileBase>()); } }

        public override IEnumerable Directories { get { return _Directories; } }

        public override IEnumerable Files { get { return _Files; } }
    }
}
