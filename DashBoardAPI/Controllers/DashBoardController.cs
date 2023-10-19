using DashBoardAPI.Entity;
using DashBoardAPI.Service.DashBoardService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DashBoardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {


        private  readonly DataContext _datacontext;

        public DashBoardController(DataContext dataContext)
        {
            this._datacontext = dataContext;
        }


        [HttpGet(Name = "GetDashBoardData")]
        public async Task<ActionResult<List<DashBoardEntity>>>GetDashBoardData()
        {
          return Ok(await _datacontext.Dashboards.ToListAsync());
        }
    }
}
