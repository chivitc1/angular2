using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;
using WebApp.ViewModels;
using Newtonsoft.Json;
using WebApp.Data;
using WebApp.Data.Items;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        #region private members
        private readonly ApplicationDbContext DbContext;
        #endregion

        #region constructor
        public ItemsController(ApplicationDbContext context)
        {
            //DI
            DbContext = context;
        }
        #endregion

        #region RESTful Conventions
        /// <summary>
        /// GET: api/items
        /// </summary>
        /// <returns>Nothing: this method will raise a HttpNotFound HTTP exception, since we're not supporting this API call.</returns>
        [HttpGet()]
        public IActionResult Get()
        {
            return NotFound(new { Error = "not found" });
        }

        /// <summary>
        /// GET: api/items/{id}
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>A Json-serialized object representing a single item.</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var item = DbContext.Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
                return new JsonResult(TinyMapper.Map<ItemViewModel>(item), DefaultJsonSettings);
            else
                return NotFound(new
                {
                    Error = $"Item id {id} has not been found"
                });
        }
        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/items/GetLatest
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of a default number of Json-serialized objects representing the last inserted items.</returns>
        [HttpGet("GetLatest")]
        public IActionResult GetLatest()
        {
            return GetLatest(DefaultNumberOfItems);
        }

        /// <summary>
        /// GET: api/items/GetLatest/{n}
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of {n} Json-serialized objects representing the last inserted items.</returns>
        [HttpGet("GetLatest/{n}")]
        public IActionResult GetLatest(int n)
        {
            if (n > MaxNumberOfItems) n = MaxNumberOfItems;
            var items = DbContext.Items.OrderByDescending(i => i.CreatedDate).Take(n).ToArray();
            return new JsonResult(ToItemViewModelList(items), DefaultJsonSettings);
        }

        /// <summary>
        /// GET: api/items/GetMostViewed
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of a default number of Json-serialized objects representing the items with most user views.</returns>
        [HttpGet("GetMostViewed")]
        public IActionResult GetMostViewed()
        {
            return GetMostViewed(DefaultNumberOfItems);
        }

        /// <summary>
        /// GET: api/items/GetMostViewed/{n}
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of {n} Json-serialized objects representing the items with most user views.</returns>
        [HttpGet("GetMostViewed/{n}")]
        public IActionResult GetMostViewed(int n)
        {
            if (n > MaxNumberOfItems) n = MaxNumberOfItems;
            var items = DbContext.Items.OrderByDescending(i => i.ViewCount).Take(n).ToArray();
            
            return new JsonResult(ToItemViewModelList(items), DefaultJsonSettings);
        }

        /// <summary>
        /// GET: api/items/GetRandom
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of a default number of Json-serialized objects representing some randomly-picked items.</returns>
        [HttpGet("GetRandom")]
        public IActionResult GetRandom()
        {
            return GetRandom(DefaultNumberOfItems);
        }

        /// <summary>
        /// GET: api/items/GetRandom/{n}
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of {n} Json-serialized objects representing some randomly-picked items.</returns>
        [HttpGet("GetRandom/{n}")]
        public IActionResult GetRandom(int n)
        {
            if (n > MaxNumberOfItems) n = MaxNumberOfItems;
            var items = DbContext.Items.OrderBy(i => Guid.NewGuid()).Take(n).ToArray();
            return new JsonResult(ToItemViewModelList(items), DefaultJsonSettings);
        }

        [HttpPost()]
        public IActionResult Add([FromBody] ItemViewModel ivm)
        {
            //client send invalid payload => Not found
            if (ivm == null) return new StatusCodeResult(500);

            var item = TinyMapper.Map<Item>(ivm);
            item.CreatedDate = item.LastModifiedDate = DateTime.Now;
            item.UserId = DbContext.Users.FirstOrDefault(u => u.UserName =="Admin")?.Id;
            DbContext.Items.Add(item);
            DbContext.SaveChanges();
            return new JsonResult(TinyMapper.Map<ItemViewModel>(item), DefaultJsonSettings);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ItemViewModel ivm)
        {
            //Bad request
            if(ivm == null) return new StatusCodeResult(500);

            var item = DbContext.Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
                return NotFound(new { Error = $"Item ID {id} not found", id });
            item.UserId = ivm.UserId;
            item.Description = ivm.Description;
            item.Flags = ivm.Flags;
            item.Notes = ivm.Notes;
            item.Text = ivm.Text;
            item.Title = ivm.Title;

            //override at server side
            item.LastModifiedDate = DateTime.Now;
            DbContext.SaveChanges();

            return new JsonResult(TinyMapper.Map<ItemViewModel>(item), DefaultJsonSettings);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = DbContext.Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
                return NotFound(new {Error = $"Item ID {id} not found", id});
            DbContext.Items.Remove(item);
            DbContext.SaveChanges();
            //Return code 200
            return new OkResult();
        }
        #endregion

        #region Private Members
        /// <summary>
        /// Generate a sample array of source Items to emulate a database (for testing purposes only).
        /// </summary>
        /// <param name="num">The number of items to generate: default is 999</param>
        /// <returns>a defined number of mock items (for testing purpose only)</returns>
        //private List<ItemViewModel> GetSampleItems(int num = 999)
        //{
        //    List<ItemViewModel> lst = new List<ItemViewModel>();
        //    DateTime date = new DateTime(2015, 12, 31).AddDays(-num);
        //    for (int id = 1; id <= num; id++)
        //    {
        //        date = date.AddDays(1);
        //        lst.Add(new ItemViewModel()
        //        {
        //            Id = id,
        //            Title = String.Format("Item {0} Title", id),
        //            Description = String.Format("This is a sample description for item {0}: Lorem ipsum dolor sit amet.", id),
        //            CreatedDate = date,
        //            LastModifiedDate = date,
        //            ViewCount = num - id
        //        });
        //    }
        //    return lst;
        //}

        /// <summary>
        /// Returns a suitable JsonSerializerSettings object that can be used to generate the JsonResult return value for this Controller's methods.
        /// </summary>
        private JsonSerializerSettings DefaultJsonSettings => new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        };

        /// <summary>
        /// Returns the default number of items to retrieve when using the parameterless overloads of the API methods retrieving item lists.
        /// </summary>
        private int DefaultNumberOfItems => 5;

        /// <summary>
        /// Returns the maximum number of items to retrieve when using the API methods retrieving item lists.
        /// </summary>
        private int MaxNumberOfItems => 100;

        /// <summary>
        /// Maps a collection of Item entities into a list of ItemViewModel objects.
        /// </summary>
        /// <param name="items">An IEnumerable collection of item entities</param>
        /// <returns>a mapped list of ItemViewModel objects</returns>
        private List<ItemViewModel> ToItemViewModelList(IEnumerable<Item> items)
        {
            return items.Select(i => TinyMapper.Map<ItemViewModel>(i)).ToList();
        }
        #endregion
    }
}
