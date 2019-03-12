using System.Xml.Linq;

namespace Nex.Core
{
    public interface IXmlSerializable<T>
	{
		T FromXml(XElement node);
		XElement ToXml();
	}
}

