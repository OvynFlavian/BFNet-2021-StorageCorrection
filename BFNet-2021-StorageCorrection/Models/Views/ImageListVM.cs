using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BFNet_2021_StorageCorrection.Models.Views
{
    public class ImageListVM
    {
        public List<string> Uris { get; private set; } = new List<string>();

        public ImageListVM AddUris(List<string> uris)
        {
            foreach(string uri in uris)
            {
                Uris.Add(uri);
            }

            return this;
        }
    }
}
