using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PostgresService
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
	[ServiceContract]
	public interface IService1
	{
		[OperationContract]
		[WebGet(UriTemplate="GetCount?min={min}&max={max}&title={title}&tag={tag}")]
		List<string> GetCount(int min, int max, string title, string tag);

		[OperationContract]
		[WebGet(UriTemplate = "GetRating?min={min}&max={max}&title={title}&tag={tag}")]
		List<string> GetRating(int min, int max, string title, string tag);

	}


}
