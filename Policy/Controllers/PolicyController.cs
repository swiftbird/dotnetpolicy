using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Policy.Services;
using Policy.Models;

namespace Policy.Controllers
{
    [ApiController]
    [Route("policies")]
    public class PolicyController : ControllerBase
    {
        private readonly ILogger<PolicyController> _logger;
        private readonly IPolicyService _policyService;

        public PolicyController(ILogger<PolicyController> logger, IPolicyService policyService)
        {
            _logger = logger;
            _policyService = policyService;
        }

        [HttpGet(Name = "GetPolicies")]
        public IEnumerable<PolicyModel> Get()
        {
            Console.WriteLine("Wazzup beatches?");
            return _policyService.GetPolicies();
        }


        [HttpPost(Name = "CreatePolicy")]
        public async Task<ActionResult<PolicyModel>> Create([FromBody] PolicyModel newPolicy)
        {
            if (newPolicy == null)
            {
                return BadRequest();
            }

            //var createdPolicy = _policyService.CreatePolicy(newPolicy);

            var createdPolicy = await _policyService.CreatePolicyAsync(newPolicy);


            return CreatedAtAction(nameof(Get), new { id = createdPolicy.Id }, createdPolicy);
        }
    }
}
