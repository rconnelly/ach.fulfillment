using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Amss.Boilerplate.Backbone.Web.WebApi
{
    using System.Linq;
    using System.Web;
    using System.Web.Caching;

    using Amss.Boilerplate.Backbone.Web.WebApi.Model;

    public class TestController : ApiController
    {
        private static IList<TestModel> Items
        {
            get
            {
                if (HttpContext.Current.Cache["Items"] == null)
                {
                    var value = new List<TestModel>
                        {
                            new TestModel { Id = 1, Date = DateTime.Now, Name = "Name " + 1 },
                            new TestModel { Id = 2, Date = DateTime.Now, Name = "Name " + 2 },
                            new TestModel { Id = 3, Date = DateTime.Now, Name = "Name " + 3 }
                        };

                    HttpContext.Current.Cache.Add(
                        "Items",
                        value,
                        null,
                        Cache.NoAbsoluteExpiration,
                        DateTime.Now.AddMinutes(10).Subtract(DateTime.Now),
                        CacheItemPriority.Normal,
                        null);
                }

                return HttpContext.Current.Cache["Items"] as IList<TestModel>;
            }
       }

        // GET api/test
        public IEnumerable<TestModel> Get()
        {
            return Items;
        }

        // GET api/test/5
        public TestModel Get(int id)
        {
            return Items.FirstOrDefault(m => m.Id == id);
        }

        // POST api/test
        public TestModel Post(TestModel value)
        {
            var max = Items.Count > 0 ? Items.Max(i => i.Id) : 0;
            value.Id = max + 1;
            Items.Add(value);

            return value;
        }

        // PUT api/test/5
        public TestModel Put(int id, TestModel value)
        {
            var item = Items.First(m => m.Id == id);
            item.Name = value.Name;
            item.Date = value.Date;

            return item;
        }

        // DELETE api/test/5
        public void Delete(int id)
        {
            var item = Items.First(m => m.Id == id);
            Items.Remove(item);
        }
    }
}
