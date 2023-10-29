using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.ComponentModel.DataAnnotations;
using WMC.Data;
using WMC.Logic;
using WMC.Data.Repositories;

namespace WMC.Web.Controllers
{
    [RoutePrefix("api/v1/orders")]
    public class OrdersController : ApiController
    {
        [HttpGet()]
        [Route("{id}")]
        public IHttpActionResult GetOrderWithId(long id)
        {

            MerchantRepsonse order = MerchantOrderUtility.GetMerchantOrder(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(new APIReponse<MerchantRepsonse> { Data = order });
        }
    }

    public class APIReponse<T>
    {
        public T Data { get; set; }
    }

}