using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ProjetoPadrao.Dados
{
    public static class DbContextFactory<T> where T : DbContext
    {
        public static T Instance
        {
            get
            {
                Type dbContextType = typeof(T);

                if (HttpContext.Current.Items[dbContextType.Name] == null)
                {
                    HttpContext.Current.Items[dbContextType.Name] = Activator.CreateInstance<T>();
                }

                return HttpContext.Current.Items[dbContextType.Name] as T;
            }
        }
    }
}
