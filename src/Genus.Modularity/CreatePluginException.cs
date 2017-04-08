using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Modularity
{
    public class CreatePluginException:Exception
    {
        public CreatePluginException(string message): base(message)
        {

        }
    }
}
