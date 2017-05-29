using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinifier.Core.Models;

namespace Tinifier.Core
{
    public static class TestRepo
    {
        public static TImage Get(int id)
        {
            return _images.FirstOrDefault(x => x.Id == id);
        }

        public static IEnumerable<TImage> GetAll()
        {
            return _images;
        }

        private static TImage[] _images =
             new TImage[]
            {
                new TImage { Id = 1, Name = "Test 1"  },
                new TImage { Id = 2, Name = "Test 2"  }
            };
    }

}
