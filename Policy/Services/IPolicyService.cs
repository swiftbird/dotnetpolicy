using System.Collections.Generic;
using Policy.Models;

namespace Policy.Services
{
    public interface IPolicyService
    {
        Task InitializeAsync();
        IEnumerable<PolicyModel> GetPolicies();
        Task<PolicyModel> CreatePolicyAsync(PolicyModel newPolicy);
    }
}