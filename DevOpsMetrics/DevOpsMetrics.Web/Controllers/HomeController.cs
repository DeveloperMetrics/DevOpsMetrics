using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DevOpsMetrics.Web.Models;
using Microsoft.Extensions.Configuration;
using DevOpsMetrics.Service.Models;

namespace DevOpsMetrics.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //Azure DevOps
            string patToken = _configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string azBranch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI

            //GitHub
            string owner = "samsmithnz";
            string repo = "samsfeatureflags";
            string ghBranch = "master";
            string workflowId = "108084";

            int numberOfDays = 7;

            ServiceApiClient service = new ServiceApiClient(_configuration);
            IndexDeploymentModel indexModel = await service.GetIndexPage(patToken, organization, project, azBranch, buildId,
                                                                   owner, repo, ghBranch, workflowId,
                                                                   numberOfDays);
            return View(indexModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
