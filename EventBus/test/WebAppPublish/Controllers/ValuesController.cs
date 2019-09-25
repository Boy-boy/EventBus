using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using Microsoft.AspNetCore.Mvc;

namespace WebAppPublish.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IEventBus _eventBus;

        public ValuesController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            _eventBus.Publish( new PublishEvent());
            return new string[] { "value1", "value2" };
        }

      
    }

    public class PublishEvent : IntegrationEvent
    {
        public PublishEvent()
        {
            Name = "高波";
        }
        public string Name { get; set; }
    }
}
