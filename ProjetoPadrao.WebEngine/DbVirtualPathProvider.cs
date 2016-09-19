using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace ProjetoPadrao.WebEngine
{
    public class DbVirtualPathProvider : VirtualPathProvider
    {
        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (base.FileExists(virtualPath))
            {
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            }

            return null;
        }

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            if (!base.FileExists(virtualPath) && FileExists(virtualPath))
            {
                return string.Join(
                    string.Empty,
                    System.Security.Cryptography.MD5
                        .Create()
                        .ComputeHash(
                            System.Text.Encoding.UTF8.GetBytes(
                                DbVirtualFileManager.GetVirtualFile(virtualPath)
                                    .CreationDate
                                    .ToString("dd/MM/yyyy hh:mm:ss.fff")
                            )
                        )
                        .Select(h => h.ToString("x2"))
                );
            }
            else
            {
                return base.GetFileHash(virtualPath, virtualPathDependencies);
            }
        }

        public override bool FileExists(string virtualPath)
        {
            return base.FileExists(virtualPath) || DbVirtualFileManager.GetVirtualFile(virtualPath) != null;
        }

        public override bool DirectoryExists(string virtualDir)
        {
            return base.DirectoryExists(virtualDir) || DbVirtualFileManager.GetVirtualDirectory(virtualDir) != null;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (base.FileExists(virtualPath))
            {
                return base.GetFile(virtualPath);
            }

            try
            {
                return DbVirtualFileManager.GetVirtualFile(virtualPath);
            }
            catch (Exception ex)
            {
                return Previous.GetFile(virtualPath);
            }
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            if (base.DirectoryExists(virtualDir))
            {
                return base.GetDirectory(virtualDir);
            }

            try
            {
                return DbVirtualFileManager.GetVirtualDirectory(virtualDir);
            }
            catch (Exception ex)
            {
                return Previous.GetDirectory(virtualDir);
            }
        }
    }
}
