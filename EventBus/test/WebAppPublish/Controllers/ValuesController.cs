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
            for (int i = 0; i < 100; i++)
            {
                _eventBus.Publish(new UserLocationUpdatedIntegrationEvent(i.ToString()));
            }
            for (int i = 0; i < 100; i++)
            {
                _eventBus.Publish(new WebAppPublish.Event.UserLocationUpdatedIntegrationEvent(i));
            }
            return new string[] { "value1", "value2" };
        }

      
    }

    public class UserLocationUpdatedIntegrationEvent : IntegrationEvent
    {
        public UserLocationUpdatedIntegrationEvent(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }


   
}
