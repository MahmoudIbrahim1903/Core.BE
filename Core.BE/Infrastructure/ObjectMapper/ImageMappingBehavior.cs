using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    class ImageMappingBehavior : IMappingBehavior
    {
        public void MapProperties<TSource, TDest>(TSource source, TDest dest)
        {
            foreach (
              var destProp in
                  typeof(TDest).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                      .Where(p => p.CanWrite && !p.GetCustomAttributes(typeof(MappingIgnorAttribute), false).Any() && p.PropertyType.FullName == "Emeint.Media.Model.image"))
            {

                var sourceProp = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name.ToLower().Contains(destProp.Name.ToLower()));

                if (sourceProp != null && source != null)
                {
                    var id = sourceProp.GetValue(source);

                    if (id != "" && id != null)
                    {
                        var image = new Emeint.Media.Model.image(Emeint.Media.Imaging.ImageManager.Instance.GetImage(Convert.ToInt32(id)));
                        destProp.SetValue(dest, image, null);
                    }

                }
            }
        }
    }
}