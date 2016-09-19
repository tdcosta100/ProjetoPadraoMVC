using ProjetoPadrao.Dados.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ProjetoPadrao.WebEngine
{
    public sealed class DbVirtualFileManager
    {
        private static readonly Lazy<DbVirtualFileManager> _LazyInstance = new Lazy<DbVirtualFileManager>(() => new DbVirtualFileManager());

        private Dictionary<string, DbVirtualFile> _Files;

        private Dictionary<string, DbVirtualDirectory> _Directories;

        public static DbVirtualFileManager Instance { get { return _LazyInstance.Value; } }

        private DbVirtualFileManager()
        {
            _Files = new Dictionary<string, DbVirtualFile>();
            _Directories = new Dictionary<string, DbVirtualDirectory>();
        }

        public static DbVirtualFile GetVirtualFile(string virtualPath)
        {
            string correctedVirtualPath = VirtualPathUtility.ToAppRelative(virtualPath);

            if (!Instance._Files.ContainsKey(correctedVirtualPath))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(virtualPath, @"^~/Views/Shared/Templates/(\w+).cshtml"))
                {
                    using (var bancoDados = new ProjetoPadrao.Dados.Entidades.ProjetoPadrao())
                    {
                        var templateName = System.Text.RegularExpressions.Regex.Replace(correctedVirtualPath, @"^~/Views/Shared/Templates/(\w+).cshtml", "$1");

                        Template template = bancoDados.Templates.AsNoTracking().FirstOrDefault(t => t.Alias == templateName);

                        if (template != null)
                        {
                            Instance._Files.Add(correctedVirtualPath, new DbVirtualFile(correctedVirtualPath, template));
                        }
                    }
                }
            }

            return Instance._Files.ContainsKey(correctedVirtualPath) ? Instance._Files[correctedVirtualPath] : null;
        }

        public static DbVirtualDirectory GetVirtualDirectory(string virtualDir)
        {
            string correctedVirtualDir = VirtualPathUtility.ToAppRelative(virtualDir);

            if (!Instance._Directories.ContainsKey(correctedVirtualDir))
            {
                if (correctedVirtualDir == "~/Views/Shared/Templates/")
                {
                    Instance._Directories.Add(correctedVirtualDir, new DbVirtualDirectory(correctedVirtualDir, new List<DbVirtualDirectory>(), Instance._Files.Values));
                }

            }

            return Instance._Directories.ContainsKey(correctedVirtualDir) ? Instance._Directories[correctedVirtualDir] : null;
        }

        public static void RemoveTemplate(int IdTemplate)
        {
            var virtualPath = Instance._Files.Where(f => f.Value.Template.IdTemplate == IdTemplate).Select(f => f.Key).FirstOrDefault();

            if (virtualPath != null)
            {
                Instance._Files.Remove(virtualPath);
            }
        }
    }
}
