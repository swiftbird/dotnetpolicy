using System;
namespace Policy.Models
{
	public class PolicyModel
	{
		//public Policy()
		//{
		//}

        public int Id { get; set; }
        public string? PolicyNumber { get; set; }
        public string? PolicyType { get; set; }
        public decimal PolicyAmount { get; set; }
    }
}

