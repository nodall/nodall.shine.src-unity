using System.Collections.Generic;
using System.IO;

namespace Nexwarp.Network
{
    public class StreamTransferModel
    {
        // los tags sirven para decidir en como se trata el archivo.
        // por ejemplo:
        //  - Type (video, image, app-chosen, app-xxx, etc
        //  - CopyAll por ejemplo permitiria copiar a todos los servidores en postprocesso el archivo, 
        //  - combinacions con la lista de tags
        public List<string> Tags { get; private set; }
        public string Name { get; private set; }
        public int TotalBytes { get; private set; }
        public Stream Stream { get; private set; }
    }
}
