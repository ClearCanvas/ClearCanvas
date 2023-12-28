using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics
{
    [ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
    public class ConfigPageProvider : IConfigurationPageProvider
    {
        public IEnumerable<IConfigurationPage> GetPages()
        {
            List<IConfigurationPage> list = new List<IConfigurationPage>();
            list.Add(new ConfigurationPage<MedicalPhysicsConfigComponent>("Medical Physics"));
            return list.AsReadOnly();
        }
    }
}
